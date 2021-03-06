﻿using Infrastructure.Enumerations;
using Infrastructure.ExtendedObjects;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using forms = System.Windows.Forms;

namespace OGV2P.AgendaModule.Views
{
    /// <summary>
    /// Interaction logic for ChooseAgendaView.xaml
    /// </summary>
    public partial class AgendaStartView : UserControl, INavigationAware, INotifyPropertyChanged
    {
        private IUnityContainer _container;
        private ISession _sessionService;
        private IUser _user;
        private forms.ImageList _treeImages = new forms.ImageList();
        private forms.ContextMenuStrip _docMenu;
        private ExtendedTreeView agendaTree = new ExtendedTreeView();
        private ExtendedTreeNode target = null;

        private IMeeting _currentMeeting;
        public IMeeting CurrentMeeting
        {
            get
            {
                return _currentMeeting;
            }

            set
            {
                _currentMeeting = value;
                OnPropertyChanged("CurrentMeeting");
            }
        }

        public AgendaStartView(IUser user, IUnityContainer container)
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

            _container = container;
            agendaTree.ImageList = _treeImages;
            agendaTree.AfterSelect += agendaTree_AfterSelect;

            _sessionService = _container.Resolve<ISession>();
            CurrentMeeting = _container.Resolve<IMeeting>();
            _user = user;

            CurrentMeeting.RaiseMeetingSetEvent += _currentMeeting_RaiseMeetingSetEvent;
            _sessionService.RaiseLoggedOut += _sessionService_RaiseLoggedOut;
            _sessionService.RaiseStopRecording += _sessionService_RaiseStopRecording;
            DataContext = CurrentMeeting;

            winFormHost.Child.Controls.Add(agendaTree);
        }

        private void _sessionService_RaiseStopRecording(object sender, EventArgs e)
        {
            SaveAgenda_Click(sender, new RoutedEventArgs());
        }

        private void AgendaTree_DragLeave(object sender, EventArgs e)
        {
            ((ExtendedTreeView)agendaTree).InvalidateNodeMarker();
        }

