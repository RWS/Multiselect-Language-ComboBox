using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Rws.MultiSelectLanguageComboBox.Models;
using Rws.MultiSelectLanguageComboBox.Services;
using Sdl.MultiSelectComboBox.Themes.Generic;

namespace Rws.MultiSelectLanguageComboBox
{
    /// <summary>
    /// Specialized MultiSelectComboBox component that shows and allows the end user to select from a set of languages.
    /// By default, the list of languages is determined from the system (specific <see cref="CultureInfo"/> objects, ordered by their English name.)
    /// You can override available languages by defining <see cref="LanguagesSource"/>, indicating strings that represent language IDs (not limited to ISO names).
    /// <see cref="SelectedLanguages"/> provides support for language multi-selection, specifying and allowing reading which languages are currently selected (their string IDs).
    /// If you use single selection (using <see cref="MultiSelectComboBox.SelectionMode"/> property, you can also use the shortcut single selection property <see cref="SelectedLanguage"/>, instead.
    /// </summary>
    public class MultiSelectLanguageComboBox : MultiSelectComboBox
    {
        private ILanguageInfoService _languageInfoService;
        private ILanguageSuggestionProvider _languageSuggestionProvider;

        private readonly Dictionary<string, LanguageItem> _languageItemsMap = new Dictionary<string, LanguageItem>();
        private bool _isDuringInternalSelectedLanuageSet;

        static MultiSelectLanguageComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectLanguageComboBox), new FrameworkPropertyMetadata(typeof(MultiSelectLanguageComboBox)));
        }

        public MultiSelectLanguageComboBox()
        {
            SetValue(LanguagesSourceProperty, new ObservableCollection<string>(CultureInfo.GetCultures(CultureTypes.SpecificCultures).OrderBy(c => c.EnglishName).Select(c => c.Name)));
            SetValue(SelectedLanguagesProperty, new ObservableCollection<string>());
        }

        public static readonly DependencyProperty LanguagesSourceProperty = 
            DependencyProperty.Register("LanguagesSource", typeof(ObservableCollection<string>), typeof(MultiSelectLanguageComboBox), 
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLanguagesSourceChanged)));
        
        /// <summary>
        /// Specifies the list of available languages, as a collection of string IDs.
        /// </summary>
        public ObservableCollection<string> LanguagesSource
        {
            get => (ObservableCollection<string>)GetValue(LanguagesSourceProperty);
            set => SetValue(LanguagesSourceProperty, value);
        }

        private static void OnLanguagesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var component = d as MultiSelectLanguageComboBox;
            component?.OnLanguagesSourceChanged(e.OldValue as ObservableCollection<string>);
        }

        public static readonly DependencyProperty SelectedLanguagesProperty = 
            DependencyProperty.Register("SelectedLanguages", typeof(ObservableCollection<string>), typeof(MultiSelectLanguageComboBox), 
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedLanguagesChanged)));

        /// <summary>
        /// Gets or sets the list of selected languages in the user interface (among languages available in <see cref="LanguagesSource"/>), as a collection of string IDs.
        /// </summary>
        public ObservableCollection<string> SelectedLanguages
        {
            get => (ObservableCollection<string>)GetValue(SelectedLanguagesProperty);
            set => SetValue(SelectedLanguagesProperty, value);
        }

        private static void OnSelectedLanguagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var component = d as MultiSelectLanguageComboBox;
            component?.OnSelectedLanguagesChanged(e.OldValue as ObservableCollection<string>);
        }

        public static readonly DependencyProperty SelectedLanguageProperty =
            DependencyProperty.Register("SelectedLanguage", typeof(string), typeof(MultiSelectLanguageComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedLanguageChanged));

        /// <summary>
        /// Gets or sets a single selected languages (among languages available in <see cref="LanguagesSource"/>), as a string IDs.
        /// If multiple languages are selected, the first selected item's ID is returned.
        /// </summary>
        public string SelectedLanguage
        {
            get => (string)GetValue(SelectedLanguageProperty);
            set => SetValue(SelectedLanguageProperty, value);
        }

        private static void OnSelectedLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var component = d as MultiSelectLanguageComboBox;
            component?.OnSelectedLanguageChanged();
        }

        /// <summary>
        /// Defines the service that provides information about languages, based on their IDs, including their names, groups, and images to be used as icons (flags).
        /// By default English names are returned, retreived using <see cref="CultureInfo"/>,
        /// all items are in a single group ("All"), and images are returned from an internal collection of language ID-based icons
        /// that correspond to known cultures, or else icons are generated on the fly displaying two letters from the language ID.
        /// </summary>
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

        /// <summary>
        /// Defines an optional service that provides language suggestions to be used to select items for displaying 
        /// in the combo box, based on their IDs and specific criteria generated by the component.
        /// When set, it is mapped automatically to the base class's <see cref="MultiSelectComboBox.SuggestionProvider"/>.
        /// </summary>
        public ILanguageSuggestionProvider LanguageSuggestionProvider
        {
            get
            {
                return _languageSuggestionProvider;
            }
            set
            {
                _languageSuggestionProvider = value;
                Refresh();
            }
        }

        /// <summary>
        /// Returns the internal <see cref="LanguageItem"/> object generated for any specific language.
        /// Internally, the component creates these internal objects to be presented in the user interface.
        /// You may need such object instances yourself too if you plan to further customize the component instance
        /// when that code would work directly on the base <see cref="MultiSelectComboBox"/>.
        /// </summary>
        /// <param name="language">The string ID of the language to get the <see cref="LanguageItem"/> for.</param>
        /// <returns>Returns the <see cref="LanguageItem"/> for the specified language ID, if it was already created (and is now cached) by the component, or otherwise it creates (and cashes) it first.</returns>
        public LanguageItem GetLanguageItem(string language)
        {
            LanguageItem item;
            if (_languageItemsMap.TryGetValue(language, out item))
            {
                return item;
            }

            return null;
        }

        /// <summary>
        /// Refreshes the entire user interface of the component, recreating all internal language items.
        /// Might be useful in case <see cref="LanguageInfoService"/> would return other values for the same language IDs,
        /// and the cached <see cref="LanguageItem"/> objects do not reflect the reality anymore.
        /// </summary>
        public void Refresh()
        {
            _languageItemsMap.Clear();
            var items = new ObservableCollection<LanguageItem>();
            AddLanguageItems(items, LanguagesSource);
            ItemsSource = items;

            SuggestionProvider = LanguageSuggestionProvider != null
                ? new SuggestionMapper(LanguageSuggestionProvider, items) 
                : null;
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
