using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace MyPoll.Model;

public enum Role {
    Normal = 1,
    Administrator = 2
}

public class User : EntityBase<MyPollContext> {
    [Key]
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; protected set; } = Role.Normal;

    public virtual ICollection<Poll> Polls { get; set; } = new HashSet<Poll>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();
    public virtual ICollection<Participation> Participations { get; set; } = new HashSet<Participation>();

    public User(string fullname, string email, string password) {
        FullName = fullname;
        Email = email;
        Password = password;
    }

    public User() {
    }

    public bool isParticipant(Poll poll) {
        return Participations.Any(p => p.Poll.Id == poll.Id);
    }

    public override string ToString() {
        return
            $"<User : " +
            $"Id = {Id}, " +
            $"FullName = {FullName}, " +
            $"Role = {Role}>";
    }
}
