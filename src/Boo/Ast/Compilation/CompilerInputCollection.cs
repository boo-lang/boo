using System;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// A collection of <see cref="ICompilerInput"/> objects.
	/// </summary>
	public class CompilerInputCollection : System.Collections.CollectionBase
	{
		public ICompilerInput this[int index]
		{
			get
			{
				return (ICompilerInput)InnerList[index];
			}
		}

		public void Add(ICompilerInput input)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input");
			}
			InnerList.Add(input);
		}
	}
}
