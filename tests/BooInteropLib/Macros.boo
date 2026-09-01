namespace BooInteropLib

import System
import System.Collections.Generic
import System.IO
import Boo.Lang.PatternMatching

public class Macros:
	[Property(Title)]
	_title as string = "macro"

	public def Classify(value as object) as string:
		match value:
			case Vector(X: 0, Y: 0):
				return "origin"
			case Vector(X: x, Y: y):
				return "vector:${x},${y}"
			case "hello":
				return "greeting"
			otherwise:
				return "other"

	public def Guarded(items as List[of string]) as int:
		total = 0
		lock self:
			for item in items:
				total += item.Length
		return total

	public static def Read(text as string) as string:
		using reader = StringReader(text):
			return reader.ReadLine()

	public static def Sum(values as (int)) as int:
		total = 0
		checked:
			for value in values:
				total += value
		return total
