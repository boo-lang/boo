PROGRAM ( ChainParameterString$ , DebugMode% )

x%=2


do until x%=5
	x% = x% +1
	print x%
loop

do
	x%=x%+1
	print x%,x%,"Hello World"

loop until x%=7

if x% = 1 then
	print 21
else if x% = 2 then
	print 22
else if x% = 5 then
	print 23
else
	print 24
end if

for x%=1 to 3
	y%=x%
	print x% , y%+2
next x%

y% = 2

end
