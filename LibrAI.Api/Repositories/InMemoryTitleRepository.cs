using LibrAI.Domain.Catalog;

namespace LibrAI.Api.Repositories
{
    public class InMemoryTitleRepository : ITitleRepository
    {
        private Dictionary<string, Title> _titles = new Dictionary<string, Title>();

        public Task<IReadOnlyList<Title>> ListAsync()
        {
            return Task.FromResult<IReadOnlyList<Title>>(_titles.Values.ToList());
        }

        public Task<Title?> GetByIdAsync(string id)
        {
            _titles.TryGetValue(id, out var title);
            return Task.FromResult(title);
        }

        public InMemoryTitleRepository()
        {
            Title book = new Title(
                "1",
                "Demo Book A",
                "DEMO-ISBN-001",
                "Demo Author A",
                "Synthetic catalog entry A for testing title queries.",
                "Demo Publisher"
            );
            _titles.Add(book.Id, book);
            book = new Title(
                "2",
                "Demo Book B",
                "DEMO-ISBN-002",
                "Demo Author B",
                "Synthetic catalog entry B for testing title queries.",
                "Demo Publisher"
            );
            _titles.Add(book.Id, book);
            book = new Title(
                "3",
                "Demo Book C",
                "DEMO-ISBN-003",
                "Demo Author C",
                "Synthetic catalog entry C for testing title queries.",
                "Demo Publisher"
            );
            _titles.Add(book.Id, book);
        }
    }
}
