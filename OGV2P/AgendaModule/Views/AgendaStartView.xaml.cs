using Infrastructure.ExtendedObjects;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using forms = System.Windows.Forms;
using Infrastructure.Enumerations;

namespace OGV2P.AgendaModule.Views
{
    /// <summary>
    /// Interaction logic for ChooseAgendaView.xaml
    /// </summary>
    public partial class AgendaStartView : UserControl, INavigationAware
    {
        private IUnityContainer _container;
        private IMeeting _currentMeeting;
        private ISession _sessionService;
        private IUser _user;
        private forms.ImageList _treeImages = new forms.ImageList();
        private forms.ContextMenuStrip _docMenu;
        private forms.TreeView agendaTree = new ExtendedTreeView();
        private ExtendedTreeNode target = null;

        public AgendaStartView(IMeeting meeting, ISession sessionService, IUser user, IUnityContainer container)
        {
            InitializeComponent();

            agendaTree.ItemDrag += AgendaTree_ItemDrag;
            agendaTree.DragEnter += AgendaTree_DragEnter;
            agendaTree.DragOver += AgendaTree_DragOver;
            agendaTree.DragDrop += AgendaTree_DragDrop;
            agendaTree.DragLeave += AgendaTree_DragLeave;

            agendaTree.KeyDown += agendaTree_KeyDown;
            agendaTree.MouseUp += agendaTree_MouseUp;

            agendaTree.NodeMouseDoubleClick += agendaTree_NodeMouseDoubleClick;
            agendaTree.HideSelection = false;
            agendaTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.Normal;
            agendaTree.DrawNode += AgendaTree_DrawNode;
            agendaTree.FullRowSelect = true;

            agendaTree.AllowDrop = true;
            agendaTree.ShowNodeToolTips = true;
            agendaTree.ShowPlusMinus = true;
            agendaTree.HotTracking = true;
            agendaTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            agendaTree.Dock = System.Windows.Forms.DockStyle.Fill;
            agendaTree.ShowRootLines = true;

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
            _sessionService.RaiseLoggedOut += _sessionService_RaiseLoggedOut;
            DataContext = _currentMeeting;

            winFormHost.Child.Controls.Add(agendaTree);
        }

        private void AgendaTree_DragLeave(object sender, EventArgs e)
        {
            ((ExtendedTreeView)agendaTree).InvalidateNodeMarker();
        }

        private void _sessionService_RaiseLoggedOut()
        {
            txtMeetingName.Text = string.Empty;
            dteMeetingDate.Text = string.Empty;
            _currentMeeting = null;
        }

        private void AgendaTree_DrawNode(object sender, forms.DrawTreeNodeEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Drawing Node: {0}", e.Node.Text));

            e.Graphics.DrawRectangle(SystemPens.Control, e.Bounds);

            forms.TextRenderer.DrawText(e.Graphics,
                                   e.Node.Text,
                                   e.Node.TreeView.Font,
                                   e.Node.Bounds,
                                   e.Node.ForeColor);
        }

        private void Meeting_RaiseMeetingItemChanged(IItem item)
        {
            if (agendaTree != null)
                if (agendaTree.Nodes.Count > 0)
                {
                    forms.TreeNode[] selection = agendaTree.Nodes.Find(item.ID, true);
                    if (selection.Length > 0)
                    {
                        selection[0].Text = item.Title;
                        if (item.TimeStamp == TimeSpan.Zero)
                            ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemUnstamped();
                        else
                            ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemStamped(txtTitle.Text, false);
                    }
                }
        }


        private void AgendaTree_DragOver(object sender, forms.DragEventArgs e)
        {
            ((ExtendedTreeView)agendaTree).Dragging = true;

            // Retrieve the client coordinates of the mouse position.
            System.Drawing.Point mouseLocation = agendaTree.PointToClient(new System.Drawing.Point(e.X, e.Y));

            //capture the previous node
            ExtendedTreeNode oldTarget = target;

            // Select the node at the mouse position.
            target = (ExtendedTreeNode)agendaTree.GetNodeAt(mouseLocation);

            if (target != null)
            {
                (agendaTree as ExtendedTreeView).MarkNode(target, true, mouseLocation);

            }

            if (oldTarget == null || target == null || target == oldTarget)
            {
                ;
            }
            else
            {
                oldTarget.BackColor = Color.Transparent;
                System.Diagnostics.Debug.WriteLine(string.Format("Target Bounds x: {0} y: {1} x2: {2} y2: {3}",
                    target.Bounds.X, target.Bounds.Y,
                    target.Bounds.X + target.Bounds.Width,
                    target.Bounds.Y + target.Bounds.Height));
            }

            System.Diagnostics.Debug.WriteLine(string.Format("Drag Over: target={0} x{1} y {2}",
                (target == null) ? "target null" : ((ExtendedTreeNode)target).AgendaItem.Title, e.X.ToString(), e.Y.ToString()));
        }

