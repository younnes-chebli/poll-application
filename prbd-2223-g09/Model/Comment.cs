using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace MyPoll.Model {
    public class Comment : EntityBase<MyPollContext> {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PollId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public virtual User Author { get; set; }
        public virtual Poll Poll { get; set; }


        public Comment(int userId, int pollId, string text, DateTime timestamp) {
            UserId = userId;
            PollId = pollId;
            Text = text;
            Timestamp = timestamp;
        }

        public Comment(int userId, int pollId, string text) {
            UserId = userId;
            PollId = pollId;
            Text = text;
        }

        public Comment() { }

        public void Delete() {
            Context.Comments.Remove(this);
            Context.SaveChanges();
        }

        public void Add() {
            Context.Add(this);
            Context.SaveChanges();
        }

        public override string ToString() {
            return
                $"<Comment : " +
                $"Id = {Id}, " +
                $"UserId = {UserId}, " +
                $"PollId = {PollId}, " +
                $"Timestamp = {Timestamp}>";
        }
    }
}
