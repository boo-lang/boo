namespace BooCompiler.Tests.SupportingClasses
{
	public struct Rectangle
	{
		Point _top;
		public Point topLeft
		{
			get { return _top; }
			set { _top = value; }
		}
	}
}