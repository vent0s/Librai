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
        public string Id { get; private set; }
        public Title Title { get; private set; }



        public CopyStatus Status { get; private set; }

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
            if (!Enum.IsDefined(typeof(CopyStatus), status))
            {
                throw new ArgumentException("Invalid status value.", nameof(status));
            }
            Status = status;
        }
    }
}