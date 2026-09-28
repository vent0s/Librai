using LibrAI.Domain.Catalog;

namespace LibrAI.Domain.Circulation
{
    public class Loan
    {
        public string Id { get; private set; }
        public Copy Copy { get; private set; }

        public string BorrowerId { get; private set; }
        public DateTime BorrowedAt { get; private set; }
        public DateTime DueAt { get; private set; }
        public DateTime? ReturnedAt { get; private set; }

        public Loan(string id, Copy copy, string borrowerId, DateTime borrowedAt, DateTime dueAt, DateTime? returnedAt = null)
        {
            //later we will have a better way to generate unique ids for copies, 
            // but for now we will just use a string
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));

            }
            Id = id;
            if (copy == null)
            {
                throw new ArgumentNullException(nameof(copy), "Copy cannot be null.");
            }
            Copy = copy;
            if (string.IsNullOrWhiteSpace(borrowerId))
            {
                throw new ArgumentException("BorrowerId cannot be null or empty.", nameof(borrowerId));
            }
            BorrowerId = borrowerId;
            if (borrowedAt > DateTime.UtcNow)
            {
                throw new ArgumentException("BorrowedAt cannot be in the future.", nameof(borrowedAt));
            }
            BorrowedAt = borrowedAt;
            if (dueAt <= borrowedAt)
            {
                throw new ArgumentException("DueAt must be after BorrowedAt.", nameof(dueAt));
            }
            DueAt = dueAt;
            if (returnedAt != null && returnedAt < borrowedAt)
            {
                throw new ArgumentException("ReturnedAt cannot be before BorrowedAt.", nameof(returnedAt));
            }
            ReturnedAt = returnedAt;
        }
    }
}