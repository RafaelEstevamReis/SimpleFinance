namespace Simple.Finance;

using System;

public class ManagerNotificationEventArgs : EventArgs
{
    public enum EventNotificationItem
    {
        Wallet,
        Category,
        Person,
        Transaction,
    }
    public enum EventNotificationAction
    {
        New,
        Update,
    }

    public EventNotificationItem Item { get; set; }
    public EventNotificationAction Action { get; set; }
    public long Id { get; set; }
}
