using System;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinGuGu2.Model;
using LinGuGu2.Service;
using LinGuGu2.UserControls;

namespace LinGuGu2.ViewModel;

public class MainViewModel : ObservableObject
{
    private StackPanel ChatStackPanel { get; set; }
    public RelayCommand DisableOtherButtonsCommand { get; private set; }
    public RelayCommand<string> SendMessageCommand { get; private set; }

    public EnableViewModel SendButtonViewModel { get; set; } = new();
    public EnableViewModel TxtMessageViewModel { get; set; } = new();
    public UserGroupViewModel UserGroupViewModel { get; set; } = new();

    private string _textBoxText;

    public string TextBoxText
    {
        get => _textBoxText;
        set => SetProperty(ref _textBoxText, value);
    }

    public MainViewModel()
    {
        ChatStackPanel = GetStackPanelFromView();

        DisableOtherButtonsCommand = new RelayCommand(() => { });

        SendMessageCommand = new RelayCommand<string>(text => { SendMessage(text); });
    }

    private void SendMessage(string text)
    {
        if (text == "")
        {
            return;
        }

        if (MainWindow.CurrentUser == null)
        {
            MessageBox.Show("请选择一个用户");
            return;
        }

        MessageType messageType = new MessageType(
            MessageTypeEnum.Normal,
            text,
            MainWindow.CurrentUser.Ip.ToString()
        );
        ChatMessage chatMessage = new ChatMessage(
            true,
            text,
            messageType.Time
        );
        MainWindow.CurrentUser.AddMessage(chatMessage);
        TextBoxText = "";

        var elements = DataLoader.GetUCsForNewMessage(MainWindow.CurrentUser.MessageList, MainWindow.CurrentUser);
        ChatStackPanel.AddAllMessage(elements);

        UdpUtil.SendMsg(messageType.ToJson(), MainWindow.CurrentUser.Ip, MainWindow.CurrentUser.Port);
    }

    private StackPanel GetStackPanelFromView()
    {
        if (Application.Current.MainWindow is Window mainWindow)
        {
            if (mainWindow.FindName("ChatStackPanel") is StackPanel stackPanel)
            {
                return stackPanel;
            }
        }

        return null;
    }
}