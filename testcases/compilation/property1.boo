"""
Si
"""
import System
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

p = Person(LastName: "Simpson")

firstLetter, secondLetter = p.LastName

Console.Write("${firstLetter}${secondLetter}")
