using System.ComponentModel;
using System.Runtime.CompilerServices;
using KoS.Apps.SharePoint.SmartCAML.Editor.Annotations;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Model
{
    public class ListTreeItem :INotifyPropertyChanged
    {
        private bool _isExpanded = true;
        public ISharePointProvider Client { get; set; }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value == _isExpanded) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
