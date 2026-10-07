using LibrAI.Domain.Catalog;
using System.Collections.Concurrent;

namespace LibrAI.Api.Repositories
{
    public class InMemoryTitleRepository : ITitleRepository
    {
        private ConcurrentDictionary<string, Title> _titles = new ConcurrentDictionary<string, Title>();

        public Task<IReadOnlyList<Title>> ListAsync()
        {
            return Task.FromResult<IReadOnlyList<Title>>(_titles.Values.ToList());
        }

        public Task<Title?> GetByIdAsync(string id)
        {
            _titles.TryGetValue(id, out var title);
            return Task.FromResult(title);
        }

        public Task<bool> TryAddAsync(Title title)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }
            return Task<bool>.FromResult(_titles.TryAdd(title.Id, title));
        }

        public InMemoryTitleRepository()
        {
            Title book = new Title(
                Guid.NewGuid().ToString(),
                "Demo Book A",
                "DEMO-ISBN-000",
                "Demo Author A",
                "Synthetic catalog entry A for testing title queries.",
                "Demo Publisher"
            );
            _titles.TryAdd(book.Id, book);
            book = new Title(
                Guid.NewGuid().ToString(),
                "Demo Book B",
                "DEMO-ISBN-001",
                "Demo Author B",
                "Synthetic catalog entry B for testing title queries.",
                "Demo Publisher"
            );
            _titles.TryAdd(book.Id, book);
            book = new Title(
                Guid.NewGuid().ToString(),
                "Demo Book C",
                "DEMO-ISBN-002",
                "Demo Author C",
                "Synthetic catalog entry C for testing title queries.",
                "Demo Publisher"
            );
            _titles.TryAdd(book.Id, book);
        }
    }
}
