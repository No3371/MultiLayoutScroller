// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System.Collections.Generic;
using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [System.Serializable]
    public class ViewSchema
    {
        public int viewID;
        public ViewLayoutType viewLayoutType;
        public float autoLayoutSpacing;
        public UnityEngine.UI.ScrollRect.MovementType movementType;
        [SerializeField] private RectOffset autoLayoutPadding;
        public RectOffset AutoLayoutPadding { get => autoLayoutPadding = autoLayoutPadding?? new RectOffset(); set => autoLayoutPadding = value; }
        [UnityEngine.SerializeField] private List<LayoutSchema> layouts;
        public List<LayoutSchema> Layouts { get => layouts = layouts?? new List<LayoutSchema>(); }
    }

    public enum ViewLayoutType
    {
        AutoHorizontal,
        AutoVertical,
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