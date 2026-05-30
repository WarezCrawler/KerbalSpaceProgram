using System;
using Smooth.Algebraics;
using Smooth.Delegates;

namespace Smooth.Slinq.Context;

public struct FuncContext<T>
{
	private bool needsMove;

	private T acc;

	private readonly DelegateFunc<T, T> selector;

	private BacktrackDetector bd;

	private static readonly Mutator<T, FuncContext<T>> skip = Skip;

	private static readonly Mutator<T, FuncContext<T>> remove = skip;

	private static readonly Mutator<T, FuncContext<T>> dispose = Dispose;

	private FuncContext(T seed, DelegateFunc<T, T> selector)
	{
		needsMove = false;
		acc = seed;
		this.selector = selector;
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, FuncContext<T>> Sequence(T seed, DelegateFunc<T, T> selector)
	{
		return new Slinq<T, FuncContext<T>>(skip, remove, dispose, new FuncContext<T>(seed, selector));
	}

	private static void Skip(ref FuncContext<T> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.acc = context.selector(context.acc);
		}
		else
		{
			context.needsMove = true;
		}
		next = new Option<T>(context.acc);
	}

	private static void Remove(ref FuncContext<T> context, out Option<T> next)
	{
		throw new NotSupportedException();
	}

	private static void Dispose(ref FuncContext<T> context, out Option<T> next)
	{
		next = default(Option<T>);
	}
}
public struct FuncContext<T, U>
{
	private bool needsMove;

	private T acc;

	private readonly DelegateFunc<T, U, T> selector;

	private readonly U parameter;

	private BacktrackDetector bd;

	private static readonly Mutator<T, FuncContext<T, U>> skip = Skip;

	private static readonly Mutator<T, FuncContext<T, U>> remove = skip;

	private static readonly Mutator<T, FuncContext<T, U>> dispose = Dispose;

	private FuncContext(T seed, DelegateFunc<T, U, T> selector, U parameter)
	{
		needsMove = false;
		acc = seed;
		this.selector = selector;
		this.parameter = parameter;
		bd = BacktrackDetector.Borrow();
	}

	public static Slinq<T, FuncContext<T, U>> Sequence(T seed, DelegateFunc<T, U, T> selector, U parameter)
	{
		return new Slinq<T, FuncContext<T, U>>(skip, remove, dispose, new FuncContext<T, U>(seed, selector, parameter));
	}

	private static void Skip(ref FuncContext<T, U> context, out Option<T> next)
	{
		if (context.needsMove)
		{
			context.acc = context.selector(context.acc, context.parameter);
		}
		else
		{
			context.needsMove = true;
		}
		next = new Option<T>(context.acc);
	}

	private static void Remove(ref FuncContext<T, U> context, out Option<T> next)
	{
		throw new NotSupportedException();
	}

	private static void Dispose(ref FuncContext<T, U> context, out Option<T> next)
	{
		next = default(Option<T>);
	}
}
