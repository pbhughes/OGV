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
    public class PublishingPointMonitor: INotifyPropertyChanged, IPublishingPointMonitor
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
            set { _state = value; OnPropertyChanged("State"); ResetButtons(); }
        }

        public DelegateCommand CheckStateCommand { get; private set; }
        public DelegateCommand StreamCommand { get; private set; }

        public PublishingPointMonitor(IUserViewModel user, IRegionManager regionManager)
        {

            _regionManager = regionManager;
            _user = user;
            _user.BoardList.AgendaSelectedEvent += BoardList_AgendaSelectedEvent;
            CheckStateCommand = new DelegateCommand(OnCheckState, CanCheckState);
            StreamCommand = new DelegateCommand(OnStream, CanStream);
        }

        void BoardList_AgendaSelectedEvent(IAgenda selected)
        {
            ResetButtons();
            State = string.Empty;
        }

        private void ResetButtons()
        {
            CheckStateCommand.RaiseCanExecuteChanged();
            StreamCommand.RaiseCanExecuteChanged();
        }

        private bool CanStream()
        {
            if (_user == null)
                return false;

            if (_user.BoardList == null)
                return false;

            if (_user.BoardList.SelectedAgenda == null)
                return false;

            if (string.IsNullOrEmpty(_user.BoardList.SelectedAgenda.PublishingPoint))
                return false;

            if (string.IsNullOrEmpty(State))
                return false;

            if (State != "Idle")
                return false;

            return true;
        }

        private void OnStream()
        {
            //navigate to the streamer control
            Uri vv = new Uri(typeof(Views.StreamerView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("SidebarRegion", vv);

            //navigate to the streamer control
            Uri xx = new Uri(typeof(Views.StreamerNavView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate("SideNavBarRegion", xx);
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
                await ResetState(_user.BoardList.SelectedAgenda.PublishingPoint, _user.UserName, _user.Password);
                await RequestState(_user.BoardList.SelectedAgenda.PublishingPoint, _user.UserName, _user.Password);

                //navigate to the streamer view

                if (State == "Idle")
                {


                    Uri vv = new Uri(typeof(Views.StreamerView).FullName, UriKind.RelativeOrAbsolute);
                    _regionManager.RequestNavigate("SidebarRegion", vv);

                }
                else
                {
                    //I guess we error and full stop
                }
            });

           
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

        private async Task ResetState(string url, string userName, string password)
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

                    
                    var requestBody = new StringBuilder();
                    //build the request for a state change
                    requestBody.Append(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                    requestBody.Append(@"<entry xmlns=""http://www.w3.org/2005/Atom"">");
                    requestBody.Append(string.Format(@"<updated>{0}</updated>", DateTime.Now.ToOADate()));
                    requestBody.Append(@"<content type=""application/xml"">");
                    requestBody.Append(
                        @"<SmoothStreaming xmlns=""http://schemas.microsoft.com/iis/media/2011/03/streaming/management"">");
                    requestBody.Append(string.Format(@"<State><Value>{0}</Value></State>", "Idle"));
                    requestBody.Append(@"</SmoothStreaming>");
                    requestBody.Append(@"</content>");
                    requestBody.Append(@"</entry>");
                    HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Put, url);
                    msg.Content = new StringContent(requestBody.ToString());
                    msg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");
                    var response = await client.SendAsync(msg);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //got a bad response error
                        Message = string.Format("Failed to change state: {0}", response.ReasonPhrase);
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
