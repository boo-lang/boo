namespace antlr.debug
{
	using System;
	
	public class SyntacticPredicateEventArgs : GuessingEventArgs
	{
		
		
		public SyntacticPredicateEventArgs()
		{
		}
		public SyntacticPredicateEventArgs(int type) : base(type)
		{
		}

		override public string ToString()
		{
			return "SyntacticPredicateEvent [" + Guessing + "]";
		}
	}
}