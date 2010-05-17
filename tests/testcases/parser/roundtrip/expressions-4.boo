"""
a = ((1 | (2 & 3)) | 2)
a = (Foo.Spam | (Foo.Eggs & Foo.All))
"""
a = 1 | 2 & 3 | 2
a = Foo.Spam | Foo.Eggs & Foo.All
