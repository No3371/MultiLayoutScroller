// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [CreateAssetMenu(menuName = "MultiLayoutScroller/ScrollerViewSchema")]
    [System.Serializable]
    public class MultiLayoutScrollerViewSchemaSO : ScriptableObject
    {
        public MutliScrollerViewSchema viewSchema;
        public MultiLayoutScrollerLayoutSchemaSO[] layoutAssets;
    }
}