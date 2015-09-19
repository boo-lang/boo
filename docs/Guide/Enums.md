The keyword enum introduces an enumerated type, a set of name to integer value associations.

```boo
enum Python:
     Eric
     John
     TerryG
     TerryJ
     Graham
     Michael
```

which is equivalent to:

```boo
enum Python:
     Eric = 0
     John = 1
     TerryG = 2
     TerryJ = 3
     Graham = 4
     Michael = 5
```

Extracting the integer value of a enum is just a matter of casting it to an int:

```boo
print((Python.TerryG cast int)*2) # prints 4
```

It's possible to define partial enums:

```boo
partial enum E:
    Foo

partial enum E:
    Bar

for value in System.Enum.GetValues(E):
    print value, "=", value cast int
```

