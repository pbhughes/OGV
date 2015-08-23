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
using OGV2P.Agenda.Panopto.Session;

namespace OGV2P.Agenda.Views
{
    /// <summary>
    /// Interaction logic for ChooseAgendaView.xaml
    /// </summary>
    public partial class AgendaStartView : UserControl
    {
        public AgendaStartView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SessionManagementClient session = new SessionManagementClient();

            AuthenticationInfo authInfo = new AuthenticationInfo() { UserKey = "barkley", Password = "seri502/dupe" };
            Guid sessionGuid = new Guid(txtSessionGuid.Text);
            Session[] sessions = session.GetSessionsById(authInfo, new Guid[] { sessionGuid } );
            Session s = sessions[0];
            session.CreateNoteByAbsoluteTime(authInfo, sessionGuid, txtNote.Text, "", DateTime.Now);
        }
    }
}
