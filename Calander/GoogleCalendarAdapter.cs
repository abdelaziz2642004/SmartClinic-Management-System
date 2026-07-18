using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Clinic.Models;

namespace Clinic.Calendar
{
    public class GoogleCalendarAdapter : ICalendarAdapter
    {
        private readonly IConfiguration _config;

        public GoogleCalendarAdapter(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> CreateEventAsync(Appointment appointment)
        {
            var clientId = _config["GoogleCalendar:ClientId"];
            var clientSecret = _config["GoogleCalendar:ClientSecret"];

            // Skip sync entirely when Google credentials aren't configured yet.
            // Without this guard, Google.Apis tries to launch an OAuth flow with
            // an empty client_id, which opens the browser to Google's error page.
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                return string.Empty;
            }

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                new[] { CalendarService.Scope.Calendar },
                "user",
                CancellationToken.None
            );

            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "SmartClinic"
            });

            var startDateTime = appointment.AppointmentDate.Date.Add(appointment.AppointmentTime);

            var newEvent = new Event
            {
                Summary = $"Appointment #{appointment.AppointmentId}",
                Description = appointment.Message ?? "Clinic Appointment",
                Start = new EventDateTime
                {
                    DateTime = startDateTime,
                    TimeZone = "Africa/Cairo"
                },
                End = new EventDateTime
                {
                    DateTime = startDateTime.AddMinutes(30),
                    TimeZone = "Africa/Cairo"
                }
            };


            var createdEvent = await service.Events.Insert(newEvent, "primary").ExecuteAsync();
            return createdEvent.Id;
        }
    }
}