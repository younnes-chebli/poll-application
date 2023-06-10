using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using PRBD_Framework;

namespace MyPoll.Model {
    public enum PollType {
        Single = 1,
        Multiple = 2
    }

    public class Poll : EntityBase<MyPollContext> {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatorId { get; set; }
        public bool Closed { get; set; } = false;
        public PollType Type { get; set; } = PollType.Multiple;

        public virtual User Creator { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Choice> Choices { get; set; } = new HashSet<Choice>();
        public virtual ICollection<Participation> Participations { get; set; } = new HashSet<Participation>();

        public Poll(string name, int creatorId, bool closed = false) {
            Name = name;
            CreatorId = creatorId;
            Closed = closed;
        }

        public Poll(string name, int creatorId, PollType type, bool closed = false) {
            Name = name;
            CreatorId = creatorId;
            Type = type;
            Closed = closed;
        }

        public Poll() {

        }

        public void Delete() {
            Comments.Clear();
            Choices.Clear();
            Participations.Clear();
            Context.Polls.Remove(this);
            Context.SaveChanges();
        }

        public override string ToString() {
            return
                $"<Poll : " +
                $"Id = {Id}, " +
                $"Creator = {CreatorId}, " +
                $"Closed = {Closed}, " +
                $"Type = {Type}>";
        }
    }
}
