!!! note
    **Function** - A sequence of code which performs a specific task, as part of a larger program, and is grouped as one, or more, statement blocks.

## Builtin Functions

You have already seen a few functions. `range()`, `print()`, and `join()`.

These are functions built into Boo.

Here's a list of all the builtin functions that Boo offers:

<table><tbody>
<tr>
<th><p> Name </p></th>
<th><p> Description </p></th>
<th><p> Syntax example </p></th>
</tr>
<tr>
<td><p> <code>print</code> </p></td>
<td><p> Prints an <code>object</code> to Standard Out. The equivilent of <code>System.Console.WriteLine</code> </p></td>
<td><p> <code>print("hey")</code> </p></td>
</tr>
<tr>
<td><p> <code>gets</code> </p></td>
<td><p> Returns a <code>string</code> of input that originates from <code>System.Console.ReadLine()</code> - Standard Input </p></td>
<td><p> <code>input = gets()</code> </p></td>
</tr>
<tr>
<td><p> <code>prompt</code> </p></td>
<td><p> Prompts the user for information. </p></td>
<td><p> <code>input = prompt("How are you? ")</code> </p></td>
</tr>
<tr>
<td><p> <code>join</code> </p></td>
<td><p> Walk through an <code>IEnumerable</code> <code>object</code> and put all of those elements into one string. </p></td>
<td><p> <code>join([1, 2, 3, 4, 5]) == "1 2 3 4 5"</code> </p></td>
</tr>
<tr>
<td><p> <code>map</code> </p></td>
<td><p> Returns an <code>IEnumerable</code> <code>object</code> that applies a specific function to each element in another <code>IEnumerable</code> <code>object</code>. </p></td>
<td><p> <code>map([1, 2, 3, 4, 5], func)</code> </p></td>
</tr>
<tr>
<td><p> <code>array</code> </p></td>
<td><p> Used to create an empty <code>array</code> or convert <code>IEnumerable</code> and <code>ICollection objects</code> to an <code>array</code> </p></td>
<td><p> <code>array(int, [1, 2, 3, 4, 5]) == (1, 2, 3, 4, 5)</code> </p></td>
</tr>
<tr>
<td><p> <code>matrix</code> </p></td>
<td><p> Creates a multidimensional <code>array</code>. See 
<a href="https://github.com/bamboo/boo/wiki/Multidimensional-Arrays">
Multidimensional Arrays for more info. </p></td>
<td><p> <code>matrix(int, 2, 2)</code> </p></td>
</tr>
<tr>
<td><p> <code>iterator</code> </p></td>
<td><p> Creates an <code>IEnumerable</code> from an <code>object</code> </p></td>
<td><p> <code>List(iterator('abcde')) == ['a', 'b', 'c', 'd', 'e']</code> </p></td>
</tr>
<tr>
<td><p> <code>shellp</code> </p></td>
<td><p> Start a <code>Process</code>. Returns a <code>Process</code> <code>object</code> </p></td>
<td><p> <code>process = shellp("MyProgram.exe", "")</code> </p></td>
</tr>
<tr>
<td><p> <code>shell</code> </p></td>
<td><p> Invoke an application. Returns a <code>string</code> containing the program's output to Standard Out </p></td>
<td><p> <code>input = shell("echo hi there", "")</code> </p></td>
</tr>
<tr>
<td><p> <code>shellm</code> </p></td>
<td><p> Execute the specified managed application in a new <code>AppDomain</code>. Returns a <code>string</code> containing the program's output to Standard Out </p></td>
<td><p> <code>input = shellm("MyProgram.exe", (,))</code> </p></td>
</tr>
<tr>
<td><p> <code>enumerate</code> </p></td>
<td><p> Creates an <code>IEnumerator</code> from another, but gives it a pairing of <code>(</code><em>index</em><code>,</code> <em>value</em><code>)</code> </p></td>
<td><p> <code>List(enumerate(range(5, 8))) == [(0, 5), (1, 6), (2, 7)]</code> </p></td>
</tr>
<tr>
<td><p> <code>range</code> </p></td>
<td><p> Returns an <code>IEnumerable</code> containing a list of <code>ints</code> </p></td>
<td><p> <code>List(range(5)) == [0, 1, 2, 3, 4]</code> </p></td>
</tr>
<tr>
<td><p> <code>reversed</code> </p></td>
<td><p> Returns an <code>IEnumerable</code> with its members in reverse order </p></td>
<td><p> <code>List(reversed(range(5))) == [4, 3, 2, 1, 0]</code> </p></td>
</tr>
<tr>
<td><p> <code>zip</code> </p></td>
<td><p> Returns an <code>IEnumerable</code> that is a "mesh" of two or more <code>IEnumerables</code>. </p></td>
<td><p> <code>array(zip([1, 2, 3], [4, 5, 6])) == [(1, 4), (2, 5), (3, 6)]</code> </p></td>
</tr>
<tr>
<td><p> <code>cat</code> </p></td>
<td><p> Concatenate two or more <code>IEnumerators</code> head-to-tail </p></td>
<td><p> <code>List(cat(range(3), range(3, 6))) == [0, 1, 2, 3, 4, 5]</code> </p></td>
</tr>
</tbody></table>

