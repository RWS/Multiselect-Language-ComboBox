using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Rws.MultiSelectLanguageComboBox.Services
{
    public interface ILanguageSuggestionProvider
    {
        Task<IList<string>> GetSuggestionsAsync(string criteria, CancellationToken cancellationToken);
        Task<IList<string>> GetSuggestionsAsync(CancellationToken cancellationToken);
        bool HasMoreSuggestions { get; }
    }
}