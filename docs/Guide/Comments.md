### Single Line Comments

Everything that comes after a hash character (`#`) or a double slash (`//`) is ignored until the end of the line.

```boo
# this is a comment
// This is also a comment
print("Hello, world!") // A comment can start anywhere in the line
```

### Multiline Comments

Multiline comments in boo are delimited by the /* and */ sequences. Just like in C. Unlike in C though, boo multiline comments are nestable.

```boo
/* this is a comment */
/* this
comment
spans mutiple
line */
 
/* this is a /*
                 nested comment */
    comment */
```

!!! warning
    Nested comments must be properly nested, however, or the parser will complain.

```boo
/* this is a /* syntax error */
```
