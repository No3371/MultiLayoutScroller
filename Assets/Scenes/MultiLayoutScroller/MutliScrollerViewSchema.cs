// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System.Collections.Generic;
using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [System.Serializable]
    public class MutliScrollerViewSchema
    {
        public int viewID;
        public ViewLayoutType viewLayoutType;
        public float autoLayoutSpacing;
        public RectOffset autoLayoutPadding;
        public bool useUnityPrefabLayout; // For UnityHorizontalLayout/UnityVerticalLayout
        public List<MutliScrollerLayoutSchema> layouts;
    }

    public enum ViewLayoutType
    {
        AutoHorizontal,
        UnityHorizontalLayout,
        AutoVertical,
        UnityVerticalLayout,
        ManuallyArranged
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