"""
Simpson, Homer
"""
using System
using Boo.Tests.Ast.Compiler from Boo.Tests

p = Person(LastName: "Simpson")
p.FirstName = "Homer"

Console.Write("${p.LastName}, ${p.FirstName}")
