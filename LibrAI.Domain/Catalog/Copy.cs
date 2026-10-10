namespace LibrAI.Domain.Catalog
{
    public enum CopyStatus
    {
        Available,
        Loaned,
        Reserved,
        Lost
    }
    public class Copy
    {
        public string Id { get; private set; } = null!;
        public Title Title { get; private set; } = null!;
        public string TitleId { get; private set; } = null!;



        public CopyStatus Status { get; private set; }

        private Copy()
        {

        }

        public Copy(string id, Title title, CopyStatus status)
        {
            //later we will have a better way to generate unique ids for copies, 
            // but for now we will just use a string
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));

            }
            Id = id;
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title), "Title cannot be null.");
            }
            Title = title;
            TitleId = title.Id;
            if (!Enum.IsDefined(typeof(CopyStatus), status))
            {
                throw new ArgumentException("Invalid status value.", nameof(status));
            }
            Status = status;
        }

        public void CheckOut()
        {
            if (Status != CopyStatus.Available)
            {
                throw new InvalidOperationException("Copy is not available for checkout.");
            }
            Status = CopyStatus.Loaned;
        }

        public void CheckIn(CopyStatus newStatus = CopyStatus.Available)
        {
            if (Status != CopyStatus.Loaned)
            {
                throw new InvalidOperationException("Copy is not currently loaned.");
            }
            if (!Enum.IsDefined(typeof(CopyStatus), newStatus) || newStatus == CopyStatus.Loaned)
            {
                throw new ArgumentException("Invalid status value for check-in.", nameof(newStatus));
            }
            Status = newStatus;
        }

    }
}