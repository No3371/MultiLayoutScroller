// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [CreateAssetMenu(menuName = "MultiLayoutScroller/FullScrollerSchema")]
    [System.Serializable]
    public class MultiLayoutScrollerSchemaSO : ScriptableObject
    {
        public MultiLayoutScrollerSchema schema;
        public MultiLayoutScrollerViewSchemaSO[] viewAssets;
        public string Comment;
    }
    
}