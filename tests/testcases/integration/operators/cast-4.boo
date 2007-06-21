import NUnit.Framework

def sum(items):
   value = 0
   for item as int in items:
      value += item
   return value

Assert.AreEqual(6, sum([1, 2, 3]))
Assert.AreEqual(6, sum([1L, 2L, 3L]))
