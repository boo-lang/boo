To handle exceptions in Boo you use the try - except block, and an optional ensure keyword, which may be used to have some code executed no matter the exception was raised or not. Just for a comparative, it is equivalent to try - catch - finally block from C# or Java.

Executes some code with exception handling:

```boo
try:
    c = SomeClass()
    c.DangerousAction(7)
except e:
    print("ooops... We've got an error: " + e)
```

to raise exceptions, use the raise keyword:

```boo
class SomeClass:
    def DangerousAction(i as int):
        if i < 10:
            raise "Invalid argument. It must not be less than 10"
 
        // Continue with the method
```

You can also handle specific Exception types using the form

```boo
except var as type:
```

as in 

```boo
class MyInvalidArgumentException(System.Exception):
    def constructor(argument, message):
        super("Error: " + argument + " is not valid. " + message)
 
class SomeClass:
    def DangerousAction(i as int):
        if i < 10:
            raise MyInvalidArgumentException(i, "It must not be less than 10")
 
        // Continue with the method
 
try:
    c = SomeClass()
    c.DangerousAction(2)
except e as MyInvalidArgumentException:
    print("Error: " + e)
except e as AnotherTypeOfException:
    // handle it
```

Using _ensure_ to do some clean-up, post-executing action etc...:

```boo
try:
    c = SomeClass()
    c.DangerousAction(33)
except e:
    print("ooops... We've got an error: " + e)
ensure:
    print("This code will always be executed.")
```
