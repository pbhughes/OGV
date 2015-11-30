using System.Windows;
using System.Windows.Controls;
using forms = System.Windows.Forms;
using Infrastructure.Interfaces;
using System.Windows.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Input;

namespace OGV2P.AgendaModule.Views
{
    /// <summary>
    /// Interaction logic for ChooseAgendaView.xaml
    /// </summary>
    public partial class AgendaStartView : UserControl
    {
        private IMeeting _currentMeeting;
        private ISession _sessionService;
        private forms.ImageList _treeImages = new forms.ImageList();
       

        public AgendaStartView(IMeeting meeting, ISession sessionService)
        {
            InitializeComponent();
            if (agendaTree == null)
            {
                agendaTree = new forms.TreeView();
            }

         


            agendaTree.ShowNodeToolTips = true;
            agendaTree.ShowPlusMinus = true;
            agendaTree.ItemHeight = agendaTree.ItemHeight * 2; 

            if (File.Exists(@"Images\unselected.png"))
            {
                System.Drawing.Image img1 = System.Drawing.Image.FromFile(@"Images\unselected.png");
                _treeImages.Images.Add("unselected", img1);
            }

            if (File.Exists(@"Images\green_check.jpeg"))
            {
                System.Drawing.Image img2 = System.Drawing.Image.FromFile(@"Images\green_check.jpeg");
                _treeImages.Images.Add("stamped", img2);

            }

            if (File.Exists(@"Images\stamped_edited.png"))
            {
                System.Drawing.Image img3 = System.Drawing.Image.FromFile(@"Images\stamped_edited.png");
                _treeImages.Images.Add("stamped_edited", img3);
            }


            if (File.Exists(@"Images\unstamped_edited.png"))
            {
                System.Drawing.Image img4 = System.Drawing.Image.FromFile(@"Images\unstamped_edited.png");
                _treeImages.Images.Add("unstamped_edited", img4);

            }

            agendaTree.ImageList = _treeImages;
            agendaTree.AfterSelect += agendaTree_AfterSelect;
            _currentMeeting = meeting;
            _sessionService = sessionService;
            _sessionService.RaiseStamped += _sessionService_RaiseStamped;
            _currentMeeting.RaiseMeetingSetEvent += _currentMeeting_RaiseMeetingSetEvent;

            DataContext = _currentMeeting;
        }

     
        private void _currentMeeting_RaiseMeetingSetEvent(object sender, EventArgs e)
        {
            if (agendaTree != null)
                if (agendaTree.Nodes.Count > 0)
                    agendaTree.SelectedNode = agendaTree.Nodes[0];
        }

        private void _sessionService_RaiseStamped(System.TimeSpan sessionTime)
        {
            MarkStampedItem();

        }

        private void MarkStampedItem()
        {
            if (_currentMeeting.IsBusy)
            {
                if (agendaTree.SelectedNode != null)
                {
                    if (agendaTree.ImageList.Images.Count >= 2)
                    {
                        if (agendaTree.SelectedNode.ImageKey.ToLower().Contains("edited"))
                        {
                            agendaTree.SelectedNode.ImageKey = "stamped_edited";
                            agendaTree.SelectedNode.SelectedImageKey = "stamped_edited";
                        }
                        else
                        {
                            agendaTree.SelectedNode.ImageKey = "stamped";
                            agendaTree.SelectedNode.SelectedImageKey = "stamped";
                        }
                    }

                }
            }
        }

        void agendaTree_AfterSelect(object sender, forms.TreeViewEventArgs e)
        {
            forms.TreeNode selectedNode = ((forms.TreeView)sender).SelectedNode;
            _currentMeeting.SelectedItem = _currentMeeting.FindItem(selectedNode.Text.GetHashCode());
        }

        private void agendaTree_DoubleClick(object sender, EventArgs e)
        {

            _sessionService.Stamp();
            MarkStampedItem();
           
        }

        private void DescriptionBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }
        

        private void txtTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                string newTitle = txtTitle.Text;
                agendaTree.SelectedNode.Text = newTitle;
                if(agendaTree.SelectedNode.ImageKey == "stamped")
                {
                    agendaTree.SelectedNode.ImageKey = "stamped_edited";
                    agendaTree.SelectedNode.SelectedImageKey = "stamped_edited";
                }
                else
                {
                    agendaTree.SelectedNode.ImageKey = "unstamped_edited";
                    agendaTree.SelectedNode.SelectedImageKey = "unstamped_edited";
                }
                
            }
        }
    }
}
