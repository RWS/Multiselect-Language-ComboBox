using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Rws.MultiselectLanguageComboBox.Models;
using Rws.MultiselectLanguageComboBox.Services;
using Sdl.MultiSelectComboBox.Themes.Generic;

namespace Rws.MultiselectLanguageComboBox
{
    public class MultiselectLanguageComboBox : MultiSelectComboBox
    {
        static MultiselectLanguageComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiselectLanguageComboBox), new FrameworkPropertyMetadata(typeof(MultiselectLanguageComboBox)));
        }

        public MultiselectLanguageComboBox()
        {
            SetValue(LanguagesSourceProperty, new ObservableCollection<string>(CultureInfo.GetCultures(CultureTypes.SpecificCultures).OrderBy(c => c.EnglishName).Select(c => c.Name)));
            SetValue(SelectedLanguagesProperty, new ObservableCollection<string>());
        }

        public static readonly DependencyProperty LanguagesSourceProperty = DependencyProperty.Register("LanguagesSource", typeof(ObservableCollection<string>), typeof(MultiselectLanguageComboBox), new PropertyMetadata(new PropertyChangedCallback(OnLanguagesSourceChanged)));
        public ObservableCollection<string> LanguagesSource
        {
            get { return (ObservableCollection<string>)GetValue(LanguagesSourceProperty); }
            set { SetValue(LanguagesSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedLanguagesProperty = DependencyProperty.Register("SelectedLanguages", typeof(ObservableCollection<string>), typeof(MultiselectLanguageComboBox), new PropertyMetadata(new PropertyChangedCallback(OnSelectedLanguagesChanged)));
        public ObservableCollection<string> SelectedLanguages
        {
            get { return (ObservableCollection<string>)GetValue(SelectedLanguagesProperty); }
            set { SetValue(SelectedLanguagesProperty, value); }
        }

        public static readonly DependencyProperty LanguageGroupServiceProperty = DependencyProperty.Register("LanguageGroupService", typeof(IGroupingService), typeof(MultiselectLanguageComboBox), new PropertyMetadata());
        public IGroupingService LanguageGroupService
        {
            get { return (IGroupingService)GetValue(LanguageGroupServiceProperty); }
            set { SetValue(LanguageGroupServiceProperty, value); }
        }

        private static void OnLanguagesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var component = d as MultiselectLanguageComboBox;
            if (component == null)
            {
                return;
            }
            
            var items = new ObservableCollection<LanguageItem>();
            component.AddLanguageItems(items, component.LanguagesSource);
            component.ItemsSource = items;
        }

        private static void OnSelectedLanguagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var component = d as MultiselectLanguageComboBox;
            if (component == null)
            {
                return;
            }

            var selectedItems = new ObservableCollection<LanguageItem>();
            component.AddLanguageItems(selectedItems, component.SelectedLanguages);
            component.SelectedItems = selectedItems;
        }

        private Dictionary<string, LanguageItem> _languageItemsMap = new Dictionary<string, LanguageItem>();

        private void AddLanguageItems(ObservableCollection<LanguageItem> items, ObservableCollection<string> languages)
        {
            if (languages == null)
            {
                return;
            }

            foreach (var language in languages)
            {
                AddLanguageItem(items, language);
            }
        }

        private LanguageItemGroup _group = new LanguageItemGroup(-1, "All");

        private void AddLanguageItem(ObservableCollection<LanguageItem> items, string language)
        {
            if (language == null)
            {
                return;
            }

            LanguageItem item;
            if (_languageItemsMap.ContainsKey(language))
            {
                item = _languageItemsMap[language];
            }
            else
            {
                var cultureInfo = CultureInfo.GetCultureInfo(language);
                item = new LanguageItem
                {
                    Id = cultureInfo.Name,
                    Name = cultureInfo.EnglishName,
                    Group = _group,
                    ImageUri = new Uri($"pack://application:,,,/Rws.MultiselectLanguageComboBox;component/Images/{language}.png"),
                    CultureInfo = cultureInfo
                };
                _languageItemsMap.Add(language, item);
            }
            items.Add(item);
        }
    }
}
