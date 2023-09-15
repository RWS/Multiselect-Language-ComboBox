using Rws.MultiselectLanguageComboBox.Models;
using Sdl.MultiSelectComboBox.API;
using System.Globalization;

namespace Rws.MultiselectLanguageComboBox.Services
{
    public class DefaultLanguageInfoService : ILanguageInfoService
    {
        private LanguageItemGroup _group = new LanguageItemGroup(-1, "All");

        public virtual string GetDisplayName(string language)
        {
            return CultureInfo.GetCultureInfo(language).EnglishName;
        }

        public virtual IItemGroup GetItemGroup(string language)
        {
            return _group;
        }
    }
}
