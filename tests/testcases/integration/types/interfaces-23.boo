"""
42
7
"""
namespace Testing

interface IFoo:
   A as int:
      get
   B as int:
      get

class Base:
   A as int:
      get: return 42

class Derived(Base, IFoo):
   B as int:
      get: return 7

var test = Derived()
print test.A
print test.B