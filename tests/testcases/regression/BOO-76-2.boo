

fname, lname = @/ /.Split("Eric Idle")
assert "Eric" == fname
assert "Idle" == lname

fname, lname = /\u0020/.Split("John Cleese")
assert "John" == fname
assert "Cleese" == lname

