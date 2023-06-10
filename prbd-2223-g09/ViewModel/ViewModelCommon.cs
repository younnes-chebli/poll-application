using MyPoll.Model;
using MyPoll;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public abstract class ViewModelCommon : ViewModelBase<User, MyPollContext> {
    public static bool IsAdmin => App.IsLoggedIn && App.CurrentUser is Administrator;

    public static bool IsNotAdmin => !IsAdmin;
}
