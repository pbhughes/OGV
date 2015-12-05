﻿using System.Windows;
using System.Windows.Controls;
using forms = System.Windows.Forms;
using Infrastructure.Interfaces;
using System.Windows.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows;
using Infrastructure.Models;

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
        private forms.ContextMenuStrip _docMenu;


        public AgendaStartView(IMeeting meeting, ISession sessionService)
        {
            InitializeComponent();

            Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged;
            
            if (agendaTree == null)
            {
                agendaTree = new forms.TreeView();
            }

            agendaTree.ItemDrag += AgendaTree_ItemDrag;
            agendaTree.DragEnter += AgendaTree_DragEnter;
            agendaTree.DragOver += AgendaTree_DragOver;
            agendaTree.DragDrop += AgendaTree_DragDrop;
           

            agendaTree.AllowDrop = true;
            agendaTree.ShowNodeToolTips = true;
            agendaTree.ShowPlusMinus = true;
            agendaTree.ItemHeight = agendaTree.ItemHeight * 2;
            agendaTree.HotTracking = true;
            


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

            if (File.Exists(@"Images\left_arrow.png"))
            {
                var brush = new System.Windows.Media.ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/left_arrow.png", UriKind.Relative));
                cmdLeft.Background = brush;

            }
            else
            {
                cmdLeft.Content = "Left";
            }

            if (File.Exists(@"Images\up_arrow.png"))
            {
                var brush = new System.Windows.Media.ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/up_arrow.png", UriKind.Relative));
                cmdUp.Background = brush;
            }
            else
            {
                cmdLeft.Content = "Up";
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

           

            agendaTree.ImageList = _treeImages;
            agendaTree.AfterSelect += agendaTree_AfterSelect;
            _currentMeeting = meeting;
            _sessionService = sessionService;
            _sessionService.RaiseStamped += _sessionService_RaiseStamped;
            _currentMeeting.RaiseMeetingSetEvent += _currentMeeting_RaiseMeetingSetEvent;

            DataContext = _currentMeeting;
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            floater.IsOpen = false;
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
            

            Item item = new Infrastructure.Models.Item() { Title = "Please add a title..." };
            forms.TreeNode tn = new forms.TreeNode() { Text = item.Title, ToolTipText = item.Title };
            tn.ContextMenuStrip = _docMenu;
            var selectedNode = agendaTree.SelectedNode;
            selectedNode.Nodes.Add(tn);
            _currentMeeting.AddNode(item, _currentMeeting.SelectedItem);
            agendaTree.SelectedNode = tn;
           

        }

        private void _sessionService_RaiseStamped(System.TimeSpan sessionTime)
        {
            MarkItemStamped();

        }

        private void _sessionService_ClearStamp()
        {
            UnstampItem();
        }

        private void MarkItemStamped()
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

        private void UnstampItem()
        {
            if (_currentMeeting.IsBusy)
            {
                if (agendaTree.SelectedNode != null)
                {
                    if (agendaTree.ImageList.Images.Count >= 2)
                    {
                        if (agendaTree.SelectedNode.ImageKey.ToLower().Contains("edited"))
                        {
                            agendaTree.SelectedNode.ImageKey = "unstamped_edited";
                            agendaTree.SelectedNode.SelectedImageKey = "unstamped_edited";
                        }
                        else
                        {
                            agendaTree.SelectedNode.ImageKey = "unselected";
                            agendaTree.SelectedNode.SelectedImageKey = "unselected";
                        }
                    }
                    _currentMeeting.SelectedItem.TimeStamp = TimeSpan.Zero;
                }
            }
        }

        void agendaTree_AfterSelect(object sender, forms.TreeViewEventArgs e)
        {
            forms.TreeNode selectedNode = ((forms.TreeView)sender).SelectedNode;
            _currentMeeting.SelectedItem = _currentMeeting.FindItem(selectedNode.Text.GetHashCode());
            if(!floater.IsOpen)
            {
                floater.IsOpen = true;
            }
            //position the floater
            floater.HorizontalOffset = agendaTree.Size.Width / 2  + (agendaTree.Width / 4);
            floater.VerticalOffset = agendaTree.Location.Y + 120;
        }

        private void agendaTree_DoubleClick(object sender, EventArgs e)
        {

            _sessionService.Stamp();
            MarkItemStamped();
           
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
            MarkItemStamped();
            _currentMeeting.SelectedItem.TimeStamp = _sessionService.CurrentVideoTime;
        }

        private void Unstamp_Click(object sender, EventArgs e)
        {
            UnstampItem();
        }

        private void agendaTree_MouseUp(object sender, forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                agendaTree.SelectedNode = agendaTree.GetNodeAt(e.X, e.Y);
        }

        private void frmHost_MouseMove(object sender, MouseEventArgs e)
        {

            if (!floater.IsOpen)
            {
                floater.IsOpen = true;
            }

            System.Windows.Point currentPos = e.GetPosition(treePanel);
            System.Diagnostics.Debug.WriteLine(string.Format("Window Mouse Move x:{0} y:{1}", currentPos.X, currentPos.Y));
        }

        private void frmHost_MouseLeave(object sender, MouseEventArgs e)
        {
          
            System.Windows.Point currentPos = e.GetPosition(treePanel);
            System.Diagnostics.Debug.WriteLine(string.Format("Window Mouse Move x:{0} y:{1}", currentPos.X, currentPos.Y));
        }

        private void floaterClose_Click(object sender, RoutedEventArgs e)
        {
            floater.IsOpen = false;
        }

        private void floaterMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            if(agendaTree.SelectedNode != null && agendaTree.SelectedNode.Parent != null)
            {
                forms.TreeNodeCollection collection = null;
                if (agendaTree.SelectedNode.Parent.Parent == null)
                {
                    collection = agendaTree.Nodes;
                }
                else
                {
                    collection = agendaTree.SelectedNode.Parent.Parent.Nodes;
                }
                MoveLeft(agendaTree.SelectedNode,collection);
            }
        }

        private void floaterMoveUp_Click(object sender, RoutedEventArgs e)
        {
           
            if(agendaTree.SelectedNode != null &&  agendaTree.SelectedNode.Index > 0)
            {
                forms.TreeNodeCollection collection = null;
                if (agendaTree.SelectedNode.Parent == null)
                {
                    collection = agendaTree.Nodes;
                }
                else
                {
                    collection = agendaTree.SelectedNode.Parent.Nodes;
                }

                MoveUp(agendaTree.SelectedNode, agendaTree.SelectedNode.PrevNode, collection);

            }
        }

        private void floaterMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if(agendaTree.SelectedNode != null)
            {
                forms.TreeNodeCollection collection = null;
                if(agendaTree.SelectedNode.Parent == null)
                {
                    collection = agendaTree.Nodes;
                }
                else
                {
                    collection = agendaTree.SelectedNode.Parent.Nodes;
                }

                MoveDown(agendaTree.SelectedNode, agendaTree.SelectedNode.NextNode, collection);
            }
        }

        private void MoveUp(forms.TreeNode moving, forms.TreeNode pivot, forms.TreeNodeCollection collection)
        {
            if(moving.Index > 0)
            {
                var temp = moving;
                pivot.Remove();
                collection.Insert(moving.Index + 1, pivot);
            }
        }

        private void MoveDown(forms.TreeNode moving, forms.TreeNode pivot, forms.TreeNodeCollection collection)
        {
            if (moving.Index < collection.Count - 1)
            {
                moving.Remove();
                collection.Insert(pivot.Index + 1, moving);
            }
        }

        private void MoveLeft(forms.TreeNode moving, forms.TreeNodeCollection colleciton)
        {

            int index = moving.Parent.Index;
            moving.Remove();
            colleciton.Insert(index + 1, moving);
        }

        private void MoveRight(forms.TreeNode moving, forms.TreeNodeCollection collection)
        {

        }
    }
}
