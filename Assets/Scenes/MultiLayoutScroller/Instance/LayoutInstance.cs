// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    public class LayoutInstance : MonoBehaviour
    {
        public RectTransform RectTransform=> (this.transform as RectTransform);
        [SerializeField] internal RectTransformData[] slotsBaked;
        internal ItemInstance[] items;
        protected CanvasGroup canvasGroup;
        internal CanvasGroup CanvasGroup { get => canvasGroup?? (canvasGroup = this.GetComponent<CanvasGroup>()); }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal void Assign(int slotIndex, ItemInstance item)
        {
            item.transform.SetParent(RectTransform, false);
            slotsBaked[slotIndex].Overwrite((RectTransform) item.transform);
            items[slotIndex] = item;
        }
        public bool CullByItem; // Should the scroller scan through and try to load/unload items 

        /// <summary>
        /// Notify that the scroller has loaded this layout.
        /// At the moment, the canvas group alpha is set to 1.
        /// </summary>
        internal virtual void OnLoaded () { }
        /// <summary>
        /// Notify that the scroller has pooled this layout.
        /// At the moment, the canvas group alpha is set to 0.
        /// </summary>
        internal virtual void OnPooled () { }
    }

    [System.Serializable]
    struct RectTransformData
    {
        public Vector2 anchorMin, anchorMax, anchoredPosition, sizeDelta;
        public RectTransformData (RectTransform source)
        {
            this.anchorMin = source.anchorMin;
            this.anchorMax = source.anchorMax;
            this.anchoredPosition = source.anchoredPosition;
            this.sizeDelta = source.sizeDelta;
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Overwrite (RectTransform dest)
        {
            dest.anchorMin = this.anchorMin;
            dest.anchorMax = this.anchorMax;
            dest.anchoredPosition = this.anchoredPosition;
            dest.sizeDelta = this.sizeDelta;
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