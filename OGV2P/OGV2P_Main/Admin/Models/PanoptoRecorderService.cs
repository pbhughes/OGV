using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace OGV2P.Admin.Models
{
    public class PanoptoService 
    {

        private ServiceController _controller;
        private string _serviceName;

        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value;  }

        }

        public ServiceControllerStatus Status
        {
            get
            {
                return _controller.Status;
            }
        }

        public PanoptoService(string serviceName)
        {
            _serviceName = serviceName;
            _controller = new ServiceController(_serviceName);
        }

        public virtual async void Stop()
        {
            try
            {
                await Task.Run(() =>
                {
                    if (_controller.Status != ServiceControllerStatus.Stopped)
                        _controller.Stop();

                    _controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 1, 0));
                });

            }
            catch (Exception ex) 
            {
                
                throw;
            }
        }

        public virtual async void Start()
        {
            try
            {
                await Task.Run(() =>
                {
                    if (_controller.Status != ServiceControllerStatus.Running)
                        _controller.Start();

                    _controller.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 1, 0));
                });

            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
