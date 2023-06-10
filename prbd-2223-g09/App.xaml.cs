using System.Windows;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll; 

public partial class App : ApplicationBase<User, MyPollContext> {
    public enum Messages {
        MSG_LOGIN,
        MSG_LOGOUT,
        MSG_REFRESH_DATA,
        MSG_NEW_POLL,
        MSG_POLL_NAME_CHANGED,
        MSG_POLL_CHANGED,
        MSG_DISPLAY_POLL,
        MSG_DISPLAY_POLL_CHOICES,
        MSG_CLOSE_TAB,
        MSG_POLL_REOPEN
    }

    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();

        Register<User>(this, Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });

        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, MyPollContext>();
        });

        //DisplayData();
    }

    private static void DisplayData() {
        Console.WriteLine();
        Console.WriteLine("Users");
        Console.WriteLine("---");
        var users = Context.Users.ToList();
        foreach (var user in users) {
            if (user != null) {
                Console.WriteLine(user);
            }
        }

        Console.WriteLine();
        Console.WriteLine("Polls");
        Console.WriteLine("---");
        var polls = Context.Polls.ToList();
        foreach (var poll in polls) {
            if (poll != null) {
                Console.WriteLine(poll);
            }
        }

        Console.WriteLine();
        Console.WriteLine("Comments");
        Console.WriteLine("---");
        var comments = Context.Comments.ToList();
        foreach (var comment in comments) {
            if (comment != null) {
                Console.WriteLine(comment);
            }
        }

        Console.WriteLine();
        Console.WriteLine("Choices");
        Console.WriteLine("---");
        var choices = Context.Choices.ToList();
        foreach (var choice in choices) {
            if (choice != null) {
                Console.WriteLine(choice);
            }
        }

        Console.WriteLine();
        Console.WriteLine("Votes");
        Console.WriteLine("---");
        var votes = Context.Votes.ToList();
        foreach (var vote in votes) {
            if (vote != null) {
                Console.WriteLine(vote);
            }
        }

        Console.WriteLine();
        Console.WriteLine("Participations");
        Console.WriteLine("---");
        var participations = Context.Participations.ToList();
        foreach (var participation in participations) {
            if (participation != null) {
                Console.WriteLine(participation);
            }
        }
    }

    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        //// Cold start
        //Console.Write("Cold starting database... ");
        //Context.Users.Find(0);
        //Console.WriteLine("done");
    }
}
