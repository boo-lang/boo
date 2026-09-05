"""
7
7
8
"""
import System

buf = (7, 8)
span = buf.AsSpan()
Console.WriteLine(span.GetPinnableReference())
Console.WriteLine(span.GetPinnableReference().ToString())
Console.WriteLine(span.GetPinnableReference() + 1)
