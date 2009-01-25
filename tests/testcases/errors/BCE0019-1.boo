"""
BCE0019-1.boo(9,22): BCE0019: 'InteropService' is not a member of 'System.Runtime'. Did you mean 'InteropServices'?
BCE0019-1.boo(10,16): BCE0019: 'Writelinne' is not a member of 'System.Console'. Did you mean 'WriteLine'?
BCE0019-1.boo(11,8): BCE0019: 'Nosonle' is not a member of 'System'. 
BCE0019-1.boo(12,8): BCE0019: 'Consle' is not a member of 'System'. Did you mean 'Console'?
BCE0019-1.boo(13,16): BCE0019: 'curssorleft' is not a member of 'System.Console'. Did you mean 'CursorLeft'?
BCE0019-1.boo(14,22): BCE0019: 'GetCursorLeft' is not a member of 'System.Console'. Did you mean 'CursorLeft'?
"""
print System.Runtime.InteropService
System.Console.Writelinne("foo")
System.Nosonle.WriteLine("bar")
System.Consle.WriteLine("foo")
System.Console.curssorleft = 0
print System.Console.GetCursorLeft()

