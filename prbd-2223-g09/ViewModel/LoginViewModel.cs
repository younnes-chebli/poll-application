using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel {
    public class LoginViewModel : ViewModelCommon {
        private string _email;
        public string Email {
            get => _email;
            set => SetProperty(ref _email, value, () => Validate());
        }

        private string _password;

        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }

        public ICommand LoginCommand { get; set; }
        public ICommand LoginAsHarryCommand { get; set; }
        public ICommand LoginAsJohnCommand { get; set; }
        public ICommand LoginAsAdminCommand { get; set; }

        public LoginViewModel() : base() {
            LoginCommand = new RelayCommand(LoginAction,
                () => { return _email != null && _password != null && !HasErrors; });
            LoginAsHarryCommand = new RelayCommand(LoginAsHarryAction);
            LoginAsJohnCommand = new RelayCommand(LoginAsJohnAction);
            LoginAsAdminCommand = new RelayCommand(LoginAsAdminAction);
        }

        private void LoginAction() {
            var user = Context.Users
            .Where(u => u.Email == Email)
            .FirstOrDefault();
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }

        private void LoginAsHarryAction() {
            var user = Context.Users
            .Where(u => u.Id == 1)
            .FirstOrDefault();
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }

        private void LoginAsJohnAction() {
            var user = Context.Users
            .Where(u => u.Id == 3)
            .FirstOrDefault();
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }

        private void LoginAsAdminAction() {
            var user = Context.Users
            .Where(u => u.Id == 9)
            .FirstOrDefault();
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }

        public override bool Validate() {
            ClearErrors();

            var user = Context.Users
                .Where(u => u.Email == Email)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(Email))
                AddError(nameof(Email), "Email is required");
            else if (user == null)
                AddError(nameof(Email), "User does not exist");
            else {
                if (string.IsNullOrEmpty(Password))
                    AddError(nameof(Password), "Password is required");
                else if (user != null && !SecretHasher.Verify(Password, user.Password))
                    AddError(nameof(Password), "Wrong password");
            }

            return !HasErrors;
        }

        protected override void OnRefreshData() {
        }
    }
}
