using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3.Data;
using GoogleCalendarService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoogleCalendarWebApp.Pages
{
    public class ShowCalendarEventsModel : PageModel
    {
        readonly private IGoogleCalendar _googleCalendar;

        public ShowCalendarEventsModel(IGoogleCalendar googleCalendar)
        {
            _googleCalendar = googleCalendar;
        }

        public void OnGet()
        {
            Events = _googleCalendar.GetEvents();
        }

        public Event[] Events { get; set; }
    }
}
