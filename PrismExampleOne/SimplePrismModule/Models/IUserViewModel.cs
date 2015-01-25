using System;
namespace OGV.Admin.Models
{
    public interface IUserViewModel
    {
        BoardList BoardList { get; set; }
        int CallTime { get; set; }
        Microsoft.Practices.Unity.IUnityContainer Container { get; set; }
        bool IsBusy { get; set; }
        System.Windows.Input.ICommand LoginCommand { get; }
        string Message { get; set; }
        string Password { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        string UserName { get; set; }
    }
}
