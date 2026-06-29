using System.Linq;
using System.Text;
using System.Windows;
using TatooShop.Models;

namespace TatooShop.Services
{
    internal static class NotificationService
    {
        public const string ReservationReminderTitle = "Напоминание";
        public const string ReservationReminderEmailSubject = "Напоминание о записи в Ink & Skin";

        public static void AddNotification(User user, string title, string message)
        {
            if (user == null)
                return;

            var db = DataConnection.GetInstance();
            db.InternalNotifications.Add(new InternalNotification(user, title, message));
            db.SaveChanges();
        }

        public static string BuildReservationReminderMessage(Reservation reservation)
        {
            return $"Вы записаны в Ink & Skin на {reservation.Date:dd MMMM yyyy} в {Reservation.GetHour(reservation.Time)}.";
        }

        public static bool HasReservationReminder(Reservation reservation)
        {
            if (reservation?.User == null)
                return false;

            var message = BuildReservationReminderMessage(reservation);
            return DataConnection.GetInstance().InternalNotifications.Any(item =>
                item.User?.Id == reservation.User.Id &&
                item.Title == ReservationReminderTitle &&
                item.Message == message);
        }

        public static void ShowUnreadNotificationsForCurrentUser()
        {
            if (Manager.CurrentUser is not User currentUser)
                return;

            var db = DataConnection.GetInstance();
            var notifications = db.InternalNotifications
                .Where(item => item.User?.Id == currentUser.Id && !item.IsRead)
                .OrderBy(item => item.CreatedAt)
                .ToList();

            if (notifications.Count == 0)
                return;

            var builder = new StringBuilder();
            foreach (var notification in notifications)
            {
                builder.AppendLine(notification.Title);
                builder.AppendLine(notification.Message);
                builder.AppendLine();
                notification.IsRead = true;
            }

            db.SaveChanges();
            MessageBox.Show(builder.ToString().Trim(), "Уведомления");
        }
    }
}
