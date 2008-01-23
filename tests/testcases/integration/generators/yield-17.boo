"""
Test 0
=====
Test 1
begin outer try
outer ensure
Caught: Exception from outer ensure
=====
Test 2
begin outer try
begin middle try
middle ensure
outer ensure
Caught: Exception from outer ensure
=====
Test 3
begin outer try
begin middle try
innermost try
innermost ensure
middle ensure
outer ensure
Caught: Exception from outer ensure
=====
Test 4
begin outer try
begin middle try
innermost try
innermost try continues
innermost ensure
middle ensure
outer ensure
Caught: Exception from outer ensure
=====
"""

def NestedEnsures():
    try:
        yield "begin outer try"
        try:
            yield "begin middle try"
            try:
                yield "innermost try"
                print "innermost try continues"
            ensure:
                print "innermost ensure"
                raise "Exception from innermost ensure"
            yield "end middle try"
        ensure:
            print "middle ensure"
            raise "Exception from middle ensure"
        yield "end outer try"
    ensure:
        print "outer ensure"
        raise "Exception from outer ensure"

def Consume(generator as System.Collections.Generic.IEnumerable of string, count as int):
    enumerator = generator.GetEnumerator()
    try:
        for i in range(count):
            enumerator.MoveNext()
            print enumerator.Current
        enumerator.Dispose()
    except ex:
        print "Caught: ${ex.Message}"

for i in range(5):
    print "Test ${i}"
    Consume(NestedEnsures(), i)
    print "====="
