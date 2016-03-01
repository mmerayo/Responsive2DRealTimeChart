using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace TestBitmap.WorkingVersion
{
	public class ChartSeries
	{
		public delegate void NewItemsAddedHandler(ChartSeries source, IEnumerable<ChartPoint> newItems);

		public readonly List<ChartPoint> Points = new List<ChartPoint>();

		public ChartSeries(string seriesId, string seriesName, Color seriesColor,
			ChartPositionCalculator positionCalculator)
		{
			SeriesId = seriesId;
			SeriesName = seriesName;
			SeriesColor = seriesColor;
			PositionCalculator = positionCalculator;
		}

		public ChartPositionCalculator PositionCalculator {get;}

		public Color SeriesColor { get; set; }

		
		public string SeriesId { get; }
		public string SeriesName { get; private set; }

		public void AddItems(IEnumerable<TimeChartPoint> newPoints)
		{
			var charPoints = new List<ChartPoint>(newPoints.Count());
			charPoints.AddRange(newPoints.Select(point => AddItem(point)));
			charPoints = charPoints.GroupBy(x => x.XPixel).Select(x => x.Last()).ToList();
			NewItemsAdded?.Invoke(this, charPoints);
		}

		public event NewItemsAddedHandler NewItemsAdded;

	

		public void Draw(WriteableBitmap writeableBmp, List<ChartPoint> chartPoints = null)
		{
			if (chartPoints == null)
				chartPoints = Points;
			if (chartPoints.Count < 2) return;
			Debug.WriteLine(chartPoints[0]);
			for (var i = 1; i < chartPoints.Count; i++)
			{
				var lastChartPoint = chartPoints[i - 1];
				var actual = chartPoints[i];
				Debug.WriteLine($"{this.SeriesId} - {actual}");
				writeableBmp.DrawLine(lastChartPoint.XPixel, lastChartPoint.YPixel, actual.XPixel, actual.YPixel, SeriesColor);
			}
		}

		private ChartPoint AddItem(TimeChartPoint point)
		{
			if (!Points.Any())
			{
				Points.Add(ChartPoint.InitialPoint(PositionCalculator, point.Time));
			}

			var p = new ChartPoint(PositionCalculator, point, Points.Any() ? Points.Last() : null);

			Points.Add(p);

			return p;
		}

		

		public class ChartPoint : IRepresentablePoint
		{
			private readonly int _millisecondsPerPixel;
			private ChartPositionCalculator _positionCalculator;

			public ChartPoint(ChartPositionCalculator positionCalculator,TimeChartPoint source, ChartPoint previousPoint):this(positionCalculator)
			{

				Value = source.Value;

				XPixel = previousPoint == null ? 0 : _positionCalculator.GetXPixelsDistance(previousPoint.XPixel, previousPoint.Time.DateTime,source.Time);
				Time = source.Time;
			}

			private ChartPoint(ChartPositionCalculator positionCalculator)
			{
				if (positionCalculator == null) throw new ArgumentNullException(nameof(positionCalculator));
				_positionCalculator = positionCalculator;
			}

			public double Value { get; private set; }

			public static ChartPoint InitialPoint(ChartPositionCalculator positionCalculator,  DateTime timeOffset)
			{
				var result = new ChartPoint(positionCalculator)
				{
					Value = 0.0,
					YPixel = 0,
					XPixel = 0,
					Time = timeOffset.Subtract(positionCalculator.RepresentableTime)
				};

				return result;
			}

			public int XPixel { get; set; }
			public int YPixel { get; set; }
			public DateTimeOffset Time { get; private set; }


			public override string ToString()
			{
				return $"Time: {Time.DateTime.ToString("HH:mm:ss.fff")}, XPixel: {XPixel}, YPixel: {YPixel}, Value: {Value}";
			}

		}
		
	}
}