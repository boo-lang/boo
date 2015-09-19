Functions do not necessarily have to be created inside of classes in Boo.

This is valid code:

```boo
def doWickedAndNaughtyThings():
     print "I'm doing evil, wicked things to you."
     print "No, don't call the police!"
     print "...crap."
 
doWickedAndNaughtyThings()
```

So is this:

```boo
class Wife:
    def MakeSandwich(toppings as (string) ):
        for items in toppings:
            GruelOverStove(items)
        return Sandwich(toppings.Length)
    def GruelOverStove(item):
        print "$(item)?! $(item)?! Who eats $(item)s?!"
 
class Sandwich:
    def constructor(length):
        self.toppingCount= length
    public toppingCount as int
 
//Here's when things go procedural!
def EatSandwich(sammich):
    print "What, only $((sammich as Sandwich).toppingCount) toppings?!"
 
redhead = Wife()
EatSandwich(redhead.MakeSandwich( ("Pickles", "Turkey", "Mayonase", "Mustard", "Lettuce")  ) )
print "Delicious! Now, where's the remote control?"
```

Functions can also be placed inside other functions (these are called Closures or "blocks" in some languages).

```boo
def SpiceyMayo():
    somethingNotDeservingOfAFunction = def():
        print "lol"
    somethingNotDeservingOfAFunction()
SpiceyMayo()
```

Closures are also handy in a variety of other situations.

There are also 3 special functions that can be used in classes. They are constructor, static constructor, and destructor. Each have no return type, modifiers, or attributes. Only the plain constructor takes parameters. Constructors are invoked when an instance is created. Static constructors are called only the first time the type is used. They should be used to initialize uninitialized static fields. Destructors perform commands when objects are freed.

### See also:

[Parameters](https://github.com/bamboo/boo/wiki/Parameters), [Callable Types](https://github.com/bamboo/boo/wiki/Callable-Types), [Events](https://github.com/bamboo/boo/wiki/Events)  
[Closures in Boo](https://github.com/bamboo/boo/wiki/Closures)  
[Martin Fowler yammering about Closures](http://www.martinfowler.com/bliki/Closure.html)
