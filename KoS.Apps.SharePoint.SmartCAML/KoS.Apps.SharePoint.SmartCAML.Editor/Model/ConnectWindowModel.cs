using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using KoS.Apps.SharePoint.SmartCAML.Editor.Annotations;
using KoS.Apps.SharePoint.SmartCAML.SharePointProvider;
using System.Collections.Generic;

using KoS.Apps.SharePoint.SmartCAML.Editor.Extensions;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Model
{
    public class ConnectWindowModel : INotifyPropertyChanged
    {
        private string _sharePoinWebtUrl;
        private bool _showAdvanceOptions;
        private SharePointProviderType _providerType;
        private bool _isConnecting;
        private static SecureString _lastPassword;

        public ConnectWindowModel()
        {
            SharePointWebUrlHistory = new ObservableCollection<string>( Config.SharePointUrlHistory );
            SharePointWebUrl = (SharePointWebUrlHistory.Count > 0) ? SharePointWebUrlHistory[0]: String.Empty;
            UserName = Config.LastUser;
            UsersHistory = new ObservableCollection<string>(Config.UsersHistory?.Where( u => !String.IsNullOrEmpty(u)));

            if (UsersHistory.Count > 0)
            {
                UserName = UsersHistory[0];
                UserPassword = SecureStringToString(_lastPassword);
            }

            ProviderType = Config.LastSelectedProvider;
            SharePointProviders = EnumExtension.ToDictionary<SharePointProviderType>();
#if !DEBUG
            SharePointProviders = SharePointProviders.Where( p => p.Key != SharePointProviderType.Fake).ToList();
#endif
            if (SharePointProviders.All(p => p.Key != ProviderType))
                ProviderType = SharePointProviders.First().Key;
        }

        [Required]
        public string SharePointWebUrl
        {
            get { return _sharePoinWebtUrl; }
            set
            {
                value = value ?? string.Empty;

                if (value == _sharePoinWebtUrl) return;
                _sharePoinWebtUrl = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> SharePointWebUrlHistory { get; set; }

        public List<KeyValuePair<SharePointProviderType, string>> SharePointProviders { get; set; }

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

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set
            {
                if (_isConnecting == value) return;
                _isConnecting = value;
                OnPropertyChanged();

                if(_isConnecting) ErrorMessage = null;
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage == value) return;
                _errorMessage = value;
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
            Config.LastSelectedProvider = ProviderType;
            Config.SharePointUrlHistory = SharePointWebUrlHistory;
            Config.UsersHistory = UsersHistory;
            _lastPassword = SecureStringFromString(UserPassword);
        }

        public void AddNewUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            var index = SharePointWebUrlHistory.IndexOf(url);
            if (index >= 0) SharePointWebUrlHistory.RemoveAt(index);
            SharePointWebUrlHistory.Insert(0, url);
        }

        public void AddUserToHistory(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return;

            var index = UsersHistory.IndexOf(userName);
            if (index >= 0) UsersHistory.RemoveAt(index);
            UsersHistory.Insert(0, userName);
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
