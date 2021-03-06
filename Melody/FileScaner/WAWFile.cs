﻿using FileScaner.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileScaner
{
    class WAVFile
    {
        const string FORMAT = "RIFF";
        const string SUBFORMAT = "WAVE";
        const string SUBSUBFORMAT = "fmt ";
        const string DATA = "data";
        const string LIST = "LIST"; 
        const int SUBCHUNK = 16;
        const int NOTCOMPRESSING = 1;

        private bool compressed;

        public int Length { get; private set; }

        // count of channels (usually 1 - Mono or 2 - Stereo)
        public int Channels { get; private set; }

        public bool IsMono()
        {
            return Channels == 1;
        }

        public bool IsStereo()
        {
            return Channels == 2;
        }

        // count of samples that sounds till one second
        public int Sampling { get; private set; }
        public int SamplesCount { get; private set; }

        // count of bits for one sample
        public int Depth { get; private set; }

        private int[,] sound;

        // return enumerable of channel sound by channel number
        public int[] GetChannel(int number)
        {
            var channel = new int[SamplesCount];

            for (var i = 0; i < SamplesCount; i++)
                channel[i] = sound[number, i];

            return channel;
        }

        public double GetDuration()
        {
            return (double)(SamplesCount) / ((double)Sampling);
        }

        /// <exception cref="FileScaner.Exceptions.FileReadingException">Reading error</exception>
        /// <exception cref="FileScaner.Exceptions.FileCompressedException">File data is compressed</exception>
        /// <exception cref="System.FormatException">Bad file format</exception>
        public WAVFile(string path)
        {
            var bytes = new byte[0];
            try
            {
                bytes = File.ReadAllBytes(path);
            } catch (Exception ex)
            {
                throw new FileReadingException(ex.Message);
            }
            // check file valid

            ReadAndCheck(bytes, FORMAT, 0, 4);
            ReadAndCheck(bytes, bytes.Length - 8, 4);
            ReadAndCheck(bytes, SUBFORMAT, 8, 4);
            ReadAndCheck(bytes, SUBSUBFORMAT, 12, 4);
            ReadAndCheck(bytes, SUBCHUNK, 16);

            // Parameters init
            Length = bytes.Length - 8;

            try
            {
                if (ReadNumberBE(bytes, 20, 2) == NOTCOMPRESSING)
                    compressed = false;
                else
                    throw new FileCompressedException("File has compressed! This programm doesn't work with compressed files");

                Channels = (int)ReadNumberBE(bytes, 22, 2);
                Sampling = (int)ReadNumberBE(bytes, 24, 4);

                // miss the bytes on second and bytes in sample, so its calculatable values

                Depth = (int)ReadNumberBE(bytes, 34, 2);
            } catch (ArgumentException ex)
            {
                throw new FormatException(ex.Message);
            } 

            var byteInSample = Channels * Depth / 8;
            ReadAndCheck(bytes, byteInSample, 32, 2);

            var byteInSecond = byteInSample * Sampling;
            ReadAndCheck(bytes, byteInSecond, 28);

            var dataIdx = 36;
            try
            {
            	ReadAndCheck(bytes, DATA, 36, 4);
            }
            catch (FormatException)
            {
            	ReadAndCheck(bytes, LIST, 36, 4);
            	var addDataLen = BitConverter.ToInt32(bytes, 40);
            	dataIdx = dataIdx + 8 + addDataLen;
            	ReadAndCheck(bytes, DATA, dataIdx, 4);
            }

            int dataLen;
            try
            {
                dataLen = (int)ReadNumberBE(bytes, dataIdx + 4, 4);
            }
            catch (ArgumentException ex)
            {
                throw new FormatException(ex.Message);
            }

            var dataStart = dataIdx + 8;

            SamplesCount = dataLen / (byteInSample);

            sound = new int[Channels, SamplesCount];

            try
            {
                for (var i = 0; i < sound.GetLength(1); i++)
                {
                    for (var j = 0; j < Channels; j++)
                    {
                        sound[j, i] = (int)ReadNumberLE(bytes, dataStart + i * byteInSample + j * Depth / 8, Depth / 8);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                throw new FormatException(ex.Message);
            }
        }

        private static void ReadAndCheck(byte[] file, string exp, int start, int length)
        {
            var bytes = new byte[length];
            Array.Copy(file, start, bytes, 0, length);
            if (Encoding.ASCII.GetString(bytes) != exp)
            {
                throw new FormatException(String.Format("File have bad format on bytes {0}-{1}: must be {2}, but actually was {3}", 
                                            start, start + length - 1, exp, Encoding.ASCII.GetString(bytes)));
            }
        }

        private static void ReadAndCheck(byte[] file, int exp, int start)
        {
            var num = BitConverter.ToInt32(file, start);
            if (num != exp)
            {
                throw new FormatException(String.Format("File have bad format on bytes {0}-{1}: must be {2}, but actually was {3}",
                                            start, start + 3, exp, num));
            }
        }

        private static void ReadAndCheck(byte[] file, int exp, int start, int len)
        {
            long num;
            try
            {
                num = ReadNumberBE(file, start, len);
            } catch (ArgumentException ex)
            {
                throw new FormatException(ex.Message);
            }
            if (num != exp)
            {
                throw new FormatException(String.Format("File have bad format on bytes {0}-{1}: must be {2}, but actually was {3}",
                                            start, start + len, exp, num));
            }
        }

        //Big Endian
        private static long ReadNumberBE(byte[] file, int start, int length)
        {
            if (length > 4)
                throw new ArgumentException("Length of long number must not be more than 4 bytes");

            long n = 0;
            long f = 1;
            for (var i = start; i < start + length; i++) {
                n += file[i] * f;
                f *= 256;
            }

            return n;
        }

        private static long ReadNumberLE(byte[] file, int start, int length)
        {
            if (length > 4)
                throw new ArgumentException("Length of long number must not be more than 4 bytes");

            long n = 0;
            int f = (int) Math.Pow(256, length - 1);
            for (var i = start; i < start + length; i++)
            {
                n += file[i] * f;
                f /= 256;
            }

            return n;
        }

        public string GetInfo()
        {
            return String.Format("File size (in bytes): {0},\n", Length) + 
                       String.Format("Count of channels: {0},\n", Channels) +
                       String.Format("Sampling rate: {0},\n", Sampling) +
                       String.Format("Audio depth (in bits): {0}.\n", Depth) +
                       String.Format("Duration: {0} seconds", GetDuration());
        }
    }
}
