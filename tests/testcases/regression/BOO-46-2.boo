

class Man:
	Speak:
		get:
			return run
			
	private def run():
		return "albatross!"
		
assert "albatross!" == Man().Speak()
