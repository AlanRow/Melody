using Melody.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Melody.Views
{   /// <summary>
	/// Description of SpectrogramRenderer.
	/// </summary>
	public class SimpleRenderer
	{
		public static int COLOR_SIZE = 3;
		public static Color INTENSITY_COLOR = Colors.Red;

		public Color IntensityColor { get; set; }

		private int lastWidth;
		private int lastHeight;
		private Image lastImg;

		// private double maxFreq;
		private double winDuration;

		private double startFreq;
		private double octavesCount;

		public double[][] Spectrum { get; set; }

		//public SimpleRenderer() : this(null) { }

		//public SimpleRenderer(double[][] spec, double winDurationInSec) : this(spec, winDurationInSec, INTENSITY_COLOR) { }

		public SimpleRenderer(double[][] spec, double winDurationInSec, SpecViewParameters options)
		{
			IntensityColor = INTENSITY_COLOR;
			Spectrum = spec;
			winDuration = winDurationInSec;
			//maxFreq = maxFreqValue;
			startFreq = options.StartFreq;
			octavesCount = options.OctavesCount;
		}

		public void DrawSpectrogram(Image img, int width, int height)
		{
			lastImg = img;
			lastWidth = width;
			lastHeight = height;

			var pixels = GeneratePixels(width, height);
			var area = new Int32Rect(0, 0, width, height);
			var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);

			bitmap.WritePixels(area, pixels, 3 * width, 0);
			img.Source = bitmap;
		}

		public void UpdateSpectrogram()
		{
			if (lastImg != null)
				DrawSpectrogram(lastImg, lastWidth, lastHeight);
		}


		private byte[] GeneratePixels(int width, int height)
		{
			var pixels = new byte[height * width * COLOR_SIZE];

			var stretchFactor = ((double)width)/Spectrum.Length;

			var intens = new double[width][];

			for (var col = 0; col < width; col++)
			{
				var specIdx = (int)((col + 1) / stretchFactor - 1);
				intens[col] = GetIntensities(Spectrum[specIdx], height);
			}

			var max = GetMaxLimit(intens);
			var min = GetMinLimit(intens);

			for (var col = 0; col < width; col++)
				for (var row = 0; row < height; row++)
					intens[col][row] = (intens[col][row] - min) / max;

			for (var col = 0; col < width; col++)
			{
				FillColumn(pixels, intens[col], col, width);
			}

			return pixels;
		}

		/*
		 * Returns max value filtered 'filterLimit' - 1 max values
		 * Returns max value with 'filterLimit' equals to 1
		 */
		private double GetMaxLimit(double[][] spectrum)
		{
			var max = double.MinValue;
			foreach (var spec in spectrum)
				// Start from 1 (0 freq is not used) to 1-st half end (2-nd half duplicates 1-st)
				for (var i = 0; i < spec.Length / 2; i++)
					if (spec[i] > max)
						max = spec[i];

			return max;
		}

		private double GetMinLimit(double[][] spectrum)
		{
			var min = double.MaxValue;
			foreach (var spec in spectrum)
				// Start from 1 (0 freq is not used) to 1-st half end (2-nd half duplicates 1-st)
				for (var i = 0; i < spec.Length / 2; i++)
					if (spec[i] < min)
						min = spec[i];

			return min;
		}

		private double[] GetIntensities(double[] specAtTime, int reqHeight)
		{
			var intens = new double[reqHeight];
			var specH = specAtTime.Length / 2 - 1;

			// freq = idx / win duration
			// idx = freq * win duration

			var startIdx = (int)(startFreq * winDuration);
			// var startIdx = (int) (startFreq / maxFreq * specH);
			var powerStep = octavesCount / reqHeight;

			var stretchFactor = ((double)reqHeight) / (specH);

			for (var i = 0; i < reqHeight; i++)
			{
				var specIdx = (int)(Math.Pow(2, i * powerStep) * startIdx);

				//var specVal = (specIdx < specH) ? (specAtTime[specIdx] - min) / max : 0;
				//var specVal = Math.Log(specAtTime[specIdx] - min + 1) / Math.Log(max);
				if (specIdx < reqHeight)
				{
					var specVal = specAtTime[specIdx];
					intens[i] = specVal;
				} else
                {
					intens[i] = 0;
                }
			}

			/*var max = intens.Max();
			var min = intens.Min();

			for (var i = 0; i < intens.Length; i++)
				intens[i] = (intens[i] - min) / max;*/

			return intens;
		}

		private void FillColumn(byte[] pixels, double[] intensities, int column, int rowWidth)
		{
			var B = IntensityColor.B;
			var G = IntensityColor.G;
			var R = IntensityColor.R;

			var idx = column * COLOR_SIZE;
			for (var i = 0; i < intensities.Length; i++)
			{
				var ints = intensities[i];
				pixels[idx] = (byte)(int)(B * ints);
				pixels[idx + 1] = (byte)(int)(G * ints);
				pixels[idx + 2] = (byte)(int)(R * ints);
				idx += rowWidth * COLOR_SIZE;
			}
		}
	}
}
