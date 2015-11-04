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
using forms = System.Windows.Forms;
using OGV2P.AgendaModule.Interfaces;

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
