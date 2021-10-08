using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

        public IActionResult OnPostInsertEvent(Event newEvent)
        {
            _googleCalendar.InsertEvent(newEvent);
            return RedirectToPage("ShowCalendarEvents");
        }

        public IActionResult OnPostAddAttendee(Event newEvent)
        {
            TempData["newEvent"] = JsonSerializer.Serialize(newEvent);
            return RedirectToPage("AddAttendee");
        }
    }
}
