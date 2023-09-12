using System.Windows;
using System.Windows.Controls;

namespace Rws.MultiselectLanguageComboBox
{
    public class MultiselectLanguageComboBox : Control
    {
        static MultiselectLanguageComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiselectLanguageComboBox), new FrameworkPropertyMetadata(typeof(MultiselectLanguageComboBox)));
        }
    }
}
