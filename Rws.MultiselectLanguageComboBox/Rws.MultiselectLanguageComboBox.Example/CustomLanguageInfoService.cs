using Rws.MultiselectLanguageComboBox.Models;
using Rws.MultiselectLanguageComboBox.Services;
using Sdl.MultiSelectComboBox.API;
using System.Collections.Generic;

namespace Rws.MultiselectLanguageComboBox.Example
{
    public class CustomLanguageInfoService : DefaultLanguageInfoService
    {
        private LanguageItemGroup _recentGroup = new LanguageItemGroup(0, "Recently used");

        public override IItemGroup GetItemGroup(string language)
        {
            return RecentLanguages?.Contains(language) ?? false ? _recentGroup : base.GetItemGroup(language);
        }

        public List<string> RecentLanguages { get; set; }
    }
}
