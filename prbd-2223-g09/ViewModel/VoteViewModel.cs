using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using MyPoll.Model;
using PRBD_Framework;
using FontAwesome6;
using Microsoft.EntityFrameworkCore;

namespace MyPoll.ViewModel
{
    public class VoteViewModel : ViewModelCommon {
        private Vote _vote;
        public Vote Vote {
            get => _vote;
            set => SetProperty(ref _vote, value);
        }

        private bool _editMode;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }

        private User _author;
        public User Author {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        private Choice _choice;
        public Choice Choice {
            get => _choice;
            set => SetProperty(ref _choice, value);
        }

        public ICommand VoteYes { get; set; }
        public ICommand VoteNo { get; set; }
        public ICommand VoteMaybe { get; set; }
        public ICommand Flush { get; set; }

        private bool _isVoted;
        public bool IsVoted {
            get => _isVoted;
            set => SetProperty(ref _isVoted, value);
        }

        private bool _isYes;
        public bool IsYes {
            get => VoteType == VoteType.Yes;
            set => SetProperty(ref _isYes, value);
        }

        private bool _isNo;
        public bool IsNo {
            get => VoteType == VoteType.No;
            set => SetProperty(ref _isNo, value);
        }

        private bool _isMaybe;
        public bool IsMaybe {
            get => VoteType == VoteType.Maybe;
            set => SetProperty(ref _isMaybe, value);
        }

        private bool _isFlush;
        public bool IsFlush {
            get => _isFlush;
            set => SetProperty(ref _isFlush, value);
        }

        private VoteType _voteType;
        public VoteType VoteType {
            get => _voteType;
            set => SetProperty(ref _voteType, value);
        }

        public VoteViewModel() { }

        public VoteViewModel(User participant, Choice choice) {
            Author = participant;
            Choice = choice;

            Vote = participant.Votes.FirstOrDefault(v => v.ChoiceId == choice.Id, new Vote() { UserId = participant.Id, ChoiceId = choice.Id });
            IsVoted = Vote.ChoiceId == choice.Id && Vote.Type != 0;

            VoteType = Vote.Type;
            VotedIcon = UpdateIcon();
            VotedColor = UpdateColor();

            VoteYes = new RelayCommand(VoteYesAction);
            VoteNo = new RelayCommand(VoteNoAction);
            VoteMaybe = new RelayCommand(VoteMaybeAction);
            Flush = new RelayCommand(FlushAction);
        }

        private void VoteYesAction() {
            IsVoted = true;
            Vote.Type= VoteType.Yes;
            VoteType = VoteType.Yes;
            Author.Votes.Add(new Vote(Author.Id, Choice.Id, VoteType.Yes));
            RaisePropertyChanged();
        }

        private void VoteNoAction() {
            IsVoted = true;
            Vote.Type = VoteType.No;
            VoteType = VoteType.No;
            Author.Votes.Add(new Vote(Author.Id, Choice.Id, VoteType.No));
            RaisePropertyChanged();
        }

        private void VoteMaybeAction() {
            IsVoted = true;
            Vote.Type = VoteType.Maybe;
            VoteType = VoteType.Maybe;
            Author.Votes.Add(new Vote(Author.Id, Choice.Id, VoteType.Maybe));
            RaisePropertyChanged();
        }

        private void FlushAction() {
            VotedIcon = EFontAwesomeIcon.None;
            VotedColor = Brushes.LightGray;
            IsVoted = false;
            VoteType = VoteType.None;
            Author.Votes.ToList().ForEach(v => {
                if(v.ChoiceId == Choice.Id)
                    Author.Votes.Remove(v);
            });
            RaisePropertyChanged();
        }

        private EFontAwesomeIcon UpdateIcon() {
            if(IsYes) {
                return EFontAwesomeIcon.Solid_Check;
            } else if(IsNo) {
                return EFontAwesomeIcon.Solid_Exclamation;
            } else if (IsMaybe) {
                return EFontAwesomeIcon.Solid_Question;
            } else {
                return EFontAwesomeIcon.None;
            }
        }

        private EFontAwesomeIcon _votedIcon;
        public EFontAwesomeIcon VotedIcon {
            get => _votedIcon;
            set => SetProperty(ref _votedIcon, value);
        }

        private Brush UpdateColor() {
            if (IsYes) {
                return Brushes.Green;
            } else if (IsNo) {
                return Brushes.Red;
            } else if (IsMaybe) {
                return Brushes.Orange;
            } else {
                return Brushes.White;
            }
        }

        private Brush _votedColor;
        public Brush VotedColor {
            get => _votedColor;
            set => SetProperty(ref _votedColor, value);
        }

        public string YesToolTip => "Yes";
        public string NoToolTip => "No";
        public string MaybeToolTip => "Maybe";
        public string FlushToolTip => "Flush";
    }
}
