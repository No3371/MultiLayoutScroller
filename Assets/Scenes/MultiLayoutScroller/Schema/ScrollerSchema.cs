// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System.Collections.Generic;

namespace BAStudio.MultiLayoutScroller
{
    // [
    // {
    //     "View": "Classic",
    //     "Layouts": [
    //     {
    //         "Layout": "SinglePortrait",
    //         "Items": [
    //         {
    //             "NameIndex": 0,
    //             "GoldTierEnabled": true,
    //             "SilverTierEnabled": false,
    //             "RibbonIndex": 2,
    //             "ImageIndex": 0
    //         }
    //         ]
    //     }
    //     ]
    // }
    // ]
    [System.Serializable]
    public class ScrollerSchema
    {
        [UnityEngine.SerializeField] private List<ViewSchema> views;

        public List<ViewSchema> Views { get => views = views?? new List<ViewSchema>(); }
    }

}