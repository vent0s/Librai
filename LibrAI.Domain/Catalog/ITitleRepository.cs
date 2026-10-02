namespace LibrAI.Domain.Catalog
{
    public interface ITitleRepository
    {
        Task<IReadOnlyList<Title>> ListAsync();
        Task<Title?> GetByIdAsync(string id);
    }
}