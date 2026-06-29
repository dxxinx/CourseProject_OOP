using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TatooShop.Models
{
    public class User : Acc
    {
        [Required]
        public string Address { get; set; }

        public User() : base(AccountType.User)
        {
        }

        public User(string login, string password, string eMail, string phone, string surname, string name, string middleName, string address)
            : base(login, password, eMail, phone, surname, name, middleName, AccountType.User)
        {
            Address = address;
        }

        public override string ForSearch() => $"{base.ForSearch()} {Address}".ToLower();
    }
}

