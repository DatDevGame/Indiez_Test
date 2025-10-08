using UnityEngine;

namespace Premium.PopupManagement
{
    public class Popup : ComposeCanvasElementVisibilityController, IPopup
    {
        [SerializeField]
        protected Canvas m_RootCanvas;
        [SerializeField]
        protected PopupSO m_PopupSO;

        protected virtual void Start()
        {
            if (m_PopupSO.overrideCanvasSortingOrder)
            {
                if (m_RootCanvas == null)
                    m_RootCanvas = GetComponentInChildren<Canvas>().rootCanvas;
                if (m_RootCanvas != null)
                {
                    m_RootCanvas.overrideSorting = true;
                    m_RootCanvas.sortingOrder = GetPriorityOrder();
                }
            }
        }

        public virtual int GetPriorityOrder()
        {
            return m_PopupSO.GetPriorityOrder();
        }
    }
}