using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using OGV.Admin.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OGV.Infrastructure.Interfaces;

namespace OGV.Admin.Views
{
    /// <summary>
    /// Interaction logic for AgendaView.xaml
    /// </summary>
    public partial class AgendaView : UserControl, INavigationAware, INotifyPropertyChanged
    {
        private IUnityContainer _container;
        private Point _lastMouseDown;
        private IRegionManager _regionManager;
        private IUserViewModel _user;
        [InjectionConstructor]
        public AgendaView(IUnityContainer container, IUserViewModel userModel)
        {
            InitializeComponent();

            _regionManager =
              Microsoft.Practices.ServiceLocation.ServiceLocator.
                                  Current.GetInstance<Microsoft.
                                  Practices.Prism.Regions.IRegionManager>();
            _container = container;
            _user = userModel;
            this.DataContext = _user.BoardList.SelectedAgenda;
        }

        public Point LastMouseDown
        {
            get { return _lastMouseDown; }
            set { _lastMouseDown = value; OnPropertyChanged("LastMouseDown"); }
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            var view = _regionManager.Regions["NavBarRegion"].Views.FirstOrDefault();
            if (view != null && view is AgendaNavView)
                _regionManager.Regions["NavBarRegion"].Remove(view);

            this.DataContext = null;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //show the Board NAV View in the NAV region
            Uri nn = new Uri(typeof(Views.AgendaNavView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("NavBarRegion", nn);

            this.DataContext = _user.BoardList.SelectedAgenda;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        #region Drag and Drop

        private AgendaItem _target;

        private AgendaItem draggedItem;

        private int dropIndex;

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Point currentPosition = e.GetPosition(tvAgenda);

                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                   (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    // Verify that this is a valid drop and then store the drop target
                    IParent item = GetNearestItem(e);
                    if (CheckDropTarget(draggedItem, item))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
            }
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                IParent TargetItem = GetNearestItem(e);
                if (TargetItem != null && draggedItem != null)
                {
                    e.Effects = DragDropEffects.Move;
                    draggedItem.Parent.RemoveItem(draggedItem);
                    if (TargetItem is Agenda)
                    {
                        TargetItem.InsertItem(draggedItem, dropIndex);
                    }
                    if (TargetItem is AgendaItem)
                    {
                        TargetItem.AddItem(draggedItem);
                    }
                    

                } 
            }
            catch (Exception)
            {
            }
        }

        private IParent GetNearestItem(DragEventArgs e)
        {

            // Verify that this is a valid drop and then store the drop target
            //translate screen point to be relative to ItemsControl
            Point point = e.GetPosition(tvAgenda);
            //find the item at that point
            var item = tvAgenda.InputHitTest(point) as FrameworkElement;
           


            if(item != null)
            {
                if(item.DataContext != null)
                {
                    if(item.DataContext is Agenda)
                    {
                        //the drop occurred in the tree not on a node
                        //find out where in the tree it could be the 
                        var rec = VisualTreeHelper.GetContentBounds(item);
                        double topEdge = 0;
                        double bottomEdge = rec.Height;
                       

                        //is it between nodes
                        var itemAbove = tvAgenda.InputHitTest(new Point(point.X, point.Y - 10)) as FrameworkElement;
                        var itemBelow = tvAgenda.InputHitTest(new Point(point.X, point.Y + 10)) as FrameworkElement;

                        

                        if ( (itemAbove != null && itemAbove.DataContext is AgendaItem) && 
                            (itemBelow != null && itemBelow.DataContext is AgendaItem))
                        {
                            //this is happening between nodes
                            dropIndex = (item.DataContext as IParent).IndexOf(itemBelow.DataContext as AgendaItem);
                            System.Diagnostics.Debug.WriteLine(string.Format("Between nodes - dropIndex = {0}", dropIndex));
                
                        }
                        else
                        {
                            
                            //this is happening at the top or bottom
                            //of the tree
                            
                            double midPoint = rec.Height / 2;
                            if (point.Y > midPoint)
                            {
                                //insert at the end
                                dropIndex = (item.DataContext as Agenda).Items.Count;
                            }
                            else
                            {
                                //insert at the head
                                dropIndex = 0;
                            }

                            System.Diagnostics.Debug.WriteLine(string.Format("Outside of nodes - dropIndex = {0}", dropIndex));
                        }
                    }
                       
                }
            }

            IParent TargetItem = item.DataContext as IParent;
            return TargetItem;
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(tvAgenda);
            }
        }

        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(tvAgenda);

                    if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    {
                        draggedItem = (AgendaItem) tvAgenda.SelectedItem;
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect =
                                            DragDrop.DoDragDrop(tvAgenda, tvAgenda.SelectedValue,
                                DragDropEffects.Move);
                            //Checking target is not null and item is
                            //dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) &&(_target != null))
                            {
                                // A Move drop was accepted
                                if (!draggedItem.Equals(_target))
                                {
                                    CopyItem(draggedItem, _target);
                                    _target = null;
                                    draggedItem = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void addChild(AgendaItem _sourceItem, AgendaItem _targetItem)
        {
            

           
        }

        static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                TObject found = parent as TObject;
                if (found != null)
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }


       
        private void CopyItem(AgendaItem sourceItem, IParent targetItem)
        {
            if(targetItem is AgendaItem)
            {
                if (MessageBox.Show("Would you like to drop " + sourceItem.Title + " into " + (targetItem as AgendaItem).Title + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        //adding dragged TreeViewItem in target TreeViewItem
                        addChild(sourceItem, targetItem as AgendaItem);

                        //finding Parent TreeViewItem of dragged TreeViewItem 
                        IParent ParentItem = sourceItem.Parent;
                        // if parent is null then remove from TreeView else remove from Parent TreeViewItem
                        if (ParentItem == null)
                        {
                            tvAgenda.Items.Remove(sourceItem);
                        }
                        else
                        {
                            ((AgendaItem)ParentItem).Items.Remove(sourceItem);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            if (targetItem is Agenda)
            {
                IParent sourceItemParent = sourceItem.Parent;
                sourceItemParent.RemoveItem(sourceItem);
                targetItem.InsertItem(sourceItem, dropIndex);
                
            }
           
        }

        private bool CheckDropTarget(AgendaItem _sourceItem, IParent _targetItem)
        {
            //Check whether the target item is meeting your condition
            bool _isEqual = false;
            if (!_sourceItem.Equals(_targetItem))
            {
                _isEqual = true;
            }
            return _isEqual;
        }

        #endregion Drag and Drop

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged
    }
}