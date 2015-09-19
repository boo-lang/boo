Boo has operator overloading. Overloaded operators must be defined as **static**. For example:

```boo
struct myNum:
    i as double
    def constructor(j as int):
        i = j
    static def op_Multiply(x as myNum, j as int):
        x.i = x.i * j
        return x
    static def op_Multiply(x as myNum, y as myNum):
        x.i = x.i * y.i
        return x
    static def op_Addition(x as myNum, j as int):
        x.i = x.i + j
        return x
    static def op_Equality(x as myNum, y as double):
        return x.i == y
    static def op_UnaryNegation(x as myNum):
        x.i = -x.i
        return x
    def ToString():
        return i.ToString()

x = myNum(5)
y = -x*x*2 + 1
assert y == -49
```

These binary operators can be overloaded:

```boo
op_Addition
op_Subtraction
op_Multiply
op_Division
op_Modulus
op_Exponentiation
op_Equality
op_LessThan
op_LessThanOrEqual
op_GreaterThan
op_GreaterThanOrEqual
op_Match
op_NotMatch
op_Member
op_NotMember
op_BitwiseOr
op_BitwiseAnd
```

When you overload a binary arithmetic operator such as op_Addition, the corresponding assignment operator ( += ) is overloaded too.

This unary operator can be overloaded:

```boo
op_UnaryNegation
```
