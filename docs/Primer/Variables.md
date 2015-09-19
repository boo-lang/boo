## Using Booish as a Calculator

There are four basic mathematical operators: addition `+`, subtraction `-`, multiplication `*`, and 
division `/`. There are more than just these, but that's what will be covered now.

```boo
    $ booish
    >>> 2 + 4 // This is a comment
    6
    >>> 2 * 4 # So is this also a comment
    8
    >>> 4 / 2 /* This is also a comment */
    2
    >>> (500/10)*2
    100
    >>> 7 / 3 // Integer division
    2
    >>> 7 % 3 // Take the remainder
    1
    >>> -7 / 3
    -2
```

You may have noticed that there are 3 types of comments available, `//`, `#`, and `/* */`. These do
not cause any affect whatsoever, but help you when writing your code.

!!! hint
    When doing single-line comments, use `//` instead of `#`

You may have noticed that 7 / 3 did not give 2.333..., this is because you were dividing two integers together.

!!! note
    **Integer** - Any positive or negative number that does not include a fraction or decimal,
    including zero.

The way computers handle integer division is by rounding to the floor afterwards.

In order to have decimal places, you must use a floating-point number.

!!! note
    **Floating point Number** - Often referred to in mathematical terms as a "rational" number,
    this is just a number that can have a fractional part.

```boo
    $ booish
    >>> 7.0 / 3.0 // Floating point division
    2.33333333333333
    >>> -8.0 / 5.0
    -1.6
```

If you give a number with a decimal place, even if it's .0, it become a floating-point number.


## Types of Numbers

There are 3 kinds of floating point numbers, `single`, `double`, and `decimal`.

The differences between `single` and `double` is the size they take up. `double` is prefered in most situations.

These two also are based on the number 2, which can cause some problems when working with our base-10 number system.

Ususally this is not the case, but in delecate situations like banking, it would not be wise to lose a cent or two
on a multi-trillion dollar contract.

Thus `decimal` was created. It is a base-10 number, which means that we wouldn't lose that precious penny.

In normal situations, `double` is perfectly fine. For a higher precision, a `decimal` should be used.
Integers, which we covered earlier, have many more types to them.

They also have the possibility to be "unsigned", which means that they must be non-negative.

The size goes in order as such: `byte`, `short`, `int`, and `long`.

In most cases, you will be using `int`, which is the default.


## Characters and Strings

!!! note
    **Character** - A written symbol that is used to represent speech.

All the letters in the alphabet are characters. All the numbers are characters. All the symbols
of the Mandarin language are characters. All the mathematical symbols are characters.

In Boo/.NET, characters are internally encoded as UTF-16, or [Unicode](http://www.unicode.org/).

!!! note
    **String** - A linear sequence of characters.

The word "apple" can be represented by a `string`.

```boo
    $ booish
    >>> s = "apple"
    'apple'
    >>> print s
    apple
    >>> s += " banana"
    'apple banana'
    >>> print s
    apple banana
    >>> c = char('C')
    C
    >>> print c
    C
```

Now you probably won't be using `chars` much, it is more likely you will be using `strings`.

To declare a `string`, you have one of three ways.

1. using double quotes. "apple"
2. using single quotes. 'apple'
3. using tripled double quotes. """apple"""

The first two can span only one line, but the tribbled double quotes can span multiple lines.

The first two also can have backslash-escaping. The third takes everything literally.

```boo
    $ booish
    >>> print "apple\nbanana"
    apple
    banana
    >>> print 'good\ttimes'
    good      times
    >>> print """Leeroy\Jenkins"""
    Leeroy\Jenkins
```

Common escapes are:

* {{ ... }} literal backslash
* \n newline
* \r carriage return
* \t tab

If you are declaring a double-quoted `string`, and you want a double quote inside it, also use a backslash.

Same goes for the single-quoted `string`.

```boo
    $ booish
    >>> print "The man said \"Hello\""
    The man said "Hello"
    >>> print 'I\'m happy'
    I'm happy
```

`strings` are immutable, which means that the characters inside them can never change. You would have to
recreate the `string` to change a `character`.

!!! note
    **Immutable** - Not capable of being modified after it is created. It is an error to
    attempt to modify an immutable object. The opposite of immutable is mutable.


## String Interpolation

String interpolation allows you to insert the value of almost any valid boo expression inside a `string`
by preceeding a lonesome variable name with $, or quoting an expression with $().

```boo
    $ booish
    >>> name = "Santa Claus"
    Santa Claus
    >>> print "Hello, $name!"
    Hello, Santa Claus!
    >>> print "2 + 2 = $(2 + 2)"
    2 + 2 = 4
```

!!! hint
    String Interpolation is the preferred way of adding `strings` together. It is preferred over
    simple `string` addition.

String Interpolation can function in double-quoted `strings`, including tripled double-quoted `string`.

It does not work in single-quoted `strings`.

To stop String Interpolation from happening, just escape the dollar sign: \${} or alternatively use
backtick quoted strings:

```boo
print `not $interpolated!`
```


## Booleans

!!! note
    **Boolean** - A value of `true` or `false` represented internally in binary notation.

Boolean values can only be `true` or `false`, which is very handy for conditional statements, covered in
the next section.

```boo
    $ booish
    >>> b = true
    true
    >>> print b
    True
    >>> b = false
    false
    >>> print b
    False
```


## Object Type

!!! note
    **Object** - The central concept in the object-oriented programming paradigm.

Everything in Boo/.NET is an object.

Although some are value types, like numbers and characters, these are still objects.

```boo
    $ booish
    >>> o as object
    >>> o = 'string'
    'string'
    >>> print o
    string
    >>> o = 42
    42
    >>> print o
    42
```

The problem with `objects` is that you can't implicitly expect a `string` or an `int`.

If I were to do that same sequence without declaring `o as object`,

```boo
    $ booish
    >>> o = 'string'
    'string'
    >>> print o
    string
    >>> o = 42
    --------^
    ERROR: Cannot convert 'System.Int32' to 'System.String'.
```

This static typing keeps the code safe and reliable.


## Declaring a Type

In the last section, you issued the statement `o as object`.

This can work with any type and goes with the syntax `<variable> as <type>`.

`<type>` can be anything from an `int` to a `string` to a `date` to a `bool` to something which
you defined yourself, but those will be discussed later.
In most cases, Boo will be smart and implicitly figure out what you wanted.

The code `i = 25` is the same thing as `i as int = 25`, just easier on your wrists.

!!! hint
    Unless you are declaring a variable beforehand, or declaring it of a different type,
    don't explicitly state what kind of variable it is. (ie: use `i = 25` instead of
    `i as int = 25`)


