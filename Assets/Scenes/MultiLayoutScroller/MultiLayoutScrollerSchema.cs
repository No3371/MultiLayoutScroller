// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System.Collections.Generic;

namespace BAStudio.MultiLayoutScroller
{
    // [
    // {
    //     "View": "Classic",
    //     "Layouts": [
    //     {
    //         "Layout": "SinglePortrait",
    //         "Items": [
    //         {
    //             "NameIndex": 0,
    //             "GoldTierEnabled": true,
    //             "SilverTierEnabled": false,
    //             "RibbonIndex": 2,
    //             "ImageIndex": 0
    //         }
    //         ]
    //     }
    //     ]
    // }
    // ]
    [System.Serializable]
    public class MultiLayoutScrollerSchema
    {
        public List<MutliScrollerViewSchema> views;
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