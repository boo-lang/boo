namespace Boo.Lang.Compiler.TypeSystem
{
	public abstract class ElementUtil
	{		 
		public static string GetSignature(IMethod tag)
		{			
			System.Text.StringBuilder sb = new System.Text.StringBuilder(tag.DeclaringType.FullName);
			sb.Append(".");
			sb.Append(tag.Name);
			sb.Append("(");
			
			IParameter[] parameters = tag.GetParameters();
			for (int i=0; i<parameters.Length; ++i)
			{				
				if (i>0) 
				{
					sb.Append(", ");
				}
				sb.Append(parameters[i].Type.FullName);
			}
			sb.Append(")");
			
			/*
			IType rt = tag.ReturnType;
			if (null != rt)
			{
				sb.Append(" as ");
				sb.Append(rt.FullName);
			}
			*/
			return sb.ToString();
		}
	}
}
