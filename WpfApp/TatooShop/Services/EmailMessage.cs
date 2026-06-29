using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace TatooShop.Services
{
    internal class EmailMessage
    {
        private static readonly string address;
        private static readonly string userName;
        private static readonly string password;
        private static readonly string cfgHost;
        private static readonly int cfgPort;
        private static readonly string salonCopyAddress;

        public static bool IsAvailable { get; }
        public static string SalonCopyAddress => salonCopyAddress;

        static EmailMessage()
        {
            try
            {
                address = (ConfigurationManager.AppSettings["SMTPAddress"] ?? string.Empty).Trim();
                userName = (ConfigurationManager.AppSettings["SMTPUsername"] ?? address).Trim();
                cfgHost = (ConfigurationManager.AppSettings["SMTPHost"] ?? string.Empty).Trim();
                cfgPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"] ?? "0");
                password = NormalizePassword(ConfigurationManager.AppSettings["SMTPPassword"] ?? string.Empty);
                salonCopyAddress = (ConfigurationManager.AppSettings["SalonCopyEmail"] ?? address).Trim();

                IsAvailable =
                    !string.IsNullOrWhiteSpace(address) &&
                    !string.IsNullOrWhiteSpace(userName) &&
                    !string.IsNullOrWhiteSpace(cfgHost) &&
                    cfgPort > 0 &&
                    password.Length > 0;
            }
            catch
            {
                address = string.Empty;
                userName = string.Empty;
                cfgHost = string.Empty;
                cfgPort = 0;
                password = string.Empty;
                salonCopyAddress = string.Empty;
                IsAvailable = false;
            }
        }

        public static bool SendMail(string fromName, string toMail, string toName, string subject, string body) =>
            TrySendMail(fromName, toMail, toName, subject, body, out _);

        public static bool TrySendMail(string fromName, string toMail, string toName, string subject, string body, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!IsAvailable)
            {
                errorMessage = "Почтовый сервис не настроен.";
                return false;
            }

            try
            {
                var from = new MailAddress(address, fromName, Encoding.UTF8);
                var to = new MailAddress(toMail.Trim());

                using var mailMessage = new MailMessage(from, to);

                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = false;
                mailMessage.SubjectEncoding = Encoding.UTF8;
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.HeadersEncoding = Encoding.UTF8;
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                using var smtp = CreateSmtpClient();
                smtp.Send(mailMessage);
                return true;
            }
            catch (FormatException)
            {
                errorMessage = "Письмо не отправлено: у клиента указан некорректный email.";
                return false;
            }
            catch (SmtpException ex)
            {
                errorMessage = BuildSmtpErrorMessage(ex);
                return false;
            }
            catch (Exception)
            {
                errorMessage = "Не удалось отправить письмо из-за внутренней ошибки почтового сервиса.";
                return false;
            }
        }

        private static string NormalizePassword(string rawPassword)
        {
            var builder = new StringBuilder();
            foreach (var ch in rawPassword)
            {
                if (!char.IsWhiteSpace(ch))
                    builder.Append(ch);
            }

            return builder.ToString();
        }

        private static SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = cfgHost,
                Port = cfgPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName, password),
                Timeout = 30000
            };
        }

        private static string BuildSmtpErrorMessage(SmtpException ex)
        {
            var message = $"{ex.Message} {ex.InnerException?.Message}".Trim();

            if (message.Contains("application password is REQUIRED", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("app password", StringComparison.OrdinalIgnoreCase))
            {
                return "Письмо не отправлено: для SMTP Gmail нужно указать пароль приложения вместо обычного пароля аккаунта.";
            }

            if (message.Contains("authentication", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("auth", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("5.7.8", StringComparison.OrdinalIgnoreCase))
            {
                return "Письмо не отправлено: ошибка авторизации SMTP. Проверьте email отправителя и пароль приложения Gmail.";
            }

            return $"Письмо не отправлено: {message}";
        }
    }
}
