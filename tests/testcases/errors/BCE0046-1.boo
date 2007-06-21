"""
BCE0046-1.boo(8,12): BCE0046: 'is' can't be used with a value type ('int')
BCE0046-1.boo(9,6): BCE0046: 'is' can't be used with a value type ('int')
BCE0046-1.boo(10,6): BCE0046: 'is not' can't be used with a value type ('int')
"""
o1 = object()
o2 = 3
b1 = o1 is o2
b2 = o2 is o1
b3 = o2 is not o1
