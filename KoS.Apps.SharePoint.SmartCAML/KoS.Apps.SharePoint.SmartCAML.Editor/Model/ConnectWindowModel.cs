using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using KoS.Apps.SharePoint.SmartCAML.Editor.Annotations;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Model
{
    public class ConnectWindowModel : INotifyPropertyChanged
    {
        private string _sharePoinWebtUrl;
        private bool _showAdvanceOptions;
        private SharePointProviderType _providerType;

        public ConnectWindowModel()
        {
            SharePointWebUrlHistory = new ObservableCollection<string>( Config.SharePointUrlHistory );
            if( SharePointWebUrlHistory.Count > 0 ) SharePointWebUrl = SharePointWebUrlHistory[0];
            ProviderType = Config.LastSelectedProvider;
            UserName = Config.LastUser;
            UsersHistory = new ObservableCollection<string>(Config.UsersHistory);
            UseCurrentUser = Config.UseCurrentUser;
        }

        [Required]
        public string SharePointWebUrl
        {
            get { return _sharePoinWebtUrl; }
            set
            {
                if (value == _sharePoinWebtUrl) return;
                _sharePoinWebtUrl = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> SharePointWebUrlHistory { get; set; }

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


        [Required]
        public string UserName { get; set; }
        public string UserPassword { get; set; }

        public SharePointProviderType ProviderType
        {
            get { return _providerType; }
            set
            {
                if (value == _providerType) return;
                _providerType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> UsersHistory { get; set; }
        public bool UseCurrentUser { get; set; }

        public bool UseSpecificUser
        {
            get { return !UseCurrentUser; }
            set { UseSpecificUser = !value; }
        }

        #region Property change
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public void Save()
        {
            Config.UseCurrentUser = UseCurrentUser;
            Config.LastSelectedProvider = ProviderType;
            Config.SharePointUrlHistory = SharePointWebUrlHistory;
            Config.UseCurrentUser = UseCurrentUser;
            Config.UsersHistory = UsersHistory;
        }

        public void AddNewUrl(string url)
        {
            var index = SharePointWebUrlHistory.IndexOf(url);
            if (index >= 0) SharePointWebUrlHistory.RemoveAt(index);
            SharePointWebUrlHistory.Insert(0, url);
        }
    }
}
