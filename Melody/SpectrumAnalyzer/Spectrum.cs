using System;

namespace Melody.SpectrumAnalyzer
{
	/// <summary>
	/// Description of Spectrum.
	/// </summary>
	public class Spectrum
	{
		public SpectrumLine[] Specs {get; set;}

		
		public Spectrum(SpectrumLine spec) : this(new SpectrumLine[] { spec }) {}
		
		public Spectrum(SpectrumLine[] specs)
		{
			Specs = specs;
		}
	}
}
