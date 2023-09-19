using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
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

        public static readonly DependencyProperty LanguagesSourceProperty = 
            DependencyProperty.Register("LanguagesSource", typeof(ObservableCollection<string>), typeof(MultiselectLanguageComboBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLanguagesSourceChanged)));
        
        public ObservableCollection<string> LanguagesSource
        {
            get { return (ObservableCollection<string>)GetValue(LanguagesSourceProperty); }
            set { SetValue(LanguagesSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedLanguagesProperty = 
            DependencyProperty.Register("SelectedLanguages", typeof(ObservableCollection<string>), typeof(MultiselectLanguageComboBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedLanguagesChanged)));
        
        public ObservableCollection<string> SelectedLanguages
        {
            get { return (ObservableCollection<string>)GetValue(SelectedLanguagesProperty); }
            set { SetValue(SelectedLanguagesProperty, value); }
        }

        public static readonly DependencyProperty SelectedLanguageProperty =
            DependencyProperty.Register("SelectedLanguage", typeof(string), typeof(MultiselectLanguageComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedLanguageChanged));

        public string SelectedLanguage
        {
            get => (string)GetValue(SelectedLanguageProperty);
            set => SetValue(SelectedLanguageProperty, value);
        }

        private ILanguageInfoService _languageInfoService;
        public ILanguageInfoService LanguageInfoService
        {
            get
            {
                if (_languageInfoService == null)
                {
                    _languageInfoService = new DefaultLanguageInfoService();
                }
                return _languageInfoService;
            }
            set
            {
                _languageInfoService = value;
                Refresh();
            }
        }

        private static void OnLanguagesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var component = d as MultiselectLanguageComboBox;
            component?.OnLanguagesSourceChanged(e.OldValue as ObservableCollection<string>);
        }

        private static void OnSelectedLanguagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var component = d as MultiselectLanguageComboBox;
            component?.OnSelectedLanguagesChanged(e.OldValue as ObservableCollection<string>);
        }

        private static void OnSelectedLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var component = d as MultiselectLanguageComboBox;
            component?.OnSelectedLanguageChanged();
        }

        private void OnLanguagesSourceChanged(ObservableCollection<string> oldLanguagesSource)
        {
            if (oldLanguagesSource != null)
            {
                oldLanguagesSource.CollectionChanged -= LanguagesSource_CollectionChanged;
            }

            Refresh();

            if (LanguagesSource != null)
            {
                LanguagesSource.CollectionChanged += LanguagesSource_CollectionChanged;
            }
        }

        public void Refresh()
        {
            _languageItemsMap.Clear();
            var items = new ObservableCollection<LanguageItem>();
            AddLanguageItems(items, LanguagesSource);
            ItemsSource = items;
        }

        private void OnSelectedLanguagesChanged(ObservableCollection<string> oldSelectedLanguages)
        {
            if (oldSelectedLanguages != null)
            {
                oldSelectedLanguages.CollectionChanged -= SelectedLanguages_CollectionChanged;
            }

            var oldSelectedItems = SelectedItems as ObservableCollection<LanguageItem>;
            if (oldSelectedItems != null)
            {
                oldSelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            }

            var selectedItems = new ObservableCollection<LanguageItem>();
            AddLanguageItems(selectedItems, SelectedLanguages);
            ResetSelectionOrder();
            SelectedItems = selectedItems;

            if (SelectedLanguages != null)
            {
                SelectedLanguages.CollectionChanged += SelectedLanguages_CollectionChanged;
            }

            if (selectedItems != null)
            {
                selectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            }

            _isDuringInternalSelectedLanuageSet = true;
            SelectedLanguage = SelectedLanguages?.FirstOrDefault();
            _isDuringInternalSelectedLanuageSet = false;
        }

        private void LanguagesSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var items = ItemsSource as ObservableCollection<LanguageItem>;
            if (items == null)
            {
                return;
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (string language in e.NewItems)
                {
                    AddLanguageItem(items, language);
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (string language in e.OldItems)
                {
                    RemoveLanguageItem(items, language);
                }
            }
        }

        private void SelectedLanguages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var selectedItems = SelectedItems as ObservableCollection<LanguageItem>;
            if (selectedItems == null)
            {
                return;
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (string language in e.NewItems)
                {
                    if (selectedItems.All(i => i.Id != language))
                    {
                        AddLanguageItem(selectedItems, language);
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (string language in e.OldItems)
                {
                    RemoveLanguageItem(selectedItems, language);
                }
            }

            ResetSelectionOrder();

            _isDuringInternalSelectedLanuageSet = true;
            SelectedLanguage = SelectedLanguages.FirstOrDefault();
            _isDuringInternalSelectedLanuageSet = false;
        }

        private bool _isDuringInternalSelectedLanuageSet;

        private void OnSelectedLanguageChanged()
        {
            if (_isDuringInternalSelectedLanuageSet)
            {
                return;
            }

            SelectedLanguages.Clear();
            
            if (SelectedLanguage != null)
            {
                SelectedLanguages.Add(SelectedLanguage);
            }
        }

        private void ResetSelectionOrder()
        {
            if (ItemsSource == null)
            {
                return;
            }

            foreach (var item in ItemsSource)
            {
                if (item is LanguageItem languageItem)
                {
                    languageItem.SelectedOrder = -1;
                }
            }

            if (SelectedItems == null)
            {
                return;
            }

            int order = 0;
            foreach (var item in SelectedItems)
            {
                if (item is LanguageItem languageItem)
                {
                    languageItem.SelectedOrder = ++order;
                }
            }
        }

        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is LanguageItem languageItem)
                    {
                        if (!SelectedLanguages.Contains(languageItem.Id))
                        {
                            SelectedLanguages.Add(languageItem.Id);
                        }
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is LanguageItem languageItem)
                    {
                        SelectedLanguages.Remove(languageItem.Id);
                    }
                }
            }
        }

        private Dictionary<string, LanguageItem> _languageItemsMap = new Dictionary<string, LanguageItem>();

        public LanguageItem GetLanguageItem(string language)
        {
            LanguageItem item;
            if (_languageItemsMap.TryGetValue(language, out item))
            {
                return item;
            }

            return null;
        }

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
                item = new LanguageItem
                {
                    Id = language,
                    Name = LanguageInfoService?.GetDisplayName(language) ?? language,
                    Group = LanguageInfoService?.GetItemGroup(language),
                    ImageProvider = () => LanguageInfoService?.GetImage(language)
                };
                _languageItemsMap.Add(language, item);
            }
            items.Add(item);
        }

        private void RemoveLanguageItem(ObservableCollection<LanguageItem> items, string language)
        {
            var item = items.FirstOrDefault(i => i.Id == language);
            if (item != null)
            {
                items.Remove(item);
            }
        }
    }
}
