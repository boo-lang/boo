using System;

namespace NDoc.Test.Preliminary
{
	/// <summary>
	/// Vaiorus uses of the &lt;preliminary/&gt; tag.
	/// </summary>
	public class NamespaceDoc {}

	/// <summary>
	/// This class is preliminary
	/// </summary>
	/// <preliminary/>
	public class Preliminary
	{
		/// <summary>
		/// This topic should have the preliminary message as well
		/// </summary>
		public void Member(){}
	}

	/// <summary>
	/// This class is preliminary and has custom text in the preliminary tag
	/// </summary>
	/// <preliminary>This documentation will change at a moment's notice</preliminary>
	public class PreliminaryWithText
	{

	}

	/// <summary>
	/// This interface is preliminary
	/// </summary>
	/// <preliminary/>
	public interface IPreliminary
	{
		/// <summary>
		/// do something
		/// </summary>
		void Method();
	}
}

