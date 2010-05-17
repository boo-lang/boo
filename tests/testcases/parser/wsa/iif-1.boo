"""
n = int.Parse(prompt('select a number: '))
print(iif((n % 2), 'sorry', 'cool'))
print(iif((n > 10), iif((n > 20), 20, 10), n))
"""
n = int.Parse(prompt('select a number: '))
print(iif((n % 2), 'sorry', 'cool'))
print(iif((n > 10), iif((n > 20), 20, 10), n))
