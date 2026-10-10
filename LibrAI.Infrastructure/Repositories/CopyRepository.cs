using LibrAI.Domain.Catalog;
using LibrAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LibrAI.Infrastructure.Repositories
{
    public class CopyRepository : ICopyRepository
    {
        private readonly LibraryDbContext _dbContext;

        public CopyRepository(LibraryDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Copy?> GetByIdAsync(string id)
        {
            return await _dbContext.Copies                  //we are seraching for copies
                .Include(c => c.Title)                      //and fetch its attaching title btw during this query
                .FirstOrDefaultAsync(c => c.Id == id);      //to seek its copy GUID matching our qurrying one
        }

        public async Task<IReadOnlyList<Copy>> ListByTitleIdAsync(string titleId)
        {
            return await _dbContext.Copies                  //we are seraching for copies
                .AsNoTracking()                             //Read-only query; do not track the returned entities.
                .Include(c => c.Title)                      //and fetch its attaching title btw during this query
                .Where(c => c.TitleId == titleId)           //that its attaching title id matches querring one
                .ToListAsync();                             //put all founded result into a list and return
        }

        public async Task<bool> TryAddAsync(Copy copy)
        {
            ArgumentNullException.ThrowIfNull(copy);
            if (_dbContext.Copies.Local.Any(c => c.Id == copy.Id))
            {
                return false;
            }
            try
            {
                //Mark only the copy as Added; its title already exists.
                _dbContext.Entry(copy).State = EntityState.Added;
                //conduct actual adding action
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException pg &&
                pg.SqlState == PostgresErrorCodes.UniqueViolation &&
                pg.ConstraintName == "PK_Copies"
            )
            {
                //Detach the failed insert so later saves do not retry it.
                _dbContext.Entry(copy).State = EntityState.Detached;
                return false;
            }
        }
    }
}