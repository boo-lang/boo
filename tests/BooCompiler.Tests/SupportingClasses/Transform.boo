namespace BooCompiler.Tests.SupportingClasses

class Transform:
	private _position as Vector3

	public position as Vector3:
		get: return _position
		set: _position = value
