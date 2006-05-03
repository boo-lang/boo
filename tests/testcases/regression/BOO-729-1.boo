"""
ok
"""

import System.Windows.Forms

class MyForm(Form):
	protected override def DefWndProc(ref m as Message):
		pass
		
m = MyForm()
print "ok"
