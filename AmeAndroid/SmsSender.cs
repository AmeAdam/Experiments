using Android.Telephony;

namespace AmeAndroid
{
    public interface ISmsSender
    {
        void Send(string destinationAddress, string message);
    }

    public class SmsSender : ISmsSender
    {
        public void Send(string destinationAddress, string message)
        {
            SmsManager.Default.SendTextMessage(destinationAddress, null, message, null, null);
        }
    }
}