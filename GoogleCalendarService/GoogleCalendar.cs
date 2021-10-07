using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;

namespace GoogleCalendarService
{
    public class GoogleCalendar
    {
        public static string[] Scopes;
        private readonly GoogleCalendarOptions _options;
        private readonly string _credentialsPath = string.Empty;

        static GoogleCalendar()
        {
            string[] textArray1 = new string[] { CalendarService.Scope.Calendar };
            Scopes = textArray1;
        }

        public GoogleCalendar(IOptions<GoogleCalendarOptions> options)
        {
            _options = options.Value;
            CalendarId = _options.CalendarId;
            ApplicationName = _options.ApplicationName;
            _credentialsPath = _options.CredentialsPath;
            FolderForToken = _options.FolderForToken;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="description"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="eventType"></param>
        /// <param name="status"></param>
        /// <param name="visibility"></param>
        /// <param name="guestsCanInviteOthers"></param>
        /// <param name="guestsCanModify"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public string CreateEvent(
            string summary,
            string description,
            DateTime start,
            DateTime end,
            string eventType = "default",
            string status = "confirmed",
            string visibility = "default",
            bool guestsCanInviteOthers = true,
            bool guestsCanModify = false,
            string location = "default")
        {
            Event calendarEvent = new Event
            {
                Summary = summary,
                Description = description,
                Start = new EventDateTime() { DateTime = start },
                End = new EventDateTime() { DateTime = end },
                EventType = eventType,
                Status = status,
                Visibility = visibility,
                GuestsCanInviteOthers = guestsCanInviteOthers,
                GuestsCanModify = guestsCanModify,
                Location = location
            };
            return InsertEvent(calendarEvent);
        }

        private UserCredential GetCredential(UserRole userRole)
        {
            UserCredential credential;
            using (var stream =
                new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                userRole.ToString(),
                CancellationToken.None,
                new FileDataStore(FolderForToken, true)).Result;
            }

            return credential;
        }

        private CalendarService GetService(UserCredential credential)
        {
            BaseClientService.Initializer initializer = new BaseClientService.Initializer();
            initializer.HttpClientInitializer = credential;
            initializer.ApplicationName = ApplicationName;
            return new CalendarService(initializer);
        }

        public string InsertEvent(Event calendarEvent)
        {
            UserCredential credential = this.GetCredential(UserRole.Admin);
            calendarEvent = GetService(credential).Events.Insert(calendarEvent, this.CalendarId).Execute();
            return calendarEvent.HtmlLink;
        }

        public string ShowUpCommingEvent()
        {
            string output = "";
            UserCredential credential = GetCredential(UserRole.User);

            // Creat Google Calendar API service.
            CalendarService service = GetService(credential);

            // Define parameters of request
            EventsResource.ListRequest request = service.Events.List(CalendarId);
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 100;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();

            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (string.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    output += string.Concat("{0} ({1})\n", eventItem.Summary, when);
                }
            }
            else
            {
                output = "Empty";
            }
            return output;
        }

        public string ApplicationName { get; private set; }

        public string CalendarId { get; private set; }

        public string FolderForToken { get; private set; }
    }
}
