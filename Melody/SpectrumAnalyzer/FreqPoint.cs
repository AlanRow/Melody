using System;
using System.Numerics;

namespace Melody.SpectrumAnalyzer
{
	/// <summary>
	/// Description of FreqPoint.
	/// </summary>
	public class FreqPoint
	{
		public readonly Complex Coords;
		public readonly double Freq;
		
		public FreqPoint(Complex coords, double hzFreq)
		{
			Coords = coords;
			Freq = hzFreq;
		}
	}
}
