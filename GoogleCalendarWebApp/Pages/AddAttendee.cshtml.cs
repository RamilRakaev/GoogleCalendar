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
    public class AddAttendeeModel : PageModel
    {

        readonly private IGoogleCalendar _googleCalendar;

        public AddAttendeeModel(IGoogleCalendar googleCalendar)
        {
            _googleCalendar = googleCalendar;
        }

        public void OnGet()
        {
            Event = JsonSerializer.Deserialize<Event>(TempData["newEvent"].ToString());
            TempData["newEvent"] = JsonSerializer.Serialize(Event);
        }

        public void OnPostAddAttendee(EventAttendee attendee)
        {
            AddAttendee(attendee);
        }

        public IActionResult OnPostInsertEvent(EventAttendee attendee)
        {
            AddAttendee(attendee);
            _googleCalendar.InsertEvent(Event);
            return RedirectToPage("ShowCalendarEvents");
        }

        private void AddAttendee(EventAttendee attendee)
        {
            Event = JsonSerializer.Deserialize<Event>(TempData["newEvent"].ToString());
            if (Event.Attendees != null)
                Event.Attendees.Add(attendee);
            else
                Event.Attendees = new List<EventAttendee>() { attendee };
            TempData["newEvent"] = JsonSerializer.Serialize(Event);
        }

        public Event Event { get; set; }
    }
}
