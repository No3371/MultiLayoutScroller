// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    public class TestMutliLayoutScroller : MonoBehaviour
    {
        MultiLayoutScroller multiLayoutScroller;
        public ViewInstance[] viewPrefabs;
        public LayoutInstance[] layoutPrefabs;
        public ItemInstance[] itemTypePrefabs;
        public MultiLayoutScroller targetScroller;
        public GameInfo[] infos;
        public MultiLayoutScrollerSchemaSO madeSchema;

        void Start ()
        {
            for (var i = 0; i < viewPrefabs.Length; i++)
            {
                targetScroller.DefineViewType(i, viewPrefabs[i]);
            }
            for (var i = 0; i < layoutPrefabs.Length; i++)
            {
                targetScroller.DefineLayoutType(i, layoutPrefabs[i], default);
            }
            for (var i = 0; i < itemTypePrefabs.Length; i++)
            {
                targetScroller.DefineItemType(i, itemTypePrefabs[i], 5);
            }
            BasicDataSource ds = new BasicDataSource();
            for (var i = 0; i < infos.Length; i++)
            {
                ds.Set(i, infos[i]);
            }
            targetScroller.DataSource = ds;
            targetScroller.Load(madeSchema.schema);
            targetScroller.enabled = true;
        }
    }
    // 

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