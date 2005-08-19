"""
print(ast { print('Hello, world') })
print ast { System.Console.WriteLine("\${message}") }
nodes = [ast { foo }, ast { bar }]
"""
print(ast { print("Hello, world") })
print ast { System.Console.WriteLine("${message}") }
nodes = [ast { foo }, ast { bar }]
