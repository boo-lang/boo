"""
Info
Release
"""
class Foo:
	
	enum LogLevel:
		None
		Info
		Error

	public static Level = LogLevel.Info
	
class Bar:

	enum LogLevel:	
		Release
		Debug
		
	public static Level = LogLevel.Release

print(Foo.Level)
print(Bar.Level)
		
