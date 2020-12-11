// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System.Collections.Generic;
using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    public class ViewInstance : MonoBehaviour
    {
        public RectTransform RectTransform=> (this.transform as RectTransform);
        [HideInInspector] public Dictionary<int, LayoutInstance> layouts;
        internal void Include (int index, LayoutInstance layout)
        {
            if (layouts == null) layouts = new Dictionary<int, LayoutInstance>();
            this.layouts[index] = layout;
            layout.RectTransform.SetParent(this.transform, false);
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