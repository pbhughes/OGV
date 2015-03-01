using System;
namespace OGV.Infrastructure.Interfaces
{
    public interface IPublishingPointMonitor
    {
        Microsoft.Practices.Prism.Commands.DelegateCommand CheckStateCommand { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand StreamCommand { get; }
        bool IsBusy { get; set; }
        string Message { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        string State { get; set; }
    }
}
