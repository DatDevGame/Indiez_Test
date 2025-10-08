namespace Premium.PoolManagement
{
    [EventCode]
    public enum PoolManagementEventCode
    {
        OnCreatePoolItem,
        OnTakePoolItem,
        OnReturnPoolItem,
        OnDestroyPoolItem
    }
}