"""
1.0
1.1
"""
class Information:
	
	[property(Version)]
	static _version = "1.0"
		
print Information.Version
Information.Version = "1.1"
print Information.Version

