namespace BooCompiler.Tests.SupportingClasses

struct Rectangle:
	private _top as Point

	public topLeft as Point:
		get: return _top
		set: _top = value
