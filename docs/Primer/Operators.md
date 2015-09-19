## Mathematical

<table><tbody>
<tr>
<th><p> Name </p></th>
<th><p> Syntax example </p></th>
<th><p> Comments </p></th>
</tr>
<tr>
<td><p> Multiplication </p></td>
<td><p> <code>a * b</code> </p></td>
</tr>
<tr>
<td><p> Division </p></td>
<td><p> <code>a / b</code> </p></td>
</tr>
<tr>
<td><p> Remainder </p></td>
<td><p> <code>a % b</code> </p></td>
<td><p> Often called mod or modulus </p></td>
</tr>
<tr>
<td><p> Addition </p></td>
<td><p> <code>a + b</code> </p></td>
</tr>
<tr>
<td><p> Subtraction </p></td>
<td><p> <code>a - b</code> </p></td>
</tr>
<tr>
<td><p> Exponent </p></td>
<td><p> <code>a ** b</code> </p></td>
<td><p> Do not confuse this with Bitwise Xor ^ </p></td>
</tr>
<tr>
<td><p> Bitshift Right </p></td>
<td><p> <code>a &gt;&gt; b</code> </p></td>
</tr>
<tr>
<td><p> Bitshift Left </p></td>
<td><p> <code>a &lt;&lt; b</code> </p></td>
</tr>
<tr>
<td><p> Bitwise And </p></td>
<td><p> <code>a &amp; b</code> </p></td>
</tr>
<tr>
<td><p> Bitwise Or </p></td>
<td><p> <code>a | b</code> </p></td>
</tr>
<tr>
<td><p> Bitwise Xor </p></td>
<td><p> <code>a ^ b</code> </p></td>
</tr>
</tbody></table>

The mathematical operators can also be used in the syntax `a <operator>= b`, for example, `a += b`.

This is merely a shortcut for `a = a <operator> b`, or in our example `a = a + b`.


## Relational and Logical

<table><tbody>
<tr>
<th><p> Name </p></th>
<th><p> Syntax Example </p></th>
<th><p> Comments </p></th>
</tr>
<tr>
<td><p> Less Than </p></td>
<td><p> <code>a &lt; b</code> </p></td>
</tr>
<tr>
<td><p> Greater Than </p></td>
<td><p> <code>a &gt; b</code> </p></td>
</tr>
<tr>
<td><p> Less Than or Equal To </p></td>
<td><p> <code>a &lt;= b</code> </p></td>
</tr>
<tr>
<td><p> Greater Than or Equal To </p></td>
<td><p> <code>a &gt;= b</code> </p></td>
</tr>
<tr>
<td><p> Equality </p></td>
<td><p> <code>a == b</code> </p></td>
</tr>
<tr>
<td><p> Inequality </p></td>
<td><p> <code>a != b</code> </p></td>
</tr>
<tr>
<td><p> Logical And </p></td>
<td><p> <code>a and b</code> </p></td>
<td><p> Only use when <code>a</code> and <code>b</code> are boolean values </p></td>
</tr>
<tr>
<td><p> Logical Or </p></td>
<td><p> <code>a or b</code> </p></td>
<td><p> Only use when <code>a</code> and <code>b</code> are boolean values </p></td>
</tr>
<tr>
<td><p> Logical Not </p></td>
<td><p> <code>not a</code> </p></td>
<td><p> Only use when <code>a</code> is a boolean value </p></td>
</tr>
</tbody></table>


## Types

<table><tbody>
<tr>
<th><p> Name </p></th>
<th><p> Syntax Example </p></th>
<th><p> Comments </p></th>
</tr>
<tr>
<td><p> Typecast </p></td>
<td><p> <code>a cast string</code><br class="atl-forced-newline"></br> </p></td>
</tr>
<tr>
<td><p> Typecast </p></td>
<td><p> <code>a as string</code> </p></td>
</tr>
<tr>
<td><p> Type Equality/Compatibility </p></td>
<td><p> <code>a isa string</code> </p></td>
</tr>
<tr>
<td><p> Type Retrieval </p></td>
<td><p> <code>typeof(string)</code> </p></td>
</tr>
<tr>
<td><p> Type Retrieval </p></td>
<td><p> <code>a.GetType()</code> </p></td>
</tr>
</tbody></table>


