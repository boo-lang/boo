"""
2 3 4
2 3 4
"""
import System.Collections

def Fetch(n as int, seq):
    i = 0
    for val in seq:
        yield val
        break if ++i == n

class Test:
    static def Arr(n as int, list):
        return array(double, Fetch(n, list))

def Arr(n as int, list):
    return array(double, Fetch(n, list))

a,b,c = Test.Arr(3, [1, 2, 3, 4, 5, 6])
print a+1, b+1, c+1   # will break if a etc aren't numbers!
d,e,f = Arr(3, [1, 2, 3, 4, 5, 6])
print d+1, e+1, f+1

