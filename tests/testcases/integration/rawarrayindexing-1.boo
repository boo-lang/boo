"""
normalarrayindexing is working
rawarrayindexing is working
time to set 10000 array items using rawarrayindexing
and then normalarrayindexing
"""

myarray = (1,2,3)

try:
	normalarrayindexing:
		myarray[-1] = 4
	print "normalarrayindexing is working"
	map(myarray, {x|print(x+" ")})
except e:
	print "error: you should not see me - normalarrayindexing is not working"

try:
	rawarrayindexing:
		myarray[-1] = 5
	print "error: you should not see me - rawarrayindexing macro is not working"
except e as System.IndexOutOfRangeException:
	print "rawarrayindexing is working"
	

//time how long it takes to set 10000 array items with rawarrayindexing on
a1 = array(int,10000)
a2 = array(int,10000)
start = date.Now

i = 0
while i < 10000:
  j = 0
  while j < 10000:
      rawarrayindexing:
        a1[i] = a2[j]
        ++j
  ++i

end = date.Now
print "T(raw) =    " + (end-start) 


start = date.Now

i = 0
while i < 10000:
  j = 0
  while j < 10000:
      normalarrayindexing:
        a1[i] = a2[j]
        ++j
  ++i

end = date.Now
print "T(normal) = " + (end-start) 

