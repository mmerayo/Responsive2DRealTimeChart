using System;
using System.Collections.Generic;
using System.Linq;

namespace TestBitmap.WorkingVersion
{
	public class ChartPositionCalculator
	{
		private int _viewPortHeight;

		public int XZero { get; set; } = 0;
		public int YZero { get; private set; } = 0;
	
		public void AdjustToViewPort(List<ChartSeries> allSeries)
		{
			var maxYValue = allSeries.Select(s => s.Points.Any() ? s.Points.Max(x => x.Value) : 0).Max();
			var minYValue = allSeries.Select(s => s.Points.Any() ? s.Points.Min(x => x.Value) : 0).Min();
			foreach (var serie in allSeries)
			{
				if (!serie.Points.Any()) return;
				var lastPoint = serie.Points.Last();
				var xOffset = lastPoint.XPixel > ViewPortWidth ? lastPoint.XPixel - ViewPortWidth : 0;
				
				serie.Points.ForEach(x =>
				{
					ApplyXOffset(x,-xOffset);
					x.YPixel=(int)GetScaledY(x.Value, maxYValue, minYValue);
				});
				serie.Points.RemoveAll(x => x.XPixel < XZero);
				var nonRepeateditems = serie.Points.GroupBy(x => x.XPixel).Select(x => x.Last()).ToArray();
				serie.Points.Clear();
				serie.Points.AddRange(nonRepeateditems);
			}
		}

		public double GetScaledY(double value, double maxYValue,double minYValue)
		{
			var result = (maxYValue - value)*ViewPortHeight/(maxYValue - minYValue);
			
			return result;
		}


		private void ApplyXOffset(IRepresentablePoint point, int xOffset)
		{
			point.XPixel = point.XPixel + xOffset;
		}

		public int ViewPortWidth { get; set; }

		public int ViewPortHeight
		{
			get { return _viewPortHeight; }
			set
			{
				_viewPortHeight = value;
				YZero = _viewPortHeight/2;
			}
		}

		private int MillisecondsPerPixel => (int)Math.Round(RepresentableTime.TotalMilliseconds / ViewPortWidth);

		public TimeSpan RepresentableTime { get; set; }

		public int GetXPixelsDistance(int xPixelFrom, DateTime timeAtX, DateTime timeAtDestination)
		{
			var millisecondsOffset = timeAtDestination.Subtract(timeAtX).TotalMilliseconds;

			return (int)(xPixelFrom + millisecondsOffset / MillisecondsPerPixel);
		}

	}
}