using KoS.Apps.SharePoint.SmartCAML.Editor.Utils;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Core.Commands
{
    public class DonateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Telemetry.Instance.Native?.TrackEvent("Command.Donate");

            try
            {
                Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=HTEBZ3Y37F2ZL");
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }
}
