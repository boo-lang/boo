a = [1, 2, 3, 4, 5, 6]
a.RemoveAll({item as int | return 0 == item % 2})
assert a == [1, 3, 5]
a = [2, 4, 6, 8]
a.RemoveAll({item as int | return 0 == item % 2})
assert a == []
