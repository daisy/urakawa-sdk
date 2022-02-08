using System;
using System.Collections.Generic;
using System.Collections;
using System.IO ;
using System.Windows.Forms;

//using urakawa.media.data;
//using  urakawa.media.data.audio ;
//using urakawa.media.timing;


namespace AudioLib
{
    public class PhraseDetection
    {
        public static readonly double DEFAULT_GAP = 300.0;              // default gap for phrase detection
        public static readonly double DEFAULT_LEADING_SILENCE = 50.0;  // default leading silence
        public static readonly double DEFAULT_THRESHOLD = 280.0;
        
        private static  readonly int m_FrequencyDivisor = 2000; // frequency inin hz to observe.
        private static bool m_CancelOperation;

        public static bool CancelOperation
        {
            get { return m_CancelOperation; }
            set { m_CancelOperation = value; }
        }


        // NewDetection

        // Detects the maximum size of noise level in a silent sample file
        public static long GetSilenceAmplitude (Stream assetStream, AudioLibPCMFormat audioPCMFormat)
        {
            CancelOperation = false;
            //m_AudioAsset = RefAsset.AudioMediaData;
            BinaryReader brRef = new BinaryReader(assetStream);

            // creates counter of size equal to clip size
            //long lSize = RefAsset.AudioMediaData.PCMFormat.Data.ConvertTimeToBytes(RefAsset.AudioMediaData.AudioDuration.AsLocalUnits);
            long lSize = audioPCMFormat.AdjustByteToBlockAlignFrameSize (assetStream.Length);

            // Block size of audio chunck which is least count of detection
            int Block;

            // determine the Block  size
            if (audioPCMFormat.SampleRate > 22500)
            {
                Block = 192;
            }
            else
            {
                Block = 96;
            }

            //set reading position after the header
            
            long lLargest = 0;
            long lBlockSum;

            // adjust the  lSize to avoid reading beyond file length
            //lSize = ((lSize / Block) * Block) - 4;
            
            // Experiment starts here
            double BlockTime = 25;
            double assetTimeInMS = audioPCMFormat.ConvertBytesToTime(assetStream.Length) / AudioLibPCMFormat.TIME_UNIT;
            //Console.WriteLine("assetTimeInMS " + assetTimeInMS);
            long Iterations = Convert.ToInt64(assetTimeInMS/ BlockTime);
            long SampleCount = Convert.ToInt64((int)audioPCMFormat.SampleRate/ (1000 / BlockTime));

            long lCurrentSum = 0;
            long lSumPrev = 0;


            for (long j = 0; j < Iterations - 1; j++)
            {
                //  BlockSum is function to retrieve average amplitude in  Block
                //lCurrentSum  = GetAverageSampleValue(brRef, SampleCount)  ;
                lCurrentSum = GetAvragePeakValue(assetStream, SampleCount, audioPCMFormat);
                lBlockSum = Convert.ToInt64((lCurrentSum + lSumPrev) / 2);
                lSumPrev = lCurrentSum;

                if (lLargest < lBlockSum)
                {
                    lLargest = lBlockSum;
                }
                if (CancelOperation) break;
            }
            long SilVal = Convert.ToInt64(lLargest);

            brRef.Close();

            return SilVal;

        }

        /// <summary>
        /// Detects phrases of the asset for which stream is provided and returns timing list of detected phrases in local units
        /// accepts time parameters GapLength and before in local units
        /// </summary>
        /// <param name="assetStream"></param>
        /// <param name="audioPCMFormat"></param>
        /// <param name="threshold"></param>
        /// <param name="GapLength"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        public static List<long> Apply(Stream assetStream, AudioLibPCMFormat audioPCMFormat, long threshold, long GapLength, long before)
        {
            //long lGapLength = ObiCalculationFunctions.ConvertTimeToByte(GapLength, (int)audio.AudioMediaData.PCMFormat.Data.SampleRate, audio.AudioMediaData.PCMFormat.Data.BlockAlign);
            //long lBefore = ObiCalculationFunctions.ConvertTimeToByte(before, (int)audio.AudioMediaData.PCMFormat.Data.SampleRate, audio.AudioMediaData.PCMFormat.Data.BlockAlign);
            long gapLengthInBytes = audioPCMFormat.ConvertTimeToBytes((long) GapLength );
            long beforeInBytes = audioPCMFormat.ConvertTimeToBytes((long) before );
            //Console.WriteLine ("GapLength " + gapLengthInBytes  + " - " + "Before " + beforeInBytes ) ;
            return ApplyPhraseDetection(assetStream, audioPCMFormat , threshold, gapLengthInBytes, beforeInBytes);
        }

