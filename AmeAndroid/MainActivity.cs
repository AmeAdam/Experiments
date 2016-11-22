using System;
using System.Net;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Views;
using Microsoft.Practices.Unity;

namespace AmeAndroid
{
    [Activity(Label = "Niwy Alarm", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private IAlarmManager alarmManager;
        private TextView stateText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            IoC.Initialize();

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            Button startButton = FindViewById<Button>(Resource.Id.StartButton);
            startButton.Click += SendStartAlarmCommand;
            Button stopButton = FindViewById<Button>(Resource.Id.StopButton);
            stopButton.Click += SendStopAlarmCommand;
            Button stanButton = FindViewById<Button>(Resource.Id.StanButton);
            stanButton.Click += SendStateAlarmCommand;
            stateText = FindViewById<TextView>(Resource.Id.AlarmStateText);

//            var web = new WebClient();
//            var txt = web.DownloadString("http://baszynscy24.pl:8080/Sms/GetMyMessages");
//            stateText.Text = txt;
        }

        protected override void OnResume()
        {
            base.OnResume();
            alarmManager = IoC.Container.Resolve<IAlarmManager>();
            RegisterReceiver(alarmManager.BroadcastReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED") {Priority = 99999999 });
            DisplayAlarmState(alarmManager.State);
            alarmManager.AlarmStateChanged += DisplayAlarmState;
        }

        private void DisplayAlarmState(AlarmState alarmState)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    if (alarmState == AlarmState.Nieznany)
                        FindViewById<RelativeLayout>(Resource.Id.loadingPanel).Visibility = ViewStates.Visible;
                    else
                        FindViewById<RelativeLayout>(Resource.Id.loadingPanel).Visibility = ViewStates.Gone;
                    stateText.Text = "Stan alarmu:: " + alarmState;
                }
                catch (Exception)
                {

                }
            });
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(alarmManager.BroadcastReceiver);
            alarmManager.AlarmStateChanged -= DisplayAlarmState;
        }

        private void SendStateAlarmCommand(object sender, EventArgs e)
        {
            alarmManager.RefreshState();
        }

        private void SendStopAlarmCommand(object sender, EventArgs e)
        {
            alarmManager.Stop();
        }

        private void SendStartAlarmCommand(object sender, EventArgs e)
        {
            alarmManager.Start();
        }
    }
}

