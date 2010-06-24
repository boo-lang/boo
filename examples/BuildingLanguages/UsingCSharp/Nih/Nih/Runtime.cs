using System;

namespace Nih
{
	public static class Runtime
	{
		public static void say(int times)
		{
			for (int i = 0; i < times; ++i)
				Console.Write("nih! ");
		}
	}
}
