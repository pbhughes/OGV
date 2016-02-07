using System.Windows;
using System.Windows.Controls;
using forms = System.Windows.Forms;
using Infrastructure.Interfaces;
using System.Windows.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Infrastructure.Models;
using Microsoft.Practices.Unity;
using System.Xml.Linq;
using Infrastructure.Extensions;

namespace OGV2P.AgendaModule.Views
{
    /// <summary>
    /// Interaction logic for ChooseAgendaView.xaml
    /// </summary>
    public partial class AgendaStartView : UserControl
    {
        private IUnityContainer _container;
        private IMeeting _currentMeeting;
        private ISession _sessionService;
        private IUser _user;
        private forms.ImageList _treeImages = new forms.ImageList();
        private forms.ContextMenuStrip _docMenu;
        private forms.TreeView agendaTree = new forms.TreeView();

        public AgendaStartView(IMeeting meeting, ISession sessionService, IUser user, IUnityContainer container)
        {
            InitializeComponent();

            Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged;
            Application.Current.MainWindow.Deactivated += MainWindow_Deactivated;
            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;

            agendaTree.ItemDrag += AgendaTree_ItemDrag;
            agendaTree.DragEnter += AgendaTree_DragEnter;
            agendaTree.DragOver += AgendaTree_DragOver;
            agendaTree.DragDrop += AgendaTree_DragDrop;
            agendaTree.KeyDown += agendaTree_KeyDown;
            agendaTree.MouseUp += agendaTree_MouseUp;
            agendaTree.NodeMouseDoubleClick += agendaTree_NodeMouseDoubleClick;
            agendaTree.BeforeSelect += AgendaTree_BeforeSelect;
            agendaTree.HideSelection = false;
            agendaTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            agendaTree.DrawNode += AgendaTree_DrawNode;
            

            agendaTree.AllowDrop = true;
            agendaTree.ShowNodeToolTips = true;
            agendaTree.ShowPlusMinus = true;
            agendaTree.HotTracking = true;
            agendaTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            agendaTree.Dock = System.Windows.Forms.DockStyle.Fill;
            agendaTree.ShowRootLines = true;

            meeting.RaiseMeetingItemChanged += Meeting_RaiseMeetingItemChanged;

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


            if (File.Exists(@"Images\down_arrow.png"))
            {
                var brush = new System.Windows.Media.ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/down_arrow.png", UriKind.Relative));
                cmdDown.Background = brush;
            }
            else
            {
                cmdDown.Content = "Down";
            }


            _container = container;
            agendaTree.ImageList = _treeImages;
            agendaTree.AfterSelect += agendaTree_AfterSelect;
            _currentMeeting = meeting;
            _sessionService = sessionService;
            _user = user;
            _currentMeeting.RaiseMeetingSetEvent += _currentMeeting_RaiseMeetingSetEvent;

            DataContext = _currentMeeting;

            winFormHost.Child.Controls.Add(agendaTree);
           
        }

        private void AgendaTree_DrawNode(object sender, forms.DrawTreeNodeEventArgs e)
        {
            Brush redBrush = Brushes.Red;
            Brush greenBrush = Brushes.Green;

            if (e.Node.IsSelected)
            {
                if (agendaTree.Focused)
                    e.Graphics.FillRectangle(greenBrush, e.Bounds);
                else
                    e.Graphics.FillRectangle(redBrush, e.Bounds);
            }
            else
            {
                Item i = _currentMeeting.FindItem(e.Node.Name);
                if (i.TimeStamp == TimeSpan.Zero)
                    e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                else
                    e.Graphics.FillRectangle(Brushes.CadetBlue, e.Bounds);
            }
                

            e.Graphics.DrawRectangle(SystemPens.Control, e.Bounds);

            forms.TextRenderer.DrawText(e.Graphics,
                                   e.Node.Text,
                                   e.Node.TreeView.Font,
                                   e.Node.Bounds,
                                   e.Node.ForeColor);
        }

        private void AgendaTree_BeforeSelect(object sender, forms.TreeViewCancelEventArgs e)
        {
           
        }

