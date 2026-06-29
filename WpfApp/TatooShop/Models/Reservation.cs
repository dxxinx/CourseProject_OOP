using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatooShop.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Master Master { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public UserTime Time { get; set; }

        public ReservationStatus Status { get; set; }

        public string StatusText => Status switch
        {
            ReservationStatus.Pending => "Ожидает подтверждения",
            ReservationStatus.Confirmed => "Подтверждена",
            ReservationStatus.Cancelled => "Отменена",
            ReservationStatus.Visited => "Посещена",
            _ => Status.ToString()
        };

        public bool CanCancel => Status != ReservationStatus.Visited;
        public string TimeText => GetHour(Time);

        public Reservation() => Id = -1;

        public Reservation(User user, Master master, DateTime date, UserTime time)
        {
            Id = -1;
            User = user;
            Master = master;
            Date = date;
            Time = time;
            Status = ReservationStatus.Pending;
        }
        public static string GetHour(UserTime time)
        {
            switch (time)
            {
                case UserTime.Eleven:
                    return "11:00";
                case UserTime.Thirteen:
                    return "15:00";
                case UserTime.Sixteen:
                    return "18:00";
                default:
                    throw new ArgumentException("Invalid UserTime value", nameof(time));
            }
        }
        public static UserTime ParseHour(string hour)
        {
            switch (hour)
            {
                case "11:00":
                    return UserTime.Eleven;
                case "15:00":
                    return UserTime.Thirteen;
                case "18:00":
                    return UserTime.Sixteen;
                default:
                    throw new ArgumentException("Invalid hour value", nameof(hour));
            }
        }

        public DateTime GetReservationDateTime()
        {
            return Date.Date.AddHours((int)Time);
        }

    }

    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Visited
    }

    public enum UserTime {
        Eleven = 11, Thirteen=15, Sixteen=18
    }

}
