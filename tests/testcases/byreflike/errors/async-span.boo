"""
async-span.boo(7,13): BCE0192: Byref-like type 'System.ReadOnlySpan[of char]' cannot be captured by a closure, a generator or an async method.
"""
import System
import System.Threading.Tasks

[async] def Work() as Task:
	span = "Hello".AsSpan()
	await(Task.Delay(1))
	Console.WriteLine(span.Length)

Work().Wait()
