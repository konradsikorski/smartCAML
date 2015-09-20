using System.ComponentModel;
using System.Runtime.CompilerServices;
using KoS.Apps.SharePoint.SmartCAML.Editor.Annotations;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Model
{
    public class ConnectWindowModel : INotifyPropertyChanged
    {
        private string _sharePoinWebtUrl;
        private bool _showAdvanceOptions;

        [NotNull]
        public string SharePoinWebtUrl
        {
            get { return _sharePoinWebtUrl; }
            set
            {
                if (value == _sharePoinWebtUrl) return;
                _sharePoinWebtUrl = value;
                OnPropertyChanged();
            }
        }

        public bool ShowAdvanceOptions
        {
            get { return _showAdvanceOptions; }
            set
            {
                if (value == _showAdvanceOptions) return;
                _showAdvanceOptions = value;
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
