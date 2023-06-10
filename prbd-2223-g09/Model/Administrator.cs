namespace MyPoll.Model;
    public class Administrator : User {
        public Administrator() {
            Role = Role.Administrator;
        }

        public Administrator(string fullname, string email, string password) : base(fullname, email, password) {
            Role = Role.Administrator;
        }
    }
