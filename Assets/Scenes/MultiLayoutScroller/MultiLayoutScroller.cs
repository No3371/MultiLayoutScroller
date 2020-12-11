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
    public class MultiLayoutScroller : ScrollRect
    {
        public IMultiLayoutScrollerDataSource DataSource;
        protected RectTransform PrototypeCell;
        protected int maxLayoutInstances, maxItemInstances;
        protected MultiLayoutScrollerSchema activeSchema;
        protected int activeViewIndex, layoutIndexMin, layoutIndexMax;
        protected MutliScrollerViewSchema activeViewSchema => activeSchema.views[activeViewIndex];
        protected ViewInstance activeViewInstance;
        protected Vector2 _prevAnchoredPos;
        protected CanvasGroup hidden;
        Vector4 pooledLayoutPaddingWSAD;
        protected override void Awake ()
        {
            viewObjects = new Dictionary<int, ViewInstance>();
            layoutPool = new Dictionary<int, Stack<LayoutInstance>>();
            itemPool = new Dictionary<int, Stack<ItemInstance>>();
            layoutTypeMeta = new Dictionary<int, LayoutTypeMeta>();
        }

        void AssurePool ()
        {
            if (hidden == null)
            {
                hidden = new GameObject("Pooled").AddComponent<CanvasGroup>();
                hidden.transform.SetParent(this.transform, false);
                hidden.alpha = 0;
                hidden.blocksRaycasts = false;
                hidden.interactable = false;
            }
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
                    if (!layoutPool.ContainsKey(lo[loIndex].typeID))
                    {
                        Debug.LogErrorFormat("Layout type {0} is undefined.", lo[loIndex].typeID);
                    }
                    var i = lo[loIndex].items;
                    for (var iIndex = 0; iIndex < i.Count; iIndex++)
                    {
                        if (!itemPool.ContainsKey(i[iIndex].type))
                        {
                            Debug.LogErrorFormat("Item type {0} is undefined.", i[iIndex].type);
                        }
                    }
                }
            }
            activeSchema = schema;
            activeViewIndex = 0;
            InitView(viewObjects[activeViewSchema.viewID], ()=> onValueChanged?.AddListener(OnValueChanged));
        }

        public void DefineViewType (int typeID, ViewInstance prefab) 
        {
            Assert.IsNotNull(prefab);
            ViewInstance vi = GameObject.Instantiate(prefab);
            vi.enabled = false;
            AssurePool();
            vi.RectTransform.SetParent(hidden.transform, false);
            viewObjects.Add(typeID, vi);
        }
        public void DefineLayoutType (int typeID, LayoutInstance template, LayoutTypeMeta meta) 
        {
            if (layoutPool == null) layoutPool = new Dictionary<int, Stack<LayoutInstance>>();
            if (layoutTypeMeta == null) layoutTypeMeta = new Dictionary<int, LayoutTypeMeta>(); 
            var stack = new Stack<LayoutInstance>();
            LayoutInstance runtimePrefab = GameObject.Instantiate(template);
            runtimePrefab.RectTransform.SetParent(hidden.transform, false);
            runtimePrefab.enabled = false;
            stack.Push(runtimePrefab);
            layoutPool.Add(typeID, stack);
            if (!meta.set || meta.usePrefabWidth) meta.layoutWidth = template.GetComponent<RectTransform>().rect.size.x;
            if (!meta.set || meta.usePrefabWidth) meta.layoutHeight = template.GetComponent<RectTransform>().rect.size.y;
            if (!meta.set)
            {
                meta.initPoolSize = 1;
                meta.set = true;
            }
            AssurePool();
            for (int i = 0; i < meta.initPoolSize - 1; i++)
            {
                LayoutInstance li = GameObject.Instantiate(runtimePrefab);
                li.RectTransform.SetParent(hidden.transform, false);
                stack.Push(li);
            }
            layoutTypeMeta.Add(typeID, meta);
        }
        public void DefineItemType (int typeID, ItemInstance template, int initPoolSize) 
        {
            if (itemPool == null) itemPool = new Dictionary<int, Stack<ItemInstance>>();
            var stack = new Stack<ItemInstance>();
            ItemInstance runtimePrefab = GameObject.Instantiate(template);
            runtimePrefab.enabled = false;
            AssurePool();
            runtimePrefab.transform.SetParent(hidden.transform);
            stack.Push(template);
            for (int i = 0; i < initPoolSize - 1; i++)
            {
                ItemInstance ii = GameObject.Instantiate(runtimePrefab);
                ii.RectTransform.SetParent(hidden.transform, false);
                stack.Push(ii);
            }
            itemPool.Add(typeID, stack);
        }               
        protected Dictionary<int, Stack<LayoutInstance>> layoutPool;
        protected Dictionary<int, Stack<ItemInstance>> itemPool;
        protected Dictionary<int, ViewInstance> viewObjects;
        protected Dictionary<int, LayoutTypeMeta> layoutTypeMeta;

        /// <summary>
        /// Initialization when selfInitalize is true. Assumes that data source is set in controller's Awake.
        /// </summary>
        private void Initialize()
        {
            if (activeSchema == null) throw new System.ArgumentNullException();
            _prevAnchoredPos = base.content.anchoredPosition;
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
        // protected virtual IEnumerator InitView(ViewInstance view, Action onInitialized)
        // {
        //     //Setting up container and bounds
        //     content = view.RectTransform;
        //     content.SetParent(viewport, false);
        //     // Make active view match viewport
        //     content.anchorMin = Vector2.zero;
        //     content.anchorMax = Vector2.one; 
        //     content.pivot = Vector2.up;
        //     content.sizeDelta = Vector2.zero;

        //     yield return null;
        //     SetRecyclingBounds();

        //     minActiveLayoutIndex = 0;
        //     maxActiveLayoutIndex = activeSchema.views[activeViewIndex].layouts.Count;

        //     switch (activeViewSchema.viewLayoutType)
        //     {
        //         case ViewLayoutType.AutoHorizontal:
        //         {
        //             activeViewInstance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CalculateViewSize().x);
        //             break;
        //         }
        //         case ViewLayoutType.AutoVertical:
        //         {
        //             activeViewInstance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CalculateViewSize().y);
        //             break;
        //         }
        //         default:
        //             throw new System.NotImplementedException();
        //     }

        //     activeViewInstance = view;
        //     if (onInitialized != null) onInitialized();
        // }
        protected virtual void InitView(ViewInstance view, Action onInitialized)
        {
            //Setting up container and bounds
            content = view.RectTransform;
            activeViewInstance = view;
            content.SetParent(viewport, false);
            // Make active view match viewport
            SetRecyclingBounds();

            layoutIndexMin = -1;
            layoutIndexMax = -1;

            switch (activeViewSchema.viewLayoutType)
            {
                case ViewLayoutType.AutoHorizontal:
                {
                    content.anchorMin = Vector2.right; // 1, 0
                    content.anchorMax = Vector2.one;   // 1, 1
                    content.pivot = Vector2.up;
                    content.sizeDelta = Vector2.zero;
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CalculateViewSize().x);
                    OnScrolledRight();
                    break;
                }
                case ViewLayoutType.AutoVertical:
                {
                    content.anchorMin = Vector2.up;    // 0, 1
                    content.anchorMax = Vector2.one;   // 1, 1
                    content.pivot = Vector2.up;
                    content.sizeDelta = Vector2.zero;
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CalculateViewSize().y);
                    break;
                }
                default:
                    throw new System.NotImplementedException("Selected view layout type is not suppported!");
            }

            if (onInitialized != null) onInitialized();
        }

        protected virtual Vector2 CalculateViewSize ()
        {
            Vector2 viewSize = activeViewInstance.RectTransform.rect.size;
            Debug.LogFormat("Padding: {0}, Spacing: {1}", activeViewSchema.autoLayoutPadding, activeViewSchema.autoLayoutSpacing);
            switch (activeViewSchema.viewLayoutType)
            {
                case ViewLayoutType.AutoHorizontal:
                {
                    viewSize.y = activeViewInstance.RectTransform.rect.height;
                    for (var i = 0; i < activeViewSchema.layouts.Count; i++) // maxActiveLayoutIndex is set in InitView()
                    {
                        viewSize.x += layoutTypeMeta[activeViewSchema.layouts[i].typeID].layoutWidth;
                    }
                    viewSize.x += activeViewSchema.autoLayoutSpacing * (layoutIndexMax - 1); // Spacing
                    viewSize.x += activeViewSchema.autoLayoutPadding.horizontal; // Padding
                    content.sizeDelta = new Vector2(viewSize.x, 0);
                    pooledLayoutPaddingWSAD.z = 0;
                    pooledLayoutPaddingWSAD.w = viewSize.x;
                    break;
                }
                case ViewLayoutType.AutoVertical:
                {
                    viewSize.x = activeViewInstance.RectTransform.rect.width;
                    for (var i = 0; i < layoutIndexMax; i++) // maxActiveLayoutIndex is set in InitView()
                    {
                        viewSize.y += layoutTypeMeta[activeSchema.views[activeViewIndex].layouts[i].typeID].layoutHeight;
                    }
                    viewSize.y += activeViewSchema.autoLayoutSpacing * (layoutIndexMax - 1); // Spacing
                    viewSize.y += activeViewSchema.autoLayoutPadding.vertical; // Padding
                    content.sizeDelta = new Vector2(0, viewSize.y);
                    pooledLayoutPaddingWSAD.x = 0;
                    pooledLayoutPaddingWSAD.y = viewSize.y;
                    break;
                }
                default:
                {
                    throw new System.NotImplementedException();
                }
            }
            Debug.LogFormat("Estimated view size: {0}, layout counted: {1}", viewSize, layoutIndexMax);
            return viewSize;
        }

        protected virtual void Relayout ()
        {
            switch (activeViewSchema.viewLayoutType)
            {
                case ViewLayoutType.AutoHorizontal:
                {
                    float fromLeft = activeViewSchema.autoLayoutPadding.left;
                    float spacing = activeViewSchema.autoLayoutSpacing;
                    for (var i = 0; i < activeViewInstance.layouts.Count; i++)
                    {
                        float layoutWidth = activeViewInstance.layouts[i].RectTransform.rect.size.x;
                        activeViewInstance.layouts[i].RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, fromLeft, layoutWidth);
                        fromLeft += layoutWidth + spacing;
                    }
                    break;
                }
                case ViewLayoutType.AutoVertical:
                {
                    float fromTop = activeViewSchema.autoLayoutPadding.top;
                    float spacing = activeViewSchema.autoLayoutSpacing;
                    for (var i = 0; i < activeViewInstance.layouts.Count; i++)
                    {
                        float layoutHeight = activeViewInstance.layouts[i].RectTransform.rect.size.y;
                        activeViewInstance.layouts[i].RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, fromTop, layoutHeight);
                        fromTop += layoutHeight + spacing;
                    }
                    break;
                }
                default:
                {
                    throw new System.NotImplementedException();
                }
            }
        }

        
        protected Bounds _recyclableViewBounds;
        protected float RecyclingThreshold = .2f; //Threshold for recycling above and below viewport
        //Temps, Flags
        protected Vector3[] _corners;
        protected bool _recycling;

        /// <summary>
        /// Sets the uppper and lower bounds for recycling cells.
        /// </summary>
        protected virtual void SetRecyclingBounds()
        {
            if (_corners == null) _corners = new Vector3[4];
            viewport.GetWorldCorners(_corners);
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
            Vector2 dir = base.content.anchoredPosition - _prevAnchoredPos;
            m_ContentStartPosition += UpdateContentStartPos(dir);
            _prevAnchoredPos = base.content.anchoredPosition;
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
                        OnScrolledRight();
                    }
                    else if (direction.x > 0)
                    {
                        OnScrolledLeft();
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
        protected virtual void OnScrolledRight()
        {
            _recycling = true;
            float leftPadding = pooledLayoutPaddingWSAD.z, rightPadding = pooledLayoutPaddingWSAD.w;
            float spacing = activeViewSchema.autoLayoutSpacing;

            //Recycle until cell at left is avaiable and current item count smaller than datasource
            if (activeViewInstance.layouts != null && activeViewInstance.layouts.Count > 0)
            while (activeViewInstance.layouts[layoutIndexMin].RectTransform.MaxX() < _recyclableViewBounds.min.x && layoutIndexMin < activeViewSchema.layouts.Count)
            {
                // pool the leftmost layout
                float delta = activeViewInstance.layouts[layoutIndexMin].RectTransform.rect.width + spacing;
                leftPadding += delta;
                LayoutInstance li = activeViewInstance.layouts[layoutIndexMin];
                li.gameObject.SetActive(false); // When toggling & reparenting a UI object, the order matters a lot when we are pursuing performance
                li.RectTransform.SetParent(hidden.transform, false);
                PoolItemsFromLayout(li, layoutIndexMin);
                layoutPool[activeViewSchema.layouts[layoutIndexMin].typeID].Push(li);
                layoutIndexMin++;
            }
            float poppingPos;
            if (activeViewInstance.layouts == null || activeViewInstance.layouts.Count == 0) poppingPos = leftPadding;
            else poppingPos = spacing + activeViewInstance.layouts[layoutIndexMax].RectTransform.MaxX(); 
            // While there're still more layouts on the right side should be loaded
            while (layoutIndexMax < activeViewSchema.layouts.Count - 1 && poppingPos < _recyclableViewBounds.max.x)
            {
                int newLayoutIndex = layoutIndexMax + 1;
                LayoutInstance li = layoutPool[activeViewSchema.layouts[newLayoutIndex].typeID].Pop().GetComponent<LayoutInstance>();
                rightPadding -= (spacing + (li.transform as RectTransform).rect.width);
                FillItemsIntoLayout(li, newLayoutIndex);
                for (int i = 0; i < li.items.Length; i++)
                {
                    li.items[i].SetData(DataSource[activeViewSchema.layouts[newLayoutIndex].items[i].id]);
                }
                li.gameObject.SetActive(true); // When toggling & reparenting a UI object, the order matters a lot when we are pursuing performance
                activeViewInstance.Include(newLayoutIndex, li);
                li.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, poppingPos, li.RectTransform.rect.width);
                layoutIndexMax++;
                poppingPos = spacing + activeViewInstance.layouts[layoutIndexMax].RectTransform.MaxX();
            }
            
            pooledLayoutPaddingWSAD.z = leftPadding;
            pooledLayoutPaddingWSAD.w = rightPadding;

            _recycling = false;
        }
        protected virtual void OnScrolledLeft()
        {
            _recycling = true;
            float leftPadding = pooledLayoutPaddingWSAD.z, rightPadding = pooledLayoutPaddingWSAD.w;
            float spacing = activeViewSchema.autoLayoutSpacing;

            if (activeViewInstance.layouts != null && activeViewInstance.layouts.Count > 0)
            while (activeViewInstance.layouts[layoutIndexMax].RectTransform.MinX() > _recyclableViewBounds.max.x && layoutIndexMax > 0)
            {
                // pool the rightmost layout
                rightPadding -= (activeViewInstance.layouts[layoutIndexMax].RectTransform.rect.width + spacing);
                LayoutInstance li = activeViewInstance.layouts[layoutIndexMax];
                li.gameObject.SetActive(false);
                li.RectTransform.SetParent(hidden.transform, false);
                PoolItemsFromLayout(li, layoutIndexMax);
                layoutPool[activeViewSchema.layouts[layoutIndexMax].typeID].Push(li);
                layoutIndexMax--;
            }
            float poppingPos;
            if (activeViewInstance.layouts == null || activeViewInstance.layouts.Count == 0) poppingPos = leftPadding;
            else poppingPos = activeViewInstance.layouts[layoutIndexMin].RectTransform.MinX() - spacing - layoutTypeMeta[activeViewSchema.layouts[layoutIndexMin - 1].typeID].layoutWidth; 
            // While there're still more layouts on the left side should be loaded
            while (layoutIndexMin > 0 && poppingPos > _recyclableViewBounds.max.x)
            {
                int newLayoutIndex = layoutIndexMin - 1;
                LayoutInstance li = layoutPool[activeViewSchema.layouts[newLayoutIndex].typeID].Pop().GetComponent<LayoutInstance>();
                li.gameObject.SetActive(false);
                activeViewInstance.Include(newLayoutIndex, li);
                li.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, poppingPos, li.RectTransform.rect.width);
                li.transform.SetAsFirstSibling();
                leftPadding -= (spacing + (li.transform as RectTransform).rect.width);
                FillItemsIntoLayout(li, newLayoutIndex);
                for (int i = 0; i < li.items.Length; i++)
                {
                    li.items[i].SetData(DataSource[activeViewSchema.layouts[newLayoutIndex].items[i].id]);
                }
                activeViewInstance.layouts.Add(newLayoutIndex, li);
                li.gameObject.SetActive(true);
                layoutIndexMin--;
                poppingPos = activeViewInstance.layouts[layoutIndexMin].RectTransform.MinX() - spacing - layoutTypeMeta[activeViewSchema.layouts[layoutIndexMin - 1].typeID].layoutWidth; 
            }

            pooledLayoutPaddingWSAD.z = leftPadding;
            pooledLayoutPaddingWSAD.w = rightPadding;
            _recycling = false;
        }

        void PoolItemsFromLayout (LayoutInstance li, int layoutIndex)
        {
            for (var i = 0; i < li.items.Length; i++)
            {
                li.items[i].gameObject.SetActive(false);
                li.items[i].transform.SetParent(hidden.transform, false);
                itemPool[activeViewSchema.layouts[layoutIndex].items[i].id].Push(li.items[i]);
            }
        }

        void FillItemsIntoLayout (LayoutInstance li, int layoutIndex)
        {
            MutliLayoutScrollerLayoutSchema targetSchema = activeViewSchema.layouts[layoutIndex];
            if (li.items == null || li.items.Length != targetSchema.items.Count) li.items = new ItemInstance[targetSchema.items.Count];
            for (var i = 0; i < targetSchema.items.Count; i++)
            {
                ItemInstance ii = itemPool[targetSchema.items[i].type].Pop();
                ii.gameObject.SetActive(false);
                li.Assign(i, ii);
                ii.gameObject.SetActive(true);
            }
        }

        // #region  HELPERS
        // /// <summary>
        // /// Anchoring cell and content rect transforms to top preset. Makes repositioning easy.
        // /// </summary>
        // /// <param name="rectTransform"></param>
        // protected virtual void SetLeftAnchor(RectTransform rectTransform)
        // {
        //     //Saving to reapply after anchoring. Width and height changes if anchoring is change. 
        //     float width = rectTransform.rect.width;
        //     float height = rectTransform.rect.height;

        //     Vector2 pos = new Vector2(0, 1);

        //     //Setting top anchor 
        //     rectTransform.anchorMin = pos;
        //     rectTransform.anchorMax = pos;
        //     rectTransform.pivot = pos;

        //     //Reapply size
        //     rectTransform.sizeDelta = new Vector2(width, height);
        // }

        // #endregion

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