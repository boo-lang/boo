using System;

namespace Boo.Ast
{
	/// <summary>
	/// Summary description for INodeWithAttributes.
	/// </summary>
	public interface INodeWithAttributes
	{
		AttributeCollection Attributes
		{
			get;
			set;
		}
	}
}
