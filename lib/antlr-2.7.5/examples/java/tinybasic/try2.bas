PROGRAM ( ChainParameterString$ , DebugMode% )

dim a%(2,3)

a%(1,2)=7

print a%(1,2)

x%=3

call xyz(x%,2,a%(,))

end


sub xyz( y% , z% ,b%(,) )

print "Are tou Watching?",y%,z%,b%(1,2)

exit sub

end sub

