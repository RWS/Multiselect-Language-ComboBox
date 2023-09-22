using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Sdl.MultiSelectComboBox.API;

namespace Rws.MultiSelectLanguageComboBox.Models
{
    public class LanguageItem : IItemEnabledAware, IItemGroupAware, INotifyPropertyChanged
    {
        private string _name;
        private bool _isEnabled;
        private int _selectedOrder;
        private IItemGroup _group;
        private ImageSource _image;

        public LanguageItem()
        {
            _isEnabled = true;
            _selectedOrder = -1;
        }

        public string Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != null && string.Compare(_name, value, StringComparison.InvariantCulture) == 0)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Identifies whether the item is enabled or not.
        /// 
        /// When the item is not enabled, then it will not be selectable from the dropdown list and removed
        /// from the selected items automatically.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled.Equals(value))
                {
                    return;
                }

                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        /// <summary>
        /// The order in which the items are added to the selected collection.  
        ///  
        /// This order is independent to the group and sort order of the items in the collection. This selected 
        /// order is visible in each of the selected items from the dropdown list and visually represented by 
        /// the order of the items in the Selected Items Panel.
        /// </summary>
        public int SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (_selectedOrder.Equals(value))
                {
                    return;
                }

                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
            }
        }

        /// <summary>
        /// Identifies the name and order of the group header
        /// </summary>
        public IItemGroup Group
        {
            get => _group;
            set
            {
                if (_group != null && _group.Equals(value))
                {
                    return;
                }

                _group = value;
                OnPropertyChanged(nameof(Group));
            }
        }

        public Func<ImageSource> ImageProvider { get; set; }

        public ImageSource Image
        {
            get => _image ?? (_image = ImageProvider());
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
