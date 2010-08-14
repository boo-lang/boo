using System;

namespace Boo.Lang.Parser
{
	public class ParserSettings
	{
		public const int DefaultTabSize = 4;
		
		private int _tabSize = DefaultTabSize;

		public int TabSize
		{
			get { return _tabSize; }

			set
			{
				if (value < 1) throw new ArgumentOutOfRangeException("TabSize");
				_tabSize = value;
			}
		}
	}
}
