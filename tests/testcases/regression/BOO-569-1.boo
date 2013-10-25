#category FailsOnMono
"""
100
100
"""
import System
import System.Windows.Forms from System.Windows.Forms

class FormSubclass(Form):

	box = PictureBox()
	
	def constructor():
		box.Size.Width = 100
		box.Size.Height = 100
		print box.Size.Height
		print box.Size.Width
		
FormSubclass()
