Here's an example illustrating some of the basics about classes.

Until more documentation is here, for examples of using classes, see the **classes-**.boo files under
[tests/testcases/integration/types](https://github.com/bamboo/boo/tree/master/tests/testcases/integration/types),
as well as **super-**.boo, **abstract-**.boo, **interfaces-**.boo, **baseclass-**.boo, **innerclasses-**.boo, **fields-**.boo,
and **properties-**.boo.

```boo
interface IAnimal:
    Name as string:
        get
 
    Legs as int:
        get
 
    def Eat(food)
 
class Dog(IAnimal):
    Name:
        get:
            return "Canine"
 
    Legs:
        get:
            return 4
 
    def Eat(food):
         if food isa IPlant:
                Speak()
 
    def Speak():
         print "bark!"
 
interface IPlant:
    pass
 
class Rice(IPlant):
    pass
 
//////////////////////////
 
d = Dog()
 
print "$(d.Name) has $(d.Legs) legs."
 
r = Rice()
d.Eat(r)
```
