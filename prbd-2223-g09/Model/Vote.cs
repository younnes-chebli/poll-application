using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model {
    public enum VoteType {
        Yes = 1,
        No = 2,
        Maybe = 3,
        None = 4
    }

    public class Vote : EntityBase<MyPollContext> {
        public int UserId { get; set; }
        public int ChoiceId { get; set; }
        public VoteType Type { get; set; }

        public virtual User Author { get; set; }
        public virtual Choice Choice { get; set; }

        public Vote(int userId, int choiceId, VoteType type) {
            UserId = userId;
            ChoiceId= choiceId;
            Type = type;
        }

        public Vote() { }

        public Vote(int userId, int choiceId) {
            UserId = userId;
            ChoiceId = choiceId;
        }

        public override string ToString() {
            return
                $"<Vote : " +
                $"UserId = {UserId}, " +
                $"ChoiceId = {ChoiceId}, " +
                $"Type = {Type}>";
        }
    }
}