        /// <summary>
        /// Detects phrases of the asset for which stream is provided and returns timing list of detected phrases in local units
        /// </summary>
        /// <param name="assetStream"></param>
        /// <param name="audioPCMFormat"></param>
        /// <param name="threshold"></param>
        /// <param name="GapLength"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        private static List<long> ApplyPhraseDetection(Stream assetStream, AudioLibPCMFormat audioPCMFormat, long threshold, double GapLength, double before)
        {
            CancelOperation = false;
            //m_AudioAsset = ManagedAsset.AudioMediaData;
            double assetTimeInMS = audioPCMFormat.ConvertBytesToTime(assetStream.Length) / AudioLibPCMFormat.TIME_UNIT;
            GapLength = audioPCMFormat.AdjustByteToBlockAlignFrameSize((long) GapLength);
            before = audioPCMFormat.AdjustByteToBlockAlignFrameSize((long) before );

            int Block = 0;

            // determine the Block  size
            if ( audioPCMFormat.SampleRate> 22500)
            {
                Block = 192;
            }
            else
            {
                Block = 96;
            }

            
            // count chunck of silence which trigger phrase detection
            long lCountSilGap =(long)  ( 2 * GapLength ) / Block; // multiplied by two because j counter is incremented by 2
            long lSum = 0;
            List<double> detectedPhraseTimingList = new List<double>();
            long lCheck = 0;

            // flags to indicate phrases and silence
            bool boolPhraseDetected = false;
            bool boolBeginPhraseDetected = false;


            double BlockTime = 25; // milliseconds
            double BeforePhraseInMS = audioPCMFormat.ConvertBytesToTime((long)before) / AudioLibPCMFormat.TIME_UNIT;
            //Console.WriteLine ("before , silgap " + BeforePhraseInMS+" , " + GapLength  ) ;
            lCountSilGap = Convert.ToInt64((audioPCMFormat.ConvertBytesToTime((long)GapLength) / AudioLibPCMFormat.TIME_UNIT) / BlockTime);

            long Iterations = Convert.ToInt64(assetTimeInMS/ BlockTime);
            long SampleCount = Convert.ToInt64(audioPCMFormat.SampleRate/ (1000 / BlockTime));
            double errorCompensatingCoefficient  = GetErrorCompensatingConstant ( SampleCount , audioPCMFormat);
            long SpeechBlockCount = 0;

            long lCurrentSum = 0;
            long lSumPrev = 0;

            BinaryReader br = new BinaryReader (assetStream);

            bool PhraseNominated = false;
            long SpeechChunkSize = 5;
            long Counter = 0;
            for (long j = 0; j < Iterations - 1; j++)
            {
                if (CancelOperation) return null;
                // decodes audio chunck inside block
                //lCurrentSum = GetAverageSampleValue(br, SampleCount);
                lCurrentSum = GetAvragePeakValue(assetStream, SampleCount, audioPCMFormat);
                lSum = (lCurrentSum + lSumPrev) / 2;
                lSumPrev = lCurrentSum;

                // conditional triggering of phrase detection
                if (lSum < threshold )
                {
                    lCheck++;

                    SpeechBlockCount = 0;
                }
                else
                {
                    if (j < lCountSilGap && boolBeginPhraseDetected == false)
                    {
                        boolBeginPhraseDetected = true;
                        detectedPhraseTimingList.Add(Convert.ToInt64(0));
                        boolPhraseDetected = true;
                        lCheck = 0;
                    }


                    // checks the length of silence
                    if (lCheck > lCountSilGap)
                    {
                        PhraseNominated = true;
                        lCheck = 0;
                    }
                    if (PhraseNominated)
                        SpeechBlockCount++;

                    if (SpeechBlockCount >= SpeechChunkSize && Counter >= 4)
                    {
                        //sets the detection flag
                        boolPhraseDetected = true;

                        // changing following time calculations to reduce concatination of rounding off errors 
                        //alPhrases.Add(((j - Counter) * BlockTime) - BeforePhraseInMS);
                        //double phraseMarkTime = ObiCalculationFunctions.ConvertByteToTime (Convert.ToInt64(errorCompensatingCoefficient  * (j - Counter)) * SampleCount * m_AudioAsset.PCMFormat.Data.BlockAlign,
                                                    //(int) m_AudioAsset.PCMFormat.Data.SampleRate,
                            //(int) m_AudioAsset.PCMFormat.Data.BlockAlign);
                        long phraseMarkTime = audioPCMFormat.ConvertBytesToTime(Convert.ToInt64(errorCompensatingCoefficient * (j - Counter)) * SampleCount * audioPCMFormat.BlockAlign) / AudioLibPCMFormat.TIME_UNIT;
                        //Console.WriteLine("mark time :" + phraseMarkTime);
                        detectedPhraseTimingList.Add ( phraseMarkTime - BeforePhraseInMS );

                        SpeechBlockCount = 0;
                        Counter = 0;
                        PhraseNominated = false;
                    }
                    lCheck = 0;
                }
                if (PhraseNominated)
                    Counter++;
                // end outer For
            }
            br.Close();

            List<long> detectedPhraseTimingsInTimeUnits = new List<long>();

            if (boolPhraseDetected == false)
            {
                return null ;
            }
            else
            {   
                for (int i = 0; i < detectedPhraseTimingList.Count; i++)
                {
                    detectedPhraseTimingsInTimeUnits.Add(Convert.ToInt64(detectedPhraseTimingList[i] * AudioLibPCMFormat.TIME_UNIT));
                }
            }

            return detectedPhraseTimingsInTimeUnits ;
        }

