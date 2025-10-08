namespace Premium.PopupManagement
{
    public class PromotionalPopup<T> : Popup where T : PromotionalPopupSO
    {
        public virtual T GetPromotionalPopupSO()
        {
            return m_PopupSO as T;
        }
    }
}