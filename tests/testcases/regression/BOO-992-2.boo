"""
regular method
generic method
"""
class Class:
  static def Method(arg as int):
    return "regular method"

  static def Method[of T](arg as int):
    return "generic method"

print Class.Method(42)
print Class.Method[of single](42)
