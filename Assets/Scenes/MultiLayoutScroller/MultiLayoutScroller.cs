// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)
# define DEBUG_MULTILAYOUT

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
        const bool _debug = false;
        static readonly Vector2 _awayPosition = new Vector2(-195623, 44236);
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
        RectTransform root;
        protected override void Awake ()
        {
            viewObjects = new Dictionary<int, ViewInstance>();
            layoutPool = new Dictionary<int, Stack<LayoutInstance>>();
            itemPool = new Dictionary<int, Stack<ItemInstance>>();
            layoutTypeMeta = new Dictionary<int, LayoutTypeMeta>();
            recyclingViewBoundsCorners = new Vector3[4];
            tempWorldCorners = new Vector3[4];
            viewInstanceWorldCorners = new Vector3[4];
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
            runtimePrefab.transform.SetParent(hidden.transform, false);
            stack.Push(runtimePrefab);
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
            activeViewInstance.RectTransform.GetWorldCorners(viewInstanceWorldCorners);
            if (root == null) root = this.transform.root.transform as RectTransform;


            layoutIndexMin = -1;
            layoutIndexMax = -1;

            switch (activeViewSchema.viewLayoutType)
            {
                case ViewLayoutType.AutoHorizontal:
                {
                    content.anchorMin = Vector2.right; // 1, 0
                    content.anchorMax = Vector2.one;   // 1, 1
                    content.pivot = Vector2.up;
                    content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, CalculateViewSize().x);
                    PopLayoutRight(false);
                    OnScrolledRight();
                    break;
                }
                case ViewLayoutType.AutoVertical:
                {
                    content.anchorMin = Vector2.up;    // 0, 1
                    content.anchorMax = Vector2.one;   // 1, 1
                    content.pivot = Vector2.up;
                    content.sizeDelta = Vector2.zero;
                    content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, CalculateViewSize().y);
                    break;
                }
                default:
                    throw new System.NotImplementedException("Selected view layout type is not suppported!");
            }

            #if DEBUG_MULTILAYOUT
                InitDebug();
                UpdateDebug();
            #endif
            if (onInitialized != null) onInitialized();
        }

        protected virtual Vector2 CalculateViewSize ()
        {
            Vector2 viewSize = activeViewInstance.RectTransform.rect.size;
            if (_debug) Debug.LogFormat("Padding: {0}, Spacing: {1}", activeViewSchema.autoLayoutPadding, activeViewSchema.autoLayoutSpacing);
            switch (activeViewSchema.viewLayoutType)
            {
                case ViewLayoutType.AutoHorizontal:
                {
                    viewSize.y = activeViewInstance.RectTransform.rect.height;
                    for (var i = 0; i < activeViewSchema.layouts.Count; i++) // maxActiveLayoutIndex is set in InitView()
                    {
                        viewSize.x += layoutTypeMeta[activeViewSchema.layouts[i].typeID].layoutWidth;
                    }
                    viewSize.x += activeViewSchema.autoLayoutSpacing * (activeViewSchema.layouts.Count - 1); // Spacing
                    viewSize.x += activeViewSchema.autoLayoutPadding.horizontal; // Padding
                    content.sizeDelta = new Vector2(viewSize.x, 0);
                    pooledLayoutPaddingWSAD.z = activeViewSchema.autoLayoutPadding.left;
                    pooledLayoutPaddingWSAD.w = viewSize.x - activeViewSchema.autoLayoutPadding.left;
                    break;
                }
                case ViewLayoutType.AutoVertical:
                {
                    viewSize.x = activeViewInstance.RectTransform.rect.width;
                    for (var i = 0; i < activeViewSchema.layouts.Count; i++) // maxActiveLayoutIndex is set in InitView()
                    {
                        viewSize.y += layoutTypeMeta[activeSchema.views[activeViewIndex].layouts[i].typeID].layoutHeight;
                    }
                    viewSize.y += activeViewSchema.autoLayoutSpacing * (activeViewSchema.layouts.Count - 1); // Spacing
                    viewSize.y += activeViewSchema.autoLayoutPadding.vertical; // Padding
                    content.sizeDelta = new Vector2(0, viewSize.y);
                    pooledLayoutPaddingWSAD.x = activeViewSchema.autoLayoutPadding.top;
                    pooledLayoutPaddingWSAD.y = viewSize.y - activeViewSchema.autoLayoutPadding.top;
                    break;
                }
                default:
                {
                    throw new System.NotImplementedException();
                }
            }
            if (_debug) Debug.LogFormat("Estimated view size: {0}, layout count: {1}", viewSize, activeViewSchema.layouts.Count);
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

        
        protected Bounds recyclingleViewBounds;
        protected float RecyclingThreshold = .2f; //Threshold for recycling above and below viewport
        //Temps, Flags
        protected Vector3[] recyclingViewBoundsCorners, viewInstanceWorldCorners, tempWorldCorners;
        protected bool _recycling;
        UnityEngine.Resolution resolutionCache;

        /// <summary>
        /// Sets the uppper and lower bounds for recycling cells.
        /// </summary>
        protected virtual void SetRecyclingBounds()
        {
            viewport.GetWorldCorners(recyclingViewBoundsCorners);
            float threshHold = RecyclingThreshold * (recyclingViewBoundsCorners[2].x - recyclingViewBoundsCorners[0].x);
            recyclingleViewBounds.min = new Vector3(recyclingViewBoundsCorners[0].x - threshHold, recyclingViewBoundsCorners[0].y);
            recyclingleViewBounds.max = new Vector3(recyclingViewBoundsCorners[2].x + threshHold, recyclingViewBoundsCorners[2].y);
            if (_debug) Debug.LogFormat("Recycling Bounds: {0}", recyclingleViewBounds);
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
            
            #if DEBUG_MULTILAYOUT
                UpdateDebug();
            #endif
        }

        /// <summary>
        /// Recyling entry point
        /// </summary>
        /// <param name="direction">scroll direction </param>
        /// <returns></returns>
        public Vector2 UpdateContentStartPos(Vector2 direction)
        {
            if (_recycling) return Vector2.zero;
            if (root == null) root = this.transform.root.transform as RectTransform;

            //Updating Recyclable view bounds since it can change with resolution changes.
            var curRes = Screen.currentResolution;
            if (curRes.width != resolutionCache.width | curRes.height != resolutionCache.height)
            {
                SetRecyclingBounds();
                resolutionCache = Screen.currentResolution;
            }

            activeViewInstance.RectTransform.GetWorldCorners(viewInstanceWorldCorners);

            switch (activeSchema.views[activeViewIndex].viewLayoutType)
            {
                case ViewLayoutType.AutoHorizontal:
                {
                    if (direction.x > 0)
                    {
                        OnScrolledLeft();
                    }
                    else if (direction.x < 0)
                    {
                        OnScrolledRight();
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
            // if (_debug) Debug.LogFormat("OnScrollRight, leftPadding: {0}, rightPadding: {1}, spacing: {2}", leftPadding, rightPadding, spacing);
            if (_debug) Debug.LogFormat("POOL <<< LOAD | Active layouts: {0} ~ {1}", layoutIndexMin, layoutIndexMax);

            // POOL <<<            // Recycle until cell at left is avaiable and current item count smaller than datasource
            while(layoutIndexMin < layoutIndexMax)
            {
                activeViewInstance.layouts[layoutIndexMin].RectTransform.GetWorldCorners(tempWorldCorners);
                if (tempWorldCorners[2].x > recyclingleViewBounds.min.x)
                    break;
                // pool the leftmost layout
                LayoutInstance li = activeViewInstance.layouts[layoutIndexMin];
                leftPadding += li.RectTransform.rect.width;
                if (activeViewInstance.layouts.Count > 1) leftPadding += spacing;
                PoolLayoutInstance(li, layoutIndexMin);
                layoutIndexMin++;
                if (_debug) Debug.LogFormat("POOL <<< | Active layouts: {0} ~ {1}", layoutIndexMin, layoutIndexMax);
            }
            float poppingPos;
            if (activeViewInstance.layouts == null || activeViewInstance.layouts.Count == 0)
            {
                Debug.Log(" <<< | No active layouts, start popping from the leftpadding.");
                poppingPos = leftPadding;
            }
            else poppingPos = leftPadding;
            //  <<< LOAD
            // if (_debug) Debug.LogFormat("Popping x start: {0} / bound x : {1} ~ {2}", viewInstanceCorners[0].x + poppingPos, innerRecyclingViewBounds.min.x, innerRecyclingViewBounds.max.x);
            while (layoutIndexMax < activeViewSchema.layouts.Count - 1)
            {
                activeViewInstance.layouts[layoutIndexMax].RectTransform.GetWorldCorners(tempWorldCorners);
                if (tempWorldCorners[2].x + (spacing) * root.localScale.x > recyclingleViewBounds.max.x)
                    break;
                layoutIndexMax++;
                LayoutInstance li = PopLayout(activeViewSchema.layouts[layoutIndexMax].typeID);
                // if (_debug) Debug.LogFormat("Popping layout {0}/{1}, it's type {2} and has {3} items", newLayoutIndex, activeViewSchema.layouts.Count - 1, activeViewSchema.layouts[newLayoutIndex].typeID ,activeViewSchema.layouts[newLayoutIndex].items.Count);
                // if (_debug) Debug.LogFormat("Right padding: {0} (-{1})", rightPadding, spacing + (li.transform as RectTransform).rect.width);
                FillItemsIntoLayout(li, layoutIndexMax);
                activeViewInstance.AddLayout(layoutIndexMax, li);
                li.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (activeViewInstance.RectTransform.rect.width - rightPadding + spacing), li.RectTransform.rect.width);
                rightPadding -= (li.transform as RectTransform).rect.width;
                if (activeViewInstance.layouts.Count > 1) rightPadding -= spacing; // There's no spacing if there's only 1 layout
                if (_debug) Debug.LogFormat(" <<< LOAD | Active layouts: {0} ~ {1}", layoutIndexMin, layoutIndexMax);
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
            // if (_debug) Debug.LogFormat("OnScrollLeft, leftPadding: {0}, rightPadding: {1}, spacing: {2}", leftPadding, rightPadding, spacing);
            if (_debug) Debug.LogFormat("LOAD >>> POOL | Active layouts: {0} ~ {1}", layoutIndexMin, layoutIndexMax);

            // >>> POOL
            while (layoutIndexMax > layoutIndexMin && layoutIndexMin >= 0)
            {
                activeViewInstance.layouts[layoutIndexMax].RectTransform.GetWorldCorners(tempWorldCorners);
                if (tempWorldCorners[0].x < recyclingleViewBounds.max.x) break;
                // pool the rightmost layout
                LayoutInstance li = activeViewInstance.layouts[layoutIndexMax];
                rightPadding += li.RectTransform.rect.width;
                if (activeViewInstance.layouts.Count > 1) rightPadding += spacing;
                PoolLayoutInstance(li, layoutIndexMax);
                layoutIndexMax--;
                if (_debug) Debug.LogFormat(">>> POOL | Active layouts: {0} ~ {1}", layoutIndexMin, layoutIndexMax);
            }

            // LOAD >>>
            // While there're still more layouts on the left side should be loaded
            while (layoutIndexMin > 0)
            {
                activeViewInstance.layouts[layoutIndexMin].RectTransform.GetWorldCorners(tempWorldCorners);
                if (tempWorldCorners[0].x - (spacing * root.localScale.x) < recyclingleViewBounds.min.x)
                    break;
                layoutIndexMin--;
                LayoutInstance li = PopLayout(activeViewSchema.layouts[layoutIndexMin].typeID);
                FillItemsIntoLayout(li, layoutIndexMin);
                activeViewInstance.AddLayout(layoutIndexMin, li);
                leftPadding -= ((li.transform as RectTransform).rect.width + spacing);
                li.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, leftPadding, li.RectTransform.rect.width);
                li.transform.SetAsFirstSibling();
                if (_debug) Debug.LogFormat("LOAD >>> | Active layouts: {0} ~ {1}", layoutIndexMin, layoutIndexMax);                
            }

            pooledLayoutPaddingWSAD.z = leftPadding;
            pooledLayoutPaddingWSAD.w = rightPadding;
            _recycling = false;
        }

        void PopLayoutRight (bool hasSpacing)
        {
            float popAt = pooledLayoutPaddingWSAD.z, rightPadding = pooledLayoutPaddingWSAD.w, spacing = activeViewSchema.autoLayoutSpacing;
            if (layoutIndexMax == -1)
            {
                layoutIndexMax = layoutIndexMin = 0;
            }
            else
            {
                 activeViewInstance.layouts[layoutIndexMax].RectTransform.GetWorldCorners(tempWorldCorners);
                 popAt = tempWorldCorners[1].x + spacing;
            }
            LayoutInstance li = PopLayout(activeViewSchema.layouts[layoutIndexMax].typeID);
            activeViewInstance.AddLayout(layoutIndexMax, li);
            li.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, popAt, li.RectTransform.rect.width);
            li.transform.SetAsFirstSibling();
            rightPadding -= (li.transform as RectTransform).rect.width;
            if (hasSpacing) rightPadding -= spacing;
            FillItemsIntoLayout(li, layoutIndexMax);
            for (int i = 0; i < li.items.Length; i++)
            {
                li.items[i].SetData(DataSource[activeViewSchema.layouts[layoutIndexMax].items[i].dataID]);
            }
            if (_debug) Debug.LogFormat("<<< LOAD | Active layouts: {0} ~ {1}", layoutIndexMin, layoutIndexMax);  
            pooledLayoutPaddingWSAD.w = rightPadding;
        }

        LayoutInstance PopLayout (int layoutTypeID)
        {
            Stack<LayoutInstance> stack = layoutPool[layoutTypeID];
            if (stack.Count <= 1)
                return GameObject.Instantiate(stack.Peek());
            else return stack.Pop();
        }

        ItemInstance PopItem (int itemTypeID)
        {
            Stack<ItemInstance> stack = itemPool[itemTypeID];
            if (stack.Count <= 1)
                return GameObject.Instantiate(stack.Peek());
            else return stack.Pop();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        void PoolLayoutInstance (LayoutInstance li, int layoutIndex)
        {
            PoolItemsFromLayout(li, layoutIndex);
            li.RectTransform.anchoredPosition = new Vector2(_awayPosition.x, li.RectTransform.anchoredPosition.y);
            activeViewInstance.layouts.Remove(layoutIndex);
            layoutPool[activeViewSchema.layouts[layoutIndex].typeID].Push(li);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        void PoolItemsFromLayout (LayoutInstance li, int layoutIndex)
        {
            for (var i = 0; i < li.items.Length; i++)
            {
                li.items[i].RectTransform.anchoredPosition = new Vector2(_awayPosition.x, li.items[i].RectTransform.anchoredPosition.y);
                itemPool[activeViewSchema.layouts[layoutIndex].items[i].type].Push(li.items[i]);
                li.items[i] = null;
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        void FillItemsIntoLayout (LayoutInstance li, int layoutIndex)
        {
            MutliLayoutScrollerLayoutSchema targetSchema = activeViewSchema.layouts[layoutIndex];
            if (li.items == null || li.items.Length != targetSchema.items.Count) li.items = new ItemInstance[targetSchema.items.Count];
            var schemaItems = activeViewSchema.layouts[layoutIndex].items;
            for (var i = 0; i < targetSchema.items.Count; i++)
            {
                ItemInstance ii = PopItem(targetSchema.items[i].type);
                li.Assign(i, ii);
                ii.SetData(DataSource[schemaItems[i].dataID]);
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
            Gizmos.DrawLine(recyclingleViewBounds.min, new Vector2(recyclingleViewBounds.min.x, recyclingleViewBounds.max.y));
            Gizmos.DrawLine(recyclingleViewBounds.max, new Vector2(recyclingleViewBounds.min.x, recyclingleViewBounds.max.y));
            Gizmos.DrawLine(recyclingleViewBounds.min, new Vector2(recyclingleViewBounds.max.x, recyclingleViewBounds.min.y));
            Gizmos.DrawLine(recyclingleViewBounds.max, new Vector2(recyclingleViewBounds.max.x, recyclingleViewBounds.min.y));
            if (activeViewInstance != null && viewInstanceWorldCorners != null && root != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(viewInstanceWorldCorners[0], viewInstanceWorldCorners[1]);
                Gizmos.DrawLine(viewInstanceWorldCorners[0], viewInstanceWorldCorners[3]);
                Gizmos.DrawLine(viewInstanceWorldCorners[2], viewInstanceWorldCorners[1]);
                Gizmos.DrawLine(viewInstanceWorldCorners[2], viewInstanceWorldCorners[3]);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector2(pooledLayoutPaddingWSAD.z*root.localScale.x + viewInstanceWorldCorners[0].x, viewInstanceWorldCorners[0].y),
                                new Vector2(pooledLayoutPaddingWSAD.z*root.localScale.x + viewInstanceWorldCorners[0].x, viewInstanceWorldCorners[2].y/2));
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector2(-pooledLayoutPaddingWSAD.w*root.localScale.x + viewInstanceWorldCorners[1].x, viewInstanceWorldCorners[2].y),
                                new Vector2(-pooledLayoutPaddingWSAD.w*root.localScale.x + viewInstanceWorldCorners[1].x, viewInstanceWorldCorners[2].y/2));
            }
        }
        #endregion

        #if DEBUG_MULTILAYOUT
        RectTransform paddingLeft, pooledLeft, body, pooledRight, paddingRight, layoutMin, layoutMax;

        void InitDebug ()
        {
            paddingLeft = new GameObject("_paddingLeft", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            paddingLeft.SetParent(activeViewInstance.transform, false);
            paddingLeft.pivot = Vector2.zero;
            paddingLeft.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 20);
            paddingLeft.GetComponent<Image>().color = Color.red;
            paddingLeft.GetComponent<Image>().maskable = false;
            pooledLeft = new GameObject("_pooledLeft", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            pooledLeft.SetParent(activeViewInstance.transform, false);
            pooledLeft.GetComponent<Image>().color = Color.green;
            pooledLeft.GetComponent<Image>().maskable = false;
            pooledLeft.pivot = Vector2.zero;
            pooledLeft.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 20);
            body = new GameObject("_body", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            body.SetParent(activeViewInstance.transform, false);
            body.GetComponent<Image>().color = Color.cyan;
            body.GetComponent<Image>().maskable = false;
            body.pivot = Vector2.zero;
            body.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 20);
            pooledRight = new GameObject("_pooledRight", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            pooledRight.SetParent(activeViewInstance.transform, false);
            pooledRight.GetComponent<Image>().color = Color.blue;
            pooledRight.GetComponent<Image>().maskable = false;
            pooledRight.pivot = Vector2.zero;
            pooledRight.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 20);
            paddingRight = new GameObject("_paddingRight", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            paddingRight.SetParent(activeViewInstance.transform, false);
            paddingRight.GetComponent<Image>().color = Color.yellow;
            paddingRight.GetComponent<Image>().maskable = false;
            paddingRight.pivot = Vector2.zero;
            paddingRight.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 20);
            layoutMin = new GameObject("_layoutMin", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            layoutMin.SetParent(activeViewInstance.transform, false);
            layoutMin.GetComponent<Image>().color = Color.magenta;
            layoutMin.GetComponent<Image>().maskable = false;
            layoutMin.anchorMin = Vector2.up/2f;
            layoutMin.anchorMax = Vector2.up/2f;
            layoutMin.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -20, 20);
            layoutMax = new GameObject("_layoutMax", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            layoutMax.SetParent(activeViewInstance.transform, false);
            layoutMax.GetComponent<Image>().color = Color.magenta;
            layoutMax.GetComponent<Image>().maskable = false;
            layoutMax.anchorMin = Vector2.up/2f;
            layoutMax.anchorMax = Vector2.up/2f;
            layoutMax.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -20, 20);
        }

        void UpdateDebug ()
        {
            paddingLeft.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, activeViewSchema.autoLayoutPadding.left);
            pooledLeft.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, activeViewSchema.autoLayoutPadding.left, pooledLayoutPaddingWSAD.z - activeViewSchema.autoLayoutPadding.left);
            body.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pooledLayoutPaddingWSAD.z, activeViewInstance.RectTransform.rect.width - pooledLayoutPaddingWSAD.z - pooledLayoutPaddingWSAD.w);
            pooledRight.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, activeViewInstance.RectTransform.rect.width - pooledLayoutPaddingWSAD.w, pooledLayoutPaddingWSAD.w - activeViewSchema.autoLayoutPadding.right);
            paddingRight.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, activeViewInstance.RectTransform.rect.width - activeViewSchema.autoLayoutPadding.right, activeViewSchema.autoLayoutPadding.right);
            RectTransform r = activeViewInstance.layouts[layoutIndexMin].RectTransform;
            layoutMin.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, r.rect.width);
            layoutMin.anchoredPosition = r.anchoredPosition;
            r = activeViewInstance.layouts[layoutIndexMax].RectTransform;
            layoutMax.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, r.rect.width);
            layoutMax.anchoredPosition = r.anchoredPosition;
        }
        #endif
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