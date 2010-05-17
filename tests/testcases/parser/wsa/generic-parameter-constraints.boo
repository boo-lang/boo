"""
import System.Collections

public class T1[of T(IEnumerable)]:
	pass

public class T2[of T(class)]:
	pass

public class T3[of T(struct)]:
	pass

public class T4[of T(constructor)]:
	pass

public class T5[of T(IEnumerable, System.Collections.IComparer, class, struct, constructor)]:
	pass

public class T6[of T(class, constructor)]:
	pass

public class T7[of T(IEnumerable, System.DateTime)]:
	pass

public class T8[of T(IEnumerable, class), U(System.DateTime), V, W(constructor)]:
	pass
"""
import System.Collections

public class T1[of T(IEnumerable)]:
end

public class T2[of T(class)]:
end

public class T3[of T(struct)]:
end

public class T4[of T(constructor)]:
end

public class T5[of T(IEnumerable, System.Collections.IComparer, class, struct, constructor)]:
end

public class T6[of T(class, constructor)]:
end

public class T7[of T(IEnumerable, System.DateTime)]:
end

public class T8[of T(IEnumerable, class), U(System.DateTime), V, W(constructor)]:
end