        public static long RemoveSilenceFromEnd(Stream assetStream, AudioLibPCMFormat audioPCMFormat, long threshold, double GapLength, double before)
        {
            GapLength = audioPCMFormat.ConvertTimeToBytes((long)GapLength);
            before = audioPCMFormat.ConvertTimeToBytes((long)before);

            CancelOperation = false;
            double assetTimeInMS = audioPCMFormat.ConvertBytesToTime(assetStream.Length) / AudioLibPCMFormat.TIME_UNIT;
            GapLength = audioPCMFormat.AdjustByteToBlockAlignFrameSize((long)GapLength);
            before = audioPCMFormat.AdjustByteToBlockAlignFrameSize((long)before);

            long lSum = 0;
            double detectedSilenceTime = 0;

            // flags to indicate phrases and silence
            bool boolPhraseDetected = false;


            double BlockTime = 25; // milliseconds

            long Iterations = Convert.ToInt64(assetTimeInMS / BlockTime);
            long SampleCount = Convert.ToInt64(audioPCMFormat.SampleRate / (1000 / BlockTime));
            double errorCompensatingCoefficient = GetErrorCompensatingConstant(SampleCount, audioPCMFormat);

            long lCurrentSum = 0;
            long lSumPrev = 0;
            bool IsSilenceDetected = false;
            long phraseMarkTimeForDeletingSilence = 0;

            BinaryReader br = new BinaryReader(assetStream);

            for (long j = 0; j < Iterations - 1; j++)
            {
                if (CancelOperation) return 0;
                // decodes audio chunck inside block
                lCurrentSum = GetAvragePeakValue(assetStream, SampleCount, audioPCMFormat);
                lSum = (lCurrentSum + lSumPrev) / 2;
                lSumPrev = lCurrentSum;

                if (lSum < threshold)
                {
                    if (!IsSilenceDetected)
                    {
                        phraseMarkTimeForDeletingSilence = audioPCMFormat.ConvertBytesToTime(Convert.ToInt64(errorCompensatingCoefficient * (j)) * SampleCount * audioPCMFormat.BlockAlign) / AudioLibPCMFormat.TIME_UNIT;
                        IsSilenceDetected = true;
                    }
                }
                else
                {
                    IsSilenceDetected = false;
                    phraseMarkTimeForDeletingSilence = 0;
                }
            }
            br.Close();

            if (phraseMarkTimeForDeletingSilence != 0)
            {
                boolPhraseDetected = true;
                detectedSilenceTime = phraseMarkTimeForDeletingSilence + before;
            }
            long detectedPhraseTimingsInTimeUnits = 0;

            if (boolPhraseDetected == false)
            {
                return 0;
            }
            else
            {
                detectedPhraseTimingsInTimeUnits = Convert.ToInt64(detectedSilenceTime * AudioLibPCMFormat.TIME_UNIT);
            }

            return detectedPhraseTimingsInTimeUnits;
        }
        private static int GetAverageSampleValue(Stream br, long SampleLength, AudioLibPCMFormat audioPCMFormat)
        {
            long AvgSampleValue = 0;

            for (long i = 0; i < SampleLength; i++)
            {
                AvgSampleValue = AvgSampleValue + GetSampleValue(br, audioPCMFormat);
            }
            AvgSampleValue = AvgSampleValue / SampleLength;

            return Convert.ToInt32(AvgSampleValue);
        }


