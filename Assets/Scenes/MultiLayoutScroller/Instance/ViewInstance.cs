// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System.Collections.Generic;
using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    public class ViewInstance : MonoBehaviour
    {
        public RectTransform RectTransform=> (this.transform as RectTransform);
        protected CanvasGroup canvasGroup;
        internal CanvasGroup CanvasGroup { get => canvasGroup?? (canvasGroup = this.GetComponent<CanvasGroup>()); }
        [HideInInspector] public Dictionary<int, LayoutInstance> layouts;
        internal void AddLayout (int index, LayoutInstance layout)
        {
            if (layouts == null) layouts = new Dictionary<int, LayoutInstance>();
            this.layouts[index] = layout;
            layout.RectTransform.SetParent(this.transform, false);
        }
        /// <summary>
        /// Notify that the scroller is switching to this view, the previous view is not switched away yet
        /// Use this for starting visual transition.
        /// At the moment, the canvas group alpha is set to 1.
        /// </summary>
        internal virtual void OnSwitchingTo (IViewTransitionHost scroller) { scroller.SignalTransitionFinished(this); }
        /// <summary>
        /// Notify that the scroller is switching away from view, the view is still present in the scroller
        /// Use this for starting visual transition.
        /// </summary>
        internal virtual void OnSwitchingAway (IViewTransitionHost scroller) { scroller.SignalTransitionFinished(this); }
        /// <summary>
        /// Notify that the scroller is swithced to this view,
        /// </summary>
        internal virtual void OnSwitchedTo() {}
        /// <summary>
        /// Notify that the scroller is swithced away from this view,
        /// At the moment, the canvas group alpha is set to 0.
        /// </summary>
        internal virtual void OnSwitchedAway() {}
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