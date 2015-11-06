using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Regions
    {
       

        private static string _main = "MainRegion";

       
        public static string Main
        {
            get
            {
                return _main;
            }

            set
            {
                _main = value;
            }
        }


        private static string _sideBar = "SideBarRegion";

        public static string SideBar
        {
            get
            {
                return _sideBar;
            }

            set
            {
                _sideBar = value;
            }
        }
    }
}
