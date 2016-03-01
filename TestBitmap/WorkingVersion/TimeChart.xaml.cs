using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TestBitmap.WorkingVersion
{
	public partial class TimeChart : UserControl
	{
		private WriteableBitmap _writeableBmp;

		public TimeSpan RepresentableTime { get; set; } = TimeSpan.FromSeconds(30);

		public TimeChart()
		{
			InitializeComponent();
			DataContext = this;
			this.SizeChanged += OnSizeChanged;

		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			Reset();
		}
		
		public DateTime LastInvalidationTime { get; } = DateTime.MinValue;

		private void NewItem_NewItemsAdded(ChartSeries source, IEnumerable<ChartSeries.ChartPoint> itemsAdded)
		{
			
			_positionCalculator.AdjustToViewPort(Series);

			using (_writeableBmp.GetBitmapContext())
			{
				_writeableBmp.Clear(BackgroundColor);
				DrawXReference();
				Series.ForEach(x => x.Draw(_writeableBmp));
			}
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			"Title",
			typeof (string),
			typeof (TimeChart), null
			);

		public string Title
		{
			get { return (string) GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		private void DrawXReference()
		{
			var max= Series.Max(x => x.Points.Any() ? x.Points.Max(y => y.Value) : 0);
			var min = Series.Min(x => x.Points.Any() ? x.Points.Min(y => y.Value) : 0);

			var highVal = max-(max-min)/4;
			txtHighHalf.Text = string.Format("{0:0.00}", highVal);
			var hMaxY =(int) _positionCalculator.GetScaledY(highVal, max, min);
			_writeableBmp.DrawLine(0, hMaxY, _positionCalculator.ViewPortWidth, hMaxY, Colors.DarkGray);

			var lowVal = min + (max - min) / 4;
			txtLowHalf.Text = string.Format("{0:0.00}", lowVal);
			var hMinY = (int)_positionCalculator.GetScaledY(lowVal, max, min);
			_writeableBmp.DrawLine(0, hMinY, _positionCalculator.ViewPortWidth, hMinY, Colors.DarkGray);

		}


		public Color BackgroundColor { get; set; }

		private void Reset()
		{
			_positionCalculator.ViewPortWidth = (int) viewPortContainer.ActualWidth;
			_positionCalculator.ViewPortHeight = (int)viewPortContainer.ActualHeight;
			_positionCalculator.RepresentableTime = RepresentableTime;


			_writeableBmp = BitmapFactory.New(_positionCalculator.ViewPortWidth, _positionCalculator.ViewPortHeight);
			viewPort.Source = _writeableBmp;
			BackgroundColor = Colors.White;
			CreateSeries();
		}

		private readonly ChartPositionCalculator _positionCalculator=new ChartPositionCalculator();

		private readonly List<ChartSeries> Series = new List<ChartSeries>();
		private List<SeriesMetadata> _seriesMetadatas = new List<SeriesMetadata>();

		public void AddSeries(SeriesMetadata definition)
		{
			_seriesMetadatas.Add(definition);
		}

		private void CreateSeries()
		{
			Series.Clear();
			while (seriesStackPanel.Children.Count > 1)
				seriesStackPanel.Children.RemoveAt(1);


			foreach (var definition in _seriesMetadatas)
			{
				var newItem = new ChartSeries(definition.SeriesId, definition.SeriesLegend, definition.SeriesColor, _positionCalculator);
				newItem.NewItemsAdded += NewItem_NewItemsAdded;
				Series.Add(newItem);

				seriesStackPanel.Children.Add(new TextBlock
				{
					Text = definition.SeriesLegend,
					Foreground = new SolidColorBrush {Color = definition.SeriesColor},
					HorizontalAlignment = HorizontalAlignment.Center
				});
			}

		}


		public ChartSeries GetSeriesById(string seriesId)
		{
			return Series.SingleOrDefault(x => x.SeriesId == seriesId);
		}

		public void Init()
		{
			Reset();
		}
	}
}