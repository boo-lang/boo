

class BaseClass:
    def Test1() as int:
        return 55

    def Test1(i as int) as int:
        return Test1()/i

    def Test2() as int:
        return 33

class ExtendedClass(BaseClass):
    def Test3(i as int) as int:
        return Test1()*i

    def Test2(i as int) as int:
        return Test2()/i

ebase = ExtendedClass()

assert 55 == ebase.Test1()
assert 33 == ebase.Test2()
assert 5 == ebase.Test2(6)
assert 11 == ebase.Test1(5)
assert 110 == ebase.Test3(2)
assert 11 == ebase.Test2(3)
