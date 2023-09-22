using Rws.MultiSelectLanguageComboBox.Models;
using Rws.MultiSelectLanguageComboBox.Services;
using Sdl.MultiSelectComboBox.API;
using System.Collections.Generic;

namespace Rws.MultiSelectLanguageComboBox.Example.Services
{
    public class CustomLanguageInfoService : DefaultLanguageInfoService
    {
        private readonly LanguageItemGroup _recentGroup = new LanguageItemGroup(0, "Recently used");

        public override IItemGroup GetItemGroup(string language)
        {
            return RecentLanguages?.Contains(language) ?? false ? _recentGroup : base.GetItemGroup(language);
        }

        public List<string> RecentLanguages { get; set; }
    }
}
