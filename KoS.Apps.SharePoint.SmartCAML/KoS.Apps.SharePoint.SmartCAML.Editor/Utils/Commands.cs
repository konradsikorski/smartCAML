using System.Windows.Input;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    static class Commands
    {
        public static RoutedCommand ConnectCommand = new RoutedCommand();

        static Commands()
        {
            ConnectCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
        }
    }
}
