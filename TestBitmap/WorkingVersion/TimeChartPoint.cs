using System;

namespace TestBitmap.WorkingVersion
{
	public class TimeChartPoint
	{
		public TimeChartPoint(DateTime time, double value)
		{
			Time = time;
			Value = value;
		}

		public DateTime Time { get;}
		public double Value { get; }
	}
}