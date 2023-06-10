using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model {
    public class Participation : EntityBase<MyPollContext> {
        public int PollId { get; set; }
        public int UserId { get; set; }

        public virtual User Participant { get; set; }
        public virtual Poll Poll { get; set; }

        public Participation(int pollId, int userId) {
            PollId = pollId;
            UserId = userId;
        }

        public Participation() { }

        public override string ToString() {
            return
                $"<Participation : " +
                $"UserId = {UserId}, " +
                $"PollId = {PollId}>";
        }
    }
}
