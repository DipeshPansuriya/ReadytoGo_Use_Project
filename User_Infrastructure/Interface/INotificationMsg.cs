namespace User_Infrastructure.Interface
{
    public interface INotificationMsg
    {
        void SaveMailNotification(string From, string To, string Subject, string Body);

        Task<bool> SendAsync(NotficationCls notfication);
    }
}