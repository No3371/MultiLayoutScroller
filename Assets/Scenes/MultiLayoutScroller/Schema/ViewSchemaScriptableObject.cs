// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [CreateAssetMenu(menuName = "MultiLayoutScroller/ViewSchema")]
    [System.Serializable]
    public class ViewSchemaScriptableObject : ScriptableObject
    {
        public ViewSchema viewSchema;
        public LayoutSchemaScriptableObject[] layoutAssets;
    }
}