using Rws.MultiselectLanguageComboBox.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rws.MultiselectLanguageComboBox.Example.Services
{
    public class CustomSuggestionProvider : ILanguageSuggestionProvider
    {
        private const int batchSize = 30;
        private string _criteria = string.Empty;
        private int _skipCount;

        private readonly ObservableCollection<string> _observableCollection = new ObservableCollection<string>();
        private readonly ICollection<string> _source;

        private readonly ILanguageInfoService _languageInfoService = new DefaultLanguageInfoService();

        public CustomSuggestionProvider(ICollection<string> source)
        {
            _source = source;
        }

        public bool HasMoreSuggestions { get; private set; } = true;

        public Task<IList<string>> GetSuggestionsAsync(string criteria, CancellationToken cancellationToken)
        {
            _criteria = criteria;
            var newItems = _source.Where(x => _languageInfoService.GetDisplayName(x).IndexOf(_criteria, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (cancellationToken.IsCancellationRequested)
                return null;
            HasMoreSuggestions = newItems.Count > batchSize;
            _skipCount = batchSize;
            return Task.FromResult<IList<string>>(newItems.Take(batchSize).Cast<string>().ToList());
        }

        public Task<IList<string>> GetSuggestionsAsync(CancellationToken cancellationToken)
        {
            var newItems = _source.Where(x => _languageInfoService.GetDisplayName(x).StartsWith(_criteria)).Skip(_skipCount).ToList();
            if (cancellationToken.IsCancellationRequested)
                return null;
            HasMoreSuggestions = newItems.Count > batchSize;
            _skipCount += batchSize;
            return Task.FromResult<IList<string>>(newItems.Take(batchSize).Where(x => !_observableCollection.Any(y => y == x)).Cast<string>().ToList());
        }
    }
}