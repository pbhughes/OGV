using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Unity;
using System.Windows.Input;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Commands;

namespace OGV.Infrastructure.Model
{
    public class Session : BindableBase
    {
        public string Token { get; set; }

        public static bool Recording { get; set; }

        private static readonly Session instance = new Session();

        

        private Session() { }

        public static Session Instance
        {
            get
            {
                return instance;
            }
        }


    }

}
