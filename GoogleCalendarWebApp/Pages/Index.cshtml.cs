using GoogleCalendarService;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;


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
            _googleCalendar.ShowUpCommingEvent();
        }
    }
}
