using System.Windows.Input;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    static class MyCommands
    {
        public static RoutedCommand Connect = new RoutedCommand();
        public static RoutedCommand NewQuery = new RoutedCommand();
        public static RoutedCommand RunQuery = new RoutedCommand();
        public static RoutedCommand SaveItems = new RoutedCommand();
        public static RoutedCommand CloseQueryTab = new RoutedCommand();

        static MyCommands()
        {
            Connect.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            NewQuery.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            RunQuery.InputGestures.Add(new KeyGesture(Key.F5));
            SaveItems.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CloseQueryTab.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
            CloseQueryTab.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Control));
        }
    }
}
