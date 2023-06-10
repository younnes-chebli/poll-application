using System.Windows.Controls;
using Azure;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();

        Register<Poll>(App.Messages.MSG_NEW_POLL, poll => DoDisplayPoll(poll, true));

        Register<Poll>(App.Messages.MSG_DISPLAY_POLL, poll => DoDisplayPoll(poll, false));

        Register<Poll>(App.Messages.MSG_DISPLAY_POLL_CHOICES, DoDisplayPollChoices);

        Register<Poll>(App.Messages.MSG_POLL_NAME_CHANGED, poll => DoRenameTab(string.IsNullOrEmpty(poll.Name) ? "<New Poll>" : poll.Name));

        Register<Poll>(App.Messages.MSG_CLOSE_TAB,
            poll => DoCloseTab(poll));
    }

    private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e) {
        NotifyColleagues(App.Messages.MSG_LOGOUT);
    }

    private void DoRenameTab(string header) {
        if (tabControl.SelectedItem is TabItem tab) {
            MyTabControl.RenameTab(tab, header);
            tab.Tag = header;
        }
    }

    private void DoDisplayPoll(Poll poll, bool isNew) {
        if (poll != null)
            OpenTab(isNew ? "<New Poll>" : poll.Name, poll.Name, () => new EditPollView(poll, isNew));
    }

    private void DoDisplayPollChoices(Poll poll) {
        if (poll != null)
            OpenTab(poll.Name, poll.Name, () => new PollChoicesView(poll));
    }

    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null)
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }

    private void DoCloseTab(Poll poll) {
        tabControl.CloseByTag(string.IsNullOrEmpty(poll.Name) ? "<New Poll>" : poll.Name);
    }
}
