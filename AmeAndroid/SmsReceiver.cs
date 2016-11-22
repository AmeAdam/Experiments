using System;
using Android.Content;
using Android.Provider;
using Android.Telephony;
using Android.Widget;

namespace AmeAndroid
{
//    [BroadcastReceiver(Enabled = true, Exported = true, Label = "SMS Receiver")]
//   [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = (int)IntentFilterPriority.HighPriority)]
    public class SmsReceiver : BroadcastReceiver, ISmsReceiver
    {
        public static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";
        public Func<SmsMessage, bool> MessageReceived { get; set; }

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (intent.Action != IntentAction)
                    return;

                var bundle = intent.Extras;
                if (bundle == null) return;

                foreach (SmsMessage message in Telephony.Sms.Intents.GetMessagesFromIntent(intent))
                {
                    var messageProcessed = MessageReceived?.Invoke(message) ?? false;
                    if (messageProcessed)
                        InvokeAbortBroadcast();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
            }
        }

        public BroadcastReceiver BroadcastReceiver => this;
    }

    public interface ISmsReceiver
    {
        Func<SmsMessage, bool> MessageReceived { get; set; }
        BroadcastReceiver BroadcastReceiver { get; }
    }
}