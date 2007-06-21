"""
100 System.Int32
100 System.Int32
110 System.Double
0.011 System.Double
100 System.Int64
00:10:00 System.TimeSpan
00:00:03.6000000 System.TimeSpan
90 System.Double
100
100
110
0.011
100
00:10:00
00:00:03.6000000
90
"""

x = 1e+2
print x, x.GetType() //100
x2 = 1e2
print x2, x2.GetType() //100
x4 = 1.1e+2
print x4, x4.GetType() //110
x5 = 1.1e-2
print x5.ToString(System.Globalization.CultureInfo.InvariantCulture), x5.GetType() //0.011
x6 = 1e+2L
print x6, x6.GetType() //100 long
x7 = 6e+2s
print x7, x7.GetType() //10:00
x8 = 10.0e-4h
print x8, x8.GetType()  //3.6 seconds
x9 = .9e2
print x9.ToString(System.Globalization.CultureInfo.InvariantCulture), x9.GetType() //90

s = "${1e+2}"
print s
s2 = "${1e2}"
print s2
s4 = "${1.1e+2}"
print s4
s5 = "${(1.1e-2).ToString(System.Globalization.CultureInfo.InvariantCulture)}"
print s5
s6 = "${1e+2L}"
print s6
s7 = "${6e+2s}"
print s7
s8 = "${10.0e-4h}"
print s8
s9 = "${.9e2}"
print s9