## List of Value Types

<table><tbody>
<tr>
<th><p> Boo type </p></th>
<th><p> .NET Framework type </p></th>
<th><p> Signed? </p></th>
<th><p> Size in bytes </p></th>
<th><p> Possible Values </p></th>
</tr>
<tr>
<td><p> sbyte </p></td>
<td><p> System.Sbyte </p></td>
<td><p> Yes </p></td>
<td><p> 1 </p></td>
<td><p> -128 to 127 </p></td>
</tr>
<tr>
<td><p> short </p></td>
<td><p> System.Int16 </p></td>
<td><p> Yes </p></td>
<td><p> 2 </p></td>
<td><p> -32768 - 32767 </p></td>
</tr>
<tr>
<td><p> int </p></td>
<td><p> System.Int32 </p></td>
<td><p> Yes </p></td>
<td><p> 4 </p></td>
<td><p> -2147483648 - 2147483647 </p></td>
</tr>
<tr>
<td><p> long </p></td>
<td><p> System.Int64 </p></td>
<td><p> Yes </p></td>
<td><p> 8 </p></td>
<td><p> -9223372036854775808 - 9223372036854775807 </p></td>
</tr>
<tr>
<td><p> byte </p></td>
<td><p> System.Byte </p></td>
<td><p> No </p></td>
<td><p> 1 </p></td>
<td><p> 0 - 255 </p></td>
</tr>
<tr>
<td><p> ushort </p></td>
<td><p> System.Uint16 </p></td>
<td><p> No </p></td>
<td><p> 2 </p></td>
<td><p> 0 - 65535 </p></td>
</tr>
<tr>
<td><p> uint </p></td>
<td><p> System.UInt32 </p></td>
<td><p> No </p></td>
<td><p> 4 </p></td>
<td><p> 0 - 4294967295 </p></td>
</tr>
<tr>
<td><p> ulong </p></td>
<td><p> System.Uint64 </p></td>
<td><p> No </p></td>
<td><p> 8 </p></td>
<td><p> 0 - 18446744073709551615 </p></td>
</tr>
<tr>
<td><p> single </p></td>
<td><p> System.Single </p></td>
<td><p> Yes </p></td>
<td><p> 4 </p></td>
<td><p> Approximately &#177;1.5 x 10-45 - &#177;3.4 x 1038 with 7 significant figures </p></td>
</tr>
<tr>
<td><p> double </p></td>
<td><p> System.Double </p></td>
<td><p> Yes </p></td>
<td><p> 8 </p></td>
<td><p> Approximately &#177;5.0 x 10-324 - &#177;1.7 x 10308 with 15 or 16 significant figures </p></td>
</tr>
<tr>
<td><p> decimal </p></td>
<td><p> System.Decimal </p></td>
<td><p> Yes </p></td>
<td><p> 12 </p></td>
<td><p> Approximately &#177;1.0 x 10-28 - &#177;7.9 x 1028 with 28 or 29 significant figures </p></td>
</tr>
<tr>
<td><p> char </p></td>
<td><p> System.Char </p></td>
<td><p> N/A </p></td>
<td><p> 2 </p></td>
<td><p> Any UTF-16 character </p></td>
</tr>
<tr>
<td><p> bool </p></td>
<td><p> System.Boolean </p></td>
<td><p> N/A </p></td>
<td><p> 1 </p></td>
<td><p> true or false </p></td>
</tr>
</tbody></table>

!!! hint
    Never call a type by its .NET Framework type, use the builtin boo types.

## Exercises

1. Declare some variables. Go wild.

