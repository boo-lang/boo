using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	/// <summary>
	/// Summary description for Assert.
	/// </summary>
	internal sealed class Assert
	{
		public static void AssertNotNull(string message, object reference)
		{
			if (null == reference)
			{
				throw new ArgumentNullException(message);
			}
		}
	}
}
