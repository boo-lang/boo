namespace Boo.Lang.Interpreter

import System

class PromptLine:
"""
current line edit wrapper
handle multiline editing
"""
	text = ""
	position = 0

	virtual WindowWidth as int:
		get:
			return Console.WindowWidth
		set:
			Console.WindowWidth = value

	virtual Left as int:
		get:
			return Console.CursorLeft
		set:
			Console.CursorLeft = value

	virtual Top as int:
		get:
			return Console.CursorTop
		set:
			Console.CursorTop = value

	Length as int:
		get:
			return text.Length

	Position as int:
		get:
			return position
		set:
			value = 0 if value < 0
			value = text.Length if value > text.Length
			return if value == position

			diff = value - position
			diffInLines = diff / WindowWidth
			diffInChars = diff % WindowWidth
			if Left + diffInChars < 0:
				diffInLines--
				diffInChars += WindowWidth
			elif Left + diffInChars >= WindowWidth:
				diffInLines++
				diffInChars -= WindowWidth
			Top += diffInLines
			Left += diffInChars
			position = value

	def Append(key as char):
		Append(key.ToString())

	def Append(key as string):
		if position == text.Length:
			text += key
			Write(key)
			position += key.Length
		else:
			text = text.Insert(position, key)
			RestorePosition:
				Write(text.Substring(position, text.Length - position))
			Position += key.Length

	Text as string:
		get:
			return text
		set:
			Position = 0
			for i in range(text.Length):
				Write(" ")
			position = text.Length
			Position = 0
			text = value
			position = text.Length
			Write(text)

	def Remove(count as int):
		return if position == text.Length or count > text.Length - position or count == 0

		text = text.Remove(position, count)
		RestorePosition:
			Write(text.Substring(position, text.Length - position))
			for i in range(count):
				Write(" ")

	private def RestorePosition(action as Action):
		preserving Top, Left:
			action()

	protected virtual def Write(data as string):
		Console.Write(data)

	override def ToString():
		return text

