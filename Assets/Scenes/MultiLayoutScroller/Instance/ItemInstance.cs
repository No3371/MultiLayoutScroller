// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    public abstract class ItemInstance : MonoBehaviour
    {
        
        public RectTransform RectTransform=> (this.transform as RectTransform);
        public abstract void SetData (object data);
        protected CanvasGroup canvasGroup;
        internal CanvasGroup CanvasGroup { get => canvasGroup?? (canvasGroup = this.GetComponent<CanvasGroup>()); }
        public TypeIDPair dataID;
        /// <summary>
        /// Notify that the scroller has loaded this item.
        /// At the moment, the canvas group alpha is set to 1.
        /// </summary>
        internal virtual void OnLoaded () { }
        /// <summary>
        /// Notify that the scroller has pooled this item.
        /// At the moment, the canvas group alpha is set to 0.
        /// </summary>
        internal virtual void OnPooled () { }
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