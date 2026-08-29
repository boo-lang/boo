{ program 4.4
  compute h(n) = 1 + 1/2 + 1/3 +...+ 1/n }

program egfor(input, output);

var
  i, n : integer;
  h : real;

begin
  read(n);
  write(n);
  h := 0;
  for i:= n downto 1 do
    h := h + 1/i;
  writeln(h);
end.