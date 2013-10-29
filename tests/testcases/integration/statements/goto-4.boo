"""
before
after
end
"""
f = def ():
	print('before')
	goto exit
	print('skipped')
	:exit
	print('after')

f()
goto exit
print('skipped')
:exit
print("end")

