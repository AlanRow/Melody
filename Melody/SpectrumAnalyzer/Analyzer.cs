/*
 * Created by SharpDevelop.
 * User: Kotya
 * Date: 08/30/2020
 * Time: 10:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Numerics;

namespace Melody.SpectrumAnalyzer
{
	/// <summary>
	/// Main class for transforming signal actions
	/// </summary>
	public class Analyzer
	{
		private FFTTransformer transformer;
		
		public Analyzer()
		{
			transformer = new FFTTransformer();
		}

		public Complex[][] GetSpectrum(ISignal signal)
		{
			return new Complex[][] {
				transformer.Transform(signal.GetValues().ToArray())
			};
		}
	}
}
