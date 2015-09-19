**Introduction**

Martin Fowler has a [good introduction to closures](http://www.martinfowler.com/bliki/Closure.html) on his bliki.

**Syntax**

There are two syntaxes for closures: a block based syntax with syntactically significant whitespace and a braces based that ignores whitespace.

**Block based syntax**

```boo
import System.Windows.Forms
button = Button(Text: "Click me")
button.Click += def ():
    print("$button was clicked!")
    print("and yes, this is just like any other block...")
```

**Braces based**

```boo
import System.Windows.Forms
button1 = Button(Text: "Click me", Click: { print "clicked!" })
button2 = Button(Text: "Me too!")
button2.Click += { print "$button2 was clicked!";
    print "whitespace is ignored inside {}...";
    print "that's why you need to use semicolons to include multiple statements...";
    print "but please, don't write code like this just because you can :)"
}
```

**Semantics**

Boo closures have have full access (to read and write) to their enclosing lexical environment. For Instance:

```boo
a = 0 # declare a new variable
getter = { return a }
setter = { value | a = value }

assert 0 == getter()
setter(42)
assert 42 == getter()
assert 42 == a
```

The best source of information right now are the test cases for closures in the [tests/testcases/integration directory](https://github.com/bamboo/boo/tree/master/tests/testcases/integration).

**Closures vs. Functions**

See [Functions As Objects](../Primer/Functions-As-Objects).

Some things you can do with named functions that you cannot with closures include recursion and overloading:

This will not work because "c" is unknown from inside the closure:

```boo
c = do(x as int):
    print x
    --x
    c(x) if x > 0
c(5)
```

so you can use a regular named function or else create a 2nd callable to hold the name:

```boo
def c(x as int):
    print x
    --x
    c(x) if x > 0
c(5)

//or:

d as callable
c = do(x as int):
    print x
    --x
    d(x) if x > 0
d = c
c(5)
```

And you can use regular named functions to overload a method:

```boo
def doit(x as int):
    print x
def doit(x as string):
    print x

doit(3)
```

**See Also**

[Functions](Functions), [Parameters](Parameters), [Callable Types](Callable-Types), [Events](Events)
