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

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Point currentPosition = e.GetPosition(tvAgenda);

                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                   (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    // Verify that this is a valid drop and then store the drop target
                    AgendaItem item = GetNearestItem(e);
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
            catch (Exception)
            {
            }
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                AgendaItem TargetItem = GetNearestItem(e);
                if (TargetItem != null && draggedItem != null)
                {
                    e.Effects = DragDropEffects.Move;
                    draggedItem.Parent.RemoveItem(draggedItem);
                    TargetItem.AddItem(draggedItem);

                }
            }
            catch (Exception)
            {
            }
        }

        private AgendaItem GetNearestItem(DragEventArgs e)
        {

            // Verify that this is a valid drop and then store the drop target
            //translate screen point to be relative to ItemsControl
            Point point = e.GetPosition(tvAgenda);
            //find the item at that point
            var item = tvAgenda.InputHitTest(point) as FrameworkElement;

            AgendaItem TargetItem = item.DataContext as AgendaItem;
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


       
        private void CopyItem(AgendaItem _sourceItem, AgendaItem _targetItem)
        {
            if (MessageBox.Show("Would you like to drop " + _sourceItem.Title + " into " + _targetItem.Title + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    //adding dragged TreeViewItem in target TreeViewItem
                    addChild(_sourceItem, _targetItem);

                    //finding Parent TreeViewItem of dragged TreeViewItem 
                    IParent ParentItem = _sourceItem.Parent;
                    // if parent is null then remove from TreeView else remove from Parent TreeViewItem
                    if (ParentItem == null)
                    {
                        tvAgenda.Items.Remove(_sourceItem);
                    }
                    else
                    {
                        ((AgendaItem)ParentItem).Items.Remove(_sourceItem);
                    }
                }
                catch
                {

                }
            }
        }

        private bool CheckDropTarget(AgendaItem _sourceItem, AgendaItem _targetItem)
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