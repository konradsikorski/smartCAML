using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Extensions
{
    public static class ComboToEnumExtension
    {
        public static void BindToEnum<T>( this ComboBox @this, int? selectedIndex = null) where T : struct
        {
            var source = Enum.GetValues(typeof (T))
                .Cast<T>()
                .Select(p => new KeyValuePair<T, string>(p, p.GetDescription()))
                .ToList();

            @this.ItemsSource = source;
            @this.DisplayMemberPath = "Value";
            @this.SelectedValuePath = "Key";

            if (selectedIndex.HasValue) @this.SelectedIndex = selectedIndex.Value;
        }

        public static CollectionViewSource BindToEnumUsingSource<T>(this ComboBox @this) where T : struct
        {
            var source = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(p => new KeyValuePair<T, string>(p, p.GetDescription()))
                .ToList();

            var viewSource = new CollectionViewSource();
            viewSource.Source = source;
            
            @this.DisplayMemberPath = "Value";
            @this.SelectedValuePath = "Key";

            return viewSource;
        }

        public static CollectionViewSource BindToEnumUsingSource<T>(this ComboBox @this, CollectionViewSource viewSource, T? defaultValue = null) where T : struct
        {
            var source = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(p => new KeyValuePair<T, string>(p, p.GetDescription()))
                .ToList();

            viewSource.Source = source;

            @this.DisplayMemberPath = "Value";
            @this.SelectedValuePath = "Key";

            if (defaultValue.HasValue) @this.SelectedValue = defaultValue.Value;

            return viewSource;
        }

        public static T? SelectedEnum<T>(this ComboBox @this) where T: struct
        {
            return @this.SelectedItem != null
                ? (T?)@this.SelectedValue
                : null;
        }
    }
}