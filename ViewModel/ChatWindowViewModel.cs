using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LinGuGu2.ViewModel;

public class ChatWindowViewModel : ObservableObject
{
    public RelayCommand<object> SendCommand { get; set; }

    public ChatWindowViewModel()
    {
    }

    private void SendMessage()
    {
    }
}