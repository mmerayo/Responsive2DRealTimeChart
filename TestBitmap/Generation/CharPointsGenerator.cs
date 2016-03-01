using System;
using TestBitmap.WorkingVersion;

namespace TestBitmap.Generation
{
	public abstract class  CharPointsGenerator: IChartPointsGenerator
	{
		public static IChartPointsGenerator Get(GeneratorType generatorType, TimeChart target, string seriesName)
		{
			switch (generatorType)
			{
				case GeneratorType.AlternateSignsGenerator:
				return new AlternateSignsChartPointsGenerator(target,seriesName);
				case GeneratorType.AlmostContinuousGenerator:
					return new AlmostContinuousChartPointsGenerator(target, seriesName);
				default:
					throw new ArgumentOutOfRangeException(nameof(generatorType), generatorType, null);
			}
		}

		public abstract void GeneratePoints(int numberPoints);
	}
}