using CommunityToolkit.Mvvm.ComponentModel;

namespace LinGuGu2.ViewModel;

public class EnableViewModel : ObservableObject
{
    private bool _isEnable = true;
    public bool IsEnable
    {
        get => _isEnable;
        set => SetProperty(ref _isEnable, value);
    }
}