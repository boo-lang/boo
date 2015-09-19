### Print

print
```boo
System.Console.WriteLine()
```

print <expr>
```boo
System.Console.WriteLine(<expr>)
```

print <expr>, [... ,] <exprN>
```boo
System.Console.Write(<expr>)
System.Console.Write(' ')
 ...
System.Console.WriteLine(<exprN>)
```


### assert

assert <expr>
```boo
unless (<expr>):
    raise Boo.AssertionFailedException('(<expr>)')
```

assert <expr>, <string>
```boo
unless (<expr>):
    raise Boo.AssertionFailedException(<string>)
```


### using

using:
    <block>
```boo
try:
    <block>
ensure:
    pass
```

using <object> [, ...]:
    <block>
```boo
try:
    <block>
ensure:
    if (__disposable__ = (<object> as System.IDisposable)):
        __disposable__.Dispose()
        __disposable__ = null
    ...
```

using <object> = <expr> [, ...]:
    <block>
```boo
try:
    __using1__ = <expr>
    ...
    <block>
ensure:
    if (__disposable__ = (__using1__ as System.IDisposable)):
        __disposable__.Dispose()
        __disposable__ = null
    ...
```


### lock

lock <expr> [, ...]:
    <block>
```boo
_monitor1__ = <expr>
System.Threading.Monitor.Enter(__monitor1__)
try:
    <block>
ensure:
    System.Threading.Monitor.Exit(__monitor1__)
```


### debug

debug
```boo
System.Diagnostics.Debug.WriteLine('<debug>')
```

debug <expr>
```boo
System.Diagnostics.Debug.WriteLine(<expr>)
```

debug <expr>, [... ,] <exprN>
```boo
System.Diagnostics.Debug.Write(<expr>)
System.Diagnostics.Debug.Write(' ')
 ...
System.Diagnostics.Debug.WriteLine(<exprN>)
```

