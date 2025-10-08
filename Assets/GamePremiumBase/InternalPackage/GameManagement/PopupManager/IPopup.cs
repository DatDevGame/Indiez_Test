namespace Premium.PopupManagement
{
    public interface IPopup : IUIVisibilityController
    {
        int GetPriorityOrder();
    }
}