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

class Foo(IFoo):
	def get():
		return null
	def set(value):
		pass
		
Foo().set("")
print Foo().get()

