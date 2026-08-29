"""
6
[foo, bar, baz, 0, 1, 2]
[foo, bar, baz, 0, 1, 2, 3]

"""
import System.Console

l = ["foo", "bar", "baz", 0, 1, 2]
WriteLine(l.Count)
WriteLine(l.ToString())
WriteLine(l.Add(3).ToString())
