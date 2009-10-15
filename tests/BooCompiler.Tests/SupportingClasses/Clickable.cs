using System;

namespace BooCompiler.Tests.SupportingClasses
{
	public class Clickable
	{
		public Clickable()
		{			
		}
		
		public event EventHandler Click;
		
		public static event EventHandler Idle; 
		
		public void RaiseClick()
		{
			if (null != Click)
			{
				Click(this, EventArgs.Empty);
			}
		}
		
		public static void RaiseIdle()
		{
			if (null != Idle)
			{
				Idle(null, EventArgs.Empty);
			}
		}
	}
}