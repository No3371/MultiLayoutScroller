// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [RequireComponent(typeof(LayoutInstance))]
    public class LayoutAuthoring : MonoBehaviour
    {
        public bool autoBakeOnLoad, destoryBakedPlaceHolders, destroyOnBaked;
        public RectTransform[] slots;
        void Start ()
        {
            if (autoBakeOnLoad)
            {
                Bake();
            }
        }

        [ContextMenu("Bake slots")]
        void Bake ()
        {
            if (slots == null || slots.Length == 0) return;
            LayoutInstance li = this.GetComponent<LayoutInstance>();
            li.slotsBaked = new RectTransformData[slots.Length];
            for (var i = 0; i < this.transform.childCount; i++)
            {
                li.slotsBaked[i] = new RectTransformData((RectTransform) slots[i].transform);
                if (destoryBakedPlaceHolders) GameObject.Destroy(slots[i].gameObject);
            }
            if (Application.isPlaying && destroyOnBaked) Destroy(this);
        }

    }

}