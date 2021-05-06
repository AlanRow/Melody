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
			// transformer = new WelchTransformer(options.WindowSize, options.StepSize, options.Type);
			// transformer = new ConstantQTransformer();
			var filter = Filter.Rectangle;

			switch (options.Type)
            {
				case Structures.FilterType.Rectangle:
					filter = Filter.Rectangle;
					break;

				case Structures.FilterType.Triangle:
					filter = Filter.Triangle;
					break;

				case Structures.FilterType.Hann:
					filter = Filter.Hann;
					break;

				case Structures.FilterType.Hamming:
					filter = Filter.Hamming;
					break;

				default:
					throw new Exception(String.Format("Неизвестный тип фильтра: {0}", options.Type.ToString()));
			}

			transformer = new LogFTTransformer(options.StartFreq, options.EndFreq, options.BoundsPerOctave, options.WindowSize, options.StepSize, filter);
		}

		public Spectrum GetSpectrum(ISignal signal)
		{
			var spectrum = transformer.Transform(signal.GetValues().ToArray(), signal.GetDurationInSeconds());

			var winDuration = signal.GetDurationInSeconds() * options.WindowSize / signal.GetActualLength();
			// DiscreteLPF.Filter(spectrum, winDuration, options.LPFLimit);
			// DiscreteHPF.Filter(spectrum, winDuration, 2000);
			return spectrum;
		}
	}
}
