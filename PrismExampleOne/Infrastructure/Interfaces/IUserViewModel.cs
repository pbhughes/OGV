using Microsoft.Practices.Prism.Commands;
using System;
using System.Windows.Controls;


namespace OGV.Infrastructure.Interfaces
{
    public interface IUserViewModel
    {
        IBoardList BoardList { get; set; }
        int CallTime { get; set; }
        Microsoft.Practices.Unity.IUnityContainer Container { get; set; }
        bool IsBusy { get; set; }
        DelegateCommand<PasswordBox> LoginCommand { get; }
        string Message { get; set; }
        string Password { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        string UserName { get; set; }
    }
}
