Interfaces are introduced thru the interface keyword:

```boo
interface IFoo:
   pass
```

Interface methods don't need to have a body:

```boo
interface IUnknown:
    def QueryInterface(id as System.Guid) as object
```

But if they do it must be empty:

```boo
interface IFoodMachine:
    def Spam():
        pass
```

If a method in a interface does not declare a return type it is assumed to be void since there's no body with a return statement where to infer the type from. Thus the above interface declaration is equivalent to the following one:

```boo
interface IFoodMachine:
    def Spam() as void
```

Interfaces can extend other interfaces:

```boo
interface ImALumberjackAndImOk:
    def SleepAllNight()
    def WorkAllDay()
 
interface IWishIdBeenAGirlie(ImALumberjackAndImOk):
    def JustLikeMyDearPappa()
```

Interfaces can be partial:

```boo
partial interface I:
    def Foo()
 
partial interface I:
    def Bar()
 
for member in typeof(I).GetMembers():
    print member.Name
```
