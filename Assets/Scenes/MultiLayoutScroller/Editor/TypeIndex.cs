
using System.Collections.Generic;

namespace BAStudio.MultiLayoutScroller
{
    public static class TypeIndex
    {
        public static Dictionary<int, string> Views, LayoutTypes, ItemPrefabTypes;
        static TypeIndex()
        {
            Views = new Dictionary<int, string>();
            LayoutTypes = new Dictionary<int, string>();
            ItemPrefabTypes = new Dictionary<int, string>();
        } 
    }
}