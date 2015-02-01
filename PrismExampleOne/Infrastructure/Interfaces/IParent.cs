using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGV.Infrastructure.Interfaces
{
    public interface IParent
    {
        void RemoveItem(IAgendaItem item);
        void AddItem(IAgendaItem item);
        void InsertItem(IAgendaItem item, int indexAt);
        int IndexOf(IAgendaItem item);
    }
}
