### The for loop

The for loop syntax is:

```boo
for var in range:
    // action
```

where range can be any type of collection. Examples:

```boo
// print numbers from 0 until 9
for i in range(0, 10):
    print(i)
```

it is equivalent to

```C#
for (int i = 0; i < 10; i++) 
    // print
```

from C# or Java. You can also use with lists:

```boo
for i in [300, 100, 23, 1, 55]:
    print(i)
```

```boo
itens = [2, 44, 56, 123, 98, 77, 1000]
for i in itens:
    print (i)
```

or arrays:

```boo
for i in (1, 4, 98, 399, 1000, 34, 199):
    print (i)
```

### The while loop

```boo
i = 0
while i < 10:
    print (i)
    ++i
```

```boo
i = 0
while not(i > 10):
    print (i)
    ++i
```

Iterating over collections:

```boo
import System.Collections
 
class Test:
    def showAllValues(items as IList):
        i = 0
        itemsLen = len(items)
        while i < itemsLen and items[i].GetType() is not int:
            print("Bad, bad type: " + items[i].GetType())
            ++i
 
class Foo:
    pass
 
t = Test()
f = Foo()
t.showAllValues(["1", t, f, 87, 31])
```
