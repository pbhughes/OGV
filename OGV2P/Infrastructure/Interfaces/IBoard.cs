using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IBoard
    {
        string Name { get; set; }
        string State { get; set; }
        string City { get; set; }
        string UserID { get; set; }
        string Password { get; set; }

    }
}
