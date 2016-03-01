using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestBitmap.WorkingVersion;

namespace TestBitmap.Generation
{
	public class AlternateSignsChartPointsGenerator : CharPointsGenerator
	{
		private readonly TimeChart _target;
		private readonly string _seriesName;

		private static readonly Random _random = new Random();
		int _sign = 1;
		private int _maxIntPart = 0;
		private bool sum = true;


		public AlternateSignsChartPointsGenerator(TimeChart target, string seriesName)
		{
			_target = target;
			_seriesName = seriesName;
		}

		public override void GeneratePoints(int numberPoints)
		{
			var newItems = new List<TimeChartPoint>();

			while (numberPoints-- > 0)
			{
				var value = (_random.Next(0, _maxIntPart) + _random.NextDouble())*_sign;
				newItems.Add(new TimeChartPoint(DateTime.UtcNow, value));
				_sign *= -1;
				Task.Delay(1).Wait();
			}
			_target.GetSeriesById(_seriesName).AddItems(newItems);

			if (_maxIntPart > 120) sum = false;
			else if (_maxIntPart == 1 & !sum) sum = true;
			if (!sum)
				_maxIntPart--;
			else
				_maxIntPart++;
		}
	}
}