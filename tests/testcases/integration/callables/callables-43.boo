#category FailsOnMono4

a = { item | return item.ToString() }, { item as string | return item.ToUpper() }

assert "3" == a[0](3)
assert "FOO" == a[-1]("foo")
