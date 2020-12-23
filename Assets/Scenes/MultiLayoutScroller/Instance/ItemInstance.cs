// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    public abstract class ItemInstance : MonoBehaviour
    {
        
        public RectTransform RectTransform=> (this.transform as RectTransform);
        public abstract void SetData (object data);
        protected CanvasGroup canvasGroup;
        internal CanvasGroup CanvasGroup { get => canvasGroup?? (canvasGroup = this.GetComponent<CanvasGroup>()); }
        public ItemTypeIDPair schemaCache;
        /// <summary>
        /// Notify that the scroller has loaded this item.
        /// At the moment, the canvas group alpha is set to 1.
        /// </summary>
        internal virtual void OnLoaded () { }
        /// <summary>
        /// Notify that the scroller has pooled this item.
        /// At the moment, the canvas group alpha is set to 0.
        /// </summary>
        internal virtual void OnPooled () { }
    }

}