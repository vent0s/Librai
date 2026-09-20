why mutable struct is anti-pattern

    struct is a value tpye variable, each time we assign this type, it generates a copy rather than passing reference of an object. 
    thus, if a struct is passing, and its variables can be changed, it might cause issue if somewhere changes its value(since, it is not an object, changing it won't affect its copies)

    idealy, we wants a datastructure that is immutable once created, there are various ways:
        read only struct, it is value type data still, but writing constructor method is pretty annoying
        class, it is object type data, by default it is mutable, and you still have to write its definition and possibly constructor
        record, it is still an object type data, but readonly (init-only), and it only take one line to define, decent and elegant. 
        readonly record struct, value type data, readonly (init-only), yet, it inherits the fine print of value semantics: a default instance that bypasses your constructor (string members come out null), boxing when treated as object, and a copy cost that grows with size — so it's reserved for small, hot, allocation-sensitive data.

        
        record is the default choice for data-carrying types; we step down to readonly record struct only when profiling shows allocation pressure on a small, hot type.

HealthzResponse r = Get();
r.status = "down";          // compiles, but mutates only this copy — the original is untouched
responses[0].status = "x";  // CS1612: compiler blocks this exact trap for collections
