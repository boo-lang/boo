// for assembly qualified using testing 
namespace System.Drawing
{
	public class Point
	{
		int _x;
		int _y;
		
		public Point(int x, int y)
		{
			_x = x;
			_y = y;
		}
		
		public override string ToString()
		{
			return string.Format("MyPoint({0}, {1})", _x, _y);
		}
	}
}
