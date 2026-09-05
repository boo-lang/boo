"""
Hello, World!
"""

import System

var span = "Hello, World!".AsSpan()
for i in range(span.Length):
	Console.Write(span[i])
Console.WriteLine()