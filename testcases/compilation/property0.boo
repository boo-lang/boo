"""
Simpson, Homer
"""
using System
using Boo.Tests.Ast.Compilation from Boo.Tests

p = Person(LastName: "Simpson")
p.FirstName = "Homer"

Console.Write("${p.LastName}, ${p.FirstName}")
