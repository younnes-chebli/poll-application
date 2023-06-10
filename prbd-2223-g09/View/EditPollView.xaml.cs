using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll.View
{
    /// <summary>
    /// Interaction logic for NewPollView.xaml
    /// </summary>
    public partial class EditPollView : UserControlBase
    {
        private readonly EditPollViewModel _vm;

        public EditPollView(Poll poll, bool isNew)
        {
            InitializeComponent();
            DataContext = _vm = new EditPollViewModel(poll, isNew);
        }
    }
}
