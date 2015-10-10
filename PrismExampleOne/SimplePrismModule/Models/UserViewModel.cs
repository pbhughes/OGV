using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using OGV.Infrastructure.Interfaces;
using System.Windows.Controls;
using System.Net;
using System.Net.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using OGV.Infrastructure.Interfaces;
using System.IO;

namespace OGV.Admin.Models
{

    public class UserViewModel: INotifyPropertyChanged, IUserViewModel
    {
        private IUnityContainer _container;
        public IUnityContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }

        private IXService _xService;

        private IRegionManager _regionManager;

        private int _callTime;
        public int CallTime
        {
            get { return _callTime; }
            set { _callTime = value; OnPropertyChanged("CallTime"); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }

        private IBoardList _boardList;
        public IBoardList BoardList
        {
            get { return _boardList; }
            set { _boardList = value; OnPropertyChanged("BoardList"); }
        }

        public DelegateCommand<PasswordBox> LoginCommand { get; private set; }

        private async void OnLogin(PasswordBox pbox)
        {
            try
            {
                //Authenticate against the web service async and reject or navigate to
                //Board Selection view
                Message = "Authenticating...";
                IsBusy = true;

                _password = pbox.Password;
                string token = await Authenticate(_userName, _password);
               
                if (token == null)
                    return;

                OGV.Infrastructure.Model.Session.Instance.Token = token;

                ////down load all the board files
                //Message = "Downloading Agenda Files...";
                //List<IAgenda> files = await DownLoadAgendaFiles(); //get the URL from the authentication token

                ////load all the board files
                //Message = "Loading Agenda Files...";
                //await LoadAgendaFiles();

                IsBusy = false;
                //show the BoardView in the main region
                Uri vv = new Uri(typeof(Views.BoardView).FullName, UriKind.RelativeOrAbsolute);
                _regionManager.RequestNavigate("MainRegion", vv);

                //show the Board NAV View in the NAV region
                Uri nn = new Uri(typeof(Views.BoardNavView).FullName, UriKind.RelativeOrAbsolute);
                _regionManager.RequestNavigate("NavBarRegion", nn);
            }
            catch (Exception ex)
            {

                Message = string.Format("Login for {0} failed, please try again", _userName);
                IsBusy = true;
            }
            finally
            {
                OnLoggedIn();
            }
           
        }

        private bool CanLogin(PasswordBox pbox)
        {
            return ! OGV.Infrastructure.Model.Session.Recording ;
        }

        public UserViewModel()
        {

            this.LoginCommand = new DelegateCommand<PasswordBox>(OnLogin, CanLogin);
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();
            _boardList = new BoardList();
        }

        public UserViewModel(IUnityContainer container, IXService xService)
        {

            this.LoginCommand = new DelegateCommand<PasswordBox>(OnLogin, CanLogin);
            _container = container;
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();
            _boardList = new BoardList();
            _xService = xService;
            
        }

        private async Task<string> Authenticate(string userName, string password)
        {
            Guid token;
            //hash them and send it to the server
            //get a token back and store it on the session
            Task t =  Task.Run( async  () =>
            {
                
                string url = string.Format("http://{0}/{1}/{2}",_xService.BaseUrl, "Authentication", "login.html");
                using (var client = new HttpClient()){
                    client.Timeout = new TimeSpan(0, 0, 5);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @": Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html");
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, password));

                     var header = new AuthenticationHeaderValue(
                               "Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Authorization = header;

                    var response = await client.GetAsync(url);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //got a bad response error
                        //throw new UnauthorizedAccessException(response.ReasonPhrase);
                        
                    }
                    else
                    {
                        
                    }
                }
            });


            await t;

            return Guid.NewGuid().ToString();

        }

        private async Task<List<IAgenda>> DownLoadAgendaFiles()
        {
            Guid token;
            //hash them and send it to the server
            //get a token back and store it on the session
            Task t = Task.Run(async () =>
            {

                string url = string.Format("http://{0}/{1}/Manifest.xml", _xService.BaseUrl, _xService.BoardFolder);
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 5);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @": Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/xml");
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _userName, _password));

                    var header = new AuthenticationHeaderValue(
                              "Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Authorization = header;

                    var response = await client.GetAsync(url);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //got a bad response error
                        throw new UnauthorizedAccessException(response.ReasonPhrase);

                    }
                    else
                    {
                        //we got back a manifest file process it
                        string manifestXml = await response.Content.ReadAsStringAsync();
                        XDocument xDoc = XDocument.Parse(manifestXml);
                        foreach (var fileElement in xDoc.Element("manifest").Elements("file"))
                        {
                            string link = fileElement.Attribute("link") == null ? null : fileElement.Attribute("link").Value;
                            string folder = fileElement.Attribute("directory") == null ? null : fileElement.Attribute("directory").Value;
                            string fileName = fileElement.Attribute("filename") == null ? null : fileElement.Attribute("filename").Value;
                            string strfileDate = fileElement.Attribute("filedate") == null ? null : fileElement.Attribute("filedate").Value;
                            DateTime? remoteFileDate = null;
                            if (!string.IsNullOrEmpty(strfileDate))
                                remoteFileDate = DateTime.Parse(strfileDate);

                            using (var fileClient = new HttpClient())
                            {
                                Message = string.Format("Downloading file {0}", fileName);
                                fileClient.Timeout = new TimeSpan(0, 0, 5);
                                fileClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @": Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)");
                                fileClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/xml");
                                var fileByteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _userName, _password));

                                var fileRequestHeader = new AuthenticationHeaderValue(
                                          "Basic", Convert.ToBase64String(fileByteArray));
                                fileClient.DefaultRequestHeaders.Authorization = header;

                                var fileGetResponse = await fileClient.GetAsync(link);
                                if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    //we had a problem
                                    Message = string.Format("Unable to download file {0} error {1}", link, response.ReasonPhrase);
                                    System.Threading.Thread.Sleep(2000);
                                }
                                else
                                {
                                    string fileText = await fileGetResponse.Content.ReadAsStringAsync();
                                    string executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                                    string agendaPath = Path.Combine(executingDirectory, "Agendas");
                                    if (!Directory.Exists(agendaPath))
                                        Directory.CreateDirectory(agendaPath);

                                    string currentFileDirectory = Path.Combine(agendaPath, folder);
                                    if (!Directory.Exists(currentFileDirectory))
                                        Directory.CreateDirectory(currentFileDirectory);

                                    string localFilePath = Path.Combine(currentFileDirectory, fileName);
                                    
                                    if(File.Exists(localFilePath))
                                    {
                                        //read it get all the text and calculate a hash
                                        int localFileHash = File.ReadAllText(localFilePath).GetHashCode();
                                        int remoteFileHash = fileText.GetHashCode();

                                        if (localFileHash != remoteFileHash)
                                        {
                                            //the files are different compare the dates
                                            FileInfo localFileInfo = new FileInfo(localFilePath);
                                            if (remoteFileDate > localFileInfo.LastWriteTime)
                                                ;
                                        }
                                    }
                                    File.WriteAllText(localFilePath,fileText);

                                    
                                }
                            }

                        }
                    }
                }


            });

            await t;

            return new List<IAgenda>();

        }

        private async Task<string> LoadAgendaFiles()
        {
            Guid token;
            //hash them and send it to the server
            //get a token back and store it on the session
            Task t = Task.Run(() =>
            {
                for (int i = 0; i < 2; i++)
                {
                    Dispatcher.CurrentDispatcher.Invoke((Action)delegate() { CallTime++; });
                    System.Threading.Thread.Sleep(1000);

                    
                }
                _boardList.Load();
            });



            await t;

            return Guid.NewGuid().ToString();

        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        public event EventHandler LoggedIn;

        public void OnLoggedIn()
        {
            if(LoggedIn != null)
                LoggedIn(this, new EventArgs());
        }

     

       
    }
}
