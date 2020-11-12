using System;

namespace Melody.SpectrumAnalyzer
{
	/// <summary>
	/// Description of SpectrumLine.
	/// </summary>
	public class SpectrumLine
	{
		public readonly FreqPoint[] Spectrum;
		public readonly double Time;
		
		public SpectrumLine(FreqPoint[] spec, double time)
		{
			Spectrum = spec;
			Time = time;
		}
	}
}
