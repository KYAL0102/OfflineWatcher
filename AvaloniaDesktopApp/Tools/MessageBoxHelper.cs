using System.Threading.Tasks;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace AvaloniaDesktopApp.Tools;

public static class MessageBoxHelper
{
public static async Task ShowMessageBox(string message)
{
    var messageBoxStandardWindow = MessageBoxManager
        .GetMessageBoxStandard(new MessageBoxStandardParams
        {
            ButtonDefinitions = ButtonEnum.Ok,
            ContentTitle = "Information",
            ContentMessage = message,
            Icon = Icon.Info,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        });

    await messageBoxStandardWindow.ShowAsync();
}
}