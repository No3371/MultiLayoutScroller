using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [System.Serializable]
    public struct LayoutPrefabBinding
    {
        public int id;
        public GameObject gameObject;
        public LayoutTypeMeta meta;
    }

}