using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel {
    public class ChoiceViewModel : ViewModelCommon {
        private EditPollViewModel _editPollViewModel;
        public Choice Choice { get; }

        private int _nbVotes;
        public int NbVotes {
            get => _nbVotes;
            set => SetProperty(ref _nbVotes, value);
        }

        public ICommand Delete { get; set; }

        public ChoiceViewModel(EditPollViewModel editPollViewModel, Choice choice) {
            _editPollViewModel = editPollViewModel;
            Choice = choice;

            RefreshNbVotes();

            Delete = new RelayCommand<int>(DeleteChoiceAction);

            RaisePropertyChanged();
        }

        private void RefreshNbVotes() {
            _nbVotes = Context.Votes
            .Where(v => v.ChoiceId == Choice.Id && v.Choice.PollId == _editPollViewModel.Poll.Id)
            .Count();
        }

        private void DeleteChoiceAction(int id) {
            _editPollViewModel.DeleteChoiceAction(id);
        }
    }
}
