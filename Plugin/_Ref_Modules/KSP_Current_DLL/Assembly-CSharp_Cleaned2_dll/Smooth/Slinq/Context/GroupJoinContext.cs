using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Slinq.Collections;

namespace Smooth.Slinq.Context;

public struct GroupJoinContext<T, U, T2, V, W>
{
	private bool needsMove;

	private readonly Lookup<U, T2> lookup;

	private readonly DelegateFunc<V, U> outerSelector;

	private readonly DelegateFunc<V, Slinq<T2, LinkedContext<T2>>, T> resultSelector;

	private readonly bool release;

	private Slinq<V, W> chained;

	private BacktrackDetector bd;

	private static readonly Mutator<T, GroupJoinContext<T, U, T2, V, W>> skip = Skip;

	private static readonly Mutator<T, GroupJoinContext<T, U, T2, V, W>> remove = Remove;

	private static readonly Mutator<T, GroupJoinContext<T, U, T2, V, W>> dispose = Dispose;

	private GroupJoinContext(Lookup<U, T2> lookup, Slinq<V, W> outer, DelegateFunc<V, U> outerSelector, DelegateFunc<V, Slinq<T2, LinkedContext<T2>>, T> resultSelector, bool release)
	{
		needsMove = false;
		this.lookup = lookup;
		this.outerSelector = outerSelector;
		this.resultSelector = resultSelector;
		chained = outer;
		this.release = release;
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, GroupJoinContext<T, U, T2, V, W>> GroupJoin(Lookup<U, T2> lookup, Slinq<V, W> outer, DelegateFunc<V, U> outerSelector, DelegateFunc<V, Slinq<T2, LinkedContext<T2>>, T> resultSelector, bool release)
	{
		return new Slinq<T, GroupJoinContext<T, U, T2, V, W>>(skip, remove, dispose, new GroupJoinContext<T, U, T2, V, W>(lookup, outer, outerSelector, resultSelector, release));
	}

	private static void Skip(ref GroupJoinContext<T, U, T2, V, W> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
		}
		else
		{
			context.needsMove = true;
		}
		if (context.chained.current.isSome)
		{
			BacktrackDetector backtrackDetector = BacktrackDetector.Borrow();
			next = new Option<T>(context.resultSelector(context.chained.current.value, context.lookup.GetValues(context.outerSelector(context.chained.current.value)).SlinqAndKeep(backtrackDetector)));
			return;
		}
		next = default(Option<T>);
		if (context.release)
		{
			context.lookup.DisposeInBackground();
		}
	}

	private static void Remove(ref GroupJoinContext<T, U, T2, V, W> context, out Option<T> next)
	{
		context.needsMove = false;
		context.chained.remove(ref context.chained.context, out context.chained.current);
		Skip(ref context, out next);
	}

	private static void Dispose(ref GroupJoinContext<T, U, T2, V, W> context, out Option<T> next)
	{
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
		if (context.release)
		{
			context.lookup.DisposeInBackground();
		}
	}
}
public struct GroupJoinContext<T, U, T2, V, W, X>
{
	private bool needsMove;

	private readonly Lookup<U, T2> lookup;

	private readonly DelegateFunc<V, X, U> outerSelector;

	private readonly DelegateFunc<V, Slinq<T2, LinkedContext<T2>>, X, T> resultSelector;

	private readonly X parameter;

	private readonly bool release;

	private Slinq<V, W> chained;

	private BacktrackDetector bd;

	private static readonly Mutator<T, GroupJoinContext<T, U, T2, V, W, X>> skip = Skip;

	private static readonly Mutator<T, GroupJoinContext<T, U, T2, V, W, X>> remove = Remove;

	private static readonly Mutator<T, GroupJoinContext<T, U, T2, V, W, X>> dispose = Dispose;

	private GroupJoinContext(Lookup<U, T2> lookup, Slinq<V, W> outer, DelegateFunc<V, X, U> outerSelector, DelegateFunc<V, Slinq<T2, LinkedContext<T2>>, X, T> resultSelector, X parameter, bool release)
	{
		needsMove = false;
		this.lookup = lookup;
		this.outerSelector = outerSelector;
		this.resultSelector = resultSelector;
		this.parameter = parameter;
		chained = outer;
		this.release = release;
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, GroupJoinContext<T, U, T2, V, W, X>> GroupJoin(Lookup<U, T2> lookup, Slinq<V, W> outer, DelegateFunc<V, X, U> outerSelector, DelegateFunc<V, Slinq<T2, LinkedContext<T2>>, X, T> resultSelector, X parameter, bool release)
	{
		return new Slinq<T, GroupJoinContext<T, U, T2, V, W, X>>(skip, remove, dispose, new GroupJoinContext<T, U, T2, V, W, X>(lookup, outer, outerSelector, resultSelector, parameter, release));
	}

	private static void Skip(ref GroupJoinContext<T, U, T2, V, W, X> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
		}
		else
		{
			context.needsMove = true;
		}
		if (context.chained.current.isSome)
		{
			BacktrackDetector backtrackDetector = BacktrackDetector.Borrow();
			next = new Option<T>(context.resultSelector(context.chained.current.value, context.lookup.GetValues(context.outerSelector(context.chained.current.value, context.parameter)).SlinqAndKeep(backtrackDetector), context.parameter));
			return;
		}
		next = default(Option<T>);
		if (context.release)
		{
			context.lookup.DisposeInBackground();
		}
	}

	private static void Remove(ref GroupJoinContext<T, U, T2, V, W, X> context, out Option<T> next)
	{
		context.needsMove = false;
		context.chained.remove(ref context.chained.context, out context.chained.current);
		Skip(ref context, out next);
	}

	private static void Dispose(ref GroupJoinContext<T, U, T2, V, W, X> context, out Option<T> next)
	{
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
		if (context.release)
		{
			context.lookup.DisposeInBackground();
		}
	}
}
