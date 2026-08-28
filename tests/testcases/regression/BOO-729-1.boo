#ignore System.Windows.Forms needs a net10.0-windows target; this one is net10.0
"""
ok
"""

import System.Windows.Forms

class MyForm(Form):
	protected override def DefWndProc(ref m as Message):
		pass
		
print "ok"
