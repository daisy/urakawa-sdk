using System;

namespace WaveForm.Data
{
    /// <summary>
    /// Interface for wave form data
    /// </summary>
    public interface IWaveFormData
    {
        /// <summary>
        /// The begin time of the (audio data behind the) wave form data
        /// </summary>
        TimeSpan BeginTime { get; }
        /// <summary>
        /// The end time of the (audio data behind the) wave form data
        /// </summary>
        TimeSpan EndTime { get;}
        /// <summary>
        /// The duration of the (audio data behind the) wave form data
        /// </summary>
        TimeSpan Duration { get;}
        /// <summary>
        /// The number of channels of the (audio data behind the) wave form data
        /// </summary>
        int NumberOfChannels { get;}
        /// <summary>
        /// The number of data points in the wave form data
        /// </summary>
        int NumberOfDataPoints { get;}
        /// <summary>
        /// Gets the maximal possible data value 
        /// </summary>
        int MaxScaleValue { get;}
        /// <summary>
        /// Gets the minimal possible value
        /// </summary>
        int MinScaleValue { get;}
        /// <summary>
        /// Gets a minimal value for a channel at one of the data points in the wave form data
        /// </summary>
        /// <param name="channel">The channel in question</param>
        /// <param name="dataPointIndex">The index of the data pount</param>
        /// <returns>The minimal value</returns>
        int GetMinDataValue(int channel, int dataPointIndex);
        /// <summary>
        /// Gets a maximal value for a channel at one of the data points in the wave form data
        /// </summary>
        /// <param name="channel">The channel in question</param>
        /// <param name="dataPointIndex">The index of the data pount</param>
        /// <returns>The maximal value</returns>
        int GetMaxDataValue(int channel, int dataPointIndex);
    }
}
