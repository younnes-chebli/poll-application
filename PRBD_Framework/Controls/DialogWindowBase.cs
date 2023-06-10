namespace PRBD_Framework; 

public class DialogWindowBase : WindowBase {

    public DialogWindowBase() {
        DataContextChanged += (o, e) => {
            if (e.OldValue != null)
                (e.OldValue as IDialogViewModelBase).DoClose -= DialogWindowBase_DoClose;
            if (e.NewValue != null)
                (e.NewValue as IDialogViewModelBase).DoClose += DialogWindowBase_DoClose;
        };
    }

    private void DialogWindowBase_DoClose() {
        Close();
    }

    public DialogViewModelBase<U, C> GetViewModel<U, C>() where U : EntityBase<C> where C : DbContextBase, new() {
        return (DialogViewModelBase<U, C>)DataContext;
    }
}