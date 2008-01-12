"""
BCE0154-1.boo(11,2): BCE0154: 'System.Runtime.InteropServices.ComVisibleAttribute' cannot be applied multiple times on the same target.
BCE0154-1.boo(12,2): BCE0154: 'System.Runtime.InteropServices.ComVisibleAttribute' cannot be applied multiple times on the same target.
"""

[System.Diagnostics.Conditional("test no false error on multiple usage")]
[System.Diagnostics.Conditional("test no false error on multiple usage")]
def TestMultipleUsageGood():
	pass

[System.Runtime.InteropServices.ComVisible(true)]
[System.Runtime.InteropServices.ComVisible(false)]
def TestMultipleUsageBad():
	pass

