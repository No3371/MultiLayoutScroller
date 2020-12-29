// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System.Collections.Generic;

namespace BAStudio.MultiLayoutScroller
{
    [System.Serializable]
    public class LayoutSchema
    {
        public int typeID;
        [UnityEngine.SerializeField] private List<ItemTypeIDPair> items;
        public List<ItemTypeIDPair> Items { get => items = items ?? new List<ItemTypeIDPair>(); }
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