using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TatooShop.Models
{
    public class Admin : Acc
    {
        public Admin() : base(AccountType.Admin)
        {
        }

        public Admin(string login, string password, string eMail, string phone, string surname, string name, string middleName)
            : base(login, password, eMail, phone, surname, name, middleName, AccountType.Admin) { }
    }


}
