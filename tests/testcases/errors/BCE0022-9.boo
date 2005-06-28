"""
BCE0022-9.boo(9,22): BCE0022: Cannot convert 'callable(System.Int32, System.String) as System.Void' to 'MyCallable'.
"""
callable MyCallable(i as int) as string

def foo(i as int, a as string) as void:
    pass

call as MyCallable = foo
