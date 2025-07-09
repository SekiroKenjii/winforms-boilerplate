using Microsoft.Extensions.Options;
using WinformsBoilerplate.Core.Abstractions.Serializers;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Constants;
using WinformsBoilerplate.Core.Entities.Settings;
using WinformsBoilerplate.Core.Extensions;
using WinformsBoilerplate.Core.Helpers;
using WinformsBoilerplate.Core.Wrappers;

namespace WinformsBoilerplate.Infrastructure.Services;

public class AppSettingService : IAppSettingService
{
    private readonly ILogService _logService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IOptionsMonitor<AppSetting> _appSettingMonitor;

    public AppSettingService(
        ILogService logService,
        IJsonSerializer jsonSerializer,
        IOptionsMonitor<AppSetting> appSettingMonitor)
    {
        _logService = logService;
        _jsonSerializer = jsonSerializer;
        _appSettingMonitor = appSettingMonitor;

        LastValue = _appSettingMonitor.CurrentValue;

        _ = _appSettingMonitor.OnChange(setting => {
            IsChanged = true;
            LastValue = setting;
        });
    }

    public bool IsChanged { get; private set; }

    public AppSetting Value => _appSettingMonitor.CurrentValue;

    public AppSetting LastValue { get; private set; }

    public bool CreateDefaultSettingFile(bool @override = false)
    {
        _logService.Info("Creating default setting file...");

        string settingFile = Path.Combine(CommonHelpers.AppStartupPath(), Files.SETTING_FILE);

        if (File.Exists(settingFile) && !@override)
        {
            _logService.Warn($"Setting file '{settingFile}' already exists. Use override to replace it.");

            return true;
        }

        var defaultSetting = new Dictionary<string, AppSetting> {
            ["appSetting"] = new AppSetting { Misc = new() }
        };
        string settingSerialized = _jsonSerializer.Serialize(defaultSetting);

        return ThrowableFunction<bool>
            .Run(() => {
                File.WriteAllText(settingFile, settingSerialized);

                return true;
            })
            .Catch(ex => _logService.Error($"Error creating default setting file: {ex.ToFormattedString()}"));
    }

    public void Save(AppSetting appSetting)
    {
        string settingFile = Path.Combine(CommonHelpers.AppStartupPath(), Files.SETTING_FILE);

        if (!File.Exists(settingFile))
        {
            File.Create(settingFile).Close();
        }

        var settingDict = new Dictionary<string, AppSetting> {
            ["appSetting"] = appSetting
        };
        string settingSerialized = _jsonSerializer.Serialize(settingDict);

        ThrowableAction
            .Run(() => File.WriteAllText(settingFile, settingSerialized))
            .Catch(ex => _logService.Error($"Error saving setting file: {ex.ToFormattedString()}"));
    }
}
