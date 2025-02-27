﻿using Rws.MultiSelectLanguageComboBox.Example.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Rws.MultiSelectLanguageComboBox.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            LanguageComboBox.LanguageInfoService = new CustomLanguageInfoService
            {
                RecentLanguages = new List<string> { "en-US", "en-GB" }
            };

            // Optionally set available languages:
            // LanguageComboBox.LanguageSuggestionProvider = new CustomSuggestionProvider(AvailableLanguages);
        }

        public ObservableCollection<string> AvailableLanguages { get; set; } = new ObservableCollection<string>(new[] { "en-US", "en-GB", "ro-RO", "ro-MD" } );
        public ObservableCollection<string> SelectedLanguages { get; set; } = new ObservableCollection<string>(new[] { "en-US", "ro-RO" });
    }
}
