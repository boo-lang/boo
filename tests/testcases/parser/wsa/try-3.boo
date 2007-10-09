"""
try:
	raise System.Exception('throw')
except:
	Console.WriteLine('catch')
ensure:
	Console.WriteLine('finally')
"""
try:
	raise System.Exception('throw')
except:
	Console.WriteLine('catch')
ensure:
	Console.WriteLine('finally')
end
