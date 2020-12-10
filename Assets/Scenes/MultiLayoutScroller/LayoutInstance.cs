// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    public class LayoutInstance : MonoBehaviour
    {
        public RectTransform RectTransform=> (this.transform as RectTransform);
        [SerializeField] internal RectTransformData[] slotsBaked;
        internal ItemInstance[] items;
        internal void Assign(int slotIndex, ItemInstance item)
        {
            item.transform.SetParent(RectTransform);
            slotsBaked[slotIndex].Overwrite((RectTransform) item.transform);
        }
        public bool CullByItem; // Should the scroller scan through and try to load/unload items 
    }

    [System.Serializable]
    struct RectTransformData
    {
        public Vector2 anchorMin, anchorMax, anchoredPosition, sizeDelta;
        public RectTransformData (RectTransform source)
        {
            this.anchorMin = source.anchorMin;
            this.anchorMax = source.anchorMax;
            this.anchoredPosition = source.anchoredPosition;
            this.sizeDelta = source.sizeDelta;
        }
        
        public void Overwrite (RectTransform dest)
        {
            dest.anchorMin = this.anchorMin;
            dest.anchorMax = this.anchorMax;
            dest.anchoredPosition = this.anchoredPosition;
            dest.sizeDelta = this.sizeDelta;
        }
    }
    // LayoutName: { 0, 1, 4, 2, 11 ,155}  => The layout pulls 

    // MultiLayoutScroller
    // - View (Where scrolling happening)
    //   - Layout (A set of predefined item positon&dimension)
    //     - Item ()
    
    // View
    // View controls how layouts are placed, moved, culled

    // Layouts
    // A layout is just a set of predefined slots, items get place under the slot transforms
    // It does not control it's own placement

    // Item
    // Prefab driving data

}