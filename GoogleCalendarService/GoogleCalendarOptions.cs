namespace GoogleCalendarService
{
    public class GoogleCalendarOptions
    {
        public string ApplicationName { get; set; }

        public string CalendarId { get; set; }

        public string CredentialsPath { get; set; } = string.Empty;

        public string FolderForToken { get; set; }
    }
}
