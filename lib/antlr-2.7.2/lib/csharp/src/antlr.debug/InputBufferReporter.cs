namespace antlr.debug
{
	using System;
	
	public class InputBufferReporter : InputBufferListenerBase, InputBufferListener
	{
		public virtual void inputBufferChanged(object source, InputBufferEventArgs e)
		{
			System.Console.Out.WriteLine(e);
		}

		/// <summary> charBufferConsume method comment.
		/// </summary>
		override public void  inputBufferConsume(object source, InputBufferEventArgs e)
		{
			System.Console.Out.WriteLine(e);
		}

		/// <summary> charBufferLA method comment.
		/// </summary>
		override public void  inputBufferLA(object source, InputBufferEventArgs e)
		{
			System.Console.Out.WriteLine(e);
		}

		override public void  inputBufferMark(object source, InputBufferEventArgs e)
		{
			System.Console.Out.WriteLine(e);
		}

		override public void  inputBufferRewind(object source, InputBufferEventArgs e)
		{
			System.Console.Out.WriteLine(e);
		}
	}
}