﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IAgendaSelector
    {
        string TargetFile { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand GetAgendaFilesCommand { get; set; }
        List<AgendaService.AgendaFile> AvailableFiles { get; set; }
    }
}
