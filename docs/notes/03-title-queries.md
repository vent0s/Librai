# 03 — In-memory title queries

## 01 What does this step solve?
previously we implemented Title class, but there is no api exposed for users to query actual book entreis.

so far, there are two methods to fetch books, either fetch all by /titles, or fetch certain entry by /titles/{id}

the entries are now storing on runtime memory, so each time a new InMemoryTitleRepository created, a new dictionary with three dummy entries created.

## 02 How does a request work?

user fires a GET request to server/titles/1

we receive "1" as a route parameter.

the endpoint receives an ITitleRepository instance from DI and calls GetByIdAsync to find the dictionary entry with key "1". The repository returns the entry or null; the endpoint returns Results.Ok(title) or Results.NotFound(). Returning the null result directly previously produced a 200 response.

for now, we create dummy entries per IMTR instance created(into the instance's dictionary), we are using entries' id to be the key value of dictionary since it is unified for everywhere to find certain entry(later if we need to design a proper SCHEMA in relationship-based database, this is the unique-id that register each data entries)

## 03 Why did I choose this design?
the endpoint receives ITitleRepository, which gives us agency to change repository instance from one to anohter, simply change how we designate it on web application instantiation

so far our design is not quite decent since we are holding all entries in runtime ram, if there are limited ram with tremendous amount of data, it will be overloaded. Later we will change it into dynamic in-ram cache dictionary holding frequent-use data only.
