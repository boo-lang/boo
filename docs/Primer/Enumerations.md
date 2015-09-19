!!! note
    **Enumeration** - A set of name to integer value associations.

## Declaring an Enumeration

Enumerations are handy to use as fields and properties in `classes`.

```boo
enum Day:
    Sunday
    Monday
    Tuesday
    Wednesday
    Thursday
    Friday
    Saturday

class Action:
    [Property(Day)]
    _day as Day
```

Enumerations are also handy in preventing "magic numbers", which can cause unreadable code.

!!! note
    **Magic Number** - Any number outside of -1, 0, 1, or 2.

Enumerations technically assign an integer value to each value, but that should generally be abstracted from view.

```boo
enum Test:
    Alpha
    Bravo
    Charlie

// is the same as

enum Test:
    Alpha = 0
    Bravo = 1
    Charlie = 2
```

!!! hint
    Except in special cases, do not assign numbers.


## Exercises

1. Think of another good instance of using enums.

