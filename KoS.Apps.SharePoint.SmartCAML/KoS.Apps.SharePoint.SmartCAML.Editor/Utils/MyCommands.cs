using System.Windows.Input;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    public static class MyCommands
    {
        public static RoutedCommand Connect = new RoutedCommand();
        public static RoutedCommand NewQuery = new RoutedCommand();
        public static RoutedCommand RunQuery = new RoutedCommand();
        public static RoutedCommand SaveItems = new RoutedCommand();
        public static RoutedCommand CloseQueryTab = new RoutedCommand();
        public static RoutedUICommand HideColumn = new RoutedUICommand();
        public static RoutedUICommand HideAllColumn = new RoutedUICommand();
        public static RoutedUICommand HideAllHiddenColumns = new RoutedUICommand();
        public static RoutedUICommand HideAllReadonlyColumns = new RoutedUICommand();
        public static RoutedUICommand HideUnPinedColumn = new RoutedUICommand();
        public static RoutedUICommand UnhideColumn = new RoutedUICommand();
        public static RoutedUICommand UnhideAllColumn = new RoutedUICommand();
        public static RoutedUICommand PinColumn = new RoutedUICommand();
        public static RoutedUICommand UnpinColumn = new RoutedUICommand();
        public static RoutedUICommand CustomizeColumn = new RoutedUICommand();

        static MyCommands()
        {
            Connect.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            NewQuery.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            RunQuery.InputGestures.Add(new KeyGesture(Key.F5));
            SaveItems.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CloseQueryTab.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
        }
    }
}
