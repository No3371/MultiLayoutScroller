// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using UnityEngine;
using UnityEngine.Events;

namespace BAStudio.MultiLayoutScroller
{
    public abstract class ItemInstance : MonoBehaviour
    {
        
        public RectTransform RectTransform=> (this.transform as RectTransform);
        public abstract void SetData (object data);
        protected CanvasGroup canvasGroup;
        internal CanvasGroup CanvasGroup { get => canvasGroup?? (canvasGroup = this.GetComponent<CanvasGroup>()); }
        internal ItemTypeIDPair schemaCache;
        public UnityEvent onLoadedCallbacks, onPooledCallbacks;
        /// <summary>
        /// Notify that the scroller has loaded this item.
        /// At the moment, the canvas group alpha is set to 1.
        /// </summary>
        internal protected virtual void OnLoaded ()
        {
            onLoadedCallbacks?.Invoke();
        }
        /// <summary>
        /// Notify that the scroller has pooled this item.
        /// At the moment, the canvas group alpha is set to 0.
        /// </summary>
        internal protected virtual void OnPooled ()
        {
            onPooledCallbacks?.Invoke();
        }

        internal protected virtual void OnAssigned (int type, LayoutInstance li) {}
    }

}