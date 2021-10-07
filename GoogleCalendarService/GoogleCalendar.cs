using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace GoogleCalendarService
{
    public class GoogleCalendar
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

        /// <summary>
        /// Создать событие
        /// </summary>
        /// <param name="summary">Заголовок события.</param>
        /// <param name="description">Описание.</param>
        /// <param name="start">Дата начала.</param>
        /// <param name="end">Дата завершения.</param>
        /// <param name="creatorName">Имя создателя.</param>
        /// <param name="creatorEmail">Почта создателя</param>
        /// <param name="eventType">Тип мероприятия. Возможные значения: default, 
        /// outOfOffice - вне офиса.</param>
        /// <param name="status">Статус. Возможные значения: confirmed - подтверждено, 
        /// tentative - предварительно подтверждено, cancelled - отменено </param>
        /// <param name="visibility">Видимость. Возможные значения: default - видимость 
        /// по умолчанию для событий в календаре, public - доступно для всех 
        /// пользователей календаря, private - доступно участникам мероприятия, 
        /// confidential - конфиденциально</param>
        /// <param name="guestsCanInviteOthers">Гости могут приглашать других.</param>
        /// <param name="guestsCanModify">Гости могут изменять событие.</param>
        /// <param name="location">Место проведение мероприятия.</param>
        /// <returns></returns>
        public string CreateEvent(
            string summary,
            string description,
            DateTime start,
            DateTime end,
            string creatorName = null,
            string creatorEmail = null,
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
                Location = location,
                Creator = new Event.CreatorData()
                {
                    DisplayName = creatorName,
                    Email = creatorEmail
                }
            };
            return InsertEvent(calendarEvent);
        }

        private UserCredential GetCredential(UserRole userRole)
        {
            UserCredential credential;
            if (File.Exists(_options.CredentialsPath) == false)
                throw new Exception("Credential file not found");
            using (var stream =
                new FileStream(_options.CredentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                userRole.ToString(),
                CancellationToken.None,
                new FileDataStore(_options.FolderForToken, true)).Result;
            }
            return credential;
        }

        private CalendarService GetService(UserCredential credential)
        {
            BaseClientService.Initializer initializer = new BaseClientService.Initializer();
            initializer.HttpClientInitializer = credential;
            initializer.ApplicationName = _options.ApplicationName;
            return new CalendarService(initializer);
        }

        public string InsertEvent(Event calendarEvent)
        {
            UserCredential credential = this.GetCredential(UserRole.Admin);
            calendarEvent = GetService(credential).Events.Insert(calendarEvent, _options.CalendarId).Execute();
            return calendarEvent.HtmlLink;
        }

        /// <summary>
        /// Вернуть мероприятия
        /// </summary>
        /// <param name="timeMin">Нижнаяя граница даты окончания события</param>
        /// <param name="timeMax">Верняя граница даты начала события</param>
        /// <param name="maxResults">Максимальное количество возвращаемых событий</param>
        /// <param name="showDeleted">Вернуть удалённые события</param>
        /// <param name="singleEvents">Вернуть повторяющиеся события</param>
        /// <param name="showHiddenInvitations">Показать скрытые события</param>
        /// <param name="q">Произвольный текстовый поиск событий по заданной строке</param>
        /// <param name="sortByModifiedDate">Сортировать по дате обновления</param>
        /// <returns></returns>
        public Event[] GetEvents(
            DateTime? timeMin = null,
            DateTime? timeMax = null,
            int maxResults = 100,
            bool showDeleted = false,
            bool singleEvents = true,
            bool showHiddenInvitations = false,
            string q = null,
            bool sortByModifiedDate = false)
        {
            UserCredential credential = GetCredential(UserRole.User);

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

        public string ShowUpCommingEvents()
        {
            string output = "";
            var events = GetEvents();
            if (events.Length > 0)
            {
                foreach (var eventItem in GetEvents())
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
    }
}
