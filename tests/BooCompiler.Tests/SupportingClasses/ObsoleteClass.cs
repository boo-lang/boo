using System;

namespace BooCompiler.Tests.SupportingClasses
{
	public class ObsoleteClass
	{
		[Obsolete("It is." )]
		public static int Bar = 42;
		
		[Obsolete("Indeed it is." )]
		public static void Foo()
		{
		}

		[Obsolete("We said so.")]
		public static int Baz
		{
			get { return 42; }
		}
	}
}