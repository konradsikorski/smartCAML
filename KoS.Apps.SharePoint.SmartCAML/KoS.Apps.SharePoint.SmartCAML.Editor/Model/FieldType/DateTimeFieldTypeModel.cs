using KoS.Apps.SharePoint.SmartCAML.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Model.FieldType
{
    class DateTimeFieldTypeModel : INotifyPropertyChanged
    {
        private bool _includeTime;
        public bool IncludeTime
        {
            get { return _includeTime; }
            set
            {
                if (value == _includeTime) return;
                _includeTime = value;
                OnPropertyChanged();
            }
        }

        private string _date;
        public string Date
        {
            get { return _date; }
            set
            {
                if (value == _date) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTimeFieldTypeModel(Field field)
        {
            _includeTime = !((FieldDateTime)field).DateOnly;
        }

        private void OnPropertyChanged( [CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
