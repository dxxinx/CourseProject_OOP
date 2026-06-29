using System;
using System.Windows;
using System.Windows.Input;
using TatooShop.Models;
using TatooShop.Services;

namespace TatooShop.Infrastructure.Commands
{
    public class ReminderCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            return parameter is Reservation reservation &&
                   reservation.Status != ReservationStatus.Visited &&
                   !NotificationService.HasReservationReminder(reservation);
        }

        public override void Execute(object parameter)
        {
            if (parameter is Reservation reservation)
            {
                if (reservation.User == null)
                {
                    MessageBox.Show("Не удалось отправить напоминание: у записи не указан пользователь.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(reservation.User.EMail))
                {
                    MessageBox.Show("Не удалось отправить напоминание: у пользователя не указана почта.");
                    return;
                }

                if (reservation.Master == null)
                {
                    MessageBox.Show("Не удалось отправить напоминание: у записи не указан мастер.");
                    return;
                }

                if (reservation.Status == ReservationStatus.Visited)
                {
                    MessageBox.Show("Нельзя отправить напоминание по записи со статусом \"Посещена\".");
                    return;
                }

                if (NotificationService.HasReservationReminder(reservation))
                {
                    MessageBox.Show("Напоминание по этой записи уже отправлено.");
                    return;
                }

                var reminderMessage = NotificationService.BuildReservationReminderMessage(reservation);
                var emailBody = BuildEmailBody(reservation, reminderMessage);
                var userEmail = reservation.User.EMail.Trim();

                if (!EmailMessage.TrySendMail(
                        "Tattoo Shop",
                        userEmail,
                        string.Empty,
                        NotificationService.ReservationReminderEmailSubject,
                        emailBody,
                        out var errorMessage))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(EmailMessage.SalonCopyAddress) &&
                    !string.Equals(EmailMessage.SalonCopyAddress, userEmail, StringComparison.OrdinalIgnoreCase))
                {
                    EmailMessage.TrySendMail(
                        "Tattoo Shop",
                        EmailMessage.SalonCopyAddress,
                        string.Empty,
                        $"{NotificationService.ReservationReminderEmailSubject}: {userEmail}",
                        BuildSalonEmailBody(reservation, reminderMessage, userEmail),
                        out _);
                }

                NotificationService.AddNotification(
                    reservation.User,
                    NotificationService.ReservationReminderTitle,
                    reminderMessage);

                MessageBox.Show($"Напоминание отправлено клиенту: {userEmail}\nКопия отправлена на почту салона.");
                CommandManager.InvalidateRequerySuggested();
            }
            else
            {
                MessageBox.Show("Не удалось отправить напоминание.");
            }
        }

        private static string BuildEmailBody(Reservation reservation, string reminderMessage)
        {
            return $"""
Здравствуйте, {reservation.User.Name}!

Напоминаем о вашей записи в тату-салон Ink & Skin.

Дата и время: {reservation.Date:dd.MM.yyyy}, {Reservation.GetHour(reservation.Time)}
Мастер: {reservation.Master.FullName}

Если у вас изменились планы, пожалуйста, свяжитесь с салоном заранее.

С уважением,
команда Ink & Skin
""";
        }

        private static string BuildSalonEmailBody(Reservation reservation, string reminderMessage, string userEmail)
        {
            return $"""
Копия напоминания клиенту.

Клиент: {reservation.User.FullName}
Email клиента: {userEmail}

{reminderMessage}
Мастер: {reservation.Master.FullName}
""";
        }
    }
}
