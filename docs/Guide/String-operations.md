### String Formatting

See [String Interpolation](../Features/String-Interpolation).  Here's a quick example:

```boo
firstname = "First"
lastname = "Last"
 
print "Your name is $firstname $lastname."
 
print "Now is $(date.Now)."
```

### Backtick quoted strings

When we want to provide a string literal in its verbatim form we can use backtick "`" characters to quote the string, disabling any interpolation or formatting in it.

```boo
print `this string is not $interpolated`
# output: this string is not $interpolated
```

Additionally backtick quoted strings can expand multiple lines while retaining the actual whitespace in the source file. This makes them very useful for DSLs that want to process the literals:

```boo
eval `
  foo = 10
  print foo
`
```

### ToString() Method

To convert a class or a type to a string, call the ToString() method. Or if you are writing your own class you can define your own ToString() method to control how your class is printed.


### Converting from int to string and Back

```boo
//string to int
val as int
val = int.Parse("1000")
print val
 
//string to double
pi as double
pi = double.Parse("3.14")
print pi
 
//int or double to string
s as string
s = val.ToString()
print s
 
//multiple values to one formatted string
astr as string
astr = "$val and $pi"
print astr
 
//date parsing
d as date
d = date.Parse("12/03/04")
```


### Parsing and Converting Other Types

See [this tutorial](http://samples.gotdotnet.com/quickstart/howto/doc/parse.aspx) on the Parse and Convert techniques, as well as date parsing.


### String Comparisons

```boo
//regular comparison
if "asdf" == "ASDF":
    print "asdf == ASDF"
else:
    print "asdf != ASDF"
 
//case-insensitive comparison
if string.Compare("asdf", "ASDF", true) == 0:
    print "case-insensitively the same"
 
s = "Another String"
 
if s.StartsWith("Another"):
    print "starts with 'Another'"
if s.EndsWith("String"):
    print "ends with 'String'"
 
print "'String' starts at:", s.IndexOf("String")
 
print "The last t is at:", s.LastIndexOf("t")
 
//use built-in regex support to split a string
words = @/ /.Split(s) //split on whitespace (could use \s)
for word in words:
    print word
```

See the .NET reference on [Basic String Operations](http://msdn.microsoft.com/en-us/library/a292he7t%28v=VS.100%29.aspx) for more information on string comparisons.
