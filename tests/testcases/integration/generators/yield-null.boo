"""
before
after
"""
def generator():
  print 'before'
  yield null
  print 'after'

for e in generator():
  assert e is null

