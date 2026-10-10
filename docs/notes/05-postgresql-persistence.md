# Database implementation

we are now officially removed in-memory repo and implemented postgreSQL, it provides persistence data storage and friendly on data querrying and modifying, later, it will be our foundation to implement Copy and Loan system.

We use EF Core and Npgsql. TitleRepository uses LibraryDbContext to access Title data, and migrations create the database table. TitleRepository is registered as scoped in the API's DI container, while the endpoints still depend on ITitleRepository.