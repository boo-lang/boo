Callable types are a generalization of the concept of a **delegate** in the CLI.

In boo, not only delegates and methods can be called as functions but also any object references of type **System.Type** or any **Boo.Lang.ICallable** implementing type.

In the case of System.Type references, the appropriate constructor will be called, if any. In the case of ICallable: ICallable.Call will.

The language itself allows new callable types to be formally defined through the callable construct:

```boo
callable MyCallable(param1 as int, param2) as bool
```

The example above defines a callable type which takes two arguments of types System.Int32 and System.Object respectively and returns a System.Bool value.

```boo
callable AnotherCallable(param)
```

This example defines a callable type which takes a single argument of type System.Object but has no return value.

```boo
callable UseCallable(c as AnotherCallable)
```

This last one defines a new callable type taking another callable type as its single argument with no return value.

The language allows free interchange of structurally compatible callable references. A callable is considered structurally compatible to a callable reference if the type of the reference is System.Object, Boo.Lang.ICallable or another callable type declaring the same number of arguments or less and all arguments types are also compatible.

For this scheme to work the compiler needs to implement some automatic conversion rules:
<table>
<th> t as Type => ICallable </th>
<tr><td> Boo.Lang.RuntimeServices.CallableType(t) </td></tr>
</table>
<table>
<th> Method reference => ICallable or System.Object </th>
<tr><td> new instance of a private ICallable implementation that takes original object reference (if any) in the constructor and calls the apropriate method in ICallable.Call </td></tr>
</table>
<table>
<th> x as CallableX => y as CallableY </th>
</table>

 
**See Also:**

[Events](Events), [Functions](Functions), [Parameters](Parameters)
