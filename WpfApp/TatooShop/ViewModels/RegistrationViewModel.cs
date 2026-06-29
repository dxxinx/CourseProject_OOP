using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TatooShop.Infrastructure.Commands;
using TatooShop.Views;
using TatooShop.Services;
using System.Windows;
using System.Windows.Controls;

namespace TatooShop.ViewModels
{
    internal class RegistrationViewModel : ViewModel, ICloseable
    {
        public event EventHandler CloseRequest;

        private string _login;

        public string Login
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _eMail;

        public string EMail
        {
            get { return _eMail; }
            set { SetProperty(ref _eMail, value); }
        }

        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); }
        }

        private string _surname;

        public string Surname
        {
            get { return _surname; }
            set { SetProperty(ref _surname, value); }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _middleName;

        public string MiddleName
        {
            get { return _middleName; }
            set { SetProperty(ref _middleName, value); }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }
        public ICommand RegistrationCommand { get; set; }

        private static bool CanRegistrationCommandExecute(object p) => Manager.CurrentUser == null;

        private void OnRegistrationCommandExecuted(object p) => ProcessRegistration(p);

        public RegistrationViewModel()
        {
            RegistrationCommand = new RelayCommand(OnRegistrationCommandExecuted, CanRegistrationCommandExecute);
        }
        private void ProcessRegistration(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
                Password = passwordBox.Password;

            var check = CheckFields();
            if (!string.IsNullOrEmpty(check))
            {
                MessageBox.Show(check);
                return;
            }
            else {
                var (result, error) = Manager.Register(Login, Password, EMail, Phone, Surname, Name, MiddleName, Address);
                if (result)
                {
                    MessageBox.Show("Аккаунт успешно зарегистрирован");
                    CloseRequest?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show(error, "Не удалось зарегистрировать аккаунт");
                }
            }
          
        }
        private string CheckFields()
        {
            string errors = string.Empty;

            errors += Validate("Login") + "\n";
            errors += Validate("Password") + "\n";
            errors += Validate("EMail") + "\n";
            errors += Validate("Phone") + "\n";
            errors += Validate("Surname") + "\n";
            errors += Validate("Name") + "\n";
            errors += Validate("MiddleName") + "\n";
            errors += Validate("Address") + "\n";

            return errors.Trim();
        }

        private string Validate(string columnName)
        {
            if (columnName == null)
                return string.Empty;

            string error = string.Empty;

            switch (columnName)
            {
                case "Login":
                    {
                        if (string.IsNullOrEmpty(Login))
                            return "Введите логин";

                        var (isValid, forbiddenSymbols) = Validator.Validate(Login, Validator.loginRegex);

                        if (!isValid)
                            return $"В логине присутсвуют недопустимые символы: {Validator.JoinSymbols(forbiddenSymbols)}";
                    }
                    break;

                case "Password":
                    {
                        if (string.IsNullOrEmpty(Password))
                            return "Введите пароль";

                        if (Password.Length < 6)
                            return "Пароль слишком короткий";
                    }
                    break;

                case "EMail":
                    {
                        if (string.IsNullOrEmpty(EMail))
                            return "Введите почту";

                        var (isValid, _) = Validator.Validate(EMail, Validator.eMailRegex);

                        if (!isValid)
                            return "Введённая строка не является электронной почтой";
                    }
                    break;

                case "Phone":
                    {
                        if (string.IsNullOrEmpty(Phone))
                            return "Введите телефон";

                        var (isValid, _) = Validator.Validate(Phone, Validator.phoneRegex);

                        if (!isValid)
                            return "Введённая строка не является телефоном";
                    }
                    break;

                case "Surname":
                    {
                        if (string.IsNullOrEmpty(Surname))
                            return "Введите фамилию";

                        var (isValid, forbiddenSymbols) = Validator.Validate(Surname, Validator.nameRegex);

                        if (!isValid)
                            return $"В фамилии присутсвуют недопустимые символы: {Validator.JoinSymbols(forbiddenSymbols)}";
                    }
                    break;

                case "Name":
                    {
                        if (string.IsNullOrEmpty(Name))
                            return "Введите имя";

                        var (isValid, forbiddenSymbols) = Validator.Validate(Name, Validator.nameRegex);

                        if (!isValid)
                            return $"В имени присутсвуют недопустимые символы: {Validator.JoinSymbols(forbiddenSymbols)}";
                    }
                    break;

                case "MiddleName":
                    {
                        if (string.IsNullOrEmpty(MiddleName))
                            return "Введите отчество";

                        var (isValid, forbiddenSymbols) = Validator.Validate(MiddleName, Validator.nameRegex);

                        if (!isValid)
                            return $"В отчестве присутсвуют недопустимые символы: {Validator.JoinSymbols(forbiddenSymbols)}";
                    }
                    break;

                case "Address":
                    {
                        if (string.IsNullOrEmpty(Address))
                            return "Введите адрес";

                        var (isValid, forbiddenSymbols) = Validator.Validate(Address, Validator.addressRegex);

                        if (!isValid)
                            return $"В адресе присутсвуют недопустимые символы: {Validator.JoinSymbols(forbiddenSymbols)}";
                    }
                    break;
            }

            return error;
        }
    }
}
