using System.Globalization;
using System.Threading;

namespace BooCompiler.Tests.SupportingClasses;

public enum Shade
{
	None = 0,
	Bright = 7,
}

// Every shape a default value arrives in through ParameterInfo.
public class OptionalParameters
{
	public static string Trailing(int a, int b = 2) => $"{a},{b}";

	public static string AllOptional(int a = 1, int b = 2) => $"{a},{b}";

	public static string Shapes(
		bool flag = true,
		char letter = 'z',
		int number = 42,
		long big = 9000000000L,
		double ratio = 1.5,
		string text = "hi",
		Shade shade = Shade.Bright)
		=> string.Join(",",
			flag.ToString(CultureInfo.InvariantCulture),
			letter.ToString(CultureInfo.InvariantCulture),
			number.ToString(CultureInfo.InvariantCulture),
			big.ToString(CultureInfo.InvariantCulture),
			ratio.ToString(CultureInfo.InvariantCulture),
			text,
			shade.ToString());

	public static string Nulls(string text = null, object any = null)
		=> $"{text ?? "<null>"},{any ?? "<null>"}";

	// A struct default arrives as null rather than a value.
	public static string Token(CancellationToken token = default) => token.CanBeCanceled.ToString();

	// An exact overload must beat one that needs a default filled in.
	public static string Prefer(int a) => "exact";

	public static string Prefer(int a, int b = 2) => "default";
}
