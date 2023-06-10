using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel
{
    public class VotesParticipantViewModel : ViewModelCommon {
        private Poll _poll;
        public Poll Poll {
            get => _poll;
            set => SetProperty(ref _poll, value);
        }

        private PollChoicesViewModel _pollChoicesViewModel;

        private List<Choice> _choices;
        public List<Choice> Choices {
            get => _choices;
            private set => SetProperty(ref _choices, value);
        }

        public User Participant { get; }

        private List<VoteViewModel> _votesVM = new();
        public List<VoteViewModel> VotesVM {
            get => _votesVM;
            private set => SetProperty(ref _votesVM, value);
        }

        private bool _editMode;

        private bool Concerned => CurrentUser.Id == Participant.Id || CurrentUser is Administrator;

        private bool _open;
        public bool Open {
            get => !Poll.Closed;
            set=> SetProperty(ref _open, value);
        }

        public bool Editable => !EditMode && !ParentEditMode && Concerned && Open;
        public bool ParentEditMode => _pollChoicesViewModel.EditMode;

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }

        public VotesParticipantViewModel(PollChoicesViewModel pollChoicesViewModel, User participant, List<Choice> choices, Poll poll) {

            _pollChoicesViewModel = pollChoicesViewModel;
            _choices = choices;
            Participant = participant;
            Poll = poll;
            RefreshVotes();

            EditCommand = new RelayCommand(() => EditMode = true);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            DeleteCommand = new RelayCommand(Delete);

            Register(App.Messages.MSG_POLL_REOPEN, RefreshPollStatus);
        }

        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value, EditModeChanged);
        }

        private void EditModeChanged() {
            foreach (VoteViewModel voteVM in _votesVM) {
                voteVM.EditMode = EditMode;
            }

            _pollChoicesViewModel.AskEditMode(EditMode);
        }

        public void ChangeButtonsVisibility() {
            RaisePropertyChanged();
        }

        private void RefreshVotes() {
            VotesVM = _choices
                .Select(c => new VoteViewModel(Participant, c))
                .ToList();

            RaisePropertyChanged();
        }

        private void RefreshPollStatus() {
            Open = !Poll.Closed;
            RaisePropertyChanged();
        }

        private void Save() {
            EditMode = false;
            Context.SaveChanges();
            RefreshVotes();
            NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
        }

        private void Cancel() {
            EditMode = false;
            RefreshVotes();
        }

        private void Delete() {
            Participant.Votes.ToList().ForEach(v => {
            if (v.Choice.PollId == Poll.Id) {
                    Participant.Votes.Remove(v);
                }
            });
            Context.SaveChanges();
            RefreshVotes();
            NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
        }
    }
}
