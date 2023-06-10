using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel {
    public class ParticipantViewModel : ViewModelCommon {
        private EditPollViewModel _editPollViewModel;
        public User Participant { get; }

        private int _nbVotes;
        public int NbVotes {
            get => _nbVotes;
            set => SetProperty(ref _nbVotes, value);
        }

        public ICommand Delete { get; set; }

        public ParticipantViewModel(EditPollViewModel editPollViewModel, User participant) {
            _editPollViewModel = editPollViewModel;
            Participant = participant;

            RefreshNbVotes();

            Delete = new RelayCommand<int>(DeleteParticipantAction);

            RaisePropertyChanged();
        }

        private void DeleteParticipantAction(int id) {
            _editPollViewModel.DeleteParticipantAction(id);
        }

        private void RefreshNbVotes() {
            _nbVotes = Context.Votes
            .Where(v => v.Author.Id == Participant.Id && v.Choice.PollId == _editPollViewModel.Poll.Id)
            .Count();
        }
    }
}
