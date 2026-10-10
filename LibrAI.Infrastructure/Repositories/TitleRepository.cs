using LibrAI.Domain.Catalog;
using LibrAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LibrAI.Infrastructure.Repositories
{
    public class TitleRepository : ITitleRepository
    {
        private readonly LibraryDbContext _dbContext;
        public TitleRepository(LibraryDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Title>> ListAsync()
        {
            //as no tracking means Entity Framework will not track query result, it is appropriate for readding-only action
            return await _dbContext.Titles.AsNoTracking().ToListAsync();
        }

        public async Task<Title?> GetByIdAsync(string id)
        {
            return await _dbContext.Titles.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> TryAddAsync(Title title)
        {
            ArgumentNullException.ThrowIfNull(title);
            if (_dbContext.Titles.Local.Any(t => t.Id == title.Id))
            {
                return false;
            }
            try
            {
                //pending add
                _dbContext.Titles.Add(title);
                //conduct actual adding action
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException pg &&
                pg.SqlState == PostgresErrorCodes.UniqueViolation &&
                pg.ConstraintName == "PK_Titles"
                )
            {
                _dbContext.Entry(title).State = EntityState.Detached;
                return false;
            }
        }
    }
}