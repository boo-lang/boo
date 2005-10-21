"""
1000000 System.Int32
1000 System.Double
00:16:40 System.TimeSpan
2000.0003 System.Double
1000 System.Int64
1000 System.Int64
1000000
1000
00:16:40
2000.0003
1000
1000
"""

n = 1_000_000
print n, n.GetType()
n2 = 1_000.000_000
print n2, n2.GetType()
n3 = 1_000_000ms //timespans and other literals work
print n3, n3.GetType()
n4 = 2_000.000_300_00
print n4, n4.GetType()
n5 = 1_000L
print n5, n5.GetType()
n6 = 1000L
print n6, n6.GetType()

s = "${1_000_000}"
print s
s2 = "${1_000.000_000}"
print s2
s3 = "${1_000_000ms}"
print s3
s4 = "${2_000.000_300_00}"
print s4
s5 = "${1_000L}"
print s5
s6 = "${1000L}"
print s6

