using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyPoll.ViewModel;
public class PollsViewModel : ViewModelCommon {
    private List<PollViewModel> _pollsVM;
    public List<PollViewModel> PollsVM => _pollsVM;


    private ObservableCollection<Vote> _votes;
    public ObservableCollection<Vote> Votes {
        get => _votes;
        set => SetProperty(ref _votes, value);
    }

    private string _filter;
    public string Filter {
        get => _filter;
        set => SetProperty(ref _filter, value, ApplyFilterAction);
    }

    public bool IsNormal => CurrentUser is not Administrator;

    public ICommand ClearFilter { get; set; }
    public ICommand NewPoll { get; set; }
    public ICommand DisplayPollChoices { get; set; }

    public PollsViewModel() : base() {
        ApplyFilterAction();

        _votes = new ObservableCollection<Vote>(Context.Votes);

        ClearFilter = new RelayCommand(() => Filter = "");

        NewPoll = new RelayCommand(() => {
            NotifyColleagues(App.Messages.MSG_NEW_POLL, new Poll());
        });

        DisplayPollChoices = new RelayCommand<PollViewModel>(vm => {
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, vm.Poll);
            NotifyColleagues(App.Messages.MSG_DISPLAY_POLL_CHOICES, vm.Poll);
        });

        Register<Poll>(App.Messages.MSG_POLL_CHANGED, poll => ApplyFilterAction());

        RaisePropertyChanged();
    }

    private void ApplyFilterAction() {
        if(CurrentUser is Administrator) {
            IEnumerable<Poll> polls = Context.Polls;

            if (!string.IsNullOrEmpty(Filter))
                polls = from p in Context.Polls
                        where p.Name.Contains(Filter)
                                || p.Creator.FullName.Contains(Filter)
                                || p.Participations.Any(p => p.Participant.FullName.Contains(Filter))
                                || p.Choices.Any(c => c.Label.Contains(Filter))
                        select p;

            _pollsVM = polls.Select(p => new PollViewModel(this, p)).ToList();
        } else {
            IEnumerable<Poll> polls = Context.Polls
                .Where(p => p.CreatorId == CurrentUser.Id || p.Participations
                .Any(part => part.UserId == CurrentUser.Id));

            if (!string.IsNullOrEmpty(Filter))
                polls = from p in Context.Polls
                        where (p.Name.Contains(Filter)
                                || p.Creator.FullName.Contains(Filter)
                                || p.Participations.Any(p => p.Participant.FullName.Contains(Filter))
                                || p.Choices.Any(c => c.Label.Contains(Filter)))
                            && (p.CreatorId == CurrentUser.Id || p.Participations
                                    .Any(part => part.UserId == CurrentUser.Id))
                        select p;

            _pollsVM = polls.Select(p => new PollViewModel(this, p)).ToList();
        }

        RaisePropertyChanged();
    }

    protected override void OnRefreshData() {
        
    }
}