        public static  int GetAvragePeakValue(Stream br, long SampleCount, AudioLibPCMFormat audioPCMFormat)
        {
                    // average value to return
            long AverageValue = 0;

            // number of samples from which peak is selected
                        long PeakCount  = Convert.ToInt64 (  audioPCMFormat.SampleRate/ m_FrequencyDivisor) ;

            // number of blocks iterated
            long AverageCount = Convert.ToInt64 ( SampleCount / PeakCount ) ;

                for (long i = 0; i < AverageCount; i++)
                {
                    AverageValue = AverageValue + GetPeak
                        (br, PeakCount, audioPCMFormat);
                }
            
            AverageValue = AverageValue / AverageCount;

            return Convert.ToInt32 (  AverageValue  ) ;

        }


        private static int GetPeak(Stream br, long UBound, AudioLibPCMFormat audioPCMFormat)
        {
            int Peak = 0;
            
            int CurrentValue = 0 ;
            for (long i = 0; i < UBound; i++)
            {
                CurrentValue = GetSampleValue(br, audioPCMFormat);
                if (CurrentValue > Peak)
                    Peak = CurrentValue;
            }
            return Peak ;
        }


        private static int GetSampleValue(Stream br, AudioLibPCMFormat audioPCMFormat)
        {
            int SampleValue1 =  0 ;
int SampleValue2 = 0 ;
                            
                
                                SampleValue1 =  br.ReadByte();
                                if (audioPCMFormat.BitDepth == 16)                    
            {
                    SampleValue1 = SampleValue1 + (br.ReadByte() * 256);

                    if (SampleValue1 > 32768)
                        SampleValue1 = SampleValue1 - 65536;

        }
        if (audioPCMFormat.NumberOfChannels == 2)
        {
            SampleValue2 = br.ReadByte();
            if (audioPCMFormat.BitDepth == 16)
            {
                SampleValue2 = SampleValue2 + (br.ReadByte() * 256);

                if (SampleValue2 > 32768)
                    SampleValue2 = SampleValue2 - 65536;

            }
            SampleValue1 = (SampleValue1 + SampleValue2) / 2;
        }


            return SampleValue1 ;

        }

        /// <summary>
        /// computes multiplying factor to compensate errors due to rounding off in average peak calculation functions
        /// </summary>
        /// <param name="SampleCount"></param>
        /// <returns></returns>
        public static double GetErrorCompensatingConstant(long SampleCount, AudioLibPCMFormat audioPCMFormat)
            {
            // number of samples from which peak is selected
                long PeakCount = Convert.ToInt64(audioPCMFormat.SampleRate / m_FrequencyDivisor);

            // number of blocks iterated
            long AverageCount = Convert.ToInt64 ( SampleCount / PeakCount );
            
            double roundedOffSampleCount = AverageCount * PeakCount;
            
            double errorCoeff = roundedOffSampleCount  / SampleCount;

            if (errorCoeff < 0.90 || errorCoeff  > 1.1)
                {
                errorCoeff  = 1.0;
                }
            return errorCoeff;
            }


    }
}