        private void AgendaTree_DragEnter(object sender, forms.DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;

            // Retrieve the client coordinates of the mouse position.
            System.Drawing.Point targetPoint = agendaTree.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Select the node at the mouse position.
            var target = (ExtendedTreeNode)agendaTree.GetNodeAt(targetPoint);



            System.Diagnostics.Debug.WriteLine(string.Format("Drag enter: target={0}", (target == null) ? "target null" : ((ExtendedTreeNode)target).AgendaItem.Title));
        }

        private void AgendaTree_ItemDrag(object sender, forms.ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.
            if (e.Button == forms.MouseButtons.Left)
            {
                agendaTree.DoDragDrop(e.Item, forms.DragDropEffects.Move);
                
            }
        }

        private void AgendaTree_DragDrop(object sender, forms.DragEventArgs e)
        {
            //clear the line
            ((ExtendedTreeView)agendaTree).InvalidateNodeMarker();

            // Retrieve the client coordinates of the drop location.
            System.Drawing.Point targetPoint = agendaTree.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            ExtendedTreeNode targetNode = agendaTree.GetNodeAt(targetPoint) as ExtendedTreeNode;

            // Retrieve the node that was dragged.
            ExtendedTreeNode draggedNode = (ExtendedTreeNode)e.Data.GetData(typeof(ExtendedTreeNode));

            // Confirm that the node at the drop location is not
            // the dragged node or a descendant of the dragged node.
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                // If it is a move operation, remove the node from its current
                // location and add it to the node at the drop location.
                if (e.Effect == forms.DragDropEffects.Move)
                {
                    draggedNode.Remove();
                   switch(( (ExtendedTreeView)agendaTree ).DropLocation)
                    {
                        case DropLocations.Top:
                            DropDraggedNodeAboveTaretNode(draggedNode, targetNode);
                            break;
                        case DropLocations.Middle:
                            DropDraggedNodeIntoTargetNode(draggedNode, targetNode);
                            break;
                        case DropLocations.Bottom:
                            DropDraggedNodeBelowTargetNode(draggedNode, targetNode);
                            break;
                    }

                    draggedNode.BackColor = Color.Transparent;
                    targetNode.BackColor = Color.Transparent;
                    
                }

                // If it is a copy operation, clone the dragged node
                // and add it to the node at the drop location.
                else if (e.Effect == forms.DragDropEffects.Copy)
                {
                    targetNode.Nodes.Add((ExtendedTreeNode)draggedNode.Clone());
                }

                // Expand the node at the location
                // to show the dropped node.
                targetNode.Expand();

                //select the dragged node
                agendaTree.SelectedNode = draggedNode;
            }
        }

        private void DropDraggedNodeBelowTargetNode(ExtendedTreeNode draggedNode, ExtendedTreeNode targetNode)
        {
            System.Diagnostics.Debug.WriteLine("Dropping below target node");

            if(targetNode == null || targetNode.Parent == null)
            {
                agendaTree.Nodes.Add( draggedNode);
            }
            else
            {
                targetNode.Parent.Nodes.Insert(targetNode.Index + 1, draggedNode);
            }
               
              
        }

        private void DropDraggedNodeIntoTargetNode(ExtendedTreeNode draggedNode, ExtendedTreeNode targetNode)
        {
            System.Diagnostics.Debug.WriteLine("Dropping into target node");
            targetNode.Nodes.Add(draggedNode);
        }

        private void DropDraggedNodeAboveTaretNode(ExtendedTreeNode draggedNode, ExtendedTreeNode targetNode)
        {
            System.Diagnostics.Debug.WriteLine("Dropping above target node");
            if (targetNode.Parent == null)
            {
                agendaTree.Nodes.Insert(targetNode.Index, draggedNode);
            }
            else
            {
                targetNode.Parent.Nodes.Insert(targetNode.Index, draggedNode);
            }
           
           
        }

        

