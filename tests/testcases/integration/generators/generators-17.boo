import System
import NUnit.Framework

functions = Math.Sin, Math.Cos

generators = []
for f in functions:
	generators.Add(f(value) for value in range(3))

for generator, function as callable(double) as double in zip(generators, functions):
	expected = join(function(i) for i in range(3))
	actual = join(generator)
	Assert.AreEqual(expected, actual)
