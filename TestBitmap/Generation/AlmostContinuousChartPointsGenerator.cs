using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestBitmap.WorkingVersion;

namespace TestBitmap.Generation
{
	public class AlmostContinuousChartPointsGenerator : CharPointsGenerator
	{
		private readonly TimeChart _target;
		private readonly string _seriesName;

		private static readonly Random _random = new Random();
		private readonly int _aroundValue=20;
		public AlmostContinuousChartPointsGenerator(TimeChart target, string seriesName)
		{
			_target = target;
			_seriesName = seriesName;
		}
		public override void GeneratePoints(int numberPoints)
		{
			var newItems = new List<TimeChartPoint>();

			while (numberPoints-- > 0)
			{
				var value = _aroundValue+ _random.NextDouble();
				newItems.Add(new TimeChartPoint(DateTime.UtcNow, value));
				Task.Delay(1).Wait();
			}
			_target.GetSeriesById(_seriesName).AddItems(newItems);
		}
	}
}