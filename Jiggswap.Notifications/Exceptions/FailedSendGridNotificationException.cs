using System;

namespace Jiggswap.Notifications.Exceptions
{
    public class FailedSendGridNotificationException : Exception
    {
        public FailedSendGridNotificationException()
        {
        }

        public FailedSendGridNotificationException(string message) : base(message)
        {
        }

        public FailedSendGridNotificationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}