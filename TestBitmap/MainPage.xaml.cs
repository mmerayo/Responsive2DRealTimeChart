using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;
using TestBitmap.Generation;
using TestBitmap.WorkingVersion;
using TimeChart = TestBitmap.WorkingVersion.TimeChart;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestBitmap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
	    private const string ASeriesName = "Serie A";
		private const string BSeriesName = "Serie B";
		private const string CSeriesName = "Serie C";
		private readonly Dictionary<string, IChartPointsGenerator> _generators=new Dictionary<string, IChartPointsGenerator>();
	    public MainPage()
        {
            this.InitializeComponent();
	       
			Loaded += MainPage_Loaded;
        }
		private  DispatcherTimer _timer1;
		private DispatcherTimer _timer2;
		//private DispatcherTimer _timer3;
		//private DispatcherTimer _timer4;
		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			_timer1=InitializeChart(chart1,GeneratorType.AlternateSignsGenerator);
			_timer2 = InitializeChart(chart2, GeneratorType.AlmostContinuousGenerator);
			//_timer3 = InitializeChart(chart3);
			//_timer4 = InitializeChart(chart4);
			
		}

	    private DispatcherTimer InitializeChart(TimeChart chart, GeneratorType generatorType)
	    {
		    AddTestSeries(generatorType,chart, ASeriesName, Colors.Red);
			AddTestSeries(generatorType, chart, BSeriesName, Colors.Green);
			AddTestSeries(generatorType, chart, CSeriesName, Colors.Blue);
			chart.Init();
			var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
		    DispatcherTimer dispatcherTimer = timer;
		    dispatcherTimer.Tick +=(object sender, object e)=> this._timer_Tick(chart, dispatcherTimer);
			timer.Start();
		    return timer;
	    }

	    private void AddTestSeries(GeneratorType generatorType, TimeChart chart, string seriesName, Color seriesColor)
	    {
		    var seriesMetadata = new SeriesMetadata { SeriesId =seriesName, SeriesLegend = seriesName, SeriesColor = seriesColor};
		    chart.AddSeries(seriesMetadata);
		    _generators.Add(GetGeneratorName(chart,seriesName), CharPointsGenerator.Get(generatorType, chart,seriesName));
	    }

	    private string GetGeneratorName(TimeChart chart, string seriesName)
	    {
		    return $"{chart.Name}{seriesName}";
	    }
		
		private void _timer_Tick(TimeChart chart, DispatcherTimer timer)
		{
			timer.Stop();
			try
			{
				GeneratePoints(chart);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				timer.Start();
			}
		}

	    private void GeneratePoints(TimeChart timeChart)
	    {
		    GenerateSeriesPoints(timeChart,ASeriesName);
			GenerateSeriesPoints(timeChart, BSeriesName);
			GenerateSeriesPoints(timeChart, CSeriesName);
		}

		private void GenerateSeriesPoints(TimeChart timeChart, string seriesName)
	    {
		    _generators[GetGeneratorName(timeChart, seriesName)].GeneratePoints(3);
	    }


	    

	    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
	    {
		    Text.Text = (int.Parse(Text.Text) + 1).ToString();

	    }
    }
}
