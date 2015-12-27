using System;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.UnityExtensions;
using System.Windows;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Practices.Prism.Regions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace OGV2P
{
    public class BootStrapper : UnityBootstrapper
    {
        private ISession _session;
        private IBoardList _boardList;
        private IUser _user;
        private IRegionManager _regionManager;
        private Window _shell;
        private IMeeting _meeting;
        

        protected override System.Windows.DependencyObject CreateShell()
        {
            _shell = this.Container.Resolve<Shell>();
            return _shell;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            OGV2P.App.Current.MainWindow = (Window)Shell;
            _shell = OGV2P.App.Current.MainWindow;
            OGV2P.App.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(OGV2P.Admin.AdminModule));
            moduleCatalog.AddModule(typeof(OGV2P.AgendaModule.AgendaModule));

        }
        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
            

        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            ConfigureServiceLocator();

            this.Container.RegisterType<object, OGV2P.Admin.Views.CameraView>(typeof(OGV2P.Admin.Views.CameraView).FullName);
            this.Container.RegisterType<object, OGV2P.Admin.Views.LoginView>(typeof(OGV2P.Admin.Views.LoginView).FullName);
            this.Container.RegisterType<object, OGV2P.AgendaModule.Views.AgendaStartView>(typeof(OGV2P.AgendaModule.Views.AgendaStartView).FullName);
            this.Container.RegisterType<object, OGV2P.Admin.Views.ServicesView>(typeof(OGV2P.Admin.Views.ServicesView).FullName);
            this.Container.RegisterType<IAgendaSelector, Infrastructure.Models.AgendaSelector>();
            this.Container.RegisterType<Infrastructure.Interfaces.IDevices, Infrastructure.Models.Devices>();
            

            _boardList = LoadBoards();
            _session = new Session( );
            _user = new User(_session, _boardList);
            _meeting = new Meeting(_session, _user);
            
            
           

            _user.RaiseLoginEvent += _user_RaiseLoginEvent;
            this.Container.RegisterInstance<ISession>(_session);
            this.Container.RegisterInstance<IUser>(_user);
            this.Container.RegisterInstance<IMeeting>(_meeting);
            this.Container.RegisterInstance<IBoardList>(_boardList);
            

        }

        void _user_RaiseLoginEvent(object sender, EventArgs e)
        {
            _regionManager = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Microsoft.Practices.Prism.Regions.IRegionManager>();

            var loginView = _regionManager.Regions[Infrastructure.Models.Regions.Middle].GetView("LoginView");
            _regionManager.Regions[Infrastructure.Models.Regions.Middle].Deactivate ( loginView );

            ((Shell)_shell).SetSideBarAllignmentTop();
            
            Uri vv = new Uri(typeof(OGV2P.AgendaModule.Views.AgendaStartView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate(Infrastructure.Models.Regions.Main, vv);

            Uri uu = new Uri(typeof(OGV2P.Admin.Views.CameraView).FullName, UriKind.RelativeOrAbsolute);
            _regionManager.RequestNavigate(Infrastructure.Models.Regions.SideBar, uu);
        }

        private IBoardList LoadBoards()
        {
            try
            {
                BoardList boards = new BoardList();
                string boardsxml = File.ReadAllText("boards.xml");
              
                XDocument xdoc = XDocument.Parse(boardsxml, LoadOptions.PreserveWhitespace );
                foreach (XElement org in xdoc.Element("organizations").Elements("org"))
                {
                    Board x = new Board();
                    x.Name = org.Element("name").Value;
                    x.State = org.Element("state").Value;
                    x.City = org.Element("city").Value;
                    x.UserID = org.Element("ftpserver").Element("username").Value;
                    x.Password = org.Element("ftpserver").Element("password").Value;
                    x.FtpServer = org.Element("ftpserver").Element("host").Value;
                    x.FtpPath = org.Element("ftpserver").Element("dir").Value;
                    x.RequireLogin = ( org.Element("ftpserver").Element("login") == null)? false : bool.Parse( org.Element("ftpserver").Element("login").Value);
                    boards.AddBoard(x);
                }
                return boards;
            }
            catch (Exception ex)
            {

                throw new Exception("Error processing the boards.xml file", ex);
            }
            
        }
    }
}
