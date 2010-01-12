namespace AudioLib
{
    /// <summary>
    /// Implementations of this interface can be used to resample WAV data, 
    /// or to uncompress WAV or MP3 data.
    /// </summary>
    public interface IWavFormatConverter
    {
        /// <summary>
        /// Resamples the given wav file using the specified PCM format,
        /// and stores the result in the given directory with a random filename (the full path is returned by this method).
        /// Only works with uncompressed wav data.
        /// </summary>
        /// <param name="sourceFile">cannot be null</param>
        /// <param name="destinationDirectory">cannot be null</param>
        /// <param name="destinationPCMFormat">cannot be null</param>
        /// <returns> absolute path to the new wave file </returns>
        string ConvertSampleRate(string sourceFile, string destinationDirectory, ushort destChannels, uint destSamplingRate, ushort destBitDepth);

        /// <summary>
        /// Uncompress the given wav file using the optional specified PCM format,
        /// and stores the result in the given directory with a random filename (the full path is returned by this method).
        /// </summary>
        /// <param name="sourceFile">cannot be null</param>
        /// <param name="destinationDirectory">cannot be null</param>
        /// <param name="destinationPCMFormat">can be null (in which case the PCM format of the given source is used)</param>
        /// <returns> absolute path to the new wave file </returns>
        string UnCompressWavFile(string sourceFile, string destinationDirectory, ushort destChannels, uint destSamplingRate, ushort destBitDepth);

        /// <summary>
        /// Uncompress the given mp3 file using the optional specified PCM format,
        /// and stores the result in the given directory with a random filename (the full path is returned by this method).
        /// </summary>
        /// <param name="sourceFile">cannot be null</param>
        /// <param name="destinationDirectory">cannot be null</param>
        /// <param name="destinationPCMFormat">can be null (in which case the PCM format of the given source is used)</param>
        /// <returns> absolute path to the new mp3 file </returns>
        string UnCompressMp3File(string sourceFile, string destinationDirectory, ushort destChannels, uint destSamplingRate, ushort destBitDepth);
    }
}