        private void Meeting_RaiseMeetingItemChanged(IItem item)
        {

            if(agendaTree != null)
                if(agendaTree.Nodes.Count > 0)
                {
                    forms.TreeNode[] selection = agendaTree.Nodes.Find(item.ID, true);                  
                    if(selection.Length > 0)
                    {
                        selection[0].Text = item.Title;
                        if (item.TimeStamp == TimeSpan.Zero)
                            UnstampItem(agendaTree.SelectedNode);
                        else
                            MarkItemStamped(agendaTree.SelectedNode, false);
                    }
                        
                }

        

           
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
           
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
        }

        private void AgendaTree_DragDrop(object sender, forms.DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            System.Drawing.Point targetPoint = agendaTree.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            forms.TreeNode targetNode = agendaTree.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            forms.TreeNode draggedNode = (forms.TreeNode)e.Data.GetData(typeof(forms.TreeNode));

            // Confirm that the node at the drop location is not 
            // the dragged node or a descendant of the dragged node.
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                // If it is a move operation, remove the node from its current 
                // location and add it to the node at the drop location.
                if (e.Effect == forms.DragDropEffects.Move)
                {
                    draggedNode.Remove();
                    targetNode.Nodes.Add(draggedNode);
                }

                // If it is a copy operation, clone the dragged node 
                // and add it to the node at the drop location.
                else if (e.Effect == forms.DragDropEffects.Copy)
                {
                    targetNode.Nodes.Add((forms.TreeNode)draggedNode.Clone());
                }

                // Expand the node at the location 
                // to show the dropped node.
                targetNode.Expand();

                //select the dragged node
                agendaTree.SelectedNode = draggedNode;
            }
        }

        // Determine whether one node is a parent 
        // or ancestor of a second node.
        private bool ContainsNode(forms.TreeNode node1, forms.TreeNode node2)
        {
            // Check the parent node of the second node.
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            // If the parent node is not null or equal to the first node, 
            // call the ContainsNode method recursively using the parent of 
            // the second node.
            return ContainsNode(node1, node2.Parent);
        }

        private void AgendaTree_DragOver(object sender, forms.DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.
            System.Drawing.Point targetPoint = agendaTree.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Select the node at the mouse position.
            agendaTree.SelectedNode = agendaTree.GetNodeAt(targetPoint);

            

            System.Diagnostics.Debug.WriteLine(agendaTree.SelectedNode.Name);
        }

