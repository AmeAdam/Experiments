using AmeCommon.Alarm;
using Microsoft.AspNetCore.Mvc;

namespace GiadaServer.Controllers
{
    public class SmsController : Controller
    {
        private readonly IAlarmManager alarmManager;

        public SmsController(IAlarmManager alarmManager)
        {
            this.alarmManager = alarmManager;
        }

        public IActionResult Index()
        {
            return View(alarmManager.GetPendingMessages());
        }

        public IActionResult SendMessage(Message message)
        {
            alarmManager.SendMessage(message);
            return RedirectToAction(nameof(Index));
        }

        public string GetMyMessages(string number)
        {
            return "abc";
        }
    }
}
