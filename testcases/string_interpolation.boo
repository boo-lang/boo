"""
print('gonna eval "2 + 2 is ${2 + 2}"')
print(string.Format('2 + 2 is {0}', ((2 + 2),)))
print(string.Format('{0} should be {1}', (((3 * 2) - 1), (true ? 5 : '?'))))

"""
print('gonna eval "2 + 2 is ${2 + 2}"')
print("2 + 2 is ${2 + 2}")
print("${3*2-1} should be ${true ? 5 : '?'}")
