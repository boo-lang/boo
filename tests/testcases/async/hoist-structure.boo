"""
Before 12
After 12
"""

namespace ConsoleApp

import System
import System.Threading.Tasks

struct TestStruct:
        public i as long
        public j as long

static class Program:
    [async] def TestAsync() as Task:
        t as TestStruct
        t.i = 12
        Console.WriteLine("Before {0}", t.i); // emits "Before 12"
        await Task.Delay(100);
        Console.WriteLine("After {0}", t.i); // emits "After 0" expecting "After 12"

def Main(args as (string)):
    Program.TestAsync().Wait()
