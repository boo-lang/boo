"""
before
after
end
"""
f = def ():
	print('before')
	goto end
	print('skipped')
	:end
	print('after')

f()
goto end
print('skipped')
:end
print("end")

