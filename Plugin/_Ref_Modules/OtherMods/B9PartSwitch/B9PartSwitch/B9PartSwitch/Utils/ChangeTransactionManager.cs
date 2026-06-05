using System;

namespace B9PartSwitch.Utils;

public class ChangeTransactionManager
{
	private enum TransactionState
	{
		PreInitialize,
		OutsideTransactionNoChangeNeeded,
		InTransactionNoChangeNeeded,
		InTransactionChangeNeeded,
		AfterTransactionChanging
	}

	private TransactionState state;

	private readonly Action change;

	public ChangeTransactionManager(Action change)
	{
		this.change = change ?? throw new ArgumentNullException("change");
	}

	public void Initialize()
	{
		if (state == TransactionState.PreInitialize)
		{
			state = TransactionState.OutsideTransactionNoChangeNeeded;
		}
	}

	public void RequestChange()
	{
		if (state != 0)
		{
			if (state == TransactionState.InTransactionNoChangeNeeded)
			{
				state = TransactionState.InTransactionChangeNeeded;
			}
			else if (state == TransactionState.OutsideTransactionNoChangeNeeded)
			{
				change();
			}
			else if (state == TransactionState.AfterTransactionChanging)
			{
				throw new InvalidOperationException("Circular change condition detected");
			}
		}
	}

	public void WithTransaction(Action action)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		if (state == TransactionState.OutsideTransactionNoChangeNeeded || state == TransactionState.PreInitialize)
		{
			state = TransactionState.InTransactionNoChangeNeeded;
		}
		try
		{
			action();
			if (state == TransactionState.InTransactionChangeNeeded)
			{
				state = TransactionState.AfterTransactionChanging;
				change();
			}
		}
		finally
		{
			state = TransactionState.OutsideTransactionNoChangeNeeded;
		}
	}
}
