using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel {
    public class CommentViewModel : ViewModelCommon {
        private PollChoicesViewModel _pollChoicesViewModel;
        public Comment Comment { get; }

        public bool IsCreatorOrAdmin => CurrentUser.Id == Comment.Poll.CreatorId || CurrentUser is Administrator;

        public ICommand Delete { get; set; }

        public CommentViewModel(PollChoicesViewModel pollChoicesViewModel, Comment comment) {
            _pollChoicesViewModel = pollChoicesViewModel;
            Comment = comment;

            Delete = new RelayCommand(DeleteCommentAction);

            RaisePropertyChanged();
        }

        private void DeleteCommentAction() {
            Comment.Delete();
            _pollChoicesViewModel.RefreshComments();
        }
    }
}
