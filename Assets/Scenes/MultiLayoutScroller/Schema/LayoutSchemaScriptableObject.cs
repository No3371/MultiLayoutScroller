// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [CreateAssetMenu(menuName = "MultiLayoutScroller/ScrollerlayoutSchema")]
    [System.Serializable]
    public class LayoutSchemaScriptableObject : ScriptableObject
    {
        public LayoutSchema viewSchema;
    }
}