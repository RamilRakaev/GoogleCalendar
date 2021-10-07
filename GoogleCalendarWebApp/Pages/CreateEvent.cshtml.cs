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
    public class CreateEventModel : PageModel
    {
        readonly private IGoogleCalendar _googleCalendar;

        public CreateEventModel(IGoogleCalendar googleCalendar)
        {
            _googleCalendar = googleCalendar;
        }

        public void OnGet()
        {
        }

        public void OnPost(Event newEvent)
        {
            _googleCalendar.InsertEvent(newEvent);
        }
    }
}
