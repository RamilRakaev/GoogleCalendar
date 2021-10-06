namespace GoogleCalendarService
{
    public class GoogleCalendarOptions
    {
        public const string GoogleCalendar = "GoogleCalendarOptions";

        public string ApplicationName { get; set; }

        public string CalendarId { get; set; }

        public string CredentialsPath { get; set; }

        public string FolderForToken { get; set; }
    }
}
