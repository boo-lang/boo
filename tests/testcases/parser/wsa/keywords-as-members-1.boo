"""
import org.com.internal.foo
import com.public.foo
import javax.swing.event

interface IFoo:

	def get() as object

	def set(value)

class Foo(IFoo):

	def get():
		return null

	def set(value):
		pass

Foo().set('')
print Foo().get()
"""
import org.com.internal.foo
import com.public.foo
import javax.swing.event

interface IFoo:
	def get() as object
	def set(value)
end

class Foo(IFoo):
	def get():
		return null
	end
	def set(value):
	end
end
		
Foo().set("")
print Foo().get()