        private void _sessionService_RaiseLoggedOut()
        {
            txtMeetingName.Text = string.Empty;
            dteMeetingDate.Text = string.Empty;
            CurrentMeeting = null;
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

        private void AgendaTree_DragOver(object sender, forms.DragEventArgs e)
        {
            //support  auto scroll while dragging
            ((ExtendedTreeView)agendaTree).Scroll();

            //turn on drag graphics
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
                    switch (((ExtendedTreeView)agendaTree).DropLocation)
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

            if (targetNode == null || targetNode.Parent == null)
            {
                agendaTree.Nodes.Add(draggedNode);
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
            if (node2 != null)
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
            _docMenu.Opening += DocMenu_Opening;
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
            _docMenu.Opening += DocMenu_Opening;
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
            CurrentMeeting.SelectedItem = item;

            Item temp = CurrentMeeting.SelectedItem;
            ItemEditor ie = new ItemEditor(_container, CurrentMeeting);
            ie.ShowDialog();
            if (ie.DialogResult.HasValue && ie.DialogResult.Value == true)
            {
                tn.Text = CurrentMeeting.SelectedItem.Title;
            }
            else
            {
                ;//forget it
            }
        }

        private void DocMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (forms.ToolStripItem i in ((System.Windows.Forms.ContextMenuStrip)sender).Items)
            {
                if (CurrentMeeting.IsBusy)
                {
                    i.Enabled = true;
                }
                else
                {
                    if (i.Text == "Stamp" || i.Text == "Clear Stamp")
                    {
                        i.Enabled = false;
                    }
                }
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
            if(CurrentMeeting == null)
            {
                CurrentMeeting = _container.Resolve<IMeeting>();
            }
            CurrentMeeting.SelectedItem = selectedNode.AgendaItem;
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
                CurrentMeeting.SelectedItem.Title = newTitle;
                CurrentMeeting.SelectedItem.Description = txtDescription.Text;
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
            System.Diagnostics.Debug.WriteLine(string.Format("Session Service INit AgendaView Time: {0}", _sessionService.InitializationTime.ToShortTimeString()));
            if (CurrentMeeting.IsBusy)
            {
                ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemStamped(txtTitle.Text, (int)_sessionService.CurrentVideoTime.TotalSeconds);
                CurrentMeeting.WriteAgendaFile(agendaTree);
            }
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
                    if (CurrentMeeting.IsBusy)
                    {
                        _sessionService.Stamp();
                        ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemStamped(txtTitle.Text, (int)_sessionService.CurrentVideoTime.TotalSeconds);
                        CurrentMeeting.WriteAgendaFile(agendaTree);
                    }
                    break;

                case forms.Keys.Space:
                    if (CurrentMeeting.IsBusy)
                    {
                        _sessionService.Stamp();
                        ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemStamped(txtTitle.Text, (int)_sessionService.CurrentVideoTime.TotalSeconds);
                        CurrentMeeting.WriteAgendaFile(agendaTree);
                    }
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
                dg.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OGV2", "Agendas");
                if (!Directory.Exists(dg.InitialDirectory))
                    Directory.CreateDirectory(dg.InitialDirectory);

                if (dg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string xml = ReadAndValidateXML(dg.FileName);
                    CurrentMeeting.LocalAgendaFileName = dg.FileName;
                    XDocument xDoc = XDocument.Parse(xml);
                    xDoc.Save(CurrentMeeting.LocalAgendaFileName);
                    CurrentMeeting.ParseAgendaFile(agendaTree, xml);
                }
            }
            catch (XmlException xmlEx)
            {
                xmlEx.WriteToLogFile();
                Xceed.Wpf.Toolkit.MessageBox.Show(
                    string.Format("Validation error in the agenda document at line: {0} position: {1}.",
                    xmlEx.LineNumber, xmlEx.LinePosition), "OpenGoVideo - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                Xceed.Wpf.Toolkit.MessageBox.Show("Unable to get the agenda file, ensure the board is setup correctly.", "OpenGoVideo - Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        
                        
                        string xml = ReadAndValidateXML(dg.FilePath);
                        CurrentMeeting.LocalAgendaFileName = dg.FilePath;
                        XDocument xDoc = XDocument.Parse(xml);
                        xDoc.Save(CurrentMeeting.LocalAgendaFileName);
                        CurrentMeeting.ParseAgendaFile(agendaTree, xml);
                        this.DataContext = CurrentMeeting;
                       
                    }
                }
            }
            catch(XmlException xmlEx)
            {
                xmlEx.WriteToLogFile();
                Xceed.Wpf.Toolkit.MessageBox.Show(
                    string.Format("Validation error in the agenda document at line: {0} position: {1}.",
                    xmlEx.LineNumber, xmlEx.LinePosition), "OpenGoVideo - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                Xceed.Wpf.Toolkit.MessageBox.Show("Unable to get and or parse the agenda file, ensure the board is setup correctly on the server.", "OpenGoVideo - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string ReadAndValidateXML(string filePath)
        {
            XmlReaderSettings rdrSettings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment };

            using (XmlReader rdr = XmlReader.Create(filePath, rdrSettings))
            {
                XDocument xDoc = XDocument.Load(rdr);
                return xDoc.ToString();
            }
        }

        private void agendaTree_NodeMouseDoubleClick(object sender, forms.TreeNodeMouseClickEventArgs e)
        {
            if (CurrentMeeting.IsBusy)
            {
                ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemStamped(txtTitle.Text, (int) _sessionService.CurrentVideoTime.TotalSeconds);
                CurrentMeeting.WriteAgendaFile(agendaTree);
                e.Node.Expand();
            }
            
        }

        private void SaveAgenda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long bytesWritten = CurrentMeeting.WriteAgendaFile((ExtendedTreeView)agendaTree);
                string message = string.Format("Do you want to publish agenda file {0}?", CurrentMeeting.MeetingName);
                string caption = string.Format("Publish to board  {0}", _user.SelectedBoard.Name);
                var result = Xceed.Wpf.Toolkit.MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    

                    SaveAgendaFileDialog dg = new SaveAgendaFileDialog(_container);

                    dg.ShowDialog();
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
           
            if (etn.AgendaItem != null)
            {
                agendaTree.SelectedNode = etn;
                etn.SetTimeStamp(0);
                ((ExtendedTreeNode)agendaTree.SelectedNode).MarkItemUnstamped();
            }

            foreach (ExtendedTreeNode node in etn.Nodes)
            {

                ClearStamp(node);
                agendaTree.SelectedNode = node;
            }
        }

        private void ClearStamps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (ExtendedTreeNode etn in agendaTree.Nodes)
                {
                    agendaTree.SelectedNode = etn;
                    ClearStamp(etn);
                    etn.BackColor = Color.Transparent;
                    
                }
                if(agendaTree != null)
                    CurrentMeeting.WriteAgendaFile((ExtendedTreeView)agendaTree);

                agendaCommandDropDown.IsOpen = false;
            }
            catch (Exception ex)
            {
                ex.WriteToLogFile();
                throw;
            }
           
        }

        private void agendaCommandDropDown_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentMeeting.IsBusy)
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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private void AgendaViewLoaded(object sender, RoutedEventArgs e)
        {
            GetAgendaFromServer_Click(this, new RoutedEventArgs());
        }

       
    }
}