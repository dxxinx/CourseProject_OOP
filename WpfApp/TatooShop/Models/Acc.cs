using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TatooShop.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Admin), "admin")]
    [JsonDerivedType(typeof(User), "user")]
    public abstract class Acc
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public AccountType AccountType { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string EMail { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string MiddleName { get; set; }

        public string FullName { get => $"{Surname} {Name} {MiddleName}"; }

        public Acc(AccountType type)
        {
            Id = -1;
            AccountType = type;
        }

        protected Acc(string login, string password, string eMail, string phone, string surname, string name, string middleName, AccountType type)
        {
            Id = -1;
            AccountType = type;

            Login = login;
            Password = password;
            EMail = eMail;
            Phone = phone;
            Name = name;
            Surname = surname;
            MiddleName = middleName;
        }

        public virtual string ForSearch() => $"{Id} {Login} {EMail} {Phone} {FullName}".ToLower();
    }

    public enum AccountType
    {
        Admin,
        User,
        None
    }
}
