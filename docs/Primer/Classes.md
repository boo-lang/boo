!!! note
    **Class** - A cohesive package that consists of a particular kind of compile-time metadata. A class describes the rules by which objects behave. A class specifies the structure of data which each instance contains as well as the methods (functions) which manipulate the data of the object.

!!! note
    **Object** - An instance of a class


## Defining a Class

`Classes` are important because they allow you to split up your code into simpler, logical parts. They also allow for better organization and data manipulation.

```boo
class Cat:
    pass

fluffy = Cat()
```

This declares a blank `class` called "Cat". It can't do anything at all, because there's nothing to do with it. `fluffy`

!!! hint
    Name all your classes using PascalCase. That is, Capitalize every word and don't use spaces. If it includes an acronym, like "URL", call it "Url".


## Fields and Properties

!!! note
    **Field** - An element in a class that contains a specific term of information.

!!! note
    **Property** - A syntax nicety to use instead of getter/setter functions.

Simply, fields hold information and properies are accessors to that information.

```boo
class Cat:
    [Property(Name)]
    _name as string

fluffy = Cat()
fluffy.Name = "Fluffy"
```

1. `class Cat:` declares the start of a class.
  1. `[Property(Name)]` declares a property around `_name`. You named the property "Name".
  2. `_name as string` declares a `field` of `Cat` that is a `string` called `_name`.
2. `fluffy = Cat()` declares an instance of `Cat`.
3. `fluffy.Name = 'Fluffy'` accesses the property `Name` of `Cat` and sets its value to 'Fluffy'. This will cause `Name` to set `_name` to 'Fluffy'.

Fields are not set directly because of security.

!!! hint
    Name all your properties using PascalCase, just like `classes`. Name all your `fields` using _underscoredCamelCase, which is similar to PascalCase, only it is prefixed with an underscore and the first letter is lowercase.

There are two other types of `properties`, a `getter` and a `setter`. Technically, a regular `property` is just the combination of the two.

```boo
class Cat:
    [Getter(Name)]
    _name = 'Meowster'

    [Setter(FavoriteFood)]
    _favoriteFood as string

fluffy = Cat()
print fluffy.Name
fluffy.FavoriteFood = 'Broccoli'

// Output: Meowster
```

If you were to try to assign a value to `fluffy.Name` or retrieve a value from `fluffy.FavoriteFood`, an error would have occurred, because the code just does not exist for you to do that.
Using the `attributes` `Property`, `Getter`, and `Setter` are very handy, but it's actually Boo's shortened version of what is really happening. Here's an example of the full code.

```boo
class Cat:
    Name as string:
        get:
            return _name
        set:
            _name = value

    _name as string

fluffy = Cat()
fluffy.Name = 'Fluffy'
```

Because `fields` are visible inside their own class, you can see that `Name` is just a wrapper around _name`. Using this expanded syntax is handy if you want to do extra verification or not have it wrap exactly around its `field`, maybe by trimming whitespace or something like that first.

`value` is a special keyword for the setter statement, that contains the value to be assigned.

!!! hint
    Property Pre-condition: It is also possible to define a precondition that must be met before setting
    a value directly through the Property shorthand.

```boo
class Cat:
    [Property(Name, Name is not null)]
    _name as string

fluffy = Cat()
fluffy.Name = null # will raise an ArgumentException
```


## Class Modifiers

<table><tbody>
<tr>
<th><p> Modifier </p></th>
<th><p> Description </p></th>
</tr>
<tr>
<td><p> <code>public</code> </p></td>
<td><p> Creates a normal, public class, fully accessible to all other types. </p></td>
</tr>
<tr>
<td><p> <code>protected</code> </p></td>
<td><p> Creates a class that is only accessible by its containing class (the class this was declared in) and any inheriting classes. </p></td>
</tr>
<tr>
<td><p> <code>internal</code> </p></td>
<td><p> A class only accessible by the assembly it was declared in. </p></td>
</tr>
<tr>
<td><p> <code>protected internal</code> </p></td>
<td><p> Combination of protected and internal. </p></td>
</tr>
<tr>
<td><p> <code>private</code> </p></td>
<td><p> Creates a class that is only accessible by its containing class (the class this was declared in.) </p></td>
</tr>
<tr>
<td><p> <code>abstract</code> </p></td>
<td><p> Creates a class that cannot be instanced. This is designed to be a base class for others. </p></td>
</tr>
<tr>
<td><p> <code>final</code> </p></td>
<td><p> Creates a class that cannot be inherited from. </p></td>
</tr>
</tbody></table>

!!! hint
    Never use the public Class Modifier. It is assumed to be public if you specify no modifier.

```boo
abstract class Cat:
    [Property(Name)]
    _name as string
```

The `abstract` keyword is the Class Modifier.


## Inheritance

!!! note
    **Inheritance** - A way to form new classes (instances of which will be objects) using pre-defined objects or classes where new ones simply take over old ones's implemetions and characterstics. It is intended to help reuse of existing code with little or no modification.

Inheritance is very simple in Boo.

```boo
class Cat(Feline):
    [Property(Name)]
    _name as string

class Feline:
    [Property(Weight)]
    _weight as single //In Kilograms
```

This causes `Cat` to inherit from `Feline`. This gives the members `Weight` and `_weight` to `Cat`, even though they were not declared in `Cat` itself.

You can also have more than one `class` inherit from the same `class`, which promotes code reuse.

More about inheritance is covered in [Part 10 - Polymorphism, or Inherited Methods](https://github.com/bamboo/boo/wiki/Boo-Primer:-%5BPart-10%5D-Polymorphism)

`Classes` can inherit from one or zero other `classes` and any number of `interfaces.

To inherit from more than one interface, you would use the notation `class Child(IBaseOne, IBaseTwo, IBaseThree):`

## Interfaces

!!! note
    **Interface** - An interface defines a list of methods that enables a class to implement the interface itself.

`Interfaces` allow you to set up an API (Application Programming Interface) for `classes` to base themselves off of.
No implementation of code is put inside `interfaces`, that is up to the `classes`.
`Interfaces` can inherit from any number of other `interfaces`. They cannot inherit from any `classes`.

```boo
interface IFeline:
    def Roar()

    Name:
        get
        set
```

This defines IFeline having one method, Roar, and one `property`, Name. `Properties` must be explicitly declared in `interfaces`. Methods are explained in Part 09 - Methods.

!!! hint
    Name your interfaces using PascalCase prefixed with the letter `I`, such as `IFeline`.


## Difference between Value and Reference Types

There are two types in the Boo/.NET world: Value and Reference types. All classes form Reference types. Numbers and such as was discussed in Part 02 - Variables#List of Value Types are value types.

!!! note
    **null** - A keyword used to specify an undefined value for reference variables.

Value types can never be set to `null`, they will always have a default value. Numbers default value will generally be 0.


## Exercises

1. Create a class that inherits from more than one interface.
2. See what happens if you try to inherit from more than one class.

