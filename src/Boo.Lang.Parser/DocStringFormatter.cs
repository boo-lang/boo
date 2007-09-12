namespace Boo.Lang.Parser
{
	public class DocStringFormatter
	{
		// every new line is transformed to '\n'
		// trailing and leading newlines are removed
		public static string Format(string s)
		{
			if (s.Length == 0) return string.Empty;

			s = s.Replace("\r\n", "\n");

			int length = s.Length;
			int startIndex = 0;
			if ('\n' == s[0])
			{
				// assumes '\n'
				startIndex++;
				length--;
			}
			if ('\n' == s[s.Length - 1])
			{
				length--;
			}
			
			if (length > 0) return s.Substring(startIndex, length);
			return string.Empty;
		}
	}
}
