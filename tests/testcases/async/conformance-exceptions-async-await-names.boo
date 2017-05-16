"""
0
"""
import System
class TestCase:
    public def Run():
        Driver.Tests++
        try:
            raise ArgumentException()
        except await as Exception:
            if await isa ArgumentException:
                Driver.Count++
        Driver.Tests++
        try:
            raise ArgumentException()
        except async as Exception:
            if async isa ArgumentException:
                Driver.Count++

class Driver:
    public static Tests as int
    public static Count as int

static def Main():
    var t = TestCase()
    t.Run()
    Console.WriteLine(Driver.Tests - Driver.Count)
