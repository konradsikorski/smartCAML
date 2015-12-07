using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using KoS.Apps.SharePoint.SmartCAML.Editor.Annotations;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Model
{
    public class ConnectWindowModel : INotifyPropertyChanged
    {
        private string _sharePoinWebtUrl;
        private bool _showAdvanceOptions;
        private SharePointProviderType _providerType;
        private bool _useCurrentUser;
        private bool _isConnecting;
        private static SecureString _lastPassword;

        public ConnectWindowModel()
        {
            SharePointWebUrlHistory = new ObservableCollection<string>( Config.SharePointUrlHistory );
            if( SharePointWebUrlHistory.Count > 0 ) SharePointWebUrl = SharePointWebUrlHistory[0];
            ProviderType = Config.LastSelectedProvider;
            UserName = Config.LastUser;
            UsersHistory = new ObservableCollection<string>(Config.UsersHistory);
            UseCurrentUser = Config.UseCurrentUser;

            if (!UseCurrentUser && UsersHistory.Count > 0)
            {
                UserName = UsersHistory[0];
                UserPassword = SecureStringToString(_lastPassword);
            }
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

        public bool UseCurrentUser
        {
            get { return _useCurrentUser; }
            set
            {
                if (value == _useCurrentUser) return;
                _useCurrentUser = value;
                OnPropertyChanged();
            }
        }

        public bool UseSpecificUser
        {
            get { return !UseCurrentUser; }
            set
            {
                if (UseCurrentUser != !value) return;
                UseCurrentUser = !value;
                OnPropertyChanged();
            }
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set
            {
                if (_isConnecting == value) return;
                _isConnecting = value;
                OnPropertyChanged();
            }
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
            _lastPassword = SecureStringFromString(UserPassword);
        }

        public void AddNewUrl(string url)
        {
            var index = SharePointWebUrlHistory.IndexOf(url);
            if (index >= 0) SharePointWebUrlHistory.RemoveAt(index);
            SharePointWebUrlHistory.Insert(0, url);
        }

        public void AddUserToHistory()
        {
            var index = UsersHistory.IndexOf(UserName);
            if (index >= 0) UsersHistory.RemoveAt(index);
            UsersHistory.Insert(0, UserName);
        }

        private SecureString SecureStringFromString(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var secure = new SecureString();
            foreach (var c in text)
            {
                secure.AppendChar(c);
            }

            return secure;
        }

        private string SecureStringToString(SecureString value)
        {
            if (value == null) return null;

            var bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
    }
}