        private void AgendaTree_DragEnter(object sender, forms.DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void AgendaTree_ItemDrag(object sender, forms.ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.
            if (e.Button == forms.MouseButtons.Left)
            {
                
                agendaTree.DoDragDrop(e.Item, forms.DragDropEffects.Move);
            }

            // Copy the dragged node when the right mouse button is used.
            else if (e.Button == forms.MouseButtons.Right)
            {
                agendaTree.DoDragDrop(e.Item, forms.DragDropEffects.Copy);
            }
        }

        

        private void _currentMeeting_RaiseMeetingSetEvent(object sender, EventArgs e)
        {
            if (agendaTree != null)
                if (agendaTree.Nodes.Count > 0)
                {
                    agendaTree.SelectedNode = agendaTree.Nodes[0];
                    foreach( forms.TreeNode tn in agendaTree.Nodes)
                    {
                        AssignContextMenu(tn);
                    }

                }

        }

        private void AssignContextMenu(forms.TreeNode tn)
        {
           
            foreach(forms.TreeNode tnSub in tn.Nodes)
            {
                AssignContextMenu(tnSub);

            }

            _docMenu = new System.Windows.Forms.ContextMenuStrip();
            forms.ToolStripMenuItem stamp = new forms.ToolStripMenuItem("Stamp");
            stamp.Click += Stamp_Click;
            forms.ToolStripMenuItem unstamp = new forms.ToolStripMenuItem("Clear Stamp");
            unstamp.Click += Unstamp_Click;
            forms.ToolStripMenuItem insert = new forms.ToolStripMenuItem("Insert Item");
            insert.Click += Insert_Click;

            _docMenu.Items.Add(insert);
            _docMenu.Items.Add(stamp);
            _docMenu.Items.Add(unstamp);
            tn.ContextMenuStrip = _docMenu;


        }

        private void Insert_Click(object sender, EventArgs e)
        {
            InsertAnItem(agendaTree.SelectedNode.Nodes);

        }

        private void InsertAnItem(forms.TreeNodeCollection collection)
        {
            
            //build the context menu
            _docMenu = new System.Windows.Forms.ContextMenuStrip();
            forms.ToolStripMenuItem stamp = new forms.ToolStripMenuItem("Stamp");
            stamp.Click += Stamp_Click;
            forms.ToolStripMenuItem unstamp = new forms.ToolStripMenuItem("Clear Stamp");
            unstamp.Click += Unstamp_Click;
            forms.ToolStripMenuItem insert = new forms.ToolStripMenuItem("Insert Item");
            insert.Click += Insert_Click;
            forms.ToolStripMenuItem delete = new forms.ToolStripMenuItem("Delete Item");
            delete.Click += Delete_Click;

            _docMenu.Items.Add(insert);
            _docMenu.Items.Add(stamp);
            _docMenu.Items.Add(unstamp);
            _docMenu.Items.Add(delete);

            //build up the item and the visual node
            Item item = new Infrastructure.Models.Item() { Title = "Please add a title..." };
            item.ID = _currentMeeting.NextID().ToString();
            forms.TreeNode tn = new forms.TreeNode() { Text = item.Title, ToolTipText = item.Title };
            tn.Name = item.ID.ToString(); 


            //add the context menu
            tn.ContextMenuStrip = _docMenu;

            //add the node to the data source
            _currentMeeting.AddNode(item, _currentMeeting.MeetingAgenda.Items);
            collection.Add(tn);
           

            if (tn.Parent != null)
                tn.Parent.Expand();

            txtTitle.Focus();
            txtTitle.SelectAll();

            agendaTree.SelectedNode = tn;
            _currentMeeting.SelectedItem = item;

            Item temp = _currentMeeting.SelectedItem;
            ItemEditor ie = new ItemEditor(_container, _currentMeeting);
            ie.ShowDialog();
            if(ie.DialogResult.HasValue && ie.DialogResult.Value == true)
            {
                tn.Text = _currentMeeting.SelectedItem.Title;
            }
            else
            {
                ;//forget it
            }

        }

        private void Delete_Click(object sender, EventArgs e)
        {
            var node = agendaTree.SelectedNode;
            Item i = _currentMeeting.FindItem(node.Name);
            node.Remove();
            
        }

       

      
        private void _sessionService_ClearStamp()
        {
            UnstampItem(agendaTree.SelectedNode);
        }

        private void MarkItemStamped(forms.TreeNode targetNode, bool advance = true)
        {
            if (agendaTree.ImageList.Images.Count >= 2)
            {
                if (targetNode.ImageKey.ToLower().Contains("edited"))
                {
                    targetNode.ImageKey = "stamped_edited";
                    targetNode.SelectedImageKey = "stamped_edited";
                }
                else
                {
                    targetNode.ImageKey = "stamped";
                    targetNode.SelectedImageKey = "stamped";
                }
            }
            string newTitle = txtTitle.Text;
            targetNode.Text = newTitle;
            targetNode.BackColor = Color.LightBlue;
            
            if (advance)
            {


                //Check for children if they exist go to them
                if (targetNode.Nodes.Count > 0)
                {
                    //current node has children move to the first
                    agendaTree.SelectedNode = agendaTree.SelectedNode.Nodes[0];
                    return;
                }
                else
                {
                    //Check for siblings if they exist go to them

                    //Current node doesn't have children
                    //is there a sibling node next
                    if (targetNode.NextNode != null)
                    {
                        agendaTree.SelectedNode = targetNode.NextNode;
                        return;
                    }
                    else
                    {

                        forms.TreeNode parent = targetNode.Parent;
                        if (parent != null)
                        {
                            while (parent.NextNode == null)
                            {
                                parent = parent.Parent;
                            }
                            agendaTree.SelectedNode = parent.NextNode;
                        }

                    }
                }
            }
        }

        private void UnstampItem(forms.TreeNode targetNode)
        {
            if (agendaTree.ImageList.Images.Count >= 2)
            {
                if (targetNode.ImageKey.ToLower().Contains("edited"))
                {
                    targetNode.ImageKey = "unstamped_edited";
                    targetNode.SelectedImageKey = "unstamped_edited";
                }
                else
                {
                    targetNode.ImageKey = "unselected";
                    targetNode.SelectedImageKey = "unselected";
                    
                }
            }

            targetNode.BackColor = Color.Transparent;

        }

        void agendaTree_AfterSelect(object sender, forms.TreeViewEventArgs e)
        {
            forms.TreeNode selectedNode = ((forms.TreeView)sender).SelectedNode;
            _currentMeeting.SelectedItem = _currentMeeting.FindItem(selectedNode.Name);
         
        }

      

        private void DescriptionBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                string newDescription = txtDescription.Text;
                if (agendaTree.SelectedNode.ImageKey == "stamped")
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
        

        private void txtTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                string newTitle = txtTitle.Text;
                agendaTree.SelectedNode.Text = newTitle;
                agendaTree.SelectedNode.ToolTipText = txtDescription.Text;
                _currentMeeting.SelectedItem.Title = newTitle;
                _currentMeeting.SelectedItem.Description = txtDescription.Text;
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

        private void Stamp_Click(object sender, EventArgs e)
        {
            MarkItemStamped(agendaTree.SelectedNode);
            _currentMeeting.SelectedItem.TimeStamp = _sessionService.CurrentVideoTime;
        }

        private void Unstamp_Click(object sender, EventArgs e)
        {
            _currentMeeting.FindItem(agendaTree.SelectedNode.Name).TimeStamp = TimeSpan.Zero;
            UnstampItem(agendaTree.SelectedNode);
        }

        private void agendaTree_MouseUp(object sender, forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                agendaTree.SelectedNode = agendaTree.GetNodeAt(e.X, e.Y);
        }

        private void frmHost_MouseMove(object sender, MouseEventArgs e)
        {

            

            System.Windows.Point currentPos = e.GetPosition(winFormHost);
            System.Diagnostics.Debug.WriteLine(string.Format("Window Mouse Move x:{0} y:{1}", currentPos.X, currentPos.Y));
        }

        private void frmHost_MouseLeave(object sender, MouseEventArgs e)
        {
          
            System.Windows.Point currentPos = e.GetPosition(winFormHost);
            System.Diagnostics.Debug.WriteLine(string.Format("Window Mouse Move x:{0} y:{1}", currentPos.X, currentPos.Y));
        }

      


        private void floaterMoveUp_Click(object sender, RoutedEventArgs e)
        {

            forms.TreeNodeCollection col;
            forms.TreeNode pivot;

            if (agendaTree.SelectedNode.Index > 0)
                pivot = agendaTree.SelectedNode.PrevNode;
            else
            {
                pivot = agendaTree.SelectedNode.Parent;
            }

            if (agendaTree.SelectedNode.Parent == null)
            {
                //its a root node the collection is the tree nodes
                col = agendaTree.Nodes;
            }
            else if (agendaTree.SelectedNode.Parent == pivot)
            {
                if (pivot.Parent != null)
                {
                    col = pivot.Parent.Nodes;
                }
                else
                    col = agendaTree.Nodes;

            }
            else
            {
                col = agendaTree.SelectedNode.Parent.Nodes;
            }

            MoveUp(agendaTree.SelectedNode, pivot, col);
            agendaTree.Focus();
        }

        private void floaterMoveDown_Click(object sender, RoutedEventArgs e)
        {

            forms.TreeNodeCollection col;
            forms.TreeNode pivot;

            if(agendaTree.SelectedNode.NextNode != null)
            {
                pivot = agendaTree.SelectedNode.NextNode;
                if(agendaTree.SelectedNode.Parent == null)
                {
                    col = agendaTree.Nodes;
                }
                else
                {
                    col = agendaTree.SelectedNode.Parent.Nodes;
                }
                
            }
            else
            {
                if (agendaTree.SelectedNode.Parent == null)
                {
                    col = agendaTree.Nodes;
                    if(agendaTree.SelectedNode.Parent == null && agendaTree.SelectedNode.NextNode == null)
                    {
                        return; //you are at the bottom
                    }
                    else
                    {
                        pivot = agendaTree.SelectedNode.Parent.NextNode;

                    }
                }
                else
                {
                    col = agendaTree.SelectedNode.Parent.Nodes;
                    pivot = agendaTree.SelectedNode.Parent.NextNode;
                }
            }
            
            MoveDown(agendaTree.SelectedNode, pivot, col);
            agendaTree.Focus();
            
        }

        private void MoveUp(forms.TreeNode moving, forms.TreeNode pivot, forms.TreeNodeCollection collection)
        {
            if(moving.Index > 0)
            {
                var temp = moving;
                pivot.Remove();
                collection.Insert(moving.Index + 1, pivot);
            }

            else
            {
                if(moving.Parent == pivot)
                {
                    moving.Remove();
                    collection.Add(moving);
                }
            }
            moving.EnsureVisible();
            
        }


        private void MoveDown(forms.TreeNode moving, forms.TreeNode pivot, forms.TreeNodeCollection collection)
        {

            if (moving.Parent == pivot)
            {
                moving.Remove();
                collection.Add(moving);
            }
            else
            {
                var temp = moving;
                pivot.Remove();
                collection.Insert(moving.Index - 1, pivot);
            }

            moving.EnsureVisible();
        }

    

        private void agendaTree_KeyDown(object sender, forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case forms.Keys.Enter:
                    _sessionService.Stamp();
                    MarkItemStamped(agendaTree.SelectedNode);
                    break;
                case forms.Keys.Space:
                    _sessionService.Stamp();
                    MarkItemStamped(agendaTree.SelectedNode);
                    break;

            }

        }

     

