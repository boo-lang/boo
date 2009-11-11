namespace BooCompiler.Tests.SupportingClasses
{
	public class ImplicitConversionToDouble
	{
		public double Value;

		public ImplicitConversionToDouble(double value)
		{
			this.Value = value;
		}

		public static implicit operator double(ImplicitConversionToDouble o)
		{
			return o.Value;
		}
	}
}
