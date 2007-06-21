"""
i = 0
goto test
:start
print('ding')
i += 1
:test
goto start if (i < 3)
"""
i = 0

goto test
:start
print("ding")
i += 1
:test
goto start if i < 3

