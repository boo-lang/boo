"""
Si
"""
import System
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

p = Person(LastName: "Simpson")

firstLetter, secondLetter = p.LastName

Console.Write("${firstLetter}${secondLetter}")