        // Determine whether one node is a parent
        // or ancestor of a second node.
        private bool ContainsNode(ExtendedTreeNode node1, ExtendedTreeNode node2)
        {
            if(node2 != null)
            {
                // Check the parent node of the second node.
                if (node2.Parent == null) return false;
                if (node2.Parent.Equals(node1)) return true;

                // If the parent node is not null or equal to the first node,
                // call the ContainsNode method recursively using the parent of
                // the second node.
                return ContainsNode(node1, node2.Parent as ExtendedTreeNode);
            }
            return false;
          
        }

        private void _currentMeeting_RaiseMeetingSetEvent(object sender, EventArgs e)
        {
            if (agendaTree != null)
                if (agendaTree.Nodes.Count > 0)
                {
                    agendaTree.SelectedNode = agendaTree.Nodes[0];
                    foreach (ExtendedTreeNode tn in agendaTree.Nodes)
                    {
                        AssignContextMenu(tn);
                    }
                }
        }

        private void AssignContextMenu(ExtendedTreeNode tn)
        {
            foreach (ExtendedTreeNode tnSub in tn.Nodes)
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
            ExtendedTreeNode tn = new ExtendedTreeNode() { Text = item.Title, ToolTipText = item.Title, AgendaItem = item };

            //add the context menu
            tn.ContextMenuStrip = _docMenu;

            //add the node to the data source
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
            if (ie.DialogResult.HasValue && ie.DialogResult.Value == true)
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
            node.Remove();
        }

        private void _sessionService_ClearStamp()
        {
            ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemUnstamped();
        }

        private void agendaTree_AfterSelect(object sender, forms.TreeViewEventArgs e)
        {
            ExtendedTreeNode selectedNode = (ExtendedTreeNode)((forms.TreeView)sender).SelectedNode;
            _currentMeeting.SelectedItem = selectedNode.AgendaItem;
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

        private void Stamp_Click(object sender, EventArgs e)
        {
            ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemStamped(txtTitle.Text);
            _currentMeeting.SelectedItem.TimeStamp = _sessionService.CurrentVideoTime;
        }

        private void Unstamp_Click(object sender, EventArgs e)
        {
            ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemUnstamped();
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

        private void MoveUp(ExtendedTreeNode moving, ExtendedTreeNode pivot, forms.TreeNodeCollection collection)
        {
            if (moving.Index > 0)
            {
                var temp = moving;
                pivot.Remove();
                collection.Insert(moving.Index + 1, pivot);
            }
            else
            {
                if (moving.Parent == pivot)
                {
                    moving.Remove();
                    collection.Add(moving);
                }
            }
            moving.EnsureVisible();
        }

        private void MoveDown(ExtendedTreeNode moving, ExtendedTreeNode pivot, forms.TreeNodeCollection collection)
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
                    ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemStamped(txtTitle.Text);
                    break;

                case forms.Keys.Space:
                    _sessionService.Stamp();
                    ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemStamped(txtTitle.Text);
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
            if (agendaTree != null)
            {
                if (agendaTree.Nodes.Count > 0)
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

                _container.RegisterInstance<IMeeting>(_currentMeeting);
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
            ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemStamped(txtTitle.Text);
            e.Node.Expand();
        }

        private void SaveAgenda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = string.Format("Do you want to publish agenda file {0}?", _currentMeeting.MeetingName);
                string caption = string.Format("Publish to board  {0}", _user.SelectedBoard.Name);
                var result = Xceed.Wpf.Toolkit.MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
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

        private void ClearStamp(ExtendedTreeNode etn)
        {
            etn.BackColor = Color.Transparent;
            etn.MarkItemUnstamped();
            if (etn.AgendaItem != null)
            {
                etn.SetTimeStamp(TimeSpan.Zero);
                ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemUnstamped();
            }

            foreach (ExtendedTreeNode node in etn.Nodes)
            {
                ClearStamp(node);
            }
        }

        private void ClearStamps_Click(object sender, RoutedEventArgs e)
        {
            foreach (ExtendedTreeNode etn in agendaTree.Nodes)
            {
                ClearStamp(etn);
                etn.BackColor = Color.Transparent;
            }
        }

        private void agendaCommandDropDown_Click(object sender, RoutedEventArgs e)
        {
            if (!_currentMeeting.IsBusy)
            {
                GetAgendaFromServer_Click(sender, e);
            }
            else
            {
                agendaCommandDropDown.IsOpen = !agendaCommandDropDown.IsOpen;
            }
        }

        #region Navigation Awareness

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion Navigation Awareness
    }
}