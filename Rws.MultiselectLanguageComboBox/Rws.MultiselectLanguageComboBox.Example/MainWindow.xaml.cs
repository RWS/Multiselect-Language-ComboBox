using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rws.MultiselectLanguageComboBox.Example
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
        }

        public ObservableCollection<string> AvailableLanguages { get; set; } = new ObservableCollection<string>(new[] { "en-US", "en-GB", "ro-RO" } );
        public ObservableCollection<string> SelectedLanguages { get; set; } = new ObservableCollection<string>(new[] { "en-US", "ro-RO" });
    }
}
