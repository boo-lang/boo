"""
Eric
Eric, John
"""
import System

[Flags]
enum MontyMembers:
	Eric = 1
	John = 2
	
print(MontyMembers.Eric)
print(MontyMembers.Eric|MontyMembers.John)
