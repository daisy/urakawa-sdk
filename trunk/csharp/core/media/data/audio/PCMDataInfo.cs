using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using urakawa.exception;
using urakawa.media.timing;

namespace urakawa.media.data.audio
{
    /// <summary>
    /// Represents information describing raw PCM data
    /// </summary>
    public class PCMDataInfo : PCMFormatInfo, IValueEquatable<PCMDataInfo>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public PCMDataInfo()
            : base()
        {
            DataLength = 0;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">The PCMDataInfo to copy</param>
        public PCMDataInfo(PCMDataInfo other)
            : base(other)
        {
            DataLength = other.DataLength;
        }

        /// <summary>
        /// Copy constructor copying from a <see cref="PCMFormatInfo"/>, using the default value for data length
        /// </summary>
        /// <param name="other">The PCMFormatInfo to copy from</param>
        public PCMDataInfo(PCMFormatInfo other)
            : base(other)
        {
            DataLength = 0;
        }

        private uint mDataLength = 0;

        /// <summary>
        /// Gets the count in bytes of the raw PCM data
        /// </summary>
        public uint DataLength
        {
            get { return mDataLength; }
            set { mDataLength = value; }
        }

        /// <summary>
        /// Gets the duration of the RAW PCM data
        /// </summary>
        /// <returns>The duration as a <see cref="TimeSpan"/></returns>
        public TimeDelta Duration
        {
            get { return GetDuration(DataLength); }
        }

        /// <summary>
        /// Writes a RIFF Wave PCM header to a given destination output <see cref="Stream"/>
        /// </summary>
        /// <param name="output">The destination output <see cref="Stream"/></param>
        public ulong WriteRiffWaveHeader(Stream output)
        {
            long initPos = output.Position;
            BinaryWriter wr = new BinaryWriter(output);
            wr.Write(Encoding.ASCII.GetBytes("RIFF")); //Chunk Uid
            uint chunkSize = 4 + 8 + 16 + 8 + DataLength;
            wr.Write(chunkSize); //Chunk Size
            wr.Write(Encoding.ASCII.GetBytes("WAVE")); //Format field
            wr.Write(Encoding.ASCII.GetBytes("fmt ")); //Format sub-chunk
            uint formatChunkSize = 16;
            wr.Write(formatChunkSize);
            ushort audioFormat = 1; //PCM format
            wr.Write(audioFormat);
            wr.Write(NumberOfChannels);
            wr.Write(SampleRate);
            wr.Write(ByteRate);
            wr.Write(BlockAlign);
            wr.Write(BitDepth);
            wr.Write(Encoding.ASCII.GetBytes("data"));
            wr.Write(DataLength);

            long endPos = output.Position;
            return (ulong) (endPos - initPos);
        }

        /// <summary>
        /// Parses a RIFF WAVE PCM header of a given input <see cref="Stream"/>
        /// </summary>
        /// <remarks>
        /// Upon succesful parsing the <paramref name="input"/> <see cref="Stream"/> is positioned at the beginning of the actual PCM data,
        /// that is at the beginning of the data field of the data sub-chunk
        /// </remarks>
        /// <param name="input">The input <see cref="Stream"/> - must be positioned at the start of the RIFF chunk</param>
        /// <returns>A <see cref="PCMDataInfo"/> containing the parsed data</returns>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when RIFF WAVE header is invalid or is not PCM data
        /// </exception>
        public static PCMDataInfo ParseRiffWaveHeader(Stream input)
        {
            System.Diagnostics.Debug.Assert(input.Position == 0);

            BinaryReader rd = new BinaryReader(input);

            //http://www.sonicspot.com/guide/wavefiles.html

            // Ensures 3x4=12 bytes available to read (RIFF Type Chunk)
            {
                long availableBytes = input.Length - input.Position;
                if (availableBytes < 12)
                {
                    throw new InvalidDataFormatException(
                        "The RIFF chunk descriptor does not fit in the input stream");
                }
            }

            //Chunk ID (4 bytes)
            {
                string chunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));
                if (chunkId != "RIFF")
                {
                    throw new InvalidDataFormatException("ChunkId is not RIFF");
                }
            }
            //Chunk Data Size (the wavEndPos variable is used further below as the upper limit position in the stream)
            long wavEndPos = 0;
            {
                // 4 bytes
                uint chunkSize = rd.ReadUInt32();

                // Ensures the given size fits within the actual stream
                wavEndPos = input.Position + chunkSize;
                if (wavEndPos > input.Length)
                {
                    throw new InvalidDataFormatException(String.Format(
                                                             "The WAVE PCM chunk does not fit in the input Stream (expected chunk end position is {0:0}, Stream count is {1:0})",
                                                             wavEndPos, input.Length));
                }
            }
            //RIFF Type (4 bytes)
            {
                string format = Encoding.ASCII.GetString(rd.ReadBytes(4));
                if (format != "WAVE")
                {
                    throw new exception.InvalidDataFormatException(String.Format(
                                                                       "RIFF format {0} is not supported. The only supported RIFF format is WAVE",
                                                                       format));
                }
            }

