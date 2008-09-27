namespace WaveForm.Data
{
    /// <summary>
    /// Interface for a <see cref="IWaveFormData"/> provider
    /// </summary>
    public interface IWaveFormDataProvider
    {
        /// <summary>
        /// Gets a <see cref="IWaveFormData"/> with the given number of data points
        /// </summary>
        /// <param name="numberOfDataPoints">Thye given number of data points</param>
        /// <returns>The resulting <see cref="IWaveFormData "/></returns>
        IWaveFormData GetData(int numberOfDataPoints);
    }
}
