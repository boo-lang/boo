"""
System.EventHandler
System.Collections.IEnumerable
System.EventHandler`1[System.EventArgs]
System.Collections.Generic.IEnumerable`1[System.String]
System.Collections.Generic.Dictionary`2[System.Int32,System.String]
System.Collections.Generic.IEnumerable`1[T]
System.Collections.Generic.List`1[T]
System.Collections.Generic.Dictionary`2[TKey,TValue]
"""

import System.Collections
import System.Collections.Generic

# non-generics
t1 = typeof(System.EventHandler)
t2 = typeof(IEnumerable)

# constructed generics
t3 = typeof(System.EventHandler of System.EventArgs)
t4 = typeof(IEnumerable of string)
t5 = typeof(Dictionary[of int, string])

# generic definitions
t6 = typeof(IEnumerable of *)
t7 = typeof(List of *)
t8 = typeof(Dictionary[of *, *])

print t1
print t2
print t3
print t4
print t5
print t6
print t7
print t8
