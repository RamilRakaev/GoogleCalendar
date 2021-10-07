using GoogleCalendarService;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;

namespace GoogleCalendarWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GoogleCalendar _googleCalendar;

        public IndexModel(ILogger<IndexModel> logger, GoogleCalendar googleCalendar)
        {
            _logger = logger;
            _googleCalendar = googleCalendar;
        }

        public void OnGet()
        {
            var result = _googleCalendar.ShowUpCommingEvent();
            var rs = _googleCalendar.CreateEvent("1", "1", DateTime.Now, new DateTime(2022, 10, 10));
            result = _googleCalendar.ShowUpCommingEvent();
        }
    }
}
