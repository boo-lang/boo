"""
v=2
2
2
2
"""
import System

buf = (1, 2)
span = buf.AsSpan()
Console.WriteLine("v=${span[1]}")
Console.WriteLine(span[1].ToString())
Console.WriteLine(cast(long, span[1]))
Console.WriteLine("{0}", span[1])
