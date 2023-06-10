using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace MyPoll.ViewModel;

public class PollViewModel : ViewModelCommon {
    public Poll Poll { get; }

    private ObservableCollection<Vote> _votes;
    public ObservableCollection<Vote> Votes {
        get => _votes;
        set => SetProperty(ref _votes, value);
    }

    private bool _answered;
    public bool Answered {
        get => _answered;
        set => SetProperty(ref _answered, value);
    }

    private bool _unanswered;
    public bool Unanswered {
        get => _unanswered;
        set => SetProperty(ref _unanswered, value);
    }

    private PollsViewModel _pollsViewModel;

    public PollViewModel() { }

    public PollViewModel(PollsViewModel pollsViewModel, Poll poll) : base() {
        _pollsViewModel = pollsViewModel;
        Poll = poll;

        Votes = new ObservableCollection<Vote>(Context.Votes.Where(v => v.Choice.PollId == Poll.Id));

        if(CurrentUser.Role == Role.Normal) {
            Answered = Poll.Choices
                .Where(c => c.Votes
                .Any(v => v.UserId == CurrentUser.Id))
                .Any();
            Unanswered = !Poll.Choices
                .Where(c => c.Votes
                .Any(v => v.UserId == CurrentUser.Id))
                .Any() && !poll.Closed;
        }
        RaisePropertyChanged();
    }

    protected override void OnRefreshData() {

    }
}
