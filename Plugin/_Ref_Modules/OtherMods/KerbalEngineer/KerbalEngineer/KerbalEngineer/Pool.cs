using System.Collections.Generic;

namespace KerbalEngineer;

public class Pool<T>
{
	public delegate R CreateDelegate<out R>();

	public delegate void ResetDelegate<in T1>(T1 a);

	private readonly Stack<T> values = new Stack<T>();

	private readonly CreateDelegate<T> create;

	private readonly ResetDelegate<T> reset;

	public Pool(CreateDelegate<T> create, ResetDelegate<T> reset)
	{
		this.create = create;
		this.reset = reset;
	}

	public T Borrow()
	{
		lock (values)
		{
			return (values.Count > 0) ? values.Pop() : create();
		}
	}

	public void Release(T value)
	{
		reset(value);
		lock (values)
		{
			values.Push(value);
		}
	}

	public int Count()
	{
		return values.Count;
	}
}
