using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ariphmetic
{
    public static class Extensions
    {
        public static int HighestOneBit(this int number)
        {
            return (int)Math.Pow(2, Convert.ToString(number, 2).Length - 1);
        }
    }

    internal class Coder
    {
       
        private long _low, _high;
        private int _addBits;
        private long _value;
        private int[] _freq;
        private int[] _bits;
        private int _bitsPos;
        private List<bool> _encodedBits;
        private List<int> _decodedBytes;

        public Coder()
        {
            _encodedBits = new List<bool>();
            _low = 0;
            _high = (1 << Util.Bits) - 1;

            _addBits = 0;//

            _freq = BuildTree(Util.End + 1);
        }

        public BitArray Encode(byte[] inBytes)
        {
            var index = 1.0D;
            foreach (var data in inBytes)
            {
                EncodeCharacter(data);
                Util.ShowPercents(inBytes.Length,ref index);
            }

            EncodeCharacter(Util.End);
            OutputBit(true);

            return new BitArray(_encodedBits.ToArray());
        }

        private void EncodeCharacter(int c)
        {
            var range = _high - _low + 1;

            _high = _low + range * Frequency(_freq, c) / Frequency(_freq, Util.End) - 1;
            _low = _low + range * Frequency(_freq, c - 1) / Frequency(_freq, Util.End);

            while (true)
            {
                if ((_low & Util.HighestBit) == (_high & Util.HighestBit))
                {
                    OutputBit((_high & Util.HighestBit) != 0);
                    _low = (_low << 1) & Util.Mask;
                    _high = ((_high << 1) + 1) & Util.Mask;
                }
                else if (_high - _low < Frequency(_freq, Util.End))
                {
                    _low = (_low - (Util.Half)) << 1;
                    _high = ((_high - (Util.Half)) << 1) + 1;
                    _addBits++;
                }
                else
                {
                    break;
                }
            }

            Increment(_freq, c);
        }

        private void OutputBit(bool bit)
        {
            _encodedBits.Add(bit);
            for (; _addBits > 0; _addBits--)
                _encodedBits.Add(!bit);
        }

        public byte[] Decode(int[] bits)
        {
            _bits = bits;
            _decodedBytes = new List<int>();
            _value = 0;
            for (_bitsPos = 0; _bitsPos < Util.Bits; _bitsPos++)
                _value = (_value << 1) + (_bitsPos < bits.Length ? bits[_bitsPos] : 0);

            while (true)
            {
                var c = Decode_char();
                if (c == Util.End) break;

                _decodedBytes.Add(c);

                Increment(_freq, c);
            }
            return _decodedBytes.Select(Convert.ToByte).ToArray();
        }

        private int Decode_char()
        {
            var range = _high - _low + 1;
            var index = (int)(((_value - _low + 1) * Frequency(_freq, Util.End) - 1) / range);
            var c = UpperBound(_freq, index);
            
            _high = _low + range * Frequency(_freq, c) / Frequency(_freq, Util.End) - 1;
            _low = _low + range * Frequency(_freq, c - 1) / Frequency(_freq, Util.End);
            while (true)
            {
                if ((_low & Util.HighestBit) == (_high & Util.HighestBit))
                {
                    _low = (_low << 1) & Util.Mask;
                    _high = ((_high << 1) + 1) & Util.Mask;
                    var b = _bitsPos < _bits.Length ? _bits[_bitsPos++] : 0;
                    _value = ((_value << 1) + b) & Util.Mask;
                }
                else if (_high - _low < Frequency(_freq, Util.End))
                {
                    _low = (_low - (Util.Half)) << 1;
                    _high = ((_high - (Util.Half)) << 1) + 1;
                    var b = _bitsPos < _bits.Length ? _bits[_bitsPos++] : 0;
                    _value = ((_value - (Util.Half)) << 1) + b;
                }
                else
                {
                    break;
                }
            }
            return c;
        }

        private int[] BuildTree(int num)
        {
            var data = new int[num];
            for (int i = 0; i < num; i++)
            {
                ++data[i];
                var j = i | (i + 1);
                if (j < num)
                    data[j] += data[i];
            }
            return data;
        }

        private void Increment(int[] t, int i)
        {
            for (; i < t.Length; i |= (i + 1))
                ++t[i];
        }

        private static int Frequency(int[] t, int i)
        {
            int summ = 0;
            for (; i >= 0; i = (i & (i + 1)) - 1)
                summ += t[i];
            return summ;
        }

        private int UpperBound(int[] t, int sum)
        {
            int position = -1;
            for (int blockSize = t.Length.HighestOneBit(); blockSize != 0; blockSize >>= 1)
            {
                var nextPos = position + blockSize;
                if (nextPos >= t.Length || sum < t[nextPos]) continue;
                sum -= t[nextPos];
                position = nextPos;
            }
            return position + 1;
        }

        public byte[] EncodeToByteArray(BitArray bits)
        {
            var arr = bits.Count % 8 == 0 ? new byte[bits.Count / 8] : new byte[bits.Count / 8 + 1];
            bits.CopyTo(arr, 0);
            return arr;
        }

        public int[] EncodeToBitArray(byte[] inBytes)
        {
            var res = new int[inBytes.Length * 8];
            int counter = 0;
            foreach (int data in inBytes)
            {
                for (byte m = 1; m != 0; m <<= 1)
                {
                    var bit = ((data & m) != 0) ? 1 : 0;
                    res[counter] = bit;
                    counter++;
                }
            }
            return res;
        }
    }
}