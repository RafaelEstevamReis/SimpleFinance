namespace Simple.Finance;

using System;

public class ManagerNotificationEventArgs : EventArgs
{
    public enum EventNotificationType
    {
        Wallet,
        Category,
        Person,
        Transaction,
    }
    public enum EventNotificationKind
    {
        New,
        Update,
    }

    public EventNotificationType Type { get; set; }
    public EventNotificationKind Kind { get; set; }
    public long Id { get; set; }
}
