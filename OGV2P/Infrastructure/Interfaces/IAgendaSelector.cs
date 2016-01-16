using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OGV2P.FTP.Utilities;

namespace Infrastructure.Interfaces
{
    public interface IAgendaSelector
    {
        string TargetFile { get; set; }
        bool IsBusy { get; set; }
        List<FTPfileInfo> AvailableFiles { get; set; }
        string GetSelectedAgendaXML(string fileName);
        List<FTPfileInfo> ListAgendaFilesOnServer();
        Exception LastError { get; set; }
        string Text { get; set; }
    }
}
