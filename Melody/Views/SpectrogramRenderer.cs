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
	public class SpectrogramRenderer
	{
		public static int COLOR_SIZE = 3;
		public static Color INTENSITY_COLOR = Colors.White;
		public static double INTENSITY_POWER = 1;
		public static int FILTER_LIMIT = 10;

		public Color IntensityColor { get; set; }

		private double intensityPower;
		public double IntensityPower
		{
			get
			{
				return intensityPower;
			}

			set
			{
				if (value <= 0)
				{
					throw new FormatException("Intensity power must be more than 0");
				}
				intensityPower = value;
			}
		}

		private int filterLimit;

		private int lastWidth;
		private int lastHeight;
		private Image lastImg;


		public int FilterLimit
		{
			get
			{
				return filterLimit;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentException("Filter limit must be positive integer");
				filterLimit = value;
			}
		}

		public double[][] Spectrum { get; set; }

		public SpectrogramRenderer() : this(null) { }

		public SpectrogramRenderer(double[][] spec) : this(spec, INTENSITY_COLOR, INTENSITY_POWER, FILTER_LIMIT) { }

		public SpectrogramRenderer(double[][] spec, Color color, double intensity, int filterLim)
		{
			IntensityColor = color;
			Spectrum = spec;
			IntensityPower = intensity;
			FilterLimit = filterLim;
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
			var max = GetFilterMaxLimit(Spectrum);
			var min = GetFilterMinLimit(Spectrum);

			var colIdx = 0;

			for (var col = 0; col < width; col++)
			{
				if (col * (Spectrum.Length - 1) > colIdx * (width - 1))
					colIdx++;

				var intens = GetIntensities(Spectrum[colIdx], height, max, min);
				FillColumn(pixels, intens, col, width);
			}

			return pixels;
		}

		/*
		 * Returns max value filtered 'filterLimit' - 1 max values
		 * Returns max value with 'filterLimit' equals to 1
		 */
		private double GetFilterMaxLimit(double[][] spectrum)
		{
			var max = new LinkedList<double>();

			foreach (var spec in spectrum)
			{
				foreach (var freq in spec)
				{
					if (max.Count == 0)
					{
						max.AddLast(freq);
					}
					else
					{
						var node = max.First;
						while (node != null)
						{
							if (freq > node.Value)
							{
								max.AddBefore(node, freq);
								if (max.Count > filterLimit)
									max.RemoveLast();
								break;
							}
							node = node.Next;
						}

						if (node == null && max.Count < filterLimit)
							max.AddLast(freq);
					}
				}
			}

			return max.Last.Value;
		}

		private double GetFilterMinLimit(double[][] spectrum)
		{
			var min = new LinkedList<double>();

			foreach (var spec in spectrum)
			{
				foreach (var freq in spec)
				{
					if (min.Count == 0)
					{
						min.AddLast(freq);
					}
					else
					{
						var node = min.First;
						while (node != null)
						{
							if (freq < node.Value)
							{
								min.AddBefore(node, freq);
								if (min.Count > filterLimit)
									min.RemoveLast();
								break;
							}
							node = node.Next;
						}

						if (node == null && min.Count < filterLimit)
							min.AddLast(freq);
					}
				}
			}

			return min.Last.Value;
		}

		private double[] GetIntensities(double[] specAtTime, int reqHeight, double max, double min)
		{
			var intens = new double[reqHeight];

			var specIdx = 0;
			var specSum = 0d;
			var specNum = 1;
			var range = max - min;

			for (var i = 0; i < intens.Length; i++)
			{
				while (i * (specAtTime.Length - 1) > specIdx * (reqHeight - 1))
				{
					specIdx++;
					specNum++;
					specSum += specAtTime[specIdx];
				}

				var magn = 0d;
				if (specSum > 1)
				{
					magn = (specAtTime[specIdx] + specSum) / specNum;
					specSum = 0;
					specNum = 1;
				}
				else
				{
					magn = specAtTime[specIdx];
				}

				if (magn > max)
					intens[i] = 1;
				else if (magn < min)
					intens[i] = 0;
				else
					intens[i] = Math.Pow((magn - min) / range, IntensityPower);
				//intens[i] = Math.Log((freqs[specIdx].Coords.Magnitude), max);
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
