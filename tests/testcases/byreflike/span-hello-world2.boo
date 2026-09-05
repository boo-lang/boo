"""
Hello, World!
"""

import System

var span = "Hello, World!".AsSpan()
for letter in span:
	Console.Write(letter)
Console.WriteLine()