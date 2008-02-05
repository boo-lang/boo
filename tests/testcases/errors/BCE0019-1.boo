"""
BCE0019-1.boo(8,22): BCE0019: 'InteropService' is not a member of 'System.Runtime'. Did you mean 'InteropServices' ?
BCE0019-1.boo(9,16): BCE0019: 'Writelinne' is not a member of 'System.Console'. Did you mean 'WriteLine' ?
BCE0019-1.boo(10,8): BCE0019: 'Nosonle' is not a member of 'System'. 
BCE0019-1.boo(11,8): BCE0019: 'Consle' is not a member of 'System'. Did you mean 'Console' ?
BCE0019-1.boo(12,16): BCE0019: 'curssorleft' is not a member of 'System.Console'. Did you mean 'CursorLeft' ?
"""
print System.Runtime.InteropService
System.Console.Writelinne("foo")
System.Nosonle.WriteLine("bar")
System.Consle.WriteLine("foo")
System.Console.curssorleft = 0
