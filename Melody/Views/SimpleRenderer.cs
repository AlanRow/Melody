﻿using Melody.Structures;
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
{
	public enum IntensityCalcMethod
    {
		Linear,
		Log,
		Pow,
	}
	public enum FreqScaleType
	{
		Linear,
		Log,
	}
	public enum IntensSumMethod
    {
		No,
		Max,
		Average,
		SquareAverage,
    }

	public class SimpleRenderer
	{
		public static int COLOR_SIZE = 3;
		public static Color INTENSITY_COLOR = Colors.Red;

		public Color IntensityColor { get; set; }

		private int lastWidth;
		private int lastHeight;
		private Image lastImg;

		private FreqScaleType scaleType = FreqScaleType.Linear;
		private IntensityCalcMethod calcMethod = IntensityCalcMethod.Linear;
		private double intensityPower = 2;
		private IntensSumMethod sumMethod = IntensSumMethod.SquareAverage;

		public double[][] Spectrum { get; set; }

		public SimpleRenderer(double[][] spec, SpecViewParameters options)
		{
			IntensityColor = INTENSITY_COLOR;
			Spectrum = spec;

			scaleType = options.ScaleType;
			calcMethod = options.CalcMethod;
			sumMethod = options.SumMethod;
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
			var max = 0d;
			var specW = Spectrum.Length;
			var specH = Spectrum[0].Length;

			// Find max value
			for (var i = 0; i < specW; i++)
				for (var j = 0; j < specH; j++)
					if (max < Spectrum[i][j])
						max = Spectrum[i][j];

			var xStretch = ((double)width) / specW;
			for (var col = 0; col < width; col++)
            {
				var i = (int) (col / xStretch);
				var intens = new double[height];
				var oldJ = 0;
				for (var row = 0; row < height; row++)
                {
					var j = GetSpecCoord(row, height);
					if (j >= specH)
						break;

					var abs = GetSummarizedIntensity(Spectrum[i], oldJ, j + 1);
					var rel = GetRelativeIntensity(abs, max);
					intens[row] = rel;
					oldJ = j;
                }
				FillColumn(pixels, intens, col, width);
            }

			return pixels;
        }

		private int GetSpecCoord(int pixelY, int pixelHeight)
        {
			var stretch = ((double)pixelHeight) / Spectrum[0].Length;

			switch (scaleType)
            {
				case FreqScaleType.Linear:
					return (int) (pixelY / stretch);
				case FreqScaleType.Log:
					throw new NotImplementedException("Логарифмическая шкала недоступна в данной версии");
			}

			return 0;
        }

		private double GetRelativeIntensity(double abs, double max)
        {
			switch (calcMethod)
            {
				case IntensityCalcMethod.Linear:
					return abs / max;
				case IntensityCalcMethod.Log:
					return Math.Log(abs + 1) / Math.Log(max + 1);
				case IntensityCalcMethod.Pow:
					return Math.Pow(abs / max, intensityPower);
			}

			return 0;
        }

		private double GetSummarizedIntensity(double[] intens, int start, int end)
        {
			switch (sumMethod)
            {
				case IntensSumMethod.No:
					return intens[end - 1];
				case IntensSumMethod.Max:
					var max = 0d;
					for (var i = start; i < end; i++)
						if (intens[i] > max)
							max = intens[i];
					return max;
				case IntensSumMethod.Average:
					var sum = 0d;
					for (var i = 0; i < end; i++)
						sum += intens[i];
					return sum / (end - start);
				case IntensSumMethod.SquareAverage:
					var sqSum = 0d;
					for (var i = 0; i < end; i++)
						sqSum += intens[i] * intens[i];
					return Math.Sqrt(sqSum / (end - start));
			}

			return 0;
        }

		private void FillColumn(byte[] pixels, double[] intensities, int column, int rowWidth)
		{
			var idx = column * COLOR_SIZE;
			var len = intensities.Length;
			for (var i = 0; i < intensities.Length; i++)
			{
				var ints = intensities[len - i - 1];

				byte b = 0;
				byte g = 0;
				byte r = 0;


				if (ints > 0.75)
                {
					r = 255;
					g = (byte)(255 - (ints - 0.75) * 1020);
                }
				else if (ints > 0.5)
                {
					r = (byte)((ints - 0.5) * 1020);
					g = 255;
                } else if (ints > 0.25)
                {
					g = 255;
					b = (byte)(255 - (ints - 0.25) * 1020);
				} else
                {
					g = (byte)(ints * 1020);
					b = 255;
                }

				pixels[idx] = b;
				pixels[idx + 1] = g;
				pixels[idx + 2] = r;
				idx += rowWidth * COLOR_SIZE;
			}
		}
	}
}
