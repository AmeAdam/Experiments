using AmeCommon.Tasks;
using Microsoft.AspNetCore.Html;

namespace AmeWeb.Model
{
    public class AmeHelper
    {
        public static IHtmlContent TaskStateImage(TaskState state, int height=20)
        {
            return new HtmlString($"<img height = '{height}' src = '/images/task-state/{state}.gif' />");
        }
    }
}
