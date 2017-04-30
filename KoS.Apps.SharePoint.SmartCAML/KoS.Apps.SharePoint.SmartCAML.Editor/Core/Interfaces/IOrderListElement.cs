using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Core.Interfaces
{
    public interface IOrderListElement
    {
        FrameworkElement Control { get; }
        event EventHandler RemoveClick;
        event EventHandler Changed;
        event EventHandler Up;
        event EventHandler Down;
    }
}
