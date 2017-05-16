"""
1
"""

import System

def Foo[of T](method as Func[of T]) as T:
    return method()

def Foo[of T](method as Func[of List[of T]]) as T:
    return method()[0]

bar = {return List[of int]((1, 2))}
x as int = Foo(bar)
print x