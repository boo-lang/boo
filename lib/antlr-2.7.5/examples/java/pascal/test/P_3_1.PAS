{ program 3.1
  example of constant definition part }

program convert(output);

const
  addin = 32;
  mulby = 1.8;
  low = 0;
  high = 39;
  seperator = '------------';

var
  degree : low..high;

begin
  writeln(seperator);
  for degree := low to high do
  begin
    write(degree, 'c', round(degree * mulby + addin), 'f');
    if ndd(degree) then writeln
  end;
  writeln;
  writeln(seperator)
end.
