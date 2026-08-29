"""
Info
"""
enum LogLevel:
	None
	Info
	Error

class Foo:		
	public static Level = LogLevel.Info
	
print(Foo.Level)
		
