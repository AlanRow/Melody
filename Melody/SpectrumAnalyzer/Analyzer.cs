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
		private ITransformer transformer;
		
		public Analyzer()
		{
			//transformer = new BartlettTransformer(8192);
			transformer = new WelchTransformer(2048, 2048, WelchTransformer.HannFilter);
		}

		public Complex[][] GetSpectrum(ISignal signal)
		{
			return transformer.Transform(signal.GetValues().ToArray());
		}
	}
}