## Primary

<table><tbody>
<tr>
<th><p> Name </p></th>
<th><p> Syntax Example </p></th>
<th><p> Comments </p></th>
</tr>
<tr>
<td><p> Member </p></td>
<td><p> <code>A.B</code> </p></td>
<td><p> Classes are described in 
<a href="./Boo-Primer:-%5BPart-08%5D-Classes">Part 08 - Classes 
 <br class="atl-forced-newline"></br> </p></td>
</tr>
<tr>
<td><p> Function Call </p></td>
<td><p> <code>f(x)</code> </p></td>
<td><p> Functions are described in 
<a href="./Boo-Primer:-%5BPart-07%5D-Functions">Part 07 - Functions
<br class="atl-forced-newline"></br> </p></td>
</tr>
<tr>
<td><p> Post Increment </p></td>
<td><p> <code>i++</code> </p></td>
<td><p> See 
<a href="./Boo-Primer:-%5BPart-06%5D-Operators">Difference between Pre and Post Increment/Decrement
<br class="atl-forced-newline"></br> </p></td>
</tr>
<tr>
<td><p> Post Decrement </p></td>
<td><p> <code>i--</code> </p></td>
<td><p> See <a href="./Boo-Primer:-%5BPart-06%5D-Operators">Difference between Pre and Post Increment/Decrement<br class="atl-forced-newline"></br> </p></td>
</tr>
<tr>
<td><p> Constructor Call </p></td>
<td><p> <code>o = MyClass()</code> </p></td>
<td><p> Classes are described in <a href="./Boo-Primer:-%5BPart-08%5D-Classes">Part 08 - Classes <br class="atl-forced-newline"></br> </p></td>
</tr>
</tbody></table>


## Unary

<table><tbody>
<tr>
<th><p> Name </p></th>
<th><p> Syntax Example </p></th>
<th><p> Comments </p></th>
</tr>
<tr>
<td><p> Negative Value </p></td>
<td><p> <code>-5</code> </p></td>
</tr>
<tr>
<td><p> Pre Increment </p></td>
<td><p> <code>++i</code> </p></td>
<td><p> See [Difference between Pre and Post Increment/Decrement](https://github.com/bamboo/boo/wiki/Boo-Primer:-%5BPart-06%5D-Operators)<br class="atl-forced-newline"></br> </p></td>
</tr>
<tr>
<td><p> Pre Decrement </p></td>
<td><p> <code>--i</code> </p></td>
<td><p> See [Difference between Pre and Post Increment/Decrement](https://github.com/bamboo/boo/wiki/Boo-Primer:-%5BPart-06%5D-Operators)<br class="atl-forced-newline"></br> </p></td>
</tr>
<tr>
<td><p> Grouping </p></td>
<td><p> <code>(a + b)</code> </p></td>
</tr>
</tbody></table>


## Difference between Pre and Post Increment/Decrement

When writing inline code, Pre Increment/Decrement (+i/-i) commit the action, then return its new value, whereas Post Increment/Decrement (i+/i-) return the current value, then commit the change.


### preincrement vs. postincrement

```boo
num = 0
for i in range(5):
    print num++
print '---'
num = 0
for i in range(5):
    print ++num

// Output:
// 0
// 1
// 2
// 3
// 4
// ---
// 1
// 2
// 3
// 4
// 5
```

!!! hint
    To make your code more readable, avoid using the incrementors and decrementors. Instead, use i += 1 and i -= 1.


## Exercises

1. Put your hands on a wall, move your left foot back about 3 feet, move the right foot back 2 feet.