These are all very handy to know. Not required, but it makes programming all that much easier.


## Defining Your Own Functions

It's very simple to define your own functions as well.

```boo
def Hello():
    return "Hello, World!"

print Hello()

// Output: Hello, World!
```

Now it's ok if you don't understand any of that, I'll go through it step-by-step.

1. `def Hello():`
  1. `def` declares that you are starting to declare a function. `def` is short for "define".
  2. `Hello` is the name of the function. You could call it almost anything you wanted, as long as it doesn't have any spaces and doesn't start with a number.
  3. `()` this means what kind of arguments the function will take. Since we don't accept any arguments, it is left blank.
2. `return "Hello, World!"`
  1. `return` is a keyword that lets the function know what to emit to its invoker.
  2. `"Hello, World!"` is the string that the `return` statement will send.
3. `print Hello()`
  1. `print` is the happy little `print` macro that we covered before.
  2. `Hello()` calls the `Hello` function with no `()` arguments.

Like variables, function types are inferred.

```boo
def Hello():
    return "Hello, World!"
```

will always return a string, so Boo will infer that string is its return type. You could have done this to achieve the same result:

```boo
def Hello() as string:
    return "Hello, World!"
```

!!! hint
    If it is not obvious, specify the return type for a function.

If Boo cannot infer a return type, it will assume `object`. If there is no return type then the return type is called 'void', which basically means nothing. To have no return type you can leave off the `return`, or have a `return` with no expression. If there are multiple return}}s with different {{return types, it will return the closest common ancestor, often times `object` but not always.


## Arguments

!!! note
    **Argument** - A way of allowing the same sequence of commands to operate on different data without re-specifying the instructions.

Arguments are very handy, as they can allow a function to do different things based on the input.

```boo
def Hello(name as string):
    return "Hello, ${name}!"

print Hello("Monkey")

// Output: Hello, Monkey!
```

Here it is again, step-by-step.

1. `def Hello(name as string):`
  1. `def` declares that you are starting to declare a function.
  2. `Hello` is the name of the function. You could call it almost anything you wanted, as long as it doesn't have any spaces and doesn't start with a number.
  3. `(name as string)` this means what kind of arguments the function will take. This function will take one argument: `name`. When you call the function, the `name` must be a string, otherwise you will get a compiler error - "The best overload for the method Hello is not compatible with the argument list '(The,Types, of, The, Parameters, Entered)'."
2. `return "Hello, ${name}!"`
  1. `return` is a keyword that exits the function, and optionally return a value to the caller.
  2. `"Hello, ${name}!"` uses [String Interpolation](https://github.com/bamboo/boo/wiki/String-Interpolation) to place the value of name directly into the string.
3. `print Hello("Monkey")`
  1. `print` is the happy little print macro that we covered [before](https://github.com/bamboo/boo/wiki/Boo-Primer:-%5BPart-01%5D-Starting-Out).
  2. `Hello("Monkey")` calls the Hello function with the ("Monkey") argument.


## Function Overloading

!!! note
    **Overloading** - Giving multiple meanings to the same name, but making them distinguishable by context. For example, two procedures with the same name are overloading that name as long as the compiler can determine which one you mean from contextual information such as the type and number of parameters that you supply when you call it.

Function overloading takes place when a function is declared multiple times with different arguments.

```boo
def Hello():
    return "Hello, World!"

def Hello(name as string):
    return "Hello, ${name}!"

def Hello(num as int):
    return "Hello, Number ${num}!"

def Hello(name as string, other as string):
    return "Hello, ${name} and ${other}!"

print Hello()
print Hello("Monkey")
print Hello(2)
print Hello("Cat", "Dog")

// Output: 
// Hello, World!
// Hello, Monkey!
// Hello, Number 2!
// Hello, Cat and Dog!
```


## Variable Arguments

There is a way to pass an arbitrary number of arguments.

```boo
def Test(*args as (object)):
    return args.Length

print Test("hey", "there")
print Test(1, 2, 3, 4, 5)
print Test("test")

a = (5, 8, 1)
print Test(*a)

// Output: 
// 2
// 5
// 1
// 3
```

The star * lets it known that everything past that is arbitrary.

It is also used to explode parameters, as in `print Test(*a)` causes 3 arguments to be passed.

You can have required parameters before the *args, just like in any other function, but not after, as after all the required parameters are supplied the rest are past into your argument array.


## Exercises

1. Write a function that prints something nice if it is fed an even number and prints something mean if it is fed an odd number.

