using System;

namespace InterstellarFuelSwitch;

internal class InterstellarDynamicResourceBuffer : PartModule
{
	[KSPField]
	public string resourceName;

	[KSPField]
	public double bufferSize;

	public void FixedUpdate()
	{
		if (!(bufferSize <= 0.0) && !string.IsNullOrEmpty(resourceName))
		{
			PartResource partResource = base.part.Resources[resourceName];
			if (partResource != null && partResource.maxAmount > 0.0)
			{
				double num = Math.Min(1.0, partResource.amount / partResource.maxAmount);
				double num2 = bufferSize * (double)(decimal)TimeWarp.fixedDeltaTime * 50.0;
				partResource.maxAmount = (IsValidNumber(num2) ? num2 : partResource.maxAmount);
				double num3 = partResource.maxAmount * num;
				partResource.amount = (IsValidNumber(num3) ? num3 : partResource.amount);
			}
		}
	}

	private bool IsValidNumber(double number)
	{
		if (!double.IsNaN(number))
		{
			return !double.IsInfinity(number);
		}
		return false;
	}
}
