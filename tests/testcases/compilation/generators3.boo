"""
0 4 8 12 16
0 4 8 12 16
"""
import NUnit.Framework

generator = i*2 for i in range(10) if 0 == i % 2
print(join(generator))

generator.GetEnumerator().Reset()

print(join(generator))
