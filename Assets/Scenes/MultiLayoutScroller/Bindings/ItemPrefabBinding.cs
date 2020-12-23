using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [System.Serializable]
    public struct ItemPrefabBinding
    {
        public int prefabIndex;
        public GameObject prefab;
        public int initPoolSize;
    }
}