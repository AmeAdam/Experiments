using System.Collections.Generic;
using AmeCommon.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AmeWeb.Model
{
    public class CardInfoViewModel
    {
        public Device Device { get; set; }
        public string InsertedInDrive { get; set; }

        public static List<SelectListItem> AvaliableCommands = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Wybierz polecenie"},
            new SelectListItem { Value = "move-directory-content", Text = "Przenieś katalogi"},
            new SelectListItem { Value = "move-directory-content-flat", Text = "Przenieś wszystkie pliki"},
            new SelectListItem { Value = "move-files", Text = "Przenieś pliki"},
        };
    }
}
