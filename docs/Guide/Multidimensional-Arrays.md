When declaring a variable or a field as a multidimensional array, use this syntax:

```boo
foo as (int, 3) //declare a 3 dimensional array of integers.
```

When creating a brand spanking new multidimensional array, use this syntax:

```boo
foo = matrix(int, 2, 3, 4)
//That creates an empty 3 dimensional array.
//1st dimension will have 2 items, 2nd has 3, 3rd has 4
```

Set and retrieve data from the array:

```boo
foo[0,0,1] = 100
print foo[0,0,1]
```

This shows looping over the array to set or get values:

```boo
n = 1
for i in range(len(foo,0)): //# of items in 1st dimension
    for j in range(len(foo,1)): //2nd dimension
        for k in range(len(foo,2)): //3rd dimension
            foo[i,j,k] = n
            ++n
 
//Print the values out in a table format:
columns = len(foo, foo.Rank - 1)
line = []
for item in foo:
    line.Add(item.ToString("00"))
    if len(line) >= columns:
        print join(line)
        line.Clear()
```

The whole code together produces this output:

```boo
100
01 02 03 04
05 06 07 08
09 10 11 12
13 14 15 16
17 18 19 20
21 22 23 24
```

See also [Lists and Arrays](Lists-and-Arrays). [Slicing](Slicing) works for multidimensional arrays too.
