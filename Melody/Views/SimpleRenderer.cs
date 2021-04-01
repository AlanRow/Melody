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
		public static Color INTENSITY_COLOR = Colors.White;

		public Color IntensityColor { get; set; }

		private int lastWidth;
		private int lastHeight;
		private Image lastImg;

		private double maxFreq;

		private double startFreq = 220;
		private double octavesCount = 6.0;

		public double[][] Spectrum { get; set; }

		//public SimpleRenderer() : this(null) { }

		public SimpleRenderer(double[][] spec, double maxFreqValue) : this(spec, maxFreqValue, INTENSITY_COLOR) { }

		public SimpleRenderer(double[][] spec, double maxFreqValue, Color color)
		{
			IntensityColor = color;
			Spectrum = spec;
			maxFreq = maxFreqValue;
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
			var max = GetMaxLimit(Spectrum);
			var min = GetMinLimit(Spectrum);

			var stretchFactor = ((double)width)/Spectrum.Length;

			for (var col = 0; col < width; col++)
			{
				var specIdx = (int)((col + 1) / stretchFactor - 1);
				var intens = GetIntensities(Spectrum[specIdx], height, max, min);
				FillColumn(pixels, intens, col, width);
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

		private double[] GetIntensities(double[] specAtTime, int reqHeight, double max, double min)
		{
			var intens = new double[reqHeight];
			var specH = specAtTime.Length / 2 - 1;

			var startIdx = (int) (startFreq / maxFreq * specH);
			var powerStep = octavesCount / reqHeight;

			var stretchFactor = ((double)reqHeight) / (specH);

			for (var i = 0; i < reqHeight; i++)
			{
				var specIdx = (int)(Math.Pow(2, i * powerStep) * startIdx);
				var specVal = (specIdx < specH) ? (specAtTime[specIdx] - min) / max : 0;
				//var specVal = Math.Log(specAtTime[specIdx] - min + 1) / Math.Log(max);
				intens[i] = specVal;
			}

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
