// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System.Collections.Generic;

namespace BAStudio.MultiLayoutScroller
{
    // Stateless!
    // The index of data is totally decided from outside.
    public class BasicDataSource : IMultiLayoutScrollerDataSource
    {
        Dictionary<int, object> items;

        public BasicDataSource(Dictionary<int, object> items = null)
        {
            this.items = items;
        }

        public int RuntimeVersion => throw new System.NotImplementedException();

        public object this[int id] => items[id];

        public void Set (int id, object item)
        {
            if (items == null) items = new Dictionary<int, object>();
            if (items.ContainsKey(id)) items[id] = item;
            else items.Add(id, item);
        }
    }
    
    // MultiLayoutScroller
    // - View (Where scrolling happening)
    //   - Layout (A set of predefined item positon&dimension)
    //     - Item ()

    // Layouts
    // A layout is a set of predefined position and dimension of items
    // It's composed of slots (virtual), and the type of each slot
    // A layout instance itself contains the index of the data it's pulling from the scroller data source

    // Item
    // A Item should be decoupled with Layouts
    // A Item should be designed to be of a specfic design, and prepared with a corresponding data struct (Prefab driving data)

}