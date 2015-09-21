!!! note
    **Struct** - Short for structure, a term meaning a data group made of related variables.

The main way `structs` are different than `classes` is that they are value types instead of a reference types. This means that whenever you return this value, or set one equal to another, it is actually copying the data not a reference to the data. This is handy, because if it is declared without a value, it will default to something besides `null`. It also cannot be compared to `null`. This eliminates a lot of error checking associated with reference types.

`Structs` also cannot inherit from `classes`, nor can `classes` inherit from `structs`. `Structs` can however, inherit from `interfaces`.

Unlike some other languages, `structs` can have methods.


## Declaring a Struct

Declaring a struct is very similar to declaring a `class`, except that the name is changed.

```boo
struct Coordinate:
    def constructor(x as int, y as int):
        _x = x
        _y = y

    _x as int
    _y as int

c as Coordinate
print c._x, c._y
c = Coordinate(3, 5)
print c._x, c._y

// Output:
// 0 0
// 3 5
```

Here you can see that the `struct` was instanced without being called, showing the how a `struct` is a value.


## Exercises

1. Figure out a good exercise for this section.

