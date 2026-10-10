namespace LibrAI.Domain.Catalog
{
    public interface ICopyRepository
    {
        //search by copy.id, if not exist, return null
        public Task<Copy?> GetByIdAsync(string id);

        //list all copies from designated book, if not exist, return empty collection
        public Task<IReadOnlyList<Copy>> ListByTitleIdAsync(string titleId);

        //add new copy, if same copy.id exist, return false, we don't overwrite physical copy
        public Task<bool> TryAddAsync(Copy copy);
    }
}