"""
1
1
"""
import System

def Measure[of T(IDisposable)](value as T, f as Func[of T, int]):
	return f(value) + 100

# The closure is offered to the first overload, which its constraint then rules
# out, before the second is chosen. The closure's parameter has to take the
# type the winning overload gives it.
def Measure[of T](values as (T), f as Func[of T, int]):
	return f(values[0])

print Measure((7, 8), {a | a.ToString().Length})
print Measure(("x", "yy"), {a | a.Length})
