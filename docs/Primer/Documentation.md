!!! note
    A communicable material used to explain some attributes of an object, system or procedure.

I've saved the most important for last, as documentation is itself, just as important as the code which it describes.

When documenting your code, be sure to remember:

1. All your documents should be in English.
2. Use full sentences.
3. Avoid spelling/grammar mistakes.
4. Use present tense.

Documentation is placed in tripled double-quoted strings right below what you are documenting.

### Documentation with Summary

```boo
def Hello():
"""Says "hello" to the world."""
    print "Hello, World!"

Hello()
```

That "docstring" is the least you can do to document your code. It gave a simple summary.

If your docstring spans more than one line, then the quotes should go on their own lines.

You may have noticed that 'Says "hello" to the world.' is not a full sentence. For the first sentence in a summary, you can imply "This member".

Parameters should also be documented.

### Parameters

```boo
def Hello(name as string):
"""
Say "hello" to the given name.
Param name: The name to say hello to.
"""
    print "Hello, $name!"

Hello("Harry")
```

To read it to yourself, it goes as such: 'Say "hello" to the given name. Parameter name is defined as the name to say hello to.'

This keeps in line with using full sentences. If describing the parameter takes more than one line, you should move it all to a new line and indent.

### Long Parameter

```boo
def Hello(name as string):
"""
Say "hello" to the given name.
Param name:
    The name to say hello to.
    It might do other things as well.
"""
    print "Hello, $name!"
```

The same goes with any block.

Here is a list of all the tags that can be used:

<table><tbody>
<tr>
<th><p> Tag </p></th>
<th><p> Description </p></th>
</tr>
<tr>
<td><p> No tag </p></td>
<td><p> A summary of the member. </p></td>
</tr>
<tr>
<td><p> <code>Param</code> <code><em>&lt;name&gt;</em></code><code>:</code> <code><em>&lt;description&gt;</em></code> </p></td>
<td><p> This specifies the parameter <code><em>&lt;name&gt;</em></code> of the method. </p></td>
</tr>
<tr>
<td><p> <code>Returns:</code> <code><em>&lt;description&gt;</em></code> </p></td>
<td><p> This describes what the method returns. </p></td>
</tr>
<tr>
<td><p> <code>Remarks:</code> <code><em>&lt;text&gt;</em></code> </p></td>
<td><p> This provides descriptive text about the member. </p></td>
</tr>
<tr>
<td><p> <code>Raises</code> <code><em>&lt;exception&gt;</em></code><code>:</code> <code><em>&lt;description&gt;</em></code> </p></td>
<td><p> Gives a reason why an <code>Exception</code> is raised. </p></td>
</tr>
<tr>
<td><p> <code>Example:</code> <code><em>&lt;short_description&gt;</em></code><code>:</code> <code><em>&lt;code_block&gt;</em></code> </p></td>
<td><p> Provides an example. </p></td>
</tr>
<tr>
<td><p> <code>Include</code> <code><em>&lt;filename&gt;</em></code><code>:</code> <code><em>&lt;tagpath&gt;</em></code><code>[@</code><code><em>&lt;name&gt;</em></code><code>="</code><code><em>&lt;id&gt;</em></code><code>"]</code> </p></td>
<td><p> Includes an excerpt from another file. </p></td>
</tr>
<tr>
<td><p> <code>Permission</code> <code><em>&lt;permission&gt;</em></code><code>:</code> <code><em>&lt;description&gt;</em></code> </p></td>
<td><p> Describe a required Permission. </p></td>
</tr>
<tr>
<td><p> <code>See Also:</code> <code><em>&lt;reference&gt;</em></code> </p></td>
<td><p> Lets you specify the reference that you might want to appear in a See Also section. </p></td>
</tr>
</tbody></table>

And a list of inline tags:

<table><tbody>
<tr>
<th><p> Tag </p></th>
<th><p> Description </p></th>
</tr>
<tr>
<td><p> <code>*</code> <code><em>&lt;item&gt;</em></code> <br class="atl-forced-newline"></br>
<code>*</code> <code><em>&lt;item&gt;</em></code> <br class="atl-forced-newline"></br>
<code>*</code> <code><em>&lt;item&gt;</em></code> </p></td>
<td><p> Bullet list </p></td>
</tr>
<tr>
<td><p> <code>#</code> <code><em>&lt;item&gt;</em></code> <br class="atl-forced-newline"></br>
<code>#</code> <code><em>&lt;item&gt;</em></code> <br class="atl-forced-newline"></br>
<code>#</code> <code><em>&lt;item&gt;</em></code> </p></td>
<td><p> Numbered List </p></td>
</tr>
<tr>
<td><p> <code>&lt;</code><code><em>&lt;reference&gt;</em></code><code>&gt;</code> </p></td>
<td><p> Provides an inline link to a reference. e.g. &lt;int&gt; or &lt;string&gt; would link. </p></td>
</tr>
<tr>
<td><p> <code>[</code><code><em>&lt;param_reference&gt;</em></code><code>]</code> </p></td>
<td><p> References to a parameter of the method. </p></td>
</tr>
</tbody></table>

Here's some examples of proper documentation:

### Documentation example

```boo
import System

class MyClass:
"""Performs specific duties."""
    def constructor():
    """Initializes an instance of <MyClass>"""
        _rand = Random()

    def Commit():
    """Commits an action."""
        pass

    def CalculateDouble(i as int) as int:
    """
    Returns double the value of [i].
    Parameter i: An <int> to be doubled.
    Returns: Double the value of [i].
    """
        return i * 2

    def CauseError():
    """
    Causes an error.
    Remarks: This method has not been implemented.
    Raises NotImplementedException: This has not been implemented yet.
    """
        return NotImplementedException("CauseError() is not implemented")

    def DoSomething() as int:
    """
    Returns a number.
    Example: Here is a short example:
        print DoSomething()
    Returns: An <int>.
    See Also: MakeChaos()
    """
        return 0

    def MakeChaos():
    """
    Creates Chaos.
    Include file.xml: Foo/Bar[@id="entropy"]
    Permission Security.PermissionSet: Everyone can access this method.
    """
        print "I am making chaos: $(_rand.Next(100))"

    def Execute():
    """
    Executes the protocol.
    Does one of two things,
    # Gets a sunbath.
    # Doesn't get a sunbath.
    """
        if _rand.Next(2) == 0:
            print "I sunbathe."
        else:
            print "I decide not to sunbathe."

    def Calculate():
    """
    Does these things, in no particular order,
    * Says "Hello"
    * Looks at you
    * Says "Goodbye"
    """
        thingsToDo = ["I look at you.", 'I say "Hello."', 'I say "Goodbye."']
        while len(thingsToDo) > 0:
            num = _rand.Next(len(thingsToDo))
            print thingsToDo[num]
            thingsToDo.RemoveAt(num)

    [Property(Name)]
    _name as string
    """A name""" // documents the property, not the field

    Age as int:
    """An age"""
        get:
            return _rand.Next(8) + 18

    _age as int

    _rand as Random
```

This should give you a good view on how to document your code.
I think Dick Brandon said it best:

!!! quote
    Documentation is like sex: when it is good, it is very, very good; and when it is bad, it is better than nothing.

