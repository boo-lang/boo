"""
print([| print('Hello, world') |])
print [| System.Console.WriteLine("\$message") |]
nodes = [[| foo |], [| bar |]]
"""
print([| print("Hello, world") |])
print [| System.Console.WriteLine("${message}") |]
nodes = [[| foo |], [| bar |]]