            PCMDataInfo pcmInfo = new PCMDataInfo();

            // We need at least the 'data' and the 'fmt ' chunks
            bool foundWavDataChunk = false;
            bool foundWavFormatChunk = false;

            // We memorize the position of the actual PCM data in the stream,
            // so we can seek back, if needed (normally, this never happens as the 'data' chunk
            // is always the last one. However the WAV format does not mandate the order of chunks so...)
            long wavDataChunkPosition = -1;

            //loop when there's at least 2x4=8 bytes to read (Chunk ID & Chunk Data Size)
            while (input.Position + 8 <= wavEndPos)
            {
                // 4 bytes
                string chunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));

                // 4 bytes
                uint chunkSize = rd.ReadUInt32();

                // Ensures the given size fits within the actual stream
                if (input.Position + chunkSize > wavEndPos)
                {
                    throw new InvalidDataFormatException(String.Format(
                                                                       "ChunkId {0} does not fit in RIFF chunk",
                                                                       chunkId));
                }

                switch(chunkId)
                {
                    case "fmt ":
                        {
                            // The default information fields fit within 16 bytes
                            int extraFormatBytes = (int)chunkSize - 16;
                            
                            // Compression code (2 bytes)
                            ushort compressionCode = rd.ReadUInt16();

                            // Number of channels (2 bytes)
                            ushort numChannels = rd.ReadUInt16();
                            if (numChannels == 0)
                            {
                                throw new InvalidDataFormatException("0 channels of audio is not supported");
                            }

                            // Sample rate (4 bytes)
                            uint sampleRate = rd.ReadUInt32();

                            // Average bytes per second, aka byte-rate (4 bytes)
                            uint byteRate = rd.ReadUInt32();

                            // Block align (2 bytes)
                            ushort blockAlign = rd.ReadUInt16();

                            // Significant bits per sample, aka bit-depth (2 bytes)
                            ushort bitDepth = rd.ReadUInt16();

                            if (compressionCode != 0 && extraFormatBytes > 0)
                            {
                                // 	Extra format bytes (2 bytes)
                                uint extraBytes = rd.ReadUInt16();
                                if (extraBytes > 0)
                                {
                                    System.Diagnostics.Debug.Assert(extraFormatBytes == extraBytes);

                                    // Skip (we ignore the extra information in this chunk field)
                                    rd.ReadBytes((int) extraBytes);

                                    // check word-alignment
                                    if ((extraBytes % 2) != 0)
                                    {
                                        rd.ReadByte();
                                    }
                                }
                            }

                            if ((bitDepth % 8) != 0)
                            {
                                throw new InvalidDataFormatException(String.Format(
                                                                                   "Invalid number of bits per sample {0:0} - must be a mulitple of 8",
                                                                                   bitDepth));
                            }
                            if (blockAlign != (numChannels * bitDepth / 8))
                            {
                                throw new InvalidDataFormatException(String.Format(
                                                                                   "Invalid block align {0:0} - expected {1:0}",
                                                                                   blockAlign, numChannels * bitDepth / 8));
                            }
                            if (byteRate != sampleRate * blockAlign)
                            {
                                throw new InvalidDataFormatException(String.Format(
                                                                                   "Invalid byte rate {0:0} - expected {1:0}",
                                                                                   byteRate, sampleRate * blockAlign));
                            }
                            pcmInfo.BitDepth = bitDepth;
                            pcmInfo.NumberOfChannels = numChannels;
                            pcmInfo.SampleRate = sampleRate;

                            foundWavFormatChunk = true;
                            break;
                        }
                    case "data":
                        {
                            if (input.Position + chunkSize > wavEndPos)
                            {
                                throw new InvalidDataFormatException(String.Format(
                                                                                   "ChunkId {0} does not fit in RIFF chunk",
                                                                                   "data"));
                            }
                            pcmInfo.DataLength = chunkSize;
                            foundWavDataChunk = true;
                            wavDataChunkPosition = input.Position;

                            // ensure we go past the PCM data, in case there are following chunks.
                            // (it's an unlikely scenario, but it's allowed by the WAV spec.)
                            input.Seek(chunkSize, SeekOrigin.Current);
                            break;
                        }
                    case "fact":
                        {
                            if (chunkSize == 4)
                            {
                                // 4 bytes
                                uint totalNumberOfSamples = rd.ReadUInt32();
                                // This value is unused, we are just reading it for debugging as we noticed that
                                // the WAV files generated by the Microsoft Audio Recorder
                                // contain the 'fact' chunk with this information. Most other recordings
                                // only contain the 'data' and 'fmt ' chunks.
                            }
                            else
                            {
                                rd.ReadBytes((int)chunkSize);
                            }
                            break;
                        }
                    case "wavl":
                    case "slnt":
                    case "cue ":
                    case "plst":
                    case "list":
                    case "labl":
                    case "note":
                    case "ltxt":
                    case "smpl":
                    case "inst":
                    default:
                        {
                            // Unsupported FOURCC codes, we skip.
                            rd.ReadBytes((int)chunkSize);
                            break;
                        }
                }
            }

            if (!foundWavDataChunk)
            {
                throw new InvalidDataFormatException("WAV 'data' chunk was not found !");
            }
            if (!foundWavFormatChunk)
            {
                throw new InvalidDataFormatException("WAV 'fmt ' chunk was not found !");
            }
            if (input.Position != wavDataChunkPosition)
            {
                input.Seek(wavDataChunkPosition, SeekOrigin.Begin);
            }
            return pcmInfo;
        }


        /// <summary>
        /// Compares the data in two data streams for equality
        /// </summary>
        /// <param name="s1">The first </param>
        /// <param name="s2"></param>
        /// <param name="length">The length of the data to compare</param>
        /// <returns>A <see cref="bool"/> indicating data equality</returns>
        public static bool CompareStreamData(Stream s1, Stream s2, int length)
        {
            byte[] d1 = new byte[length];
            byte[] d2 = new byte[length];
            if (s1.Read(d1, 0, length) != length) return false;
            if (s2.Read(d2, 0, length) != length) return false;
            for (int i = 0; i < length; i++)
            {
                if (d1[i] != d2[i])
                {
                    return false;
                }
            }
            return true;
        }

        #region IXukAble Members

        /// <summary>
        /// Reads the attributes of a PCMDataInfo xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
        protected override void XukInAttributes(System.Xml.XmlReader source)
        {
            base.XukInAttributes(source);
            string attr = source.GetAttribute("dataLength");
            if (attr == null)
            {
                throw new exception.XukException("Attribute DataLength is missing");
            }
            uint dl;
            if (!UInt32.TryParse(attr, out dl))
            {
                throw new exception.XukException(String.Format(
                                                     "Attribute DataLength value {0} is not an unsigned integer",
                                                     attr));
            }
            DataLength = dl;
        }

        /// <summary>
        /// Writes the attributes of a PCMDataInfo element
        /// </summary>
        /// <param name="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(System.Xml.XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString("dataLength", DataLength.ToString());
            base.XukOutAttributes(destination, baseUri);
        }

        #endregion

        #region IValueEquatable<PCMDataInfo> Members

        /// <summary>
        /// Determines if <c>this</c> has the same value as a given other <see cref="PCMDataInfo"/>
        /// </summary>
        /// <param name="other">The given other PCMDataInfo with which to compare</param>
        /// <returns>A <see cref="bool"/> indicating value equality</returns>
        public bool ValueEquals(PCMDataInfo other)
        {
            if (!base.ValueEquals(other)) return false;
            if (DataLength != other.DataLength) return false;
            return true;
        }

        #endregion
    }
}