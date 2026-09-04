namespace BooInteropLib

import System
import System.Collections.Generic
import System.Runtime.InteropServices
import System.Xml
import System.Threading.Tasks

public enum Kind:
	Round = 1
	Flat = 2

public callable Transform(input as string) as string

public class TagsAttribute(Attribute):
	public Names as (string)

	public def constructor(*names as (string)):
		Names = names

public class MarkerAttribute(Attribute):
	public Subject as Type

	public def constructor(subject as Type):
		Subject = subject

[Marker(typeof(Dictionary[of string, int]))]
public class Annotated:
	pass

[StructLayout(LayoutKind.Explicit)]
public struct Overlay:
	[FieldOffset(0)] public AsInt as int
	[FieldOffset(0)] public AsBytes as long

[Tags("alpha", "beta")]
public class Edges:
	public static final Answer = 42
	public static final Pi = 3.14
	public static final Letter = char('B')

	_values = List[of string]()

	public class Nested:
		public def Ping() as string:
			return "nested"

	public event Ping as Action[of string]

	public Label as string:
		get: return "edges"

	public self[index as int] as string:
		get: return _values[index]
		set: _values[index] = value

	public def Fire(message as string):
		Ping(message)

	public def Fill(*parts as (string)):
		_values.AddRange(parts)

	public static def Swap(ref a as int, ref b as int):
		temp = a
		a = b
		b = temp

	public static def Join(*parts as (string)) as string:
		return string.Join("-", parts)

	public static def Largest[of T(IComparable[of T])](values as (T)) as T:
		best = values[0]
		for value in values:
			if value.CompareTo(best) > 0:
				best = value
		return best

	# Uri and XmlDocument are defined by System.Private.Uri and
	# System.Private.Xml, not by the core implementation assembly.
	public def HostOf(address as Uri) as string:
		return address.Host

	public def Document() as XmlDocument:
		document = XmlDocument()
		document.LoadXml("<root/>")
		return document

	public def Countdown(n as int) as IEnumerable[of int]:
		while n > 0:
			yield n
			n -= 1

	[async] public def DelayedAsync(value as int) as Task[of int]:
		await Task.Delay(1)
		return value * 2

	public def Grid() as (int, 2):
		grid = matrix(int, 2, 3)
		grid[1, 2] = 9
		return grid

	public def Jagged() as ((int)):
		return (of (int): (1, 2), (3,))

public class Vector:
	public X as int
	public Y as int

	public def constructor(x as int, y as int):
		X = x
		Y = y

	public static def op_Addition(left as Vector, right as Vector) as Vector:
		return Vector(left.X + right.X, left.Y + right.Y)

	public override def ToString() as string:
		return "(${X},${Y})"

public class Bag(Boo.Lang.IQuackFu):
	_values = Dictionary[of string, object]()

	public def QuackGet(name as string, parameters as (object)) as object:
		return _values[name] if _values.ContainsKey(name)
		return null

	public def QuackSet(name as string, parameters as (object), value as object) as object:
		_values[name] = value
		return value

	public def QuackInvoke(name as string, args as (object)) as object:
		return "${name}/${args.Length}"

public class Ducks:
	public static def Speak(target as object) as object:
		d as duck = target
		return d.Speak()

	public static def Poke(bag as Bag) as object:
		d as duck = bag
		return d.Whatever(1, 2)

public class Extensions:
	[Extension]
	public static def Shout(text as string) as string:
		return text.ToUpper() + "!"

public class Native:
	# One declaration per platform C runtime, so the marshalled parameter is
	# exercised wherever the tests run, not only where libc exists.
	[DllImport("libc", CharSet: CharSet.Ansi)]
	public static def strlen([MarshalAs(UnmanagedType.LPStr)] text as string) as int:
		pass

	[DllImport("kernel32", CharSet: CharSet.Ansi)]
	public static def lstrlenA([MarshalAs(UnmanagedType.LPStr)] text as string) as int:
		pass

	public static def Length(text as string) as int:
		if OperatingSystem.IsWindows():
			return lstrlenA(text)
		return strlen(text)

public class Defaults:
	"""A Boo declared optional parameter, for C# to leave out."""
	public static def Greet(name as string, greeting as string = "Hi") as string:
		return "${greeting}, ${name}!"

	public static def Sum(a as int, b as int = 2, c as int = 3) as int:
		return a + b + c
