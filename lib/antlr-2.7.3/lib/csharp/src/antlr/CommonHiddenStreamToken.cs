using System;

namespace antlr
{
	/*ANTLR Translator Generator
	* Project led by Terence Parr at http://www.jGuru.com
	* Software rights: http://www.antlr.org/RIGHTS.html
	*
	* $Id: $
	*/

	//
	// ANTLR C# Code Generator by Micheal Jordan
	//                            Kunle Odutola       : kunle UNDERSCORE odutola AT hotmail DOT com
	//                            Anthony Oguntimehin
	//
	// With many thanks to Eric V. Smith from the ANTLR list.
	//
	
	public class CommonHiddenStreamToken : CommonToken
	{
		protected internal CommonHiddenStreamToken hiddenBefore;
		protected internal CommonHiddenStreamToken hiddenAfter;
		
		public CommonHiddenStreamToken() : base()
		{
		}
		
		public CommonHiddenStreamToken(int t, string txt) : base(t, txt)
		{
		}
		
		public CommonHiddenStreamToken(string s) : base(s)
		{
		}
		
		public virtual CommonHiddenStreamToken getHiddenAfter()
		{
			return hiddenAfter;
		}
		
		public virtual CommonHiddenStreamToken getHiddenBefore()
		{
			return hiddenBefore;
		}
		
		protected internal virtual void  setHiddenAfter(CommonHiddenStreamToken t)
		{
			hiddenAfter = t;
		}
		
		protected internal virtual void  setHiddenBefore(CommonHiddenStreamToken t)
		{
			hiddenBefore = t;
		}
	}
}