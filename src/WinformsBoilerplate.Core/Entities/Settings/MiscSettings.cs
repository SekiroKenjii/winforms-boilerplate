using WinformsBoilerplate.Core.Entities.Contracts;

namespace WinformsBoilerplate.Core.Entities.Settings;

public class MiscSettings : ObservableEntityBase
{
    private bool _startAppOnBoot;
    private bool _showAfterBoot;

    public bool StartAppOnBoot
    {
        get => _startAppOnBoot;
        set => SetProperty(ref _startAppOnBoot, value);
    }

    public bool ShowAfterBoot
    {
        get => _showAfterBoot;
        set => SetProperty(ref _showAfterBoot, value);
    }
}
