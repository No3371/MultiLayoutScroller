// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BAStudio.MultiLayoutScroller
{
    // A MultiLayoutScroller should be able to load its required meta from json and can sustain flawed value
    public abstract class MultiLayoutScroller : ScrollRect
    {
        public IMultiLayoutScrollerDataSource DataSource;
        protected RectTransform Viewport, Content;
        protected RectTransform PrototypeCell;
        protected int maxLayoutInstances, maxItemInstances;
        protected MultiLayoutScrollerSchema activeSchema;
        protected int activeViewIndex, minActiveLayoutIndex, maxActiveLayoutIndex, activeLayoutCount;
        protected MutliScrollerViewSchema activeViewSchema => activeSchema.views[activeViewIndex];
        protected ViewInstance activeViewInstance;
        protected Vector2 _prevAnchoredPos;
        protected CanvasGroup hidden;
        protected override void Awake ()
        {
            base.Awake();
            viewObjects = new Dictionary<int, ViewInstance>();
            layoutTemplate = new Dictionary<int, Stack<LayoutInstance>>();
            itemTemplate = new Dictionary<int, Stack<ItemInstance>>();
            layoutTypeMeta = new Dictionary<int, LayoutTypeMeta>();
            hidden = new GameObject("Pooled").AddComponent<CanvasGroup>();
            hidden.transform.parent = this.transform;
            hidden.alpha = 0;
            hidden.blocksRaycasts = false;
            hidden.interactable = false;
        }

        public void Load (MultiLayoutScrollerSchema schema)
        {
            onValueChanged?.RemoveListener(OnValueChanged);
            if (viewObjects.Count == 0)
                throw new ArgumentNullException("You have to define at least 1 view first!");

            var v = schema.views;
            for (var vIndex = 0; vIndex < v.Count; vIndex++)
            {
                var lo = v[vIndex].layouts;
                for (var loIndex = 0; loIndex < lo.Count; loIndex++)
                {
                    if (!layoutTemplate.ContainsKey(lo[loIndex].typeID))
                    {
                        Debug.LogErrorFormat("Layout type {0} is undefined.", lo[loIndex].typeID);
                    }
                    var i = lo[loIndex].items;
                    for (var iIndex = 0; iIndex < i.Length; iIndex++)
                    {
                        if (!itemTemplate.ContainsKey(i[iIndex].type))
                        {
                            Debug.LogErrorFormat("Item type {0} is undefined.", i[iIndex].type);
                        }
                    }
                }
            }
            InitView(viewObjects[schema.views[0].viewID],
                     ()=> onValueChanged?.AddListener(OnValueChanged));
        }

        public void DefineViewType (int typeID, ViewInstance prefab) 
        {
            Assert.IsNotNull(prefab);
            ViewInstance vi = GameObject.Instantiate(prefab);
            vi.enabled = false;
            vi.RectTransform.SetParent(hidden.transform);
            viewObjects.Add(typeID, vi);
        }
        public void DefineLayoutType (int typeID, LayoutInstance template, LayoutTypeMeta meta) 
        {
            if (layoutTemplate == null) layoutTemplate = new Dictionary<int, Stack<LayoutInstance>>();
            if (layoutTypeMeta == null) layoutTypeMeta = new Dictionary<int, LayoutTypeMeta>(); 
            var stack = new Stack<LayoutInstance>();
            LayoutInstance runtimePrefab = GameObject.Instantiate(template);
            runtimePrefab.RectTransform.SetParent(hidden.transform);
            runtimePrefab.enabled = false;
            stack.Push(runtimePrefab);
            layoutTemplate.Add(typeID, stack);
            if (!meta.set || meta.usePrefabWidth) meta.layoutWidth = template.GetComponent<RectTransform>().rect.size.x;
            if (!meta.set || meta.usePrefabWidth) meta.layoutHeight = template.GetComponent<RectTransform>().rect.size.y;
            if (!meta.set)
            {
                meta.initPoolSize = 1;
                meta.set = true;
            }
            for (int i = 0; i < meta.initPoolSize - 1; i++)
            {
                LayoutInstance li = GameObject.Instantiate(runtimePrefab);
                li.RectTransform.SetParent(hidden.transform);
                stack.Push(li);
            }
            layoutTypeMeta.Add(typeID, meta);
        }
        public void DefineItemType (int typeID, ItemInstance template, int initPoolSize) 
        {
            if (itemTemplate == null) itemTemplate = new Dictionary<int, Stack<ItemInstance>>();
            var stack = new Stack<ItemInstance>();
            ItemInstance runtimePrefab = GameObject.Instantiate(template);
            runtimePrefab.enabled = false;
            runtimePrefab.transform.SetParent(hidden.transform);
            stack.Push(template);
            for (int i = 0; i < initPoolSize - 1; i++)
            {
                ItemInstance ii = GameObject.Instantiate(runtimePrefab);
                ii.RectTransform.SetParent(hidden.transform);
                stack.Push(ii);
            }
            itemTemplate.Add(typeID, stack);
        }
        protected Dictionary<int, Stack<LayoutInstance>> layoutTemplate;
        protected Dictionary<int, Stack<ItemInstance>> itemTemplate;
        protected Dictionary<int, ViewInstance> viewObjects;
        protected Dictionary<int, LayoutTypeMeta> layoutTypeMeta;

        /// <summary>
        /// Initialization when selfInitalize is true. Assumes that data source is set in controller's Awake.
        /// </summary>
        private void Initialize()
        {
            if (activeSchema == null) throw new System.ArgumentNullException();
            _prevAnchoredPos = content.anchoredPosition;
            onValueChanged.RemoveListener(OnValueChanged);
        }

        public void Populate ()
        {
            
        }
        /// <summary>
        /// Corotuine for initiazation.
        /// Using coroutine for init because few UI stuff requires a frame to update
        /// </summary>
        /// <param name="onInitialized">callback when init done</param>
        /// <returns></returns>
        protected virtual IEnumerator InitView(ViewInstance view, Action onInitialized)
        {
            //Setting up container and bounds
            Content = view.RectTransform;
            SetLeftAnchor(Content);
            Content.anchoredPosition = Vector3.zero;
            yield return null;
            SetRecyclingBounds();

            minActiveLayoutIndex = 0;
            maxActiveLayoutIndex = activeSchema.views[activeViewIndex].layouts.Count;

            float contentXSize = 0, contentYSize = 0;
            switch (activeViewSchema.viewLayoutType)
            {
                case ViewLayoutType.AutoHorizontal:
                {
                    contentYSize = Content.sizeDelta.y + activeViewSchema.autoLayoutPadding.top + activeViewSchema.autoLayoutPadding.bottom;
                    for (var i = 0; i < maxActiveLayoutIndex; i++)
                    {
                        contentXSize += layoutTypeMeta[activeSchema.views[activeViewIndex].layouts[i].typeID].layoutWidth;
                    }
                    contentXSize += activeViewSchema.autoLayoutSpacing * (maxActiveLayoutIndex - 1); // Spacing
                    contentXSize += activeViewSchema.autoLayoutPadding.left + activeViewSchema.autoLayoutPadding.right; // Padding
                    Content.sizeDelta = new Vector2(contentXSize, contentYSize);
                    break;
                }
                case ViewLayoutType.AutoVertical:
                {
                    contentXSize = Content.sizeDelta.x + activeViewSchema.autoLayoutPadding.left + activeViewSchema.autoLayoutPadding.right;
                    for (var i = 0; i < maxActiveLayoutIndex; i++)
                    {
                        contentYSize += layoutTypeMeta[activeSchema.views[activeViewIndex].layouts[i].typeID].layoutHeight;
                    }
                    contentYSize += activeViewSchema.autoLayoutSpacing * (maxActiveLayoutIndex - 1); // Spacing
                    contentYSize += activeViewSchema.autoLayoutPadding.top + activeViewSchema.autoLayoutPadding.bottom; // Padding
                    Content.sizeDelta = new Vector2(contentXSize, contentYSize);
                    break;
                }
                default:
                {
                    throw new System.NotImplementedException();
                }
            }
            SetLeftAnchor(Content);

            if (onInitialized != null) onInitialized();
        }


        #region RECYCLING
        
        protected Bounds _recyclableViewBounds;
        protected float RecyclingThreshold = .2f; //Threshold for recycling above and below viewport
        //Temps, Flags
        protected readonly Vector3[] _corners = new Vector3[4];
        protected bool _recycling;

        /// <summary>
        /// Sets the uppper and lower bounds for recycling cells.
        /// </summary>
        protected virtual void SetRecyclingBounds()
        {
            Viewport.GetWorldCorners(_corners);
            float threshHold = RecyclingThreshold * (_corners[2].x - _corners[0].x);
            _recyclableViewBounds.min = new Vector3(_corners[0].x - threshHold, _corners[0].y);
            _recyclableViewBounds.max = new Vector3(_corners[2].x + threshHold, _corners[2].y);
        }
        
        /// <summary>
        /// Added as a listener to the OnValueChanged event of Scroll rect.
        /// Recycling entry point for recyling systems.
        /// </summary>
        /// <param name="direction">scroll direction</param>
        public void OnValueChanged(Vector2 normalizedPos)
        {
            Vector2 dir = content.anchoredPosition - _prevAnchoredPos;
            m_ContentStartPosition += UpdateContentStartPos(dir);
            _prevAnchoredPos = content.anchoredPosition;
        }

        /// <summary>
        /// Recyling entry point
        /// </summary>
        /// <param name="direction">scroll direction </param>
        /// <returns></returns>
        public Vector2 UpdateContentStartPos(Vector2 direction)
        {
            if (_recycling) return Vector2.zero;

            //Updating Recyclable view bounds since it can change with resolution changes.
            SetRecyclingBounds();

            switch (activeSchema.views[activeViewIndex].viewLayoutType)
            {
                case ViewLayoutType.AutoHorizontal:
                {
                    if (direction.x < 0)
                    {
                        return RecycleLeftToRight();
                    }
                    else if (direction.x > 0)
                    {
                        return RecycleRightToleft();
                    }
                    break;
                }
                case ViewLayoutType.AutoVertical:
                {
                    throw new System.NotImplementedException();
                    break;
                }
                case ViewLayoutType.ManuallyArranged:
                {
                    throw new System.NotImplementedException();
                    break;
                }
            }
            return Vector2.zero;
        }

        /// <summary>
        /// Recycles cells from Left to Right in the List heirarchy
        /// </summary>
        protected virtual Vector2 RecycleLeftToRight()
        {
            // _recycling = true;

            // float posX = 0;
            // float posY = 0;

            // //to determine if content size needs to be updated
            // int additionalColoums = 0;

            // //Recycle until cell at left is avaiable and current item count smaller than datasource
            // while (activeViewInstance.layouts[minActiveLayoutIndex].rectTransform.MaxX() < _recyclableViewBounds.min.x && activeLayoutCount < activeSchema.views[activeViewIndex].layouts.Count)
            // {
            //     //Move Left most cell to right
            //     posX = activeViewInstance.layouts[maxActiveLayoutIndex].rectTransform.anchoredPosition.x + activeViewInstance.layouts[maxActiveLayoutIndex].rectTransform.sizeDelta.x;
            //     activeViewInstance.layouts[minActiveLayoutIndex].rectTransform.anchoredPosition = new Vector2(posX, activeViewInstance.layouts[minActiveLayoutIndex].rectTransform.anchoredPosition.y);

            //     //Cell for row at
            //     DataSource.Pull(minActiveLayoutIndex)
            //     DataSource.SetCell(_cachedCells[minActiveLayoutIndex], activeLayoutCount);

            //     //set new indices
            //     maxActiveLayoutIndex = minActiveLayoutIndex;
            //     minActiveLayoutIndex = (minActiveLayoutIndex + 1) % activeViewInstance.layouts.Length;

            //     activeLayoutCount++;
            // }

            // //Content anchor position adjustment.
            // foreach(LayoutInstance li in activeViewInstance.layouts)
            //     li.rectTransform.anchoredPosition -= n * Vector2.right * activeViewInstance.layouts[minActiveLayoutIndex].rectTransform.sizeDelta.x);
            // Content.anchoredPosition += n * Vector2.right * activeViewInstance.layouts[minActiveLayoutIndex].rectTransform.sizeDelta.x;
            // _recycling = false;
            // return n * Vector2.right * activeViewInstance.layouts[minActiveLayoutIndex].rectTransform.sizeDelta.x;
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Recycles cells from Right to Left in the List heirarchy
        /// </summary>
        protected virtual Vector2 RecycleRightToleft()
        {
            // _recycling = true;

            // int n = 0;
            // float posX = IsGrid ? _cellPool[minActiveLayoutIndex].anchoredPosition.x : 0;
            // float posY = 0;

            // //to determine if content size needs to be updated
            // int additionalColoums = 0;
            // //Recycle until cell at Right end is avaiable and current item count is greater than cellpool size
            // while (_cellPool[maxActiveLayoutIndex].MinX() > _recyclableViewBounds.max.x && currentItemCount > _cellPool.Count)
            // {
            //     if (IsGrid)
            //     {
            //         if (--_leftMostCellRow < 0)
            //         {
            //             n++;
            //             _leftMostCellRow = _rows - 1;
            //             posX = _cellPool[minActiveLayoutIndex].anchoredPosition.x - _cellWidth;
            //             additionalColoums++;
            //         }

            //         //Move Right most cell to left
            //         posY = -_leftMostCellRow * _cellHeight;
            //         _cellPool[maxActiveLayoutIndex].anchoredPosition = new Vector2(posX, posY);

            //         if (--_RightMostCellRow < 0)
            //         {
            //             _RightMostCellRow = _rows - 1;
            //             additionalColoums--;
            //         }
            //     }
            //     else
            //     {
            //         //Move Right most cell to left
            //         posX = _cellPool[minActiveLayoutIndex].anchoredPosition.x - _cellPool[minActiveLayoutIndex].sizeDelta.x;
            //         _cellPool[maxActiveLayoutIndex].anchoredPosition = new Vector2(posX, _cellPool[maxActiveLayoutIndex].anchoredPosition.y);
            //         n++;
            //     }

            //     currentItemCount--;
            //     //Cell for row at
            //     DataSource.SetCell(_cachedCells[maxActiveLayoutIndex], currentItemCount - _cellPool.Count);

            //     //set new indices
            //     minActiveLayoutIndex = maxActiveLayoutIndex;
            //     maxActiveLayoutIndex = (maxActiveLayoutIndex - 1 + _cellPool.Count) % _cellPool.Count;
            // }

            // //Content size adjustment
            // if (IsGrid)
            // {
            //     Content.sizeDelta += additionalColoums * Vector2.right * _cellWidth;
            //     if (additionalColoums > 0)
            //     {
            //         n -= additionalColoums;
            //     }
            // }

            // //Content anchor position adjustment.
            // _cellPool.ForEach((RectTransform cell) => cell.anchoredPosition += n * Vector2.right * _cellPool[minActiveLayoutIndex].sizeDelta.x);
            // Content.anchoredPosition -= n * Vector2.right * _cellPool[minActiveLayoutIndex].sizeDelta.x;
            // _recycling = false;
            // return -n * Vector2.right * _cellPool[minActiveLayoutIndex].sizeDelta.x;
            throw new System.NotImplementedException();
        }
        #endregion

        #region  HELPERS
        /// <summary>
        /// Anchoring cell and content rect transforms to top preset. Makes repositioning easy.
        /// </summary>
        /// <param name="rectTransform"></param>
        protected virtual void SetLeftAnchor(RectTransform rectTransform)
        {
            //Saving to reapply after anchoring. Width and height changes if anchoring is change. 
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            Vector2 pos = new Vector2(0, 1);

            //Setting top anchor 
            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;
            rectTransform.pivot = pos;

            //Reapply size
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        #endregion

        #region  TESTING
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_recyclableViewBounds.min - new Vector3(0, 2000), _recyclableViewBounds.min + new Vector3(0, 2000));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_recyclableViewBounds.max - new Vector3(0, 2000), _recyclableViewBounds.max + new Vector3(0, 2000));
        }
        #endregion
    }

    public struct LayoutTypeMeta
    {
        public bool set;
        public bool usePrefabWidth, usePrefabHeight;
        public float layoutWidth, layoutHeight;
        public int initPoolSize;
    }

    // LayoutName: { 0, 1, 4, 2, 11 ,155}  => The layout pulls 

    // MultiLayoutScroller
    // - View (Where scrolling happening)
    //   - Layout (A set of predefined item positon&dimension)
    //     - Item ()
    
    // View
    // View controls how layouts are placed, moved, culled

    // Layouts
    // A layout is just a set of predefined slots, items get place under the slot transforms
    // It does not control it's own placement

    // Item
    // Prefab driving data

}