public struct HsvColor
{
	public double double_0;

	public double double_1;

	public double double_2;

	public float normalizedH
	{
		get
		{
			return (float)double_0 / 360f;
		}
		set
		{
			double_0 = (double)value * 360.0;
		}
	}

	public float normalizedS
	{
		get
		{
			return (float)double_1;
		}
		set
		{
			double_1 = value;
		}
	}

	public float normalizedV
	{
		get
		{
			return (float)double_2;
		}
		set
		{
			double_2 = value;
		}
	}

	public HsvColor(double h, double s, double v)
	{
		double_0 = h;
		double_1 = s;
		double_2 = v;
	}

	public override string ToString()
	{
		return "{" + double_0.ToString("f2") + "," + double_1.ToString("f2") + "," + double_2.ToString("f2") + "}";
	}
}
