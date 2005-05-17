public class E {
	public static void main(String[] args) {
		new ChildOfInner();
	}
}

class Outer {
	public Outer () {
		System.out.println("Outer()");
	}
	class Inner {
		public Inner() {
			System.out.println("Inner()");
		}
	}
}

class ChildOfInner extends Outer.Inner {
	ChildOfInner() {
		(new Outer()).super();
		// super(); --> makes no sense here; no enclosing Outer instance
		System.out.println("ChildOfInner()");
	}
}
