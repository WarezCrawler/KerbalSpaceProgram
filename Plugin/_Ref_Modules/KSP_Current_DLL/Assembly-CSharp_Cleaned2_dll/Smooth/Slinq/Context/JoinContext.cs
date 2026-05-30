using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Slinq.Collections;

namespace Smooth.Slinq.Context;

public struct JoinContext<T, U, T2, V, W>
{
	private bool needsMove;

	private readonly Lookup<U, T2> lookup;

	private readonly DelegateFunc<V, U> outerSelector;

	private readonly DelegateFunc<V, T2, T> resultSelector;

	private readonly bool release;

	private Slinq<V, W> chained;

	private Linked<T2> inner;

	private BacktrackDetector bd;

	private static readonly Mutator<T, JoinContext<T, U, T2, V, W>> skip = Skip;

	private static readonly Mutator<T, JoinContext<T, U, T2, V, W>> remove = Remove;

	private static readonly Mutator<T, JoinContext<T, U, T2, V, W>> dispose = Dispose;

	private JoinContext(Lookup<U, T2> lookup, Slinq<V, W> outer, DelegateFunc<V, U> outerSelector, DelegateFunc<V, T2, T> resultSelector, bool release)
	{
		needsMove = false;
		this.lookup = lookup;
		this.outerSelector = outerSelector;
		this.resultSelector = resultSelector;
		chained = outer;
		this.release = release;
		inner = ((!chained.current.isSome || !lookup.dictionary.TryGetValue(outerSelector(chained.current.value), out var value)) ? null : value.head);
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, JoinContext<T, U, T2, V, W>> Join(Lookup<U, T2> lookup, Slinq<V, W> outer, DelegateFunc<V, U> outerSelector, DelegateFunc<V, T2, T> resultSelector, bool release)
	{
		return new Slinq<T, JoinContext<T, U, T2, V, W>>(skip, remove, dispose, new JoinContext<T, U, T2, V, W>(lookup, outer, outerSelector, resultSelector, release));
	}

	private static void Skip(ref JoinContext<T, U, T2, V, W> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.inner = context.inner.next;
		}
		else
		{
			context.needsMove = true;
		}
		if (context.inner == null && context.chained.current.isSome)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
			while (context.chained.current.isSome)
			{
				context.inner = context.lookup.GetValues(context.outerSelector(context.chained.current.value)).head;
				if (context.inner != null)
				{
					break;
				}
				context.chained.skip(ref context.chained.context, out context.chained.current);
			}
		}
		if (context.chained.current.isSome)
		{
			next = new Option<T>(context.resultSelector(context.chained.current.value, context.inner.value));
			return;
		}
		next = default(Option<T>);
		if (context.release)
		{
			context.lookup.DisposeInBackground();
		}
	}

	private static void Remove(ref JoinContext<T, U, T2, V, W> context, out Option<T> next)
	{
		context.needsMove = false;
		context.chained.remove(ref context.chained.context, out context.chained.current);
		context.inner = (context.chained.current.isSome ? context.lookup.GetValues(context.outerSelector(context.chained.current.value)).head : null);
		Skip(ref context, out next);
	}

	private static void Dispose(ref JoinContext<T, U, T2, V, W> context, out Option<T> next)
	{
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
		if (context.release)
		{
			context.lookup.DisposeInBackground();
		}
	}
}
public struct JoinContext<T, U, T2, V, W, X>
{
	private bool needsMove;

	private readonly Lookup<U, T2> lookup;

	private readonly DelegateFunc<V, X, U> outerSelector;

	private readonly DelegateFunc<V, T2, X, T> resultSelector;

	private readonly X parameter;

	private readonly bool release;

	private Slinq<V, W> chained;

	private Linked<T2> inner;

	private BacktrackDetector bd;

	private static readonly Mutator<T, JoinContext<T, U, T2, V, W, X>> skip = Skip;

	private static readonly Mutator<T, JoinContext<T, U, T2, V, W, X>> remove = Remove;

	private static readonly Mutator<T, JoinContext<T, U, T2, V, W, X>> dispose = Dispose;

	private JoinContext(Lookup<U, T2> lookup, Slinq<V, W> outer, DelegateFunc<V, X, U> outerSelector, DelegateFunc<V, T2, X, T> resultSelector, X parameter, bool release)
	{
		needsMove = false;
		this.lookup = lookup;
		this.outerSelector = outerSelector;
		this.resultSelector = resultSelector;
		this.parameter = parameter;
		chained = outer;
		this.release = release;
		inner = ((!chained.current.isSome || !lookup.dictionary.TryGetValue(outerSelector(chained.current.value, parameter), out var value)) ? null : value.head);
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, JoinContext<T, U, T2, V, W, X>> Join(Lookup<U, T2> lookup, Slinq<V, W> outer, DelegateFunc<V, X, U> outerSelector, DelegateFunc<V, T2, X, T> resultSelector, X parameter, bool release)
	{
		return new Slinq<T, JoinContext<T, U, T2, V, W, X>>(skip, remove, dispose, new JoinContext<T, U, T2, V, W, X>(lookup, outer, outerSelector, resultSelector, parameter, release));
	}

	private static void Skip(ref JoinContext<T, U, T2, V, W, X> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.inner = context.inner.next;
		}
		else
		{
			context.needsMove = true;
		}
		if (context.inner == null && context.chained.current.isSome)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
			while (context.chained.current.isSome)
			{
				context.inner = context.lookup.GetValues(context.outerSelector(context.chained.current.value, context.parameter)).head;
				if (context.inner != null)
				{
					break;
				}
				context.chained.skip(ref context.chained.context, out context.chained.current);
			}
		}
		if (context.chained.current.isSome)
		{
			next = new Option<T>(context.resultSelector(context.chained.current.value, context.inner.value, context.parameter));
			return;
		}
		next = default(Option<T>);
		if (context.release)
		{
			context.lookup.DisposeInBackground();
		}
	}

	private static void Remove(ref JoinContext<T, U, T2, V, W, X> context, out Option<T> next)
	{
		context.needsMove = false;
		context.chained.remove(ref context.chained.context, out context.chained.current);
		context.inner = (context.chained.current.isSome ? context.lookup.GetValues(context.outerSelector(context.chained.current.value, context.parameter)).head : null);
		Skip(ref context, out next);
	}

	private static void Dispose(ref JoinContext<T, U, T2, V, W, X> context, out Option<T> next)
	{
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
		if (context.release)
		{
			context.lookup.DisposeInBackground();
		}
	}
}
