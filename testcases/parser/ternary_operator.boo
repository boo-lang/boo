"""
n = int(prompt('select a number: '))
print(((n % 2) ? 'sorry' : 'cool'))
a = ((n > 10) ? ((n > 20) ? 20 : 10) : n)

"""
n = int(prompt("select a number: "))
print(n % 2 ? "sorry" : "cool")
a = n > 10 ? n > 20 ? 20 : 10 : n
