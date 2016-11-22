using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.Telephony;
using Newtonsoft.Json;

namespace AmeAndroid
{
    public interface IAlarmManager : IDisposable
    {
        event Action<AlarmState> AlarmStateChanged;
        AlarmState State { get; }
        void RefreshState();
        void Start();
        void Stop();
        BroadcastReceiver BroadcastReceiver { get; }
        DateTime StateUpdateTime { get; set; }
    }

    public class AlarmManager : IAlarmManager
    {
        public const string AlarmNiwyPhoneNumber = "+48503901587";
        private readonly ISmsSender smsSender;
        private readonly ISmsReceiver smsReceiver;
        public event Action<AlarmState> AlarmStateChanged;
        public AlarmState State { get; private set; } = AlarmState.Nieznany;
        public DateTime StateUpdateTime { get; set; } = DateTime.MinValue;
        HomeServerAlarmState alarmServer = new HomeServerAlarmState();

        public AlarmManager(ISmsSender smsSender)
        {
            this.smsSender = smsSender;
            this.smsReceiver = new SmsReceiver();
            smsReceiver.MessageReceived = ProcessMessage;
            RefreshState();
        }

        private bool ProcessMessage(SmsMessage sms)
        {
            if (sms.OriginatingAddress != AlarmNiwyPhoneNumber)
                return false;
            switch (sms.MessageBody)
            {
                case AlarmMessages.StanRozbrojony:
                    FireAlarmStateChanged(AlarmState.Rozbrojony, ConvertTime(sms.TimestampMillis));
                    return true;
                case AlarmMessages.StanUzbrojony:
                    FireAlarmStateChanged(AlarmState.Uzbrojony, ConvertTime(sms.TimestampMillis));
                    return true;
                case AlarmMessages.SterowanieRozbrojono:
                    FireAlarmStateChanged(AlarmState.Rozbrojony, ConvertTime(sms.TimestampMillis));
                    return true;
                case AlarmMessages.SterowanieUzbrojono:
                    FireAlarmStateChanged(AlarmState.Uzbrojony, ConvertTime(sms.TimestampMillis));
                    return true;
                default:
                    return false;
            }
        }

        private DateTime ConvertTime(long millis)
        {
            DateTime reference = new DateTime(1970, 1, 1, 0, 0, 0);
            return reference.AddMilliseconds(millis);
        }

        public void RefreshState()
        {
            FireAlarmStateChanged(AlarmState.Nieznany, DateTime.UtcNow);
            alarmServer.GetStateFromHomeServerAsync(FireAlarmStateChanged);
            smsSender.Send(AlarmNiwyPhoneNumber, AlarmCommands.Stan);
        }

        public void Start()
        {
            smsSender.Send(AlarmNiwyPhoneNumber, AlarmCommands.Start);
        }

        public void Stop()
        {
            smsSender.Send(AlarmNiwyPhoneNumber, AlarmCommands.Stop);
            smsSender.Send(AlarmNiwyPhoneNumber, AlarmCommands.Kasuj);
        }

        public BroadcastReceiver BroadcastReceiver => smsReceiver.BroadcastReceiver;


        protected virtual void FireAlarmStateChanged(AlarmState newState, DateTime updateTime)
        {
            if (updateTime < StateUpdateTime)
                return;
            State = newState;
            if (newState != AlarmState.Nieznany)
                alarmServer.UpdateHomeServerAlarmStateAsync(newState, updateTime);
            AlarmStateChanged?.Invoke(newState);
        }

        public void Dispose()
        {
            smsReceiver.MessageReceived = null;
        }
    }



    public enum AlarmState
    {
        Rozbrojony,
        Uzbrojony,
        Nieznany
    }

    public class AlarmMessages
    {
        public const string StanRozbrojony = "Stan systemu: Strefa  1:ok";
        public const string StanUzbrojony = "Stan systemu: Strefa  1:czuwa";
        public const string SterowanieSkasowano = "Sterowanie: skasowano alarm";
        public const string SterowanieRozbrojono = "Sterowanie: rozbrojono alarm";
        public const string SterowanieUzbrojono = "Sterowanie: uzbrojono alarm";
    }

    public class AlarmCommands
    {
        public const string Start = "Start";
        public const string Stop = "Stop";
        public const string Kasuj = "Kasuj";
        public const string Stan = "Stan";
    }
}