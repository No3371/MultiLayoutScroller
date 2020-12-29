// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [CreateAssetMenu(menuName = "MultiLayoutScroller/FullScrollerSchema")]
    [System.Serializable]
    public class ScrollerSchemaScriptableObject : ScriptableObject
    {
        public ScrollerSchema schema;
        public ViewSchemaScriptableObject[] viewAssets;
        public string Comment;
    }
    
}