using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;
using static System.Net.Mime.MediaTypeNames;

namespace MyPoll.Model {
    public class Choice : EntityBase<MyPollContext> {
        [Key]
        public int Id { get; set; }
        public int PollId { get; set; }
        public string Label { get; set; }

        public virtual Poll Poll { get; set; }
        public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();

        public Choice(int pollId, string label) {
            PollId = pollId;
            Label = label;
        }

        public Choice() { }

        public void Delete() {
            Votes.Clear();
            Context.Choices.Remove(this);
            Context.SaveChanges();
        }

        public void Add() {
            Context.Add(this);
            Context.SaveChanges();
        }

        public override string ToString() {
            return
                $"<Choice : " +
                $"Id = {Id}, " +
                $"PollId = {PollId}, " +
                $"Label = {Label}>";
        }
    }
}
