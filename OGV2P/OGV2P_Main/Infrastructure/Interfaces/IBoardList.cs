using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IBoardList
    {
        List<IBoard> Boards { get; set; }
        void AddBoard(IBoard board);
    }
}
