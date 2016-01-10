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
        Microsoft.Practices.Prism.Commands.DelegateCommand GetAgendaFilesCommand { get; set; }
        List<FTPfileInfo> AvailableFiles { get; set; }
        string GetXml(string fileName);
        Task LoadAgendaFiles();
        Exception LastError { get; set; }
    }
}
