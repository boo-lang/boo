### Lists

Lists are mutable sequences of items. They are similar to lists in Python.

Lists are surrounded by brackets, just like python: [1,2,3]

Here is some sample code demonstrating different operations you can do on lists:

```boo
//the "as List" is optional:
mylist as List = ["foo", "bar", "baz", 0, 1, 2]
print mylist
 
for item in mylist:
    print item
 
print "len(mylist): " + len(mylist)
 
for i in range(mylist.Count):
    print mylist[i]
 
 
print mylist.Join("=") //foo=bar=baz=0=1=2
 
mylist[1] = "bbb" //set 2nd item
print mylist
print "mylist[1]: " + mylist[1] //print 2nd item
print "Index of \"bbb\": " + mylist.IndexOf("bbb")
mylist.Add("new item") //also mylist.Push does same thing
print mylist
mylist.Insert(4,"abc") //insert "abc" at 5th spot
print mylist
mylist.Remove("bbb") //remove the "bbb" item
print mylist
mylist.RemoveAt(1) //remove the 2nd item
print mylist
mylist.Remove(0) //remove the zero item (not 1st item)
print mylist
mylist += ["test"]
print mylist
mylist.Extend(["one","two"])
print mylist
lastitem = mylist.Pop() //removes last item from list and returns it
print mylist
 
if mylist == ["foo","abc",1,2,"new item","test","one"]:
    print "equals other list"
 
//List comprehensions:
print "just the string items: " + [i for i in mylist if i isa string]
print "just the int items:    " + [i for i in mylist if i isa int]
//"i isa string" is the same as "i.GetType() == string"
 
//another way to do it using the "as" cast: (see Casting Types)
//   (i as string) will be null if item is an integer
print "just the string items: " + [stritem for i in mylist
                    if stritem=(i as string)]
print "just 3 letter items:   " + [stritem for i in mylist
                    if stritem=(i as string)
                    and len(stritem) == 3]
 
if "foo" in mylist:
    print "foo is in mylist"
 
//another way to do the same thing:
if mylist.Contains("foo"):
    print "contains foo"
 
if "baditem" not in mylist:
    print "baditem is not in mylist"
 
//You can also pass a special search function or closure to Find.
//Find returns the first item that returns true from the search function.
if mylist.Find({item | return item == "foo"}):
    print "found foo"
 
//using a multiline do/def closure with Find:
result = [1, 2, 3].Find() do (item as int):
        return item > 1
print "item > 1 in [1,2,3]: " + result
 
//Collect is similar to Find but it returns a list of all items that
//return true from the search function.
result = [1, 2, 3].Collect() do (item as int):
        return item > 1
print "all items > 1 in [1,2,3]: " + result
 
//Sorting a list.  Only works if all the items are comparable
//(all ints or strings for example)
//or if you pass your own compare function.
newlist =  [2, 4, 3, 5, 1]
print newlist
print newlist.Sort()
result = newlist.Sort() do (left as int, right as int):
    return right - left //reverse sort order
print result
 
mylist = mylist * 3
print mylist
 
mylist.Clear()
```

### Arrays

Arrays are similar to arrays in C, C#, or Java. They are initialized to a certain fixed length, and all the items in an array are generally of the same type (int, string, etc.).

In boo, arrays use parentheses to surround the items instead of brackets: (1,2,3).

If you need to declare the type of an array, use parentheses surrounding the type of item in the array, such as (int) for an array of ints or (object) for an array of objects.

Boo's arrays and lists are zero-based, so the first item is item 0, the 2nd is item 1, etc.

```boo
//Create an array with 3 strings - the 'as (string)' part is optional
myarray as (string) = ("one", "two", "three")
for i in myarray:
    print i
 
//Create an array to hold 10 integers:
a as (int) = array(int,10)
//set the 2nd item to 3
a[1] = 3
print a[1]
 
//Convert from array to list:
mylist = [item for item in myarray]
print mylist
 
//Convert from list back to array:
 
//This is the preferred way to do it:
array2 = array(string, mylist)
 
//but there is this way too: (compiler can't tell the type though)
array3 = mylist.ToArray(string)
for i in array2:
    print i
 
// Append two arrays:
a = (1, 2) + (3, 4)
assert a == (1, 2, 3, 4)
```

### Byte Arrays and Char Arrays

Creating byte arrays - arrays of type (byte).

Boo right now doesn't support implicitly converting int literals to type byte, so you need to use the array() builtin function.

```boo
#won't work: bytes as (byte) = (1,2,3,4,5)
 
//int to byte
bytes = array(byte,(1,2,3,4,5))
 
print bytes.GetType() //-> System.Byte[]
 
for b in bytes:
    print b
 
//hex literals to byte
bytes = array(byte, (0xFF, 0xBB, 0x3F))
for b in bytes:
    print b
 
//chars or string to a byte array:
bytes = System.Text.ASCIIEncoding().GetBytes("abcABC")
for b in bytes:
    print b
```

Char arrays - arrays of type (char).

Boo also right now doesn't have support for implicitly converting single character string literals (like "a") to the char type. A workaround it to use slicing ("a"0), but in this case you can use the ToCharArray method of the string class instead:

```boo
#won't work: charbytes as (char) = ("a", "b", "c")
 
//String.ToCharArray()
charbytes = "abcABC".ToCharArray()
 
print charbytes.GetType() //-> System.Char[]
 
for b in charbytes:
    print b
```

### Creating an empty, zero-length array

Sometimes a system function might require passing an array even if it is empty. Here are some ways to create an empty zero-length array of type (object):

```boo
emptyArray = array(object, 0)
 
//other ways:
emptyArray = array(object,[])
emptyArray = [].ToArray(object)
emptyArray = array([])
 
//If the type is to be (object), then you can simply use:
emptyArray = (,)
```

### Slicing Lists and Arrays

You can also "slice" lists and arrays to get a particular subset of a list or array.

### Multidimensional Arrays

Recently boo added support for [Multidimensional Arrays](Multidimensional-Arrays), also called rectangular arrays.
