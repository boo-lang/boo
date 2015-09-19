!!! note
    **Namespace** - A name that uniquely identifies a set of objects so there is no ambiguity
    when objects from different sources are used together.

`Namespaces` are useful because if you have, for example, a `Dog` namespace and a `Furniture namespace`, and they both have a `Leg class`, you can refer to `Dog.Leg` and `Furniture.Leg` and be clear about which `class` you are mentioning.


## Declaring a Namespace

To declare a `namespace`, all that is required is that you put `namespace` followed by a name at the top of your file.

```boo
namespace Tutorial

class Thing():
    pass
```

This creates your class `Tutorial.Thing`. While coding inside your `namespace`, it will be transparently Thing.
To declare a `namespace` within a `namespace`, just place a dot . inbetween each other.

!!! hint
    Declare a namespace at the top of all your files. Use PascalCase for all your namespaces.


## Importing Another Namespace

To use `classes` from another `namespace`, you would use the `import` keyword.

The most common `namespace` you will import is `System`.

```boo
import System
Console.WriteLine()
```

not importing from a namespace:

```boo
System.Console.WriteLine()
```

Both produce the exact same code, it's just easier and clearer with the `import`.

!!! hint
    Don't be afraid to import, just don't import namespaces that you aren't using.

!!! hint
    When importing, import included namespaces first, such as `System` or `Boo.Lang`. Then import
    your 3rd party namespaces. Alphabetize the two groups seperately.

If you are importing from another assembly, you would use the phrase `import <target> from <assembly>`, for example

importing from an external assembly:

```boo
import System.Data from System.Data
import Gtk from "gtk-sharp"
```

`System.Data` is part of an external library which can be added, `System.Data.dll`. `Gtk` is part of the Gtk# library, which, since it has a special name (with a dash in it), it must be quoted.

!!! hint
    Only use the `import <target> from <assembly>` if you are using one file and one file only. If you are using more than that, you should be using a build tool, such as [NAnt](http://nant.sourceforge.net/), which is discussed in [Part 19 - Using the Boo Compiler](https://github.com/bamboo/boo/wiki/Boo-Primer:-%5BPart-19%5D-Using-the-Boo-Compiler).


## Exercises

1. Figure out a good exercise for this section.

