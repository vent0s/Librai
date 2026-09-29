# 02 domain entities (Title/Copy/Loan) and state transitions

## 1. what does it solve?
- We designed basic data structure of entities and actions,
its parameter are read-only, and can only be tweaked on their own methods.
therefore, once other access the class, they can only access to those 
variables and methods, thus, we can protect those variable from outsider-tweaking by
setting protection and boundary check from within the class

## 2. design strategy and industrial standard implementation
- entity layer (09-28): why do Title and Copy exist as separate entities? why does Loan point to Copy instead of Title? (your own three-questions derivation)
- A: A title is a grand catagory referencing a specific book, but there might be a bunch of 
physical copies of those book, each of them having their own status, available, loaned or 
lost. We need to hold a list of data entities representing exact physical copy, so we know
their exact condition in real life. Loan however is an action conducted by user, we tracking
those action, and granting it a life-cycle, so we could tracking each loan action, inspecting
loaner and copy it attach to.

- Here, we put constructor and read-only attributes on those classes, and designing methods
for foreign access to change the attribute. Because we are designing each class as an enclosed
entity, and of course we are setting boundary to those attributes, so instead of making another manager class to set those parameters, putting method inside the class and apply
our designed constrain on it is more intuitive and strait forward.

- For exception, we throw `InvalidOperationException` for "wrong current state" but `ArgumentException` for "bad input". Because we need to identify which should be blame for the exception, wether from within(state from object itself), from outside(the attributes passing in), or something else.

- why do `Return(DateTime)` / `Renew(DateTime)` take the timestamp as a parameter instead of reading `DateTime.UtcNow` inside? 
- We are going to design test pipeline, so we need passing constructed DateTime for a proper testable scenario. 
   
- why must the "already returned" guard read `Loan.ReturnedAt` instead of `Copy.Status`? 
- If there are two loaner, A return the book, but didn't close the page, B loan the book, 
Then A hit the return again, it will corrupting Copy's status. The more decent way to handle
this is to create a session system for user's action, all of their actions are enclosed inside
their session. Therefore, even if someone reserve the page, they can only conduct action within our boundary.

- rulings to record:
  - CheckIn's `newStatus` parameter: kept or simplified to always-Available? why?
  - We might want to use reuse it for admin input to update abnormal status, such as lost or others, but for now, we are sharing it with general return.

  - `Loan.Return()` calls `Copy.CheckIn()` internally (entity cross-call) instead of the endpoint orchestrating both — why this way, and what does it cost at stage-3 concurrency?
  - Current design might putting actual concurrency endpoint into calling chain, when applying a lock to the Copy, we might need to lock both Loan and Copy, and we also need to write control logic within entities themselves, which is pretty glueish and reduntant. But we don't need an external endpoint at this moment to update a Copy's status, unless we have an Copy management system later, from there we could start considering hosting entities in a more decent way. 
  
  - Renew on an overdue loan: allowed or rejected? decided 
  - we are allowing renewing an overdue loan for now, otherwise user might just took it and vanish away, or later we could having an alert system when renewing overdue loan, applying charges to user, or simply when overdue, giving user certain amount of time to return or renew it, otherwise we fine the loaner, or giving an alert to admin to seeking help from public administration(depending on configuration)


## 3. relevant code
- `LibrAI.Domain/Catalog/Copy.cs` — CheckOut / CheckIn
- `LibrAI.Domain/Circulation/Loan.cs` — Return / Renew
- `docs/adr/0002-domain-modeling-basics.md` — the four recorded decisions behind this step
