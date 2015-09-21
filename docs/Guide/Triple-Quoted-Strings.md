Triple quoted strings in Boo are used to preserve spacing and linebreaks without a thick slathering of \n's all over your precious code.

It is good for visually representing XML, for instance:

```boo
xml = """
<xml>
     <randomElement>
     </randomElement>
</xml>
"""
```

It is also most excellent for formatting console output:

```boo
name = prompt("Name: ")
age = prompt("Age: ")
sex = prompt("Sex: ")
 
print """
 
Hello, ${name}!
 
My name is Hal, and I will be your Operating System today.
 
My sensors indicate that you are a ${age} year old ${sex}
 
If you need any assistance navigating this user interface, please ask.
 
...
 
Humans are so fragile...
 
"""
```

