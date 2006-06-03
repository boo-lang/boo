namespace Boo.Lang.Useful

import System

class PlatformInformation:

	static IsMono as bool:
		get:
			return Type.GetType("System.MonoType", false) is not null
			
	static IsWindows as bool:
		get:
			return Environment.OSVersion.Platform in (
					PlatformID.Win32NT,
					PlatformID.Win32Windows,)
					
	static IsLinux as bool:
		get:
			return cast(int, Environment.OSVersion.Platform) in (4, 128)
