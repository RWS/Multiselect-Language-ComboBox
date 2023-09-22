using Sdl.MultiSelectComboBox.API;
using System.Windows.Media;

namespace Rws.MultiSelectLanguageComboBox.Services
{
	public interface ILanguageInfoService
	{
        string GetDisplayName(string language);
        IItemGroup GetItemGroup(string language);
		ImageSource GetImage(string language);
	}
}