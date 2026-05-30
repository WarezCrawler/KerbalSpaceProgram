using Smooth.Algebraics;
using Smooth.Delegates;

namespace Smooth.Slinq.Context;

public struct SelectSlinqContext<T, U, V, W>
{
	private bool needsMove;

	private Slinq<V, W> chained;

	private Slinq<T, U> selected;

	private readonly DelegateFunc<V, Slinq<T, U>> selector;

	private BacktrackDetector bd;

	private static readonly Mutator<T, SelectSlinqContext<T, U, V, W>> skip = Skip;

	private static readonly Mutator<T, SelectSlinqContext<T, U, V, W>> remove = Remove;

	private static readonly Mutator<T, SelectSlinqContext<T, U, V, W>> dispose = Dispose;

	private SelectSlinqContext(Slinq<V, W> chained, DelegateFunc<V, Slinq<T, U>> selector)
	{
		needsMove = false;
		this.chained = chained;
		selected = (chained.current.isSome ? selector(chained.current.value) : default(Slinq<T, U>));
		this.selector = selector;
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, SelectSlinqContext<T, U, V, W>> SelectMany(Slinq<V, W> slinq, DelegateFunc<V, Slinq<T, U>> selector)
	{
		return new Slinq<T, SelectSlinqContext<T, U, V, W>>(skip, remove, dispose, new SelectSlinqContext<T, U, V, W>(slinq, selector));
	}

	private static void Skip(ref SelectSlinqContext<T, U, V, W> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.selected.skip(ref context.selected.context, out context.selected.current);
		}
		else
		{
			context.needsMove = true;
		}
		if (!context.selected.current.isSome && context.chained.current.isSome)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
			while (context.chained.current.isSome)
			{
				context.selected = context.selector(context.chained.current.value);
				if (context.selected.current.isSome)
				{
					break;
				}
				context.chained.skip(ref context.chained.context, out context.chained.current);
			}
		}
		next = context.selected.current;
	}

	private static void Remove(ref SelectSlinqContext<T, U, V, W> context, out Option<T> next)
	{
		context.needsMove = false;
		context.selected.remove(ref context.selected.context, out context.selected.current);
		Skip(ref context, out next);
	}

	private static void Dispose(ref SelectSlinqContext<T, U, V, W> context, out Option<T> next)
	{
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
		context.selected.dispose(ref context.selected.context, out context.selected.current);
	}
}
public struct SelectSlinqContext<T, U, V, W, X>
{
	private bool needsMove;

	private Slinq<V, W> chained;

	private Slinq<T, U> selected;

	private readonly DelegateFunc<V, X, Slinq<T, U>> selector;

	private readonly X parameter;

	private BacktrackDetector bd;

	private static readonly Mutator<T, SelectSlinqContext<T, U, V, W, X>> skip = Skip;

	private static readonly Mutator<T, SelectSlinqContext<T, U, V, W, X>> remove = Remove;

	private static readonly Mutator<T, SelectSlinqContext<T, U, V, W, X>> dispose = Dispose;

	private SelectSlinqContext(Slinq<V, W> chained, DelegateFunc<V, X, Slinq<T, U>> selector, X parameter)
	{
		needsMove = false;
		this.chained = chained;
		selected = (chained.current.isSome ? selector(chained.current.value, parameter) : default(Slinq<T, U>));
		this.selector = selector;
		this.parameter = parameter;
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, SelectSlinqContext<T, U, V, W, X>> SelectMany(Slinq<V, W> slinq, DelegateFunc<V, X, Slinq<T, U>> selector, X parameter)
	{
		return new Slinq<T, SelectSlinqContext<T, U, V, W, X>>(skip, remove, dispose, new SelectSlinqContext<T, U, V, W, X>(slinq, selector, parameter));
	}

	private static void Skip(ref SelectSlinqContext<T, U, V, W, X> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.selected.skip(ref context.selected.context, out context.selected.current);
		}
		else
		{
			context.needsMove = true;
		}
		if (!context.selected.current.isSome && context.chained.current.isSome)
		{
			context.chained.skip(ref context.chained.context, out context.chained.current);
			while (context.chained.current.isSome)
			{
				context.selected = context.selector(context.chained.current.value, context.parameter);
				if (context.selected.current.isSome)
				{
					break;
				}
				context.chained.skip(ref context.chained.context, out context.chained.current);
			}
		}
		next = context.selected.current;
	}

	private static void Remove(ref SelectSlinqContext<T, U, V, W, X> context, out Option<T> next)
	{
		context.needsMove = false;
		context.selected.remove(ref context.selected.context, out context.selected.current);
		Skip(ref context, out next);
	}

	private static void Dispose(ref SelectSlinqContext<T, U, V, W, X> context, out Option<T> next)
	{
		next = default(Option<T>);
		context.chained.dispose(ref context.chained.context, out context.chained.current);
		context.selected.dispose(ref context.selected.context, out context.selected.current);
	}
}
