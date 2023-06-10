using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelCommon {

    public static string Title {
        get => $"My Poll App ({CurrentUser?.FullName})";
    }
}
