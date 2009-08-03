"""
BCE0173-1.boo(9,1): BCE0173: `z' is not a regex option. Valid options are: g, i, m, s, x, l, n, c and e.
BCE0173-1.boo(9,1): BCE0173: `Z' is not a regex option. Valid options are: g, i, m, s, x, l, n, c and e.
BCE0173-1.boo(8,1): BCE0173: `I' is not a regex option. Valid options are: g, i, m, s, x, l, n, c and e.
"""


/ok/I.IsMatch("OK")
/ok/izZ.IsMatch("OK")

