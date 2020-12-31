// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;
using UnityEngine.Events;

namespace BAStudio.MultiLayoutScroller
{
    public class LayoutInstance : MonoBehaviour
    {
        public RectTransform RectTransform => (RectTransform) this.transform;
        [SerializeField] internal RectTransformData[] slotsBaked;
        [SerializeField] internal int[] slotsSibingIndex;
        internal ItemInstance[] items;
        protected CanvasGroup canvasGroup;
        protected internal CanvasGroup CanvasGroup { get => canvasGroup?? (canvasGroup = this.GetComponent<CanvasGroup>()); }
        internal LayoutSchema schemaCache;
        public UnityEvent onLoadedCallbacks, onPooledCallbacks;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal virtual void Assign(int slotIndex, ItemInstance item)
        {
            item.transform.SetParent(RectTransform, false);
            slotsBaked[slotIndex].Overwrite((RectTransform) item.transform);
            item.OnAssigned(schemaCache.typeID, this);
            items[slotIndex] = item;
        }

        internal virtual void AssignSameItemPrefabType (int slotIndex)
        {
            slotsBaked[slotIndex].Overwrite((RectTransform) items[slotIndex].transform);
            items[slotIndex].OnAssigned(schemaCache.typeID, this);
        }

        internal void LateSiblingIndexOverwrite ()
        {
            if (slotsSibingIndex == null || slotsSibingIndex.Length == 0) return;
            for (var i = 0; i < items.Length; i++)
            {
                items[i].transform.SetSiblingIndex(slotsSibingIndex[i]);
            }
        }

        /// <summary>
        /// Notify that the scroller has loaded this layout.
        /// At the moment, the canvas group alpha is set to 1.
        /// </summary>
        protected internal virtual void OnLoaded (int hostViewID, int indexInView, bool isViewSwitch)
        {
            onLoadedCallbacks?.Invoke();
        }
        /// <summary>
        /// Notify that the scroller has pooled this layout.
        /// At the moment, the canvas group alpha is set to 0.
        /// </summary>
        protected internal virtual void OnPooled ()
        {
            onPooledCallbacks?.Invoke();
        }

        #if UNITY_EDITOR
        [ContextMenu("Recreate Placeholders")]
        public void RecreatePlaceHolders ()
        {
            for (var i = 0; i < slotsBaked.Length; i++)
            {
                var p = new GameObject("Placeholder" + i, typeof(RectTransform));
                p.transform.SetParent(this.transform);
                slotsBaked[i].Overwrite(p.transform as RectTransform);
            }
        }

        [ContextMenu("CopyConfigToAnotherInstance")]
        public void CopyConfigToAnotherInstance ()
        {
            foreach (var i in this.GetComponents<LayoutInstance>())
            {
                if (i == this) continue;

                i.onLoadedCallbacks = this.onLoadedCallbacks;
                i.onPooledCallbacks = this.onPooledCallbacks;
                i.slotsBaked = this.slotsBaked;
                i.slotsSibingIndex = this.slotsSibingIndex;
                break;
            }
        }
        #endif
    }

    [System.Serializable]
    public struct RectTransformData
    {
        public Vector2 anchorMin, anchorMax, anchoredPosition, sizeDelta, localScale, pivot;
        public RectTransformData (RectTransform source)
        {
            this.anchorMin = source.anchorMin;
            this.anchorMax = source.anchorMax;
            this.anchoredPosition = source.anchoredPosition;
            this.sizeDelta = source.sizeDelta;
            this.localScale = source.localScale;
            this.pivot = source.pivot;
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Overwrite (RectTransform dest)
        {
            dest.pivot = this.pivot;
            dest.anchorMin = this.anchorMin;
            dest.anchorMax = this.anchorMax;
            dest.anchoredPosition = this.anchoredPosition;
            dest.sizeDelta = this.sizeDelta;
            dest.localScale = this.localScale;
        }

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