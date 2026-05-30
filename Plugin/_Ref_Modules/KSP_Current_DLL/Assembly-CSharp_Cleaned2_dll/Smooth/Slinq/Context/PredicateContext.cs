using Smooth.Algebraics;
using Smooth.Delegates;

namespace Smooth.Slinq.Context;

public struct PredicateContext<T, U>
{
	private bool needsMove;

	private Slinq<T, U> chained;

	private readonly DelegateFunc<T, bool> predicate;

	private BacktrackDetector bd;

	private static readonly Mutator<T, PredicateContext<T, U>> dispose = Dispose;

	private static readonly Mutator<T, PredicateContext<T, U>> takeWhileSkip = TakeWhileSkip;

	private static readonly Mutator<T, PredicateContext<T, U>> takeWhileRemove = TakeWhileRemove;

	private static readonly Mutator<T, PredicateContext<T, U>> whereSkip = WhereSkip;

	private static readonly Mutator<T, PredicateContext<T, U>> whereRemove = WhereRemove;

	private PredicateContext(Slinq<T, U> chained, DelegateFunc<T, bool> predicate)
	{
		needsMove = false;
		this.chained = chained;
		this.predicate = predicate;
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, PredicateContext<T, U>> TakeWhile(Slinq<T, U> slinq, DelegateFunc<T, bool> predicate)
	{
		return new Slinq<T, PredicateContext<T, U>>(takeWhileSkip, takeWhileRemove, dispose, new PredicateContext<T, U>(slinq, predicate));
	}

	public static Slinq<T, PredicateContext<T, U>> Where(Slinq<T, U> slinq, DelegateFunc<T, bool> predicate)
	{
		return new Slinq<T, PredicateContext<T, U>>(whereSkip, whereRemove, dispose, new PredicateContext<T, U>(slinq, predicate));
	}

	private static void Dispose(ref PredicateContext<T, U> context, out Option<T> next)
	{
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
	}

	private static void TakeWhileSkip(ref PredicateContext<T, U> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
		}
		else
		{
			context.needsMove = true;
		}
		if (!context.chained.current.isSome)
		{
			next = context.chained.current;
			return;
		}
		if (context.predicate(context.chained.current.value))
		{
			next = context.chained.current;
			return;
		}
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
	}

	private static void TakeWhileRemove(ref PredicateContext<T, U> context, out Option<T> next)
	{
		context.needsMove = false;
		context.chained.remove(ref context.chained.context, out context.chained.current);
		TakeWhileSkip(ref context, out next);
	}

	private static void WhereSkip(ref PredicateContext<T, U> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
		}
		else
		{
			context.needsMove = true;
		}
		while (context.chained.current.isSome && !context.predicate(context.chained.current.value))
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
		}
		next = context.chained.current;
	}

	private static void WhereRemove(ref PredicateContext<T, U> context, out Option<T> next)
	{
		context.needsMove = false;
		context.chained.remove(ref context.chained.context, out context.chained.current);
		WhereSkip(ref context, out next);
	}
}
public struct PredicateContext<T, U, V>
{
	private bool needsMove;

	private Slinq<T, U> chained;

	private readonly DelegateFunc<T, V, bool> predicate;

	private readonly V parameter;

	private BacktrackDetector bd;

	private static readonly Mutator<T, PredicateContext<T, U, V>> dispose = Dispose;

	private static readonly Mutator<T, PredicateContext<T, U, V>> takeWhileSkip = TakeWhileSkip;

	private static readonly Mutator<T, PredicateContext<T, U, V>> takeWhileRemove = TakeWhileRemove;

	private static readonly Mutator<T, PredicateContext<T, U, V>> whereSkip = WhereSkip;

	private static readonly Mutator<T, PredicateContext<T, U, V>> whereRemove = WhereRemove;

	private PredicateContext(Slinq<T, U> chained, DelegateFunc<T, V, bool> predicate, V parameter)
	{
		needsMove = false;
		this.chained = chained;
		this.predicate = predicate;
		this.parameter = parameter;
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, PredicateContext<T, U, V>> TakeWhile(Slinq<T, U> slinq, DelegateFunc<T, V, bool> predicate, V parameter)
	{
		return new Slinq<T, PredicateContext<T, U, V>>(takeWhileSkip, takeWhileRemove, dispose, new PredicateContext<T, U, V>(slinq, predicate, parameter));
	}

	public static Slinq<T, PredicateContext<T, U, V>> Where(Slinq<T, U> slinq, DelegateFunc<T, V, bool> predicate, V parameter)
	{
		return new Slinq<T, PredicateContext<T, U, V>>(whereSkip, whereRemove, dispose, new PredicateContext<T, U, V>(slinq, predicate, parameter));
	}

	private static void Dispose(ref PredicateContext<T, U, V> context, out Option<T> next)
	{
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
	}

	private static void TakeWhileSkip(ref PredicateContext<T, U, V> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
		}
		else
		{
			context.needsMove = true;
		}
		if (!context.chained.current.isSome)
		{
			next = context.chained.current;
			return;
		}
		if (context.predicate(context.chained.current.value, context.parameter))
		{
			next = context.chained.current;
			return;
		}
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
	}

	private static void TakeWhileRemove(ref PredicateContext<T, U, V> context, out Option<T> next)
	{
		context.needsMove = false;
		context.chained.remove(ref context.chained.context, out context.chained.current);
		TakeWhileSkip(ref context, out next);
	}

	private static void WhereSkip(ref PredicateContext<T, U, V> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
		}
		else
		{
			context.needsMove = true;
		}
		while (context.chained.current.isSome && !context.predicate(context.chained.current.value, context.parameter))
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
		}
		next = context.chained.current;
	}

	private static void WhereRemove(ref PredicateContext<T, U, V> context, out Option<T> next)
	{
		context.needsMove = false;
		context.chained.remove(ref context.chained.context, out context.chained.current);
		WhereSkip(ref context, out next);
	}
}
