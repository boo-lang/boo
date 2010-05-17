"""
page = Builder()
page.Foo()
page = Builder()
page.Html.Head.Body.Output()
print 'foo'
"""
page = Builder()
page.Foo()

page = Builder()
page.
    Html.
        Head.
			Body.
    Output()
	
print "foo"
