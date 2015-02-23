using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Practices.Prism.Regions;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using OGV.Infrastructure.Interfaces;

namespace OGV.Streaming.Models
{
    public class PublishingPointMonitor: INotifyPropertyChanged
    {

        private IRegionManager _regionManager;
        private IUserViewModel _user;

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged("State"); }
        }

        public DelegateCommand CheckStateCommand { get; private set; }

        public PublishingPointMonitor(IUserViewModel user)
        {
            _regionManager =
               Microsoft.Practices.ServiceLocation.ServiceLocator.
                                   Current.GetInstance<Microsoft.
                                   Practices.Prism.Regions.IRegionManager>();

            _user = user;

            CheckStateCommand = new DelegateCommand(OnCheckState, CanCheckState);
        }

        private bool CanCheckState()
        {
            if (_user == null)
                return false;

            if (_user.BoardList == null)
                return false;

            if (_user.BoardList.SelectedAgenda == null)
                return false;

            return !string.IsNullOrEmpty(_user.BoardList.SelectedAgenda.PublishingPoint);
        }

        private void OnCheckState()
        {
            IsBusy = true;
            Message = "Getting Publishing Point State...";

            //_regionManager.RegisterViewWithRegion("SidebarRegion", typeof(OGV.Streaming.Views.StreamerView));

            Task x = Task.Factory.StartNew(async () =>
            {
                await RequestState(_user.BoardList.SelectedAgenda.PublishingPoint, _user.UserName, _user.Password);
            });

            //navigate to the streamer view

            if (State == "Idle")
            {


                Uri vv = new Uri(typeof(Views.StreamerView).FullName, UriKind.RelativeOrAbsolute);
                _regionManager.RequestNavigate("SidebarRegion", vv);

            }
        }

        private async Task RequestState(string url, string userName, string password)
        {
            url = url + "/state";

            try
            {
                using (var client = new HttpClient())
                {

                    client.Timeout = new TimeSpan(0, 0, 5);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @": Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/json");

                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, password));
                    var header = new AuthenticationHeaderValue(
                               "Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Authorization = header;

                    var response = await client.GetAsync(url);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //got a bad response error
                        Message = string.Format("Failed to get state: {0}", response.ReasonPhrase);
                        IsBusy = true;
                        
                    }
                    else
                    {
                        string x = await response.Content.ReadAsStringAsync();
                        var xDoc = XDocument.Parse(x);
                        State = xDoc.Descendants().Skip(12).First().Value;
                        IsBusy = false;
                      

                    }


                }
            }
            catch (Exception ex)
            {
                IsBusy = true;
                Message = ex.Message;
            }

        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
