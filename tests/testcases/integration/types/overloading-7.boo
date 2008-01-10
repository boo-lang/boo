"""
int:33
string:test
T,int:42,64
T:42
"""
def Onur[of T](x as T):
        print "T:"+x

def Onur[of T](x as T, d as int):
        print "T,int:" + x + "," + d

def Onur(x as int):
        print "int:"+x

def Onur(x as string):
        print "string:"+x

Onur(33)
Onur("test")
Onur[of int](42, 64)
Onur[of int](42)
