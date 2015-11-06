using System.Windows;
using System.Windows.Controls;
using forms = System.Windows.Forms;
using Infrastructure.Interfaces;

namespace OGV2P.AgendaModule.Views
{
    /// <summary>
    /// Interaction logic for ChooseAgendaView.xaml
    /// </summary>
    public partial class AgendaStartView : UserControl
    {
        private IMeeting _currentMeeting;
        

        public AgendaStartView( IMeeting meeting )
        {
            InitializeComponent();
            if (agendaTree == null)
            {
                agendaTree = new forms.TreeView();
            }
            agendaTree.AfterSelect += agendaTree_AfterSelect;
            _currentMeeting = meeting;
            DataContext = _currentMeeting;
        }

        void agendaTree_AfterSelect(object sender, forms.TreeViewEventArgs e)
        {
            forms.TreeNode selectedNode = ((forms.TreeView)sender).SelectedNode;
            _currentMeeting.FindItem(selectedNode.Text.GetHashCode());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
