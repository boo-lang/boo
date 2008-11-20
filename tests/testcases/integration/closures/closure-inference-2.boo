#ignore Closure inference in field declarations is not yet supported
"""
007
"""

public class Class:
	[property(Closure)]
	field as callable(int) as string = { i | i.ToString("000") }

print Class().Closure(7)