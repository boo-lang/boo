"""
BCE0126-5.boo(8,7): BCE0126: It is not possible to evaluate an expression of type 'void'.
BCE0126-5.boo(10,7): BCE0126: It is not possible to evaluate an expression of type 'void'.
"""
def z(g as int):
   pass

f = (z(i) for i as int in [1,2,3])

g = [z(i) for i as int in [1,2,3]]

