"""
before
after
"""
def generator() as System.Collections.IEnumerator:
  print 'before'
  yield null
  print 'after'

for e in generator():
  assert e is null
