using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinGuGu2.Model;

namespace LinGuGu2.ViewModel;

public class UserGroupViewModel : ObservableObject
{
    public static ObservableCollection<User> UserList { get; set; } = new();

    // ClearChat_Click
    public RelayCommand ClearChatCommand { get; private set; }

    // DeleteUser_Click
    public RelayCommand DeleteUserCommand { get; private set; }

    public UserGroupViewModel()
    {
        ClearChatCommand = new RelayCommand(() => { MessageBox.Show("ClearChat_Click"); });
        DeleteUserCommand = new RelayCommand(() => { });
    }
}