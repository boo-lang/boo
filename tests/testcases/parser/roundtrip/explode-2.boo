"""
pattern1 = Container(Items: (first, second, (*_)))
pattern2 = Container(Items: [first, (*_)])
"""
pattern1 = Container(Items: (first, second, *_))
pattern2 = Container(Items: [first, *_])

