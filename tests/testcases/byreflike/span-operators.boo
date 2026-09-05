"""
False
True
True
-2
-3
"""
import System

buf = (1, 2)
span = buf.AsSpan()
Console.WriteLine(span[0] > 5)
Console.WriteLine(span[0] == 1)
Console.WriteLine(span[1] > span[0])
Console.WriteLine(-span[1])
Console.WriteLine(~span[1])
