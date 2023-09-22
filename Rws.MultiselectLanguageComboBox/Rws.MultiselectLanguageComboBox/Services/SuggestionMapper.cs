using Rws.MultiSelectLanguageComboBox.Models;
using Sdl.MultiSelectComboBox.API;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rws.MultiSelectLanguageComboBox.Services
{
    internal class SuggestionMapper : ISuggestionProvider
    {
        private readonly ILanguageSuggestionProvider _languageSuggestionProvider;
        private readonly List<LanguageItem> _languageItems;

        public SuggestionMapper(ILanguageSuggestionProvider languageSuggestionProvider, ObservableCollection<LanguageItem> languageItems)
        {
            _languageSuggestionProvider = languageSuggestionProvider;
            _languageItems = new List<LanguageItem>(languageItems);
        }

        public bool HasMoreSuggestions => _languageSuggestionProvider.HasMoreSuggestions;

        public async Task<IList<object>> GetSuggestionsAsync(string criteria, CancellationToken cancellationToken)
        {
            var languageSuggestions = await _languageSuggestionProvider.GetSuggestionsAsync(criteria, cancellationToken);
            return languageSuggestions.Select(l => _languageItems.FirstOrDefault(i => i.Id == l)).Where(i => i != null).Cast<object>().ToList();
        }

        public async Task<IList<object>> GetSuggestionsAsync(CancellationToken cancellationToken)
        {
            var languageSuggestions = await _languageSuggestionProvider.GetSuggestionsAsync(cancellationToken);
            return languageSuggestions.Select(l => _languageItems.FirstOrDefault(i => i.Id == l)).Where(i => i != null).Cast<object>().ToList();
        }
    }
}
