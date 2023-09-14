using Sdl.MultiSelectComboBox.API;

namespace Rws.MultiselectLanguageComboBox.Services
{
	public interface IGroupingService
	{
		IItemGroup GetItemGroup(string language);
	}
}