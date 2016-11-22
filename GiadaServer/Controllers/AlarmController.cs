using System;
using GiadaServer.Model;
using Microsoft.AspNetCore.Mvc;

namespace GiadaServer.Controllers
{
    public class AlarmController : Controller
    {
        private readonly IAlarmProvider alarmProvider;

        public AlarmController(IAlarmProvider alarmProvider)
        {
            this.alarmProvider = alarmProvider;
        }

        public IActionResult Index()
        {
            return View(alarmProvider.GetAlarmState());
        }

        public IActionResult GetAlarmState()
        {
            return Json(alarmProvider.GetAlarmState());
        }

        public IActionResult SetAlarmState(bool armed, long timeMillisUtc)
        {
            alarmProvider.UpdateAlarmState(armed, new DateTime(timeMillisUtc, DateTimeKind.Utc));
            return new EmptyResult();
        }
    }
}
