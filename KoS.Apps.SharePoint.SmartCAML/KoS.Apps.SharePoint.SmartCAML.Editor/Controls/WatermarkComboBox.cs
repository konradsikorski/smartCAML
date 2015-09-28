using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Controls
{
    public class WatermarkComboBox : ComboBox
    {
        private string _watermark;
        private Brush _notEmptyBrush;

        public string Watermark
        {
            get { return _watermark; }
            set
            {
                _watermark = value;
                if (IsEmpty) Text = value;
            }
        }

        public string NotEmptyText
        {
            get { return IsEmpty ? String.Empty : Text; }
            set
            {
                if (String.IsNullOrEmpty(value)) SetEmpty();
                else Text = value;
            }
        }

        public bool IsEmpty { get; private set; } = true;

        public WatermarkComboBox()
        {
            this.IsEditable = true;
            this.TextInput += WatermarkComboBox_TextInput;
            this.GotKeyboardFocus += WatermarkComboBox_GotKeyboardFocus;
            this.GotFocus += WatermarkComboBox_GotFocus;
            this.PreviewGotKeyboardFocus += WatermarkComboBox_PreviewGotKeyboardFocus;
            this.KeyUp += WatermarkComboBox_KeyUp;
            
        }

        private void WatermarkComboBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }

        private void WatermarkComboBox_PreviewGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
        }

        private void WatermarkComboBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void WatermarkComboBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
        }

        private void WatermarkComboBox_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (this.Text == String.Empty) SetEmpty();
            else RemoveEmpty();
        }

        private void RemoveEmpty()
        {
            this.IsEmpty = false;
            this.Foreground = _notEmptyBrush;
        }

        private void SetEmpty()
        {
            if (IsEmpty) return;

            this.IsEmpty = true;
            this.Text = Watermark;
            _notEmptyBrush = this.Foreground;
            this.Foreground = new SolidColorBrush(Color.FromRgb(148,148,148));
        }
    }
}
