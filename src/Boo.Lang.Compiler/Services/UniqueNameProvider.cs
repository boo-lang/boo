namespace Boo.Lang.Compiler.Services
{
	public class UniqueNameProvider
	{
		private int _localIndex;

		///<summary>Generates a name that will be unique within the CompilerContext.</summary>
		///<param name="components">Zero or more string(s) that will compose the generated name.</param>
		///<returns>Returns the generated unique name.</returns>
		public string GetUniqueName(params string[] components)
		{
			var suffix = string.Concat("$", (++_localIndex).ToString());

			var len = null != components ? components.Length : 0;
			if (0 == len)
				return suffix;

			var sb = new System.Text.StringBuilder();
			foreach (string component in components)
			{
				sb.Append("$");
				sb.Append(component);
			}
			sb.Append(suffix);
			return sb.ToString();
		}
	}
}
