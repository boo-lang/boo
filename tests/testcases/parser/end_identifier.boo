public class T:
    public end as object
    public override def ToString() as string:
        return "$(self.end)"

t = T(end: 'foo')
print t