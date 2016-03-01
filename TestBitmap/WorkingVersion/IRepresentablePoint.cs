namespace TestBitmap.WorkingVersion
{
	public interface IRepresentablePoint
	{
		int XPixel { get; set; }
		int YPixel { get; set; }

		double Value { get; }
	}
}