        private void GetAgendaFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                forms.OpenFileDialog dg = new forms.OpenFileDialog();
                dg.DefaultExt = ".xml";
                dg.AddExtension = true;
                dg.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase", "Agendas");
                if (!Directory.Exists(dg.InitialDirectory))
                    Directory.CreateDirectory(dg.InitialDirectory);


                if (dg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _currentMeeting.LocalAgendaFileName = dg.FileName;
                    string allXml = File.ReadAllText(dg.FileName);
                    _currentMeeting.ParseAgendaFile(agendaTree, allXml);
                }

                

            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                Xceed.Wpf.Toolkit.MessageBox.Show("Unable to get the agenda file, ensure the board is setup correctly on the server.", "OpenGoVideo - Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private bool QuestionUserAboutAgenda()
        {
            if (_currentMeeting != null)
            {
                if (_currentMeeting.MeetingAgenda != null)
                {
                    if (_currentMeeting.MeetingAgenda.Items.Count > 0)
                    {

                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(
                            "You have an agenda loaded proceeding will clear all changes that have not been saved, Continue?",
                            "Warning possible data loss",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Stop);

                        if (result == MessageBoxResult.No)
                        {
                            return false;
                        }

                    }
                }
            }
            _currentMeeting.MeetingAgenda.Items.Clear();
            agendaTree.Nodes.Clear();
            return true;
        }

        private void GetAgendaFromServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (QuestionUserAboutAgenda() == true)
                {


                    GetAgendaFileDialog dg = new GetAgendaFileDialog(_user);
                    dg.ShowDialog();
                    if (dg.DialogResult.Value)
                    {
                        string xml = dg.AgendaXml;
                        _currentMeeting.LocalAgendaFileName = dg.FilePath;
                        XDocument xDoc = XDocument.Parse(xml);
                        xDoc.Save(_currentMeeting.LocalAgendaFileName);
                        _currentMeeting.ParseAgendaFile(agendaTree, xml);
                        
                    }

                }


            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                Xceed.Wpf.Toolkit.MessageBox.Show("Unable to get the agenda file, ensure the board is setup correctly on the server.", "OpenGoVideo - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
            
            
        }

        private void agendaTree_NodeMouseDoubleClick(object sender, forms.TreeNodeMouseClickEventArgs e)
        {
            _sessionService.Stamp();
            MarkItemStamped(agendaTree.SelectedNode);
            e.Node.Expand();
        }

        private void SaveAgenda_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string message = string.Format("Do you want to publish agenda file {0}", _currentMeeting.MeetingName);
                string caption = string.Format("Publish to board  {0}", _user.SelectedBoard.Name);
                var result = Xceed.Wpf.Toolkit.MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    long bytesWritten = _currentMeeting.WriteAgendaFile(agendaTree);
                    

                    SaveAgendaFileDialog dg = new SaveAgendaFileDialog(_container);

                    dg.ShowDialog();
                }
                else
                {
                    long bytesWritten = _currentMeeting.WriteAgendaFile(agendaTree);

                }

            }
            catch (Exception ex)
            {

                Xceed.Wpf.Toolkit.MessageBox.Show(string.Format("An error occurred while trying to save the agenda file to server.  Error: {0}", ex.Message));
            }
          
        }

        private void AddAgenadaItem_click(object sender, RoutedEventArgs e)
        {
            InsertAnItem(agendaTree.Nodes);
        }

        private void txtTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            txtTitle.SelectAll();
        }

        private void txtTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtTitle.SelectAll();
        }
    }
}
