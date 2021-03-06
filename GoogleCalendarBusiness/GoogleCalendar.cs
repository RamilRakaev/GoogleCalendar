using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleCalendarService;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCalendarBusiness
{
    public class GoogleCalendar : IGoogleCalendar
    {
        public static string[] Scopes;

        private readonly GoogleCalendarOptions _options;

        static GoogleCalendar()
        {
            Scopes = new string[] { CalendarService.Scope.Calendar };
        }

        public GoogleCalendar(IOptions<GoogleCalendarOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> CreateEvent(
            string summary,
            string description,
            DateTime start,
            DateTime end,
            string creatorName = null,
            string creatorEmail = null,
            List<EventAttendee> attendees = null,
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
                Creator = new Event.CreatorData()
                {
                    DisplayName = creatorName,
                    Email = creatorEmail
                },
                Attendees = attendees,
                EventType = eventType,
                Status = status,
                Visibility = visibility,
                GuestsCanInviteOthers = guestsCanInviteOthers,
                GuestsCanModify = guestsCanModify,
                Location = location
            };
            return await InsertEvent(calendarEvent);
        }

        public async Task<string> InsertEvent(Event calendarEvent)
        {
            UserCredential credential = await GetCredential(UserRole.Admin);
            calendarEvent = GetService(credential).Events.Insert(calendarEvent, _options.CalendarId).Execute();
            return calendarEvent.HtmlLink;
        }

        public async Task<string> ShowUpCommingEvents()
        {
            string output = "";
            var events = await GetEvents();
            if (events.Length > 0)
            {
                foreach (var eventItem in events)
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
                output = "Нет запланированных событий";
            }
            return output;
        }

        public async Task<Event[]> GetEvents(
            DateTime? timeMin = null,
            DateTime? timeMax = null,
            int maxResults = 100,
            bool showDeleted = false,
            bool singleEvents = true,
            bool showHiddenInvitations = false,
            string q = null,
            bool sortByModifiedDate = false)
        {
            UserCredential credential = await GetCredential(UserRole.User);

            // Creat Google Calendar API service.
            CalendarService service = GetService(credential);

            // Define parameters of request
            EventsResource.ListRequest request = service.Events.List(_options.CalendarId);
            request.TimeMin = timeMin == null ? DateTime.Now : timeMin;
            request.TimeMax = timeMax;
            request.ShowDeleted = showDeleted;
            request.SingleEvents = singleEvents;
            request.ShowHiddenInvitations = showHiddenInvitations;
            request.MaxResults = maxResults;
            request.OrderBy = sortByModifiedDate ? EventsResource.ListRequest.OrderByEnum.Updated : EventsResource.ListRequest.OrderByEnum.StartTime;
            request.Q = q;
            Events events = request.Execute();
            return events.Items != null && events.Items.Count > 0 ? events.Items.ToArray() : new Event[0];
        }

        private async Task<UserCredential> GetCredential(UserRole userRole)
        {
            UserCredential credential;
            if (File.Exists(_options.CredentialsPath) == false)
                throw new Exception("Credential file not found");
            using (var stream =
                new FileStream(_options.CredentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                userRole.ToString(),
                CancellationToken.None,
                new FileDataStore(_options.FolderForToken, true));
            }
            return credential;
        }

        private CalendarService GetService(UserCredential credential)
        {
            BaseClientService.Initializer initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _options.ApplicationName
            };
            return new CalendarService(initializer);
        }
    }
}
