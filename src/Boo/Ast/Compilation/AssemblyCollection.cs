using System;
using System.Collections;
using Assembly = System.Reflection.Assembly;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// Referenced assemblies collection.
	/// </summary>
	public class AssemblyCollection : CollectionBase
	{
		public AssemblyCollection()
		{
		}

		public void Add(Assembly assembly)
		{
			if (null == assembly)
			{
				throw new ArgumentNullException("assembly");
			}
			if (!InnerList.Contains(assembly))
			{
				InnerList.Add(assembly);
			}
		}
	}
}
