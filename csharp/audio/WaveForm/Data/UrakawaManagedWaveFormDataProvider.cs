using System;
using System.IO;
using urakawa.exception;
using urakawa.media.data.audio;

namespace WaveForm.Data
{
    public class UrakawaManagedWaveFormDataProvider : IWaveFormDataProvider
    {
        private class WaveFormData : IWaveFormData
        {
            public TimeSpan mBeginTime, mEndTime;
            public int mNumberOfChannels, mNumberOfDataPoints, mMinScaleValue, mMaxScaleValue;
            public int[, ,] mData;

            #region Implementation of IWaveFormData

            /// <summary>
            /// The begin time of the (audio data behind the) wave form data
            /// </summary>
            public TimeSpan BeginTime
            {
                get { return mBeginTime; }
            }



            /// <summary>
            /// The end time of the (audio data behind the) wave form data
            /// </summary>
            public TimeSpan EndTime
            {
                get { return mEndTime; }
            }

            /// <summary>
            /// The duration of the (audio data behind the) wave form data
            /// </summary>
            public TimeSpan Duration
            {
                get { return EndTime - BeginTime; }
            }

            /// <summary>
            /// The number of channels of the (audio data behind the) wave form data
            /// </summary>
            public int NumberOfChannels
            {
                get { return mNumberOfChannels; }
            }

            /// <summary>
            /// The number of data points in the wave form data
            /// </summary>
            public int NumberOfDataPoints
            {
                get { return mNumberOfDataPoints; }
            }

            /// <summary>
            /// Gets the maximal possible data value 
            /// </summary>
            public int MaxScaleValue
            {
                get { return mMaxScaleValue; }
            }

            /// <summary>
            /// Gets the minimal possible value
            /// </summary>
            public int MinScaleValue
            {
                get { return mMinScaleValue; }
            }

            /// <summary>
            /// Gets a minimal value for a channel at one of the data points in the wave form data
            /// </summary>
            /// <param name="channel">The channel in question (zero indexed)</param>
            /// <param name="dataPointIndex">The index of the data pount</param>
            /// <returns>The minimal value</returns>
            public int GetMinDataValue(int channel, int dataPointIndex)
            {
                if (0<=channel && channel<NumberOfChannels && 0<=dataPointIndex && dataPointIndex<NumberOfDataPoints)
                {
                    return mData[dataPointIndex, channel, 0];
                }
                return 0;
            }

            /// <summary>
            /// Gets a maximal value for a channel at one of the data points in the wave form data
            /// </summary>
            /// <param name="channel">The channel in question (zero indexed)</param>
            /// <param name="dataPointIndex">The index of the data pount</param>
            /// <returns>The maximal value</returns>
            public int GetMaxDataValue(int channel, int dataPointIndex)
            {
                if (0 <= channel && channel < NumberOfChannels && 0 <= dataPointIndex && dataPointIndex < NumberOfDataPoints)
                {
                    return mData[dataPointIndex, channel, 0];
                }
                return 0;
            }

            #endregion
        }

        private AudioMediaData mSourceAudioMediaData;

        public UrakawaManagedWaveFormDataProvider(AudioMediaData source)
        {
            if (source==null) throw new MethodParameterIsNullException("The source AudioMediaData cannot be null");
            mSourceAudioMediaData = source;
        }
        
        public AudioMediaData SourceAudioMediaData
        {
            get { return mSourceAudioMediaData; }
        }

        #region IWaveFormDataProvider Members

        public IWaveFormData GetData(int numberOfDataPoints)
        {
            WaveFormData res = new WaveFormData();
            if (numberOfDataPoints<1)
            {
                throw new MethodParameterIsOutOfBoundsException("The number of data points must be a positive integer");
            }
            res.mNumberOfDataPoints = numberOfDataPoints;
            PCMFormatInfo fmt = SourceAudioMediaData.PCMFormat;
            res.mNumberOfChannels = fmt.NumberOfChannels;
            Stream audio = SourceAudioMediaData.GetAudioData();
            res.mBeginTime = TimeSpan.Zero;
            res.mEndTime = SourceAudioMediaData.AudioDuration.TimeDeltaAsTimeSpan;
            res.mData = new int[res.NumberOfDataPoints, res.NumberOfChannels, 2];
            switch (fmt.BitDepth)
            {
                case 8:
                    res.mMinScaleValue = byte.MinValue;
                    res.mMaxScaleValue = byte.MaxValue;
                    Calculate8BitMinMax(audio, res);
                    break;
                case 16:
                    res.mMinScaleValue = short.MinValue;
                    res.mMaxScaleValue = short.MaxValue;
                    Calculate16BitMinMax(audio, res);
                    break;
                default:
                    throw new Exception(
                        String.Format(
                            "Bit Depth {0:0} not supported (only bit depths 8 and 16 are supported",
                            SourceAudioMediaData.PCMFormat.BitDepth));
            }
            return res;
        }

        private void Calculate8BitMinMax(Stream audio, WaveFormData res)
        {

        }

        private void Calculate16BitMinMax(Stream audio, WaveFormData res)
        {

        }

        #endregion
    }
}