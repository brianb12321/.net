using System;

namespace MTDataAccess.Models
{

    public readonly struct TimeSignature
    {
        public static TimeSignature Default => new TimeSignature(4, 4);

        /// <summary>
        /// The number of beats per musical measure.
        /// </summary>
        public uint BeatsPerMeasure { get; }
        /// <summary>
        /// The beat-value given to each beat. 8 for eight-note (quaver), 4 for quarter-note (crotchet), etc. The beat-value can be non-dyadic (not powers of two).
        /// </summary>
        public uint BeatSubdivision { get; }

        public TimeSignature(uint beatsPerMeasure, uint beatSubdivision)
        {
            BeatsPerMeasure = beatsPerMeasure;
            BeatSubdivision = beatSubdivision;
        }

        public override string ToString()
        {
            return $"{BeatsPerMeasure}/{BeatSubdivision}";
        }
        /// <summary>
        /// Encodes the time-signature into a numerical representation.
        /// </summary>
        /// <returns>The encoded integer</returns>
        public uint Encode()
        {
            //TODO: Figure out algorithm used by current song data-set.

            if (BeatsPerMeasure == 2 && BeatSubdivision == 4) return 1;
            if (BeatsPerMeasure == 4 && BeatSubdivision == 4) return 3;
            if (BeatsPerMeasure == 6 && BeatSubdivision == 8) return 13;
            if (BeatsPerMeasure == 12 && BeatSubdivision == 8) return 18;
            else throw new NotImplementedException();
        }

        public static TimeSignature ParseEncodedNumber(uint encodedNum)
        {
            switch (encodedNum)
            {
                case 1: return new TimeSignature(2, 4);
                case 3: return new TimeSignature(4, 4);
                case 13: return new TimeSignature(6, 8);
                case 18: return new TimeSignature(12, 8);
                default: throw new NotImplementedException();
            }
        }
        
    }
}