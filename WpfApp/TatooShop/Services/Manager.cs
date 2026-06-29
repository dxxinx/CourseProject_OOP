using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TatooShop.Models;

namespace TatooShop.Services
{
    public static class Manager
    {
        public static Acc CurrentUser { get; private set; }
        public static AccountType AccountType { get; private set; }

        static Manager()
        {
            AccountType = AccountType.None;
        }

        public static bool Login(string login, string password)
        {
            if (CurrentUser != null)
            {
                CurrentUser = null;
                AccountType = AccountType.None;
            }

            var hashedPassword = HashPassword(password);

            var user = DataConnection.GetInstance().Accounts.ToList()
                .Find(item => item.Login == login && item.Password == hashedPassword);

            if (user == null)
            {
                user = DataConnection.GetInstance().Accounts.ToList()
                    .Find(item => item.EMail == login && item.Password == hashedPassword);

                if (user == null)
                {
                user = DataConnection.GetInstance().Accounts.ToList()
                    .Find(item => item.Phone == login && item.Password == hashedPassword);

                if (user == null)
                    return false;
                }
            }

            CurrentUser = user;
            AccountType = user.AccountType;
            return true;
        }

        public static (bool result, string error) Register(string login, string password, string email, string phone, string surname, string name, string middleName, string address)
        {
            try
            {
                var db = DataConnection.GetInstance();

                if (db.Accounts.Any(item => item.Login == login))
                    return (false, "Логин уже занят");

                if (db.Accounts.Any(item => item.EMail == email))
                    return (false, "Почта уже занята");

                if (db.Accounts.Any(item => item.Phone == phone))
                    return (false, "Телефон уже занят");

                var hashedPassword = HashPassword(password);

                db.Accounts.Add(new User(login, hashedPassword, email, phone, surname, name, middleName, address));
                db.SaveChanges();

                return (true, null);
            }
            catch
            {
                return (false, "Не удалось зарегистрироваться");
            }
        }



        public static string HashPassword(string password)
        {
            var hashBytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));

            string hash = string.Empty;
            foreach (var item in hashBytes)
                hash += string.Format("{0:x2}", item);

            return hash;
        }

        public static void Logout()
        {
            CurrentUser = null;
            AccountType = AccountType.None;
        }
    }
}
