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
        public ScrollerSchemaScriptableObject madeSchema;

        void Start ()
        {
            for (var i = 0; i < viewPrefabs.Length; i++)
            {
                targetScroller.AssignViewInstance(i, viewPrefabs[i]);
            }
            for (var i = 0; i < layoutPrefabs.Length; i++)
            {
                targetScroller.AssignLayoutPrefab(i, layoutPrefabs[i], new LayoutTypeMeta { initPoolSize = 5 });
            }
            for (var i = 0; i < itemTypePrefabs.Length; i++)
            {
                targetScroller.AssginItemPrefab(i, itemTypePrefabs[i], 10);
            }
            BasicDataSource ds = new BasicDataSource();
            for (var i = 0; i < infos.Length; i++)
            {
                ds.Set(i, infos[i]);
            }
            targetScroller.DataSource = ds;
            targetScroller.enabled = true;
            targetScroller.Load(madeSchema.schema);
            targetScroller.SwitchToLoadedViewStage1(0);
        }

        public void SwitchToView (int viewIndex)
        {
            targetScroller.SwitchToLoadedViewStage1(viewIndex);
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