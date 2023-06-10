using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class PollChoicesViewModel : ViewModelCommon {
    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    private bool _closed;
    public bool Closed {
        get => (bool)(Poll?.Closed);
        set => SetProperty(ref _closed, value);
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    private List<Choice> _choices;
    public List<Choice> Choices => _choices;

    private ObservableCollection<CommentViewModel> _commentsVM;
    public ObservableCollection<CommentViewModel> CommentsVM {
        get => _commentsVM;
        set => SetProperty(ref _commentsVM, value);
    }

    private string _commentInput;
    public string CommentInput {
        get => _commentInput;
        set => SetProperty(ref _commentInput, value);
    }

    private List<VotesParticipantViewModel> _votesParticipantsVM;
    public List<VotesParticipantViewModel> VotesParticipantsVM => _votesParticipantsVM;

    private bool Concerned => CurrentUser.isParticipant(Poll);
    public bool IsCreatorOrAdmin => CurrentUser.Id == Poll.CreatorId || CurrentUser is Administrator;
    public bool IsCreator => CurrentUser.Id == Poll.CreatorId;
    public bool CanComment => !Poll.Closed && (Concerned || IsCreator);
    public bool CommentsVisible => CommentsVM.Count != 0;

    public ICommand Edit { get; set; }
    public ICommand Delete { get; set; }
    public ICommand Reopen { get; set; }
    public ICommand AddComment { get; set; }

    public PollChoicesViewModel(Poll poll) {
        Poll = poll;

        _choices = Context.Choices
            .Where(c => c.PollId == Poll.Id)
            .OrderBy(c => c.Label)
            .ToList();

        var participants = Context.Participations
               .Where(p => p.PollId == Poll.Id)
               .Select(p => p.Participant)
               .OrderBy(p => p.FullName)
               .ToList();

        _votesParticipantsVM = participants.Select(p => new VotesParticipantViewModel(this, p, _choices, _poll)).ToList();

        RefreshComments();

        Edit = new RelayCommand(() => {
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
            NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, Poll);
        });
        Delete = new RelayCommand(DeletePollAction);
        Reopen = new RelayCommand(ReopenAction);
        AddComment = new RelayCommand(AddCommentAction, CanAddCommentAction);
    }

    public void AskEditMode(bool editMode) {
        EditMode = editMode;

        foreach (var vpVM in VotesParticipantsVM)
            vpVM.ChangeButtonsVisibility();
    }

    private void DeletePollAction() {
        bool confirmed = MessageBox.Show("Are you sure you want to delete this poll and all it's dependencies? This action cannot be undone", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

        if(confirmed) {
            Poll.Delete();
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
            NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
        }
    }

    private void ReopenAction() {
        Poll.Closed = false;
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_POLL_REOPEN);
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
    }

    private void AddCommentAction() {
        CommentInput = CommentInput.Trim();

        if (!string.IsNullOrEmpty(CommentInput)) {
            var newComment = new Comment(CurrentUser.Id, Poll.Id, CommentInput);
            newComment.Add();
            CommentInput = "";
            RefreshComments();
        }
    }

    private bool CanAddCommentAction() {
        return !string.IsNullOrEmpty(CommentInput);
    }

    public void RefreshComments() {
        var comments = Poll.Comments
            .OrderByDescending(c => c.Timestamp);

        _commentsVM = new ObservableCollection<CommentViewModel>(comments.Select(c => new CommentViewModel(this, c)));

        RaisePropertyChanged();
    }
}
