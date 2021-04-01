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
		private Structures.TransformParameters options;
		
		public Analyzer(Structures.TransformParameters transformOptions)
		{
			options = transformOptions;
			transformer = new WelchTransformer(options.WindowSize, options.StepSize, options.Type);
		}

		public Complex[][] GetSpectrum(ISignal signal)
		{
			var spectrum = transformer.Transform(signal.GetValues().ToArray());

			var winDuration = signal.GetDurationInSeconds() * options.WindowSize / signal.GetActualLength();
			//DiscreteLPF.Filter(spectrum, winDuration, 100);
			// DiscreteHPF.Filter(spectrum, winDuration, 2000);
			return spectrum;
		}
	}
}
