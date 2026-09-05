"""
H
H
"""
import System

span = "Hi".AsSpan()
e = span.GetEnumerator()
e.MoveNext()
Console.WriteLine(e.Current)
Console.WriteLine(e.Current.ToString())
