namespace LibrAI.Domain.Catalog
{
    public class Title
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Isbn { get; private set; }
        public string? Description { get; private set; }
        public string Author { get; private set; }

        public string? Publisher { get; private set; }

        public Title(string id, string name, string isbn, string author, string? description = null, string? publisher = null)
        {
            //later we will have a better way to generate unique ids for copies, 
            // but for now we will just use a string
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));

            }
            Id = id;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            Name = name;
            if (string.IsNullOrWhiteSpace(isbn))
            {
                throw new ArgumentException("ISBN cannot be null or empty.", nameof(isbn));
            }
            Isbn = isbn;
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Author cannot be null or empty.", nameof(author));
            }
            Author = author;
            Description = description;
            Publisher = publisher;
        }
    }
}