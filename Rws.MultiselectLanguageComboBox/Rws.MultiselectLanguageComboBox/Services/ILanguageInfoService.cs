using Sdl.MultiSelectComboBox.API;

namespace Rws.MultiselectLanguageComboBox.Services
{
	public interface ILanguageInfoService
	{
        string GetDisplayName(string language);
        IItemGroup GetItemGroup(string language);
	}
}