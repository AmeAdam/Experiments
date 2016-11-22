using Microsoft.Practices.Unity;

namespace AmeAndroid
{
    public static class IoC
    {
        public static UnityContainer Container { get; set; }

        public static void Initialize()
        {
            Container = new UnityContainer();
            Container.RegisterType<ISmsSender, SmsSender>();
            Container.RegisterType<ISmsReceiver, SmsReceiver>();
            Container.RegisterType<IAlarmManager, AlarmManager>();
        }

        public static void Dispose()
        {
            Container.Dispose();
        }
    }
}