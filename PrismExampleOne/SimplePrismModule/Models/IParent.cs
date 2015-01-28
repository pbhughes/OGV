using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGV.Admin.Models
{
    public interface IParent
    {
        void RemoveItem(AgendaItem item);
        void AddItem(AgendaItem item);
    }
}
