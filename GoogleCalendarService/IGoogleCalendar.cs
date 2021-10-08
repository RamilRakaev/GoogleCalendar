using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleCalendarService
{
    public interface IGoogleCalendar
    {
        public static string[] Scopes;

        /// <summary>
        /// Создать событие
        /// </summary>
        /// <param name="summary">Заголовок события.</param>
        /// <param name="description">Описание.</param>
        /// <param name="start">Дата начала.</param>
        /// <param name="end">Дата завершения.</param>
        /// <param name="creatorName">Имя создателя.</param>
        /// <param name="creatorEmail">Почта создателя</param>
        /// <param name="attendees">Учатники проекта</param>
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
        public Task<string> CreateEvent(
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
            string location = "default");

        public Task<string> InsertEvent(Event calendarEvent);

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
        public Task<Event[]> GetEvents(
            DateTime? timeMin = null,
            DateTime? timeMax = null,
            int maxResults = 100,
            bool showDeleted = false,
            bool singleEvents = true,
            bool showHiddenInvitations = false,
            string q = null,
            bool sortByModifiedDate = false);

        public Task<string> ShowUpCommingEvents();
    }
}
