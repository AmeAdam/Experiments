using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmeAndroid
{
    public class HomeServerAlarmState
    {
        private readonly object sync = new object();
        public AlarmState AlarmState { get; private set; }
        public DateTime UpdateTime { get; private set; } = DateTime.MinValue;

        public void GetStateFromHomeServerAsync(Action<AlarmState, DateTime> callback)
        {
            Task.Factory.StartNew(() => GetStateFromHomeServer(callback));
        }

        public void GetStateFromHomeServer(Action<AlarmState, DateTime> callback)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString("http://baszynscy24.pl:8080/Alarm/GetAlarmState");
                    var alarmInfo = JsonConvert.DeserializeObject<AlarmInfo>(json);
                    lock (sync)
                    {
                        if (alarmInfo.UpdateTime <= UpdateTime)
                            return;
                        UpdateTime = alarmInfo.UpdateTime;
                        AlarmState = alarmInfo.Armed ? AlarmState.Uzbrojony : AlarmState.Rozbrojony;
                    }
                    callback?.Invoke(AlarmState, UpdateTime);
                }
            }
            catch (WebException)
            {
            }
        }

        public void UpdateHomeServerAlarmStateAsync(AlarmState state, DateTime datetime)
        {
            Task.Factory.StartNew(() => UpdateHomeServerAlarmState(state, datetime));
        }

        public void UpdateHomeServerAlarmState(AlarmState state, DateTime datetime)
        {
            try
            {
                lock (sync)
                {
                    if (datetime <= UpdateTime)
                        return;

                    using (WebClient wc = new WebClient())
                    {
                        wc.OpenRead("http://baszynscy24.pl:8080/Alarm/SetAlarmState?armed=" + (state == AlarmState.Uzbrojony)+ "&timeMillisUtc="+datetime.Ticks);
                    }
                    UpdateTime = datetime;
                    AlarmState = state;
                }
            }
            catch (WebException ex)
            {
            }
        }
    }

    public class AlarmInfo
    {
        public bool Armed { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}