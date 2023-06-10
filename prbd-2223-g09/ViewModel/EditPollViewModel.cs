using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class EditPollViewModel : ViewModelCommon {
    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }

    private string _choiceInput;
    public string ChoiceInput {
        get => _choiceInput;
        set => SetProperty(ref _choiceInput, value);
    }

    public string Name {
        get => Poll?.Name;
        set => SetProperty(Poll.Name, value, Poll, (p, v) => {
            Validate();
            p.Name = v;
            NotifyColleagues(App.Messages.MSG_POLL_NAME_CHANGED, Poll);
        });
    }

    public User Creator {
        get => Poll?.Creator;
        set => SetProperty(Poll.Creator, value, Poll, (p, v) => p.Creator = v);
    }

    public PollType Type {
        get => (PollType)(Poll?.Type);
        set => SetProperty(Poll.Type, value, Poll, (p, v) => {
            p.Type = v;
        });
    }

    private List<PollType> _types;
    public List<PollType> Types {
        get => _types;
        set => SetProperty(ref _types, value);
    }

    public bool Closed {
        get => Poll?.Closed ?? false;
        set => SetProperty(Poll.Closed, value, Poll, (p, v) => p.Closed = v);
    }

    private string _pollTitle;
    public string PollTitle {
        get => _pollTitle;
        set => SetProperty(ref _pollTitle, value);
    }

    private ObservableCollection<ParticipantViewModel> _participantsVM;
    public ObservableCollection<ParticipantViewModel> ParticipantsVM {
        get => _participantsVM;
        set => SetProperty(ref _participantsVM, value);
    }

    private ObservableCollection<User> _participantsToAdd;
    public ObservableCollection<User> ParticipantsToAdd {
        get => _participantsToAdd;
        set => SetProperty(ref _participantsToAdd, value);
    }

    private ObservableCollection<ChoiceViewModel> _choicesVM;
    public ObservableCollection<ChoiceViewModel> ChoicesVM {
        get => _choicesVM;
        set => SetProperty(ref _choicesVM, value);
    }

    private User _selectedParticipant;
    public User SelectedParticipant {
        get => _selectedParticipant;
        set => SetProperty(ref _selectedParticipant, value);
    }

    public ICommand SavePoll { get; set; }
    public ICommand Cancel { get; set; }
    public ICommand AddParticipant { get; set; }
    public ICommand AddEverybody { get; set; }
    public ICommand AddChoice { get; set; }
    public ICommand AddMyself { get; set; }

    public EditPollViewModel() { }

    public EditPollViewModel(Poll poll, bool isNew) {
        Poll = poll;

        var creator = Context.Users.FirstOrDefault(u => u.Id == CurrentUser.Id);

        IsNew = isNew;
        if (isNew)
            Creator = creator;

        PollTitle = IsNew ? "New Poll" : Poll.Name;

        Types = Enum.GetValues(typeof(PollType)).Cast<PollType>().ToList();

        ParticipantsToAdd = new ObservableCollection<User>(Context.Users
            .Where(u => u.Role == Role.Normal
            && !Context.Participations.Any(p => p.UserId == u.Id && p.PollId == Poll.Id)));

        var participants = Poll.Participations
            .Where(p => p.PollId == Poll.Id)
            .Select(p => p.Participant);

        _participantsVM = new ObservableCollection<ParticipantViewModel>(participants.Select(p => new ParticipantViewModel(this, p)));

        var choices = Poll.Choices
            .Where(p => p.PollId == Poll.Id);

        _choicesVM = new ObservableCollection<ChoiceViewModel>(choices.Select(c => new ChoiceViewModel(this, c)));

        SavePoll = new RelayCommand(SaveAction, CanSaveAction);
        Cancel = new RelayCommand(CancelAction);
        AddParticipant = new RelayCommand<User>(AddParticipantAction, CanAddParticipantAction);
        AddEverybody = new RelayCommand(AddEverybodyAction, CanAddEverybodyAction);
        AddChoice = new RelayCommand(AddChoiceAction, CanAddChoiceAction);
        AddMyself = new RelayCommand(AddMyselfAction, CanAddMySelfAction);

        RaisePropertyChanged();
    }

    public override bool Validate() {
        ClearErrors();

        if (string.IsNullOrEmpty(Name))
            AddError(nameof(Name), "Poll name required");

        return !HasErrors;
    }

    public void DeleteParticipantAction(int participantId) {
        var u = ParticipantsVM.FirstOrDefault(p => p.Participant.Id == participantId);
        var participationToRemove = Poll.Participations
                                        .FirstOrDefault(p => p.PollId == Poll.Id
                                            && p.UserId == u.Participant.Id);

        if (u.NbVotes > 0) {
            bool confirmed = MessageBox.Show("Are you sure you want to delete this participation and all it's votes?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

            if (confirmed) {
                DeleteParticipation(participationToRemove, u);
            }
        } else {
            DeleteParticipation(participationToRemove, u);
        }
    }

    private void DeleteParticipation(Participation participationToRemove, ParticipantViewModel u) {
        ParticipantsVM.Remove(u);
        ParticipantsToAdd.Add(u.Participant);
        Poll.Participations.Remove(participationToRemove);
        RaisePropertyChanged();
    }

    public void DeleteChoiceAction(int choiceId) {
        var choiceVM = ChoicesVM.FirstOrDefault(c => c.Choice.Id == choiceId);
        var choice = Poll.Choices.FirstOrDefault(c => c.Id == choiceId);

        if(choiceVM.NbVotes > 0) {
            bool confirmed = MessageBox.Show("Are you sure you want to delete this choice and all it's votes? This action cannot be undone", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

            if (confirmed) {
                DeleteChoice(choice, choiceVM);
            }
        } else {
            DeleteChoice(choice, choiceVM);
        }
    }

    private void DeleteChoice(Choice choiceToDelete, ChoiceViewModel choiceVM) {
        choiceToDelete.Delete();
        ChoicesVM.Remove(choiceVM);
    }

    private void AddMyselfAction() {
        var participantVM = new ParticipantViewModel(this, CurrentUser);
        ParticipantsToAdd.ToList().ForEach(pA => {
            if (pA.Id == CurrentUser.Id)
                ParticipantsToAdd.Remove(pA);
        });
        ParticipantsVM.Add(participantVM);

        Poll.Participations.Add(new Participation(Poll.Id, CurrentUser.Id));

        RaisePropertyChanged();
    }

    private bool CanAddMySelfAction() {
        return !ParticipantsVM.Any(p => p.Participant.Id == CurrentUser.Id);
    }

    private void AddParticipantAction(User u) {
        var participantVM = new ParticipantViewModel(this, u);
        ParticipantsToAdd.ToList().ForEach(pA => {
            if (pA.Id == u.Id)
                ParticipantsToAdd.Remove(pA);
        });
        ParticipantsVM.Add(participantVM);

        Poll.Participations.Add(new Participation(Poll.Id, u.Id));

        RaisePropertyChanged();
    }

    private void AddEverybodyAction() {
        foreach (var user in ParticipantsToAdd) {
            Poll.Participations.Add(new Participation(Poll.Id, user.Id));
            var participantVM = new ParticipantViewModel(this, user);
            ParticipantsVM.Add(participantVM);
        }
        ParticipantsToAdd.Clear();
    }

    private void AddChoiceAction() {
        ChoiceInput = ChoiceInput.Trim();

        if (!string.IsNullOrEmpty(ChoiceInput)) {
            var newChoice = new Choice(Poll.Id, ChoiceInput);
            var choiceVM = new ChoiceViewModel(this, newChoice);
            newChoice.Add();
            ChoicesVM.Add(choiceVM);
            ChoiceInput = "";
        }
    }

    public override void SaveAction() {
        if (IsNew) {
            Poll.CreatorId = CurrentUser.Id;
            Context.Polls.Add(Poll);
            IsNew = false;
        }

        Context.SaveChanges();
        NotifyColleagues(App.Messages.MSG_POLL_CHANGED, Poll);
        NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
    }

    private bool CanSaveAction() {
        return !string.IsNullOrEmpty(Name) && !HasErrors;
    }

    public override void CancelAction() {
        if (IsNew) {
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
        } else {
            Poll.Reload();
            Poll.Participations = Context.Participations
                .Where(p => p.PollId == Poll.Id).ToList();
            Poll.Choices = Context.Choices
                .Where(c => c.PollId == Poll.Id).ToList();
            Context.SaveChanges();

            RaisePropertyChanged();
            NotifyColleagues(App.Messages.MSG_POLL_NAME_CHANGED, Poll);
        }
    }

    private bool CanAddParticipantAction(User u) {
        return SelectedParticipant != null;
    }

    private bool CanAddEverybodyAction() {
        return !ParticipantsToAdd.IsNullOrEmpty();
    }

    private bool CanAddChoiceAction() {
        return !ChoicesVM.Any(c => c.Choice.Label.Equals(ChoiceInput)) && !string.IsNullOrEmpty(ChoiceInput);
    }

    protected override void OnRefreshData() {
    }
}
