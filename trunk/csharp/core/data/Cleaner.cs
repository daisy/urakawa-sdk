using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using AudioLib;
using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.ExternalFiles;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.data.utilities;
using urakawa.media.timing;

namespace urakawa.data
{
    public class Cleaner : DualCancellableProgressReporter
    {
        private readonly Presentation m_Presentation;
        private readonly string m_FullPathToDeletedDataFolder;
        private readonly double m_cleanAudioMaxFileMegaBytes;
        private bool m_enableFileDataProviderPreservation;

        private readonly bool m_isNET2;

        public Cleaner(Presentation presentation, string fullPathToDeletedDataFolder, double cleanAudioMaxFileMegaBytes, bool enableFileDataProviderPreservation)
        {
            m_Presentation = presentation;
            m_FullPathToDeletedDataFolder = fullPathToDeletedDataFolder;
            m_cleanAudioMaxFileMegaBytes = cleanAudioMaxFileMegaBytes;
            m_enableFileDataProviderPreservation = enableFileDataProviderPreservation;


            //string coreLibVersion = null;
            //string name = "";
            //foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (item.GlobalAssemblyCache)
            //    {
            //        if (!string.IsNullOrEmpty(item.FullName)
            //            && item.FullName.Contains("mscorlib"))
            //        {
            //            name = item.FullName;
            //            coreLibVersion = item.ImageRuntimeVersion;
            //            break;
            //        }
            //    }
            //}
            //m_isNET2 = !string.IsNullOrEmpty(coreLibVersion) && coreLibVersion.Contains("v2.")
            //    || name.Contains("Version=2");
        }

        public override void DoWork()
        {
            try
            {
                Cleanup();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new Exception("Cleanup", ex);
            }
        }

        private List<DataProvider> m_fullyUsedFileDataProviders = null;
        private Dictionary<DataProvider, List<WavClip>> m_FileDataProvidersWavClipMap = null;
        private Dictionary<DataProvider, List<WavClip>> m_FileDataProvidersHolesMap = null;

        private void generateDataProviderWavClipHolesMap(List<MediaData> usedMediaData)
        {
            if (m_fullyUsedFileDataProviders != null)
            {
                return;
            }

            // List of DataProviders (WAV files) that are entirely used (no unused gaps)
            m_fullyUsedFileDataProviders = new List<DataProvider>();

            // Associate each DataProvider (WAV file) with a list of potentially-overlapping and non-ordered WavClips (begin-end time range)
            // that represent actual file usage.
            // Note that those WavClips may belong to different WavAudioMediaData, as FileDataProviders can be reused / shared.
            m_FileDataProvidersWavClipMap = new Dictionary<DataProvider, List<WavClip>>();

            // Associate each DataProvider (WAV file) with an ordered list of non-overlapping WavClips (begin-end time range)
            // that represent holes (unused gaps).
            // This data is directly generated from m_FileDataProvidersWavClipMap
            // Typically, this data is used to determine what raw PCM ranges can be discarded, 
            // by means of rebuilding the WAV file from the parts that are in use.
            // Note that several files may be merged together to decrease filesystem I/O (network storage prefers larger fewer files)
            m_FileDataProvidersHolesMap = new Dictionary<DataProvider, List<WavClip>>();

            foreach (MediaData md in usedMediaData)
            {
                if (md is WavAudioMediaData)
                {
                    WavAudioMediaData wMd = (WavAudioMediaData)md;

                    // TODO: wamd.mWavClips should not be exposed publicly
                    foreach (WavClip clip in wMd.mWavClips)
                    {
                        if (clip.AppData == null)
                        {
                            clip.AppData = wMd; // back-reference is memory-leak safe, because WavClips are discarded, never reused.
                        }
                        else
                        {
#if DEBUG
                            DebugFix.Assert(clip.AppData is WavAudioMediaData);
                            DebugFix.Assert(clip.AppData == wMd);
#endif
                        }

                        // see WavClip.Copy() => FileDataProviders can be reused
                        // (underlying WAV file is unmutable, this avoids redundant multiple identical instances in DataProviderManager,
                        // and large resulting XUK serialization)
                        // so, clip.DataProvider may already be present in m_fullyUsedFileDataProviders and m_FileDataProvidersWavClipMap,
                        // but we go ahead anyway to update the map

                        List<WavClip> list;
                        m_FileDataProvidersWavClipMap.TryGetValue(clip.DataProvider, out list);
                        if (list == null)
                        {
                            list = new List<WavClip>();
                            m_FileDataProvidersWavClipMap.Add(clip.DataProvider, list);
                        }
                        if (!list.Contains(clip))
                        {
                            list.Add(clip);
                        }
#if DEBUG
                        // Unlike FileDataProviders (which can be resused, see comment above),
                        // WavClip instances are unique.
                        else
                        {
                            Debugger.Break();
                        }
#endif

                        // ensures init the internal FileDataProvider.AppData cache of PCM format and duration
                        Time dur = clip.MediaDuration;
#if DEBUG
                        DebugFix.Assert(clip.DataProvider.AppData != null);
                        DebugFix.Assert(clip.DataProvider.AppData is WavClip.PcmFormatAndTime);
#endif

                        if (clip.ClipBegin.IsEqualTo(Time.Zero) && clip.ClipEnd.IsEqualTo(dur))
                        {
                            if (!m_fullyUsedFileDataProviders.Contains(clip.DataProvider))
                            {
                                m_fullyUsedFileDataProviders.Add(clip.DataProvider);
                            }
                        }
                    }
                }
                else
                {
#if DEBUG
                    bool first = true;
#endif
                    foreach (DataProvider dp in md.UsedDataProviders)
                    {
#if DEBUG
                        DebugFix.Assert(first);
                        first = false;
#endif
                        //if (!m_fullyUsedFileDataProviders.Contains(dp))
                        //{
                        //    m_fullyUsedFileDataProviders.Add(dp);
                        //}
                    }
                }
            }

            if (m_FileDataProvidersWavClipMap.Count <= 0)
            {
                //#if DEBUG
                //                Debugger.Break();
                //#endif
                return;
            }

            foreach (DataProvider dp in m_FileDataProvidersWavClipMap.Keys)
            {
#if DEBUG
                DebugFix.Assert(dp.AppData != null);
                DebugFix.Assert(dp.AppData is WavClip.PcmFormatAndTime);
#endif

                WavClip whole = new WavClip(dp); // From beginning to end
#if DEBUG
                DebugFix.Assert(whole.ClipEnd.IsEqualTo(((WavClip.PcmFormatAndTime)dp.AppData).mTime));
#endif
                List<WavClip> holes = new List<WavClip>();
                holes.Add(whole);

                List<WavClip> wavClips = m_FileDataProvidersWavClipMap[dp];
                foreach (WavClip clip in wavClips)
                {
#if DEBUG
                    DebugFix.Assert(clip.DataProvider == dp);

                    DebugFix.Assert(clip.DataProvider.AppData != null);
                    DebugFix.Assert(clip.DataProvider.AppData is WavClip.PcmFormatAndTime);

                    DebugFix.Assert(clip.MediaDuration.IsEqualTo(((WavClip.PcmFormatAndTime)clip.DataProvider.AppData).mTime));
#endif
                    wavClipRangeMerge(holes, clip);
                }

                if (holes.Count == 0)
                {
                    if (!m_fullyUsedFileDataProviders.Contains(dp))
                    {
                        m_fullyUsedFileDataProviders.Add(dp);
                    }
                }
                else
                {
                    m_FileDataProvidersHolesMap.Add(dp, holes);
                }
            }

#if DEBUG
            foreach (DataProvider dp in m_fullyUsedFileDataProviders)
            {
                DebugFix.Assert(m_FileDataProvidersWavClipMap.ContainsKey(dp));
                DebugFix.Assert(!m_FileDataProvidersHolesMap.ContainsKey(dp));
            }

            foreach (DataProvider dp in m_FileDataProvidersHolesMap.Keys)
            {
                DebugFix.Assert(m_FileDataProvidersWavClipMap.ContainsKey(dp));
                DebugFix.Assert(!m_fullyUsedFileDataProviders.Contains(dp));
            }

            // REMOVED AS DEBUG OUTPUT KILLS PERFORMANCE
            //Console.WriteLine(" ");
            //Console.WriteLine(" ");
            //Console.WriteLine("## CLEANUP -- fully-used WAV files:");
            //foreach (DataProvider dp in m_fullyUsedFileDataProviders)
            //{
            //    Console.Write("[" + ((FileDataProvider)dp).DataFileRelativePath + "] ");
            //}
            //Console.WriteLine(" ");
            //Console.WriteLine(" ");
            //Console.WriteLine("## CLEANUP -- WAV files with unused gaps:");
            //foreach (DataProvider dp in m_FileDataProvidersHolesMap.Keys)
            //{
            //    Console.WriteLine("[" + ((FileDataProvider)dp).DataFileRelativePath + "] ");
            //    foreach (WavClip hole in m_FileDataProvidersHolesMap[dp])
            //    {
            //        Console.WriteLine(hole.ClipBegin.Format_Standard() + " - " + hole.ClipEnd.Format_Standard());
            //    }
            //    Console.WriteLine(" ");
            //}
#endif
        }

        // Must be greater than AudioLibPCMFormat.MILLISECONDS_TOLERANCE, otherwise Time comparison API uses this tolerance threshold for equality checks!
        private Time m_minimumDurationToPreserveHole = new Time(AudioLibPCMFormat.MILLISECONDS_TOLERANCE * 2 * AudioLibPCMFormat.TIME_UNIT); // 5ms * 2

        private void wavClipRangeMerge(List<WavClip> holes, WavClip clip)
        {
            if (holes.Count == 0)
            {
                return;
            }

            List<WavClip> holesToRemove = new List<WavClip>();
            List<WavClip> holesToAdd = new List<WavClip>();
            foreach (WavClip hole in holes)
            {
                // clip covers hole entirely
                if (clip.ClipBegin.IsLessThanOrEqualTo(hole.ClipBegin) && clip.ClipEnd.IsGreaterThanOrEqualTo(hole.ClipEnd))
                {
                    holesToRemove.Add(hole);
                    continue;
                }

                bool leftClipEdgeIsInsideHole = clip.ClipBegin.IsGreaterThanOrEqualTo(hole.ClipBegin)
                                                && clip.ClipBegin.IsLessThan(hole.ClipEnd);
                bool rightClipEdgeIsInsideHole = clip.ClipEnd.IsGreaterThan(hole.ClipBegin)
                                                && clip.ClipEnd.IsLessThanOrEqualTo(hole.ClipEnd);

                // clip is inside hole, or possibly overlapping / crossing either the left side, or the right side (but not both)
                if (leftClipEdgeIsInsideHole || rightClipEdgeIsInsideHole)
                {
                    holesToRemove.Add(hole);

                    Time begin1 = new Time(hole.ClipBegin);
                    Time end1 = new Time(clip.ClipBegin);
                    Time dur1 = end1.GetDifference(begin1);
                    if (end1.IsGreaterThan(begin1) && dur1.IsGreaterThanOrEqualTo(m_minimumDurationToPreserveHole))
                    {
                        holesToAdd.Add(new WavClip(clip.DataProvider, begin1, end1));
                    }

                    Time begin2 = new Time(clip.ClipEnd);
                    Time end2 = new Time(hole.ClipEnd);
                    Time dur2 = end2.GetDifference(begin2);
                    if (end2.IsGreaterThan(begin2) && dur2.IsGreaterThanOrEqualTo(m_minimumDurationToPreserveHole))
                    {
                        holesToAdd.Add(new WavClip(clip.DataProvider, begin2, end2));
                    }
                }
            }

            foreach (WavClip hole in holesToRemove)
            {
                holes.Remove(hole);
            }

            holes.AddRange(holesToAdd);
        }

        public class DataProviderUsedSizeComparer : IComparer<DataProvider>
        {
            private List<DataProvider> m_fullyUsedFileDataProviders = null;
            private Dictionary<DataProvider, List<WavClip>> m_FileDataProvidersWavClipMap = null;
            private Dictionary<DataProvider, List<WavClip>> m_FileDataProvidersHolesMap = null;

            public DataProviderUsedSizeComparer(
                List<DataProvider> fullyUsedFileDataProviders,
                Dictionary<DataProvider, List<WavClip>> FileDataProvidersWavClipMap,
                Dictionary<DataProvider, List<WavClip>> FileDataProvidersHolesMap)
            {
                m_fullyUsedFileDataProviders = fullyUsedFileDataProviders;
                m_FileDataProvidersWavClipMap = FileDataProvidersWavClipMap;
                m_FileDataProvidersHolesMap = FileDataProvidersHolesMap;
            }

            public int Compare(DataProvider dp1, DataProvider dp2)
            {
                FileDataProvider fdp1 = dp1 as FileDataProvider;
                FileDataProvider fdp2 = dp2 as FileDataProvider;

                if (fdp1 == null || fdp2 == null || fdp1.AppData == null || fdp2.AppData == null || !(fdp1.AppData is WavClip.PcmFormatAndTime) || !(fdp2.AppData is WavClip.PcmFormatAndTime))
                {
#if DEBUG
                    Debugger.Break();
#endif
                    return 0; // equal
                }

                long bytes1 = ((WavClip.PcmFormatAndTime)fdp1.AppData).mFormat.ConvertTimeToBytes(
                    ((WavClip.PcmFormatAndTime)fdp1.AppData).mTime.AsLocalUnits);

                long bytes2 = ((WavClip.PcmFormatAndTime)fdp2.AppData).mFormat.ConvertTimeToBytes(
                    ((WavClip.PcmFormatAndTime)fdp2.AppData).mTime.AsLocalUnits);

                if (!m_fullyUsedFileDataProviders.Contains(fdp1))
                {
                    foreach (DataProvider dp in m_FileDataProvidersHolesMap.Keys)
                    {
                        if (dp == dp1)
                        {
                            long bytesToRemove = 0;

                            List<WavClip> holes = m_FileDataProvidersHolesMap[dp];
                            foreach (WavClip hole in holes)
                            {
                                bytesToRemove += ((WavClip.PcmFormatAndTime)fdp1.AppData).mFormat.ConvertTimeToBytes(
                                    hole.Duration.AsLocalUnits);
                            }

                            bytes1 -= bytesToRemove;

                            break;
                        }
                    }
                }

                if (!m_fullyUsedFileDataProviders.Contains(fdp2))
                {
                    foreach (DataProvider dp in m_FileDataProvidersHolesMap.Keys)
                    {
                        if (dp == dp2)
                        {
                            long bytesToRemove = 0;

                            List<WavClip> holes = m_FileDataProvidersHolesMap[dp];
                            foreach (WavClip hole in holes)
                            {
                                bytesToRemove += ((WavClip.PcmFormatAndTime)fdp2.AppData).mFormat.ConvertTimeToBytes(
                                    hole.Duration.AsLocalUnits);
                            }

                            bytes2 -= bytesToRemove;

                            break;
                        }
                    }
                }

                return (int)(bytes2 - bytes1);
            }
        }

        private void Cleanup_EnableFileDataProviderPreservation(List<MediaData> usedMediaData, List<DataProvider> usedDataProviders)
        {
            DataProvider curentAudioDataProvider = null;
            long currentFileDataProviderIndex = 0;
            string prefixFormat = @"{0:D4}_";

            long nMaxBytes = (m_cleanAudioMaxFileMegaBytes <= 0 ? 0 : (long)Math.Round(m_cleanAudioMaxFileMegaBytes * 1024 * 1024));
            long currentBytes = 0;
            FileDataProvider currentFileDataProvider = null;
            Stream currentFileDataProviderOutputStream = null;
            PCMFormatInfo pCMFormat = null;
            ulong riffHeaderLength = 0;

            int progress = 10;
            int index = 0;

            generateDataProviderWavClipHolesMap(usedMediaData);

            IComparer<DataProvider> comparer = new DataProviderUsedSizeComparer(m_fullyUsedFileDataProviders,
                m_FileDataProvidersWavClipMap, m_FileDataProvidersHolesMap);

            // TODO: how to detect .NET2 at compile time for Obi? (OrderBy() is NET3+)
#if false //NET40
            Func<DataProvider, DataProvider> keySelector = delegate (DataProvider dp) { return dp; };
            IOrderedEnumerable<DataProvider> orderedDPs = m_FileDataProvidersWavClipMap.Keys.OrderBy(keySelector,
                comparer);
#else
            List<DataProvider> orderedDPs = new List<DataProvider>(m_FileDataProvidersWavClipMap.Keys);
            orderedDPs.Sort(comparer);
#endif
            index = 0;

            foreach (DataProvider dp in orderedDPs)
            {
                index++;
                progress = 100 * index / m_FileDataProvidersWavClipMap.Keys.Count;

                reportProgress_Throttle(progress, index + " / " + m_FileDataProvidersWavClipMap.Keys.Count);


                if (RequestCancellation) return;


                WavClip.PcmFormatAndTime dpAppData = ((WavClip.PcmFormatAndTime)((FileDataProvider)dp).AppData);

#if DEBUG
                // REMOVED AS DEBUG OUTPUT KILLS PERFORMANCE
                //Console.WriteLine("[" + ((FileDataProvider)dp).DataFileRelativePath + "] ");
                //Console.WriteLine("[" +
                //                  dpAppData.mTime.Format_Standard
                //                      () + "] ");
#endif

                bool isFullyUsed = m_fullyUsedFileDataProviders.Contains(dp);
                List<WavClip> listOfHoles = null;
                if (!isFullyUsed)
                {
                    listOfHoles = m_FileDataProvidersHolesMap[dp];
                }

                AudioLibPCMFormat audioPCMFormat = dpAppData.mFormat;
                if (pCMFormat == null)
                {
                    pCMFormat = new PCMFormatInfo(audioPCMFormat);
                }

                long dpLength = dpAppData.mFormat.ConvertTimeToBytes(dpAppData.mTime.AsLocalUnits);
                if (!isFullyUsed)
                {
                    foreach (WavClip hole in listOfHoles)
                    {
                        long dur = audioPCMFormat.ConvertTimeToBytes(hole.Duration.AsLocalUnits);
                        dpLength -= dur;

#if DEBUG
                        long begin = audioPCMFormat.ConvertTimeToBytes(hole.ClipBegin.AsLocalUnits);
                        long end = audioPCMFormat.ConvertTimeToBytes(hole.ClipEnd.AsLocalUnits);
                        //dpLength -= (end - begin);
                        long dur2 = end - begin;
                        long diff = dur2 - dur;
                        DebugFix.Assert(Math.Abs(diff) <= audioPCMFormat.BlockAlign);
#endif
                    }
                }

                long dpLengthNext = 0;
                int i = index - 1; // index is one-based, no zero-based!
                if (i < (orderedDPs.Count - 1))
                {
                    DataProvider dpNext = orderedDPs[i + 1];

                    WavClip.PcmFormatAndTime dpAppDataNext = ((WavClip.PcmFormatAndTime)((FileDataProvider)dpNext).AppData);

                    bool isFullyUsedNext = m_fullyUsedFileDataProviders.Contains(dpNext);
                    List<WavClip> listOfHolesNext = null;
                    if (!isFullyUsedNext)
                    {
                        listOfHolesNext = m_FileDataProvidersHolesMap[dpNext];
                    }

                    dpLengthNext = dpAppDataNext.mFormat.ConvertTimeToBytes(dpAppDataNext.mTime.AsLocalUnits);
                    if (!isFullyUsedNext)
                    {
                        foreach (WavClip hole in listOfHolesNext)
                        {
                            long dur = audioPCMFormat.ConvertTimeToBytes(hole.Duration.AsLocalUnits);
                            dpLengthNext -= dur;

#if DEBUG
                            long begin = audioPCMFormat.ConvertTimeToBytes(hole.ClipBegin.AsLocalUnits);
                            long end = audioPCMFormat.ConvertTimeToBytes(hole.ClipEnd.AsLocalUnits);
                            //dpLength -= (end - begin);
                            long dur2 = end - begin;
                            long diff = dur2 - dur;
                            DebugFix.Assert(Math.Abs(diff) <= audioPCMFormat.BlockAlign);
#endif
                        }
                    }
                }

                // no batching of WAV files, no ForceSingleDataProvider either!
                //bool noWavCombine = nMaxBytes <= 0;

                bool isBatchingDisabled = nMaxBytes <= 0;
                bool isFileTooLarge = dpLength >= nMaxBytes;
                bool isFileNoFit = (currentBytes + dpLength) >= nMaxBytes;
                bool isFileLast = dpLengthNext == 0; //LAST => index == (orderedDPs.Count - 1)
                bool isFileNextNoFit = (dpLength + dpLengthNext) >= nMaxBytes;

                if (currentFileDataProvider != null &&
                    (
                    isBatchingDisabled
                    || isFileTooLarge
                    || isFileNoFit
                    )
                )
                {
                    currentFileDataProviderOutputStream.Position = 0;
                    try
                    {
                        riffHeaderLength =
                            audioPCMFormat.RiffHeaderWrite(currentFileDataProviderOutputStream,
                                (uint)currentBytes);
                    }
                    finally
                    {
                        currentFileDataProviderOutputStream.Close();
                        currentFileDataProviderOutputStream = null;
                    }

                    currentFileDataProvider = null;
                    currentBytes = 0;
                    isFileNoFit = isFileTooLarge;
                }

                // PRESERVE
                if (isFullyUsed &&
                    (isBatchingDisabled
                    || isFileTooLarge
                    || isFileNoFit
                    || (isFileLast && (currentFileDataProvider == null))
                    || isFileNextNoFit
                    )
                    )
                {
                    ((FileDataProvider)dp).Rename(String.Format(prefixFormat, ++currentFileDataProviderIndex));

                    if (!usedDataProviders.Contains(dp))
                    {
                        usedDataProviders.Add(dp);
                    }
                }
                else // partial (holes), or isFullyUsed that fits within nMaxBytes (minus currentBytes already in output stream)
                {
                    if (currentFileDataProvider == null)
                    {
                        currentBytes = 0;
                        currentFileDataProvider =
                            (FileDataProvider)
                                m_Presentation.DataProviderFactory.Create(
                                    DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                        currentFileDataProvider.SetNamePrefix(String.Format(prefixFormat,
                            ++currentFileDataProviderIndex));

                        currentFileDataProviderOutputStream = currentFileDataProvider.OpenOutputStream();
                        try
                        {
                            riffHeaderLength =
                                audioPCMFormat.RiffHeaderWrite(currentFileDataProviderOutputStream, (uint)0);
                        }
                        catch (Exception ex)
                        {
                            currentFileDataProviderOutputStream.Close();
                            currentFileDataProviderOutputStream = null;

                            throw ex;
                        }


                        Object appData = new WavClip.PcmFormatAndTime(
                                new AudioLibPCMFormat(audioPCMFormat.NumberOfChannels, audioPCMFormat.SampleRate, audioPCMFormat.BitDepth),
                                new Time(0)
                                );
                        currentFileDataProvider.AppData = appData;
                    }


                    if (!usedDataProviders.Contains(currentFileDataProvider))
                    {
                        usedDataProviders.Add(currentFileDataProvider);
                    }

                    Stream stream = null;
                    uint dataLength;
                    try
                    {
                        stream = dp.OpenInputStream();
                        AudioLibPCMFormat.RiffHeaderParse(stream, out dataLength);
#if DEBUG
                        DebugFix.Assert(stream.Position == 44 || stream.Position == 46);
#endif
                    }
                    catch (Exception ex)
                    {
                        currentFileDataProviderOutputStream.Close();
                        currentFileDataProviderOutputStream = null;

                        if (stream != null)
                        {
                            stream.Close();
                        }
                        stream = null;

                        throw ex;
                    }

                    long startPosition = stream.Position;
                    long availableToRead = stream.Length - startPosition;

                    long prevByteOffset = 0;

                    long currentBytesBeforeAppend = currentBytes;

                    if (isFullyUsed)
                    {
                        try
                        {
                            //currentFileDataProvider.AppendData(stream, availableToRead);

#if RIFF_HEADER_INCREMENTAL_MAINTAIN
                                currentFileDataProviderOutputStream.Seek(0, SeekOrigin.End);
#endif //RIFF_HEADER_INCREMENTAL_MAINTAIN

                            const uint BUFFER_SIZE = 1024 * 300; // 300 KB MAX BUFFER
                            StreamUtils.Copy(stream, (ulong)availableToRead, currentFileDataProviderOutputStream,
                                BUFFER_SIZE);
#if DEBUG
                            long length = (long)riffHeaderLength + currentBytes + availableToRead;
                            DebugFix.Assert(currentFileDataProviderOutputStream.Length == length);
#endif
                        }
                        catch (Exception ex)
                        {
                            currentFileDataProviderOutputStream.Close();
                            currentFileDataProviderOutputStream = null;

                            stream.Close();
                            stream = null;

                            throw ex;
                        }


#if RIFF_HEADER_INCREMENTAL_MAINTAIN
                        currentFileDataProviderOutputStream.Position = 0;
                        try
                        {
                            riffHeaderLength = wMd.PCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream, (uint)(currentBytes + availableToRead));
                        }
                        catch (Exception ex)
                        {
                            currentFileDataProviderOutputStream.Close();
                            currentFileDataProviderOutputStream = null;

                            throw ex;
                        }
#endif //RIFF_HEADER_INCREMENTAL_MAINTAIN

                        Time insertedDuration = new Time(audioPCMFormat.ConvertBytesToTime(availableToRead));

                        Object appData = currentFileDataProvider.AppData;
                        if (appData != null)
                        {
                            if (appData is WavClip.PcmFormatAndTime)
                            {
                                ((WavClip.PcmFormatAndTime)appData).mTime.Add(insertedDuration);
                            }
#if DEBUG
                            else
                            {
                                Debugger.Break();
                            }
#endif
                        }
                        else
                        {
#if DEBUG
                            Debugger.Break();
#endif
                            DebugFix.Assert(currentBytes == 0);

                            appData = new WavClip.PcmFormatAndTime(
                                new AudioLibPCMFormat(audioPCMFormat.NumberOfChannels, audioPCMFormat.SampleRate,
                                    audioPCMFormat.BitDepth),
                                insertedDuration
                                );
                            currentFileDataProvider.AppData = appData;
                        }

                        currentBytes += availableToRead;
                    }
                    else
                    {
                        foreach (WavClip hole in listOfHoles)
                        {
                            bool isLast = listOfHoles[listOfHoles.Count - 1] == hole;

                            long nextByteOffset = audioPCMFormat.ConvertTimeToBytes(hole.ClipBegin.AsLocalUnits);

                            if (hole.ClipBegin.AsLocalUnits == 0)
                            {
                                prevByteOffset = audioPCMFormat.ConvertTimeToBytes(hole.ClipEnd.AsLocalUnits);

                                if (!isLast)
                                {
                                    continue;
                                }

                                isLast = false;
                                nextByteOffset = stream.Length - startPosition;
                            }

                            // lookahead
                            oneLast:

                            stream.Position = startPosition + prevByteOffset;
                            //stream.Seek(startPosition + prevByteOffset, SeekOrigin.Begin);

                            availableToRead = nextByteOffset - prevByteOffset;

                            try
                            {
                                //currentFileDataProvider.AppendData(stream, availableToRead);

#if RIFF_HEADER_INCREMENTAL_MAINTAIN
                                currentFileDataProviderOutputStream.Seek(0, SeekOrigin.End);
#endif //RIFF_HEADER_INCREMENTAL_MAINTAIN

                                const uint BUFFER_SIZE = 1024 * 300; // 300 KB MAX BUFFER
                                StreamUtils.Copy(stream, (ulong)availableToRead, currentFileDataProviderOutputStream,
                                    BUFFER_SIZE);
#if DEBUG
                                long length = (long)riffHeaderLength + currentBytes + availableToRead;
                                DebugFix.Assert(currentFileDataProviderOutputStream.Length == length);
#endif
                            }
                            catch (Exception ex)
                            {
                                currentFileDataProviderOutputStream.Close();
                                currentFileDataProviderOutputStream = null;

                                stream.Close();
                                stream = null;

                                throw ex;
                            }


#if RIFF_HEADER_INCREMENTAL_MAINTAIN
                            currentFileDataProviderOutputStream.Position = 0;
                            try
                            {
                                riffHeaderLength = wMd.PCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream, (uint)(currentBytes + availableToRead));
                            }
                            catch (Exception ex)
                            {
                                currentFileDataProviderOutputStream.Close();
                                currentFileDataProviderOutputStream = null;

                                throw ex;
                            }
#endif //RIFF_HEADER_INCREMENTAL_MAINTAIN

                            Time insertedDuration = new Time(audioPCMFormat.ConvertBytesToTime(availableToRead));

                            Object appData = currentFileDataProvider.AppData;
                            if (appData != null)
                            {
                                if (appData is WavClip.PcmFormatAndTime)
                                {
                                    ((WavClip.PcmFormatAndTime)appData).mTime.Add(insertedDuration);
                                }
#if DEBUG
                                else
                                {
                                    Debugger.Break();
                                }
#endif
                            }
                            else
                            {
#if DEBUG
                                Debugger.Break();
#endif
                                DebugFix.Assert(currentBytes == 0);

                                appData = new WavClip.PcmFormatAndTime(
                                    new AudioLibPCMFormat(audioPCMFormat.NumberOfChannels, audioPCMFormat.SampleRate,
                                        audioPCMFormat.BitDepth),
                                    insertedDuration
                                    );
                                currentFileDataProvider.AppData = appData;
                            }

                            currentBytes += availableToRead;

                            prevByteOffset = audioPCMFormat.ConvertTimeToBytes(hole.ClipEnd.AsLocalUnits);


                            if (isLast)
                            {
                                isLast = false;
                                nextByteOffset = stream.Length - startPosition;
                                //if (!audioPCMFormat.BytesAreEqualWithMillisecondsTolerance(prevByteOffset, nextByteOffset))
                                if (prevByteOffset < nextByteOffset)
                                {
                                    goto oneLast;
                                }
                            }
                        }
                    }

                    stream.Close();
                    stream = null;



                    foreach (WavClip clip in m_FileDataProvidersWavClipMap[dp])
                    {
                        Time timeOffset_RemovedHoles = new Time(0);

                        if (!isFullyUsed)
                        {
                            foreach (WavClip hole in m_FileDataProvidersHolesMap[dp])
                            {
                                if (hole.ClipBegin.IsLessThan(clip.ClipBegin))
                                {
                                    timeOffset_RemovedHoles.Add(hole.Duration);
                                }
                            }
                        }

                        Time timeOffset_currentBytesBeforeAppend = new Time(audioPCMFormat.ConvertBytesToTime(currentBytesBeforeAppend));

                        WavAudioMediaData wamd = (WavAudioMediaData)clip.AppData;

                        // TODO: wamd.mWavClips should not be exposed publicly
                        int iClip = wamd.mWavClips.IndexOf(clip);

                        wamd.mWavClips.RemoveAt(iClip);

                        WavClip newClip = new WavClip(currentFileDataProvider);
                        if (clip.ClipBegin != null)
                        {
                            Time newBegin = clip.ClipBegin.Copy();
                            newBegin.Substract(timeOffset_RemovedHoles);
                            newBegin.Add(timeOffset_currentBytesBeforeAppend);

                            newClip.ClipBegin = newBegin;
                        }
                        if (clip.ClipEnd != null)
                        {
                            Time newEnd = clip.ClipEnd.Copy();
                            newEnd.Substract(timeOffset_RemovedHoles);
                            newEnd.Add(timeOffset_currentBytesBeforeAppend);

                            newClip.ClipEnd = newEnd;
                        }

                        if (iClip >= (wamd.mWavClips.Count - 1))
                        {
                            wamd.mWavClips.Add(newClip);
                        }
                        else
                        {
                            wamd.mWavClips.Insert(iClip, newClip);
                        }
                    }
                }
            }

            if (currentFileDataProvider != null)
            {
                currentFileDataProviderOutputStream.Position = 0;
                try
                {
                    riffHeaderLength = pCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream, (uint)currentBytes);
                }
                finally
                {
                    currentFileDataProviderOutputStream.Close();
                    currentFileDataProviderOutputStream = null;
                }

                currentFileDataProvider = null;
            }


            foreach (MediaData md in usedMediaData)
            {
                if (md is WavAudioMediaData)
                {
#if DEBUG
                    foreach (DataProvider dp in md.UsedDataProviders)
                    {
                        DebugFix.Assert(usedDataProviders.Contains(dp));
                    }
#endif
                    continue;
                }

                foreach (DataProvider dp in md.UsedDataProviders)
                {
                    if (!usedDataProviders.Contains(dp))
                    {
                        usedDataProviders.Add(dp);
                    }
                }
            }
        }

        private void Cleanup_DefaultPreservePlaybackOrder(List<MediaData> usedMediaData, List<DataProvider> usedDataProviders)
        {
            DataProvider curentAudioDataProvider = null;
            long currentFileDataProviderIndex = 0;
            string prefixFormat = @"{0:D4}_";

            long nMaxBytes = (m_cleanAudioMaxFileMegaBytes <= 0 ? 0 : (long)Math.Round(m_cleanAudioMaxFileMegaBytes * 1024 * 1024));
            long currentBytes = 0;
            FileDataProvider currentFileDataProvider = null;
            Stream currentFileDataProviderOutputStream = null;
            PCMFormatInfo pCMFormat = null;
            ulong riffHeaderLength = 0;

            int progress = 10;
            int index = 0;

            // The usedMediaData list is ordered according to document order.
            // When m_cleanAudioMaxFileMegaBytes (nMaxBytes) is activated in user preferences,
            // the wav clips are concatenated sequentially (batched together to reduce the number of WAV files),
            // therefore preserving the playback order (and as the audio files are numbered, this allows checking directly in the filesystem view).
            // However, when m_enableFileDataProviderPreservation is activated in user preferences,
            // the audio order may not be maintained as the goal is to minimize filesystem I/O by leaving files untouched as much as possible.
            foreach (MediaData md in usedMediaData)
            {
                index++;
                progress = 100 * index / usedMediaData.Count;
                //reportProgress(progress, "[5]...");

                if (RequestCancellation) return;

                if (md is WavAudioMediaData)
                {
                    reportProgress_Throttle(progress, index + " / " + usedMediaData.Count);

                    WavAudioMediaData wMd = (WavAudioMediaData)md;

                    uint thisAudioByteLength =
                        (uint)wMd.PCMFormat.Data.ConvertTimeToBytes(wMd.AudioDuration.AsLocalUnits);

                    if (nMaxBytes <= 0 || thisAudioByteLength >= nMaxBytes)
                    {
                        wMd.ForceSingleDataProvider(true,
                            String.Format(prefixFormat, ++currentFileDataProviderIndex));
                    }
                    else
                    {
                        long nextSize = currentBytes + thisAudioByteLength;
                        if (nextSize > nMaxBytes)
                        {
                            if (currentFileDataProvider != null)
                            {
                                //Stream stream = currentFileDataProvider.OpenOutputStream();
                                currentFileDataProviderOutputStream.Position = 0;
                                try
                                {
                                    riffHeaderLength =
                                        wMd.PCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream,
                                            (uint)currentBytes);
                                }
                                finally
                                {
                                    currentFileDataProviderOutputStream.Close();
                                    currentFileDataProviderOutputStream = null;
                                }

                                currentFileDataProvider = null;
                                currentBytes = 0;
                            }
                        }

                        if (currentFileDataProvider == null)
                        {
                            currentBytes = 0;
                            currentFileDataProvider =
                                (FileDataProvider)
                                    m_Presentation.DataProviderFactory.Create(
                                        DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                            currentFileDataProvider.SetNamePrefix(String.Format(prefixFormat,
                                ++currentFileDataProviderIndex));

                            currentFileDataProviderOutputStream = currentFileDataProvider.OpenOutputStream();
                            try
                            {
                                riffHeaderLength =
                                    wMd.PCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream, (uint)0);
                            }
                            catch (Exception ex)
                            {
                                currentFileDataProviderOutputStream.Close();
                                currentFileDataProviderOutputStream = null;

                                throw ex;
                            }

                            pCMFormat = wMd.PCMFormat;
                        }

                        Stream stream = wMd.OpenPcmInputStream();

                        long availableToRead = stream.Length - stream.Position;

                        //DebugFix.Assert(availableToRead == thisAudioByteLength);
                        if (thisAudioByteLength != availableToRead)
                        {
#if DEBUG
                            long diff = thisAudioByteLength - availableToRead;
                            if (Math.Abs(diff) > 2)
                            {
                                Console.WriteLine(">> audio bytes diff: " + diff);
                                Debugger.Break();
                            }
#endif
                            thisAudioByteLength = (uint)availableToRead;
                        }

                        //bool okay = false;
                        try
                        {
                            //currentFileDataProvider.AppendData(stream, availableToRead);

#if RIFF_HEADER_INCREMENTAL_MAINTAIN
                            currentFileDataProviderOutputStream.Seek(0, SeekOrigin.End);
#endif //RIFF_HEADER_INCREMENTAL_MAINTAIN

                            const uint BUFFER_SIZE = 1024 * 300; // 300 KB MAX BUFFER
                            StreamUtils.Copy(stream, (ulong)availableToRead, currentFileDataProviderOutputStream,
                                BUFFER_SIZE);

#if DEBUG
                            long length = (long)riffHeaderLength + currentBytes + availableToRead;
                            DebugFix.Assert(currentFileDataProviderOutputStream.Length == length);
#endif
                            //okay = true;
                        }
                        catch (Exception ex)
                        {
                            currentFileDataProviderOutputStream.Close();
                            currentFileDataProviderOutputStream = null;

                            throw ex;
                        }
                        finally
                        {
                            stream.Close();
                            stream = null;
                        }
                        //if (okay)
                        //{
                        Object appData = currentFileDataProvider.AppData;
                        if (appData != null)
                        {
                            if (appData is WavClip.PcmFormatAndTime)
                            {
                                ((WavClip.PcmFormatAndTime)appData).mTime.Add(wMd.AudioDuration);

                                //DebugFix.Assert(currentBytes == ((WavClip.PcmFormatAndTime)appData).Bytes);
                                //((WavClip.PcmFormatAndTime)appData).Bytes = currentBytes + thisAudioByteLength;

                                //((WavClip.PcmFormatAndTime)appData).mFormat;
                            }
#if DEBUG
                            else
                            {
                                Debugger.Break();
                            }
#endif
                        }
                        else
                        {
                            DebugFix.Assert(currentBytes == 0);
                            Time dur = new Time(wMd.AudioDuration);
                            //DebugFix.Assert(currentBytes + thisAudioByteLength == wMd.PCMFormat.Data.ConvertTimeToBytes(dur.AsLocalUnits));
                            appData = new WavClip.PcmFormatAndTime(
                                new AudioLibPCMFormat(wMd.PCMFormat.Data.NumberOfChannels,
                                    wMd.PCMFormat.Data.SampleRate, wMd.PCMFormat.Data.BitDepth),
                                dur
                                //, currentBytes + thisAudioByteLength
                                );
                            currentFileDataProvider.AppData = appData;
                        }

#if RIFF_HEADER_INCREMENTAL_MAINTAIN
                        currentFileDataProviderOutputStream.Position = 0;
                        try
                        {
                            riffHeaderLength = wMd.PCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream, (uint)(currentBytes + thisAudioByteLength));
                        }
                        catch (Exception ex)
                        {
                            currentFileDataProviderOutputStream.Close();
                            currentFileDataProviderOutputStream = null;

                            throw ex;
                        }
#endif //RIFF_HEADER_INCREMENTAL_MAINTAIN

                        wMd.RemovePcmData(Time.Zero);

                        Time clipBegin = new Time(wMd.PCMFormat.Data.ConvertBytesToTime(currentBytes));
                        Time clipEnd =
                            new Time(wMd.PCMFormat.Data.ConvertBytesToTime(currentBytes + thisAudioByteLength));

                        wMd.AppendPcmData(currentFileDataProvider, clipBegin, clipEnd);

                        currentBytes += thisAudioByteLength;
                        //                            }
                        //                            else
                        //                            {
                        //#if DEBUG
                        //                                Debugger.Break();
                        //#endif
                        //                                currentFileDataProviderOutputStream.Position = 0;
                        //                                try
                        //                                {
                        //                                    riffHeaderLength =
                        //                                        wMd.PCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream,
                        //                                            (uint)currentBytes);
                        //                                }
                        //                                finally
                        //                                {
                        //                                    currentFileDataProviderOutputStream.Close();
                        //                                    currentFileDataProviderOutputStream = null;
                        //                                }
                        //                                currentFileDataProvider = null;
                        //                            }
                    }
                }

                // md may have changed, so we updated the DPs
                foreach (DataProvider dp in md.UsedDataProviders)
                {
                    if (RequestCancellation) return;

                    if (!usedDataProviders.Contains(dp))
                    {
                        usedDataProviders.Add(dp);
                    }
                }
            }


            if (currentFileDataProvider != null)
            {
                currentFileDataProviderOutputStream.Position = 0;
                try
                {
                    riffHeaderLength = pCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream, (uint)currentBytes);
                }
                finally
                {
                    currentFileDataProviderOutputStream.Close();
                    currentFileDataProviderOutputStream = null;
                }

                currentFileDataProvider = null;
            }
        }

        /// <summary>
        /// Removes any <see cref="MediaData"/> and <see cref="DataProvider"/>s that are not used by any <see cref="TreeNode"/> in the document tree
        /// or by any <see cref="Command"/> in the <see cref="undo.UndoRedoManager"/> stacks (undo/redo/transaction).
        /// or by ExternalFileData
        /// </summary>
        public void Cleanup()
        {
            //reportProgress(-1, "...");

            const int progressStep = 10;
            int progress = progressStep;

            // We collect references of MediaData used in the UndoRedoManager
            List<MediaData> usedMediaData = new List<MediaData>();

            //reportProgress(-1, "[3]...");

            if (RequestCancellation) return;

            // We collect references of MediaData used in the tree of TreeNodes
            CollectManagedMediaTreeNodeVisitor collectorVisitor = new CollectManagedMediaTreeNodeVisitor();
            if (m_Presentation.RootNode != null)
            {
                m_Presentation.RootNode.AcceptDepthFirst(collectorVisitor);
            }

            if (RequestCancellation) return;

            progress = 10;

            int index = 0;

            //bool firstMediaSeemsUnclean = false;
            //bool lastMediaSeemsUnclean = false;

            int indexWAMD = -1;

            List<IManaged> list3 = collectorVisitor.CollectedMedia;
            foreach (IManaged mm in list3)
            {
                index++;
                progress = 100 * index / list3.Count;
                //reportProgress(progress, "[4]...");

                if (RequestCancellation) return;

                if (mm.MediaData != null && !usedMediaData.Contains(mm.MediaData))
                {
                    usedMediaData.Add(mm.MediaData);

                    //                    if (mm.MediaData is WavAudioMediaData)
                    //                    {
                    //                        WavAudioMediaData wamd = (WavAudioMediaData) mm.MediaData;

                    //                        indexWAMD++;
                    //#if DEBUG
                    //                        DebugFix.Assert(wamd.mWavClips.Count > 0);
                    //#endif

                    //                        if (
                    //                            wamd.mWavClips.Count > 1
                    //                            ||
                    //                            m_cleanAudioMaxFileMegaBytes > 0
                    //                            &&
                    //                            wamd.PCMFormat.Data.ConvertTimeToBytes(wamd.mWavClips[0].MediaDuration.AsLocalUnits) < m_cleanAudioMaxFileMegaBytes
                    //                            )
                    //                        {
                    //                            if (indexWAMD == 0)
                    //                            {
                    //                                firstMediaSeemsUnclean = true;
                    //                            }

                    //                            if (indexWAMD == (list3.Count - 1))
                    //                            {
                    //                                lastMediaSeemsUnclean = true;
                    //                            }
                    //                        }
                    //                    }
                }
#if DEBUG
                else if (mm.MediaData != null)
                {
                    Debugger.Break();
                }
#endif
            }

            //            if (m_enableFileDataProviderPreservation && m_cleanAudioMaxFileMegaBytes > 0 && firstMediaSeemsUnclean && lastMediaSeemsUnclean)
            //            {
            //#if DEBUG
            //                Debugger.Break();
            //#endif
            //                m_enableFileDataProviderPreservation = false;
            //                Console.WriteLine("CLEANUP: mode was preserve-file (optimize file usage), but forcing initial batching of WAV files to start from fresh state.");
            //            }

            // ensure that media datas in undo redo manager are also preserved
            // it should be iterated after traversing tree because most of the media datas in undo redo manager are already in the tree
            foreach (MediaData md in m_Presentation.UndoRedoManager.UsedMediaData)
            {
                //progress += progressStep;
                //if (progress > 100) progress = progressStep;
                //reportProgress(progress, "[2]...");

                if (RequestCancellation) return;

                if (!usedMediaData.Contains(md))
                {
#if DEBUG
                    //DebugFix.Assert();
                    Debugger.Break();
#endif
                    usedMediaData.Add(md);
                }
            }
            progress = progressStep;

            List<DataProvider> usedDataProviders = new List<DataProvider>();

            // We eliminate MediaData registered in the MediaDataManager that is unused
            // (not in the list of collected MediaData so far)
            // and we collect references of DataProviders used by the MediaData collected so far


            List<MediaData> list = m_Presentation.MediaDataManager.ManagedObjects.ContentsAs_ListCopy;
            index = 0;
            foreach (MediaData md in list)
            {
                if (RequestCancellation) return;

                if (!usedMediaData.Contains(md))
                {
                    index++;
                    progress = 100 * index / list.Count;
                    //reportProgress(progress, "[5]...");

                    // Does not actually delete any file, just frees references.
                    md.Delete();
                }
            }

            if (m_enableFileDataProviderPreservation)
            {
                Cleanup_EnableFileDataProviderPreservation(usedMediaData, usedDataProviders);
            }
            else
            {
                Cleanup_DefaultPreservePlaybackOrder(usedMediaData, usedDataProviders);
            }


            // We collect references of DataProviders used by the registered ExternalFileData
            foreach (ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                foreach (DataProvider dp in efd.UsedDataProviders)
                {
                    if (RequestCancellation) return;

                    if (!usedDataProviders.Contains(dp))
                    {
                        usedDataProviders.Add(dp);
                    }
                }
            }

            // We eliminate DataProviders that are unused
            // (i.e. not in our list of collected used DataProviders so far)

            //int idx = 0; //to test exception handling and unmove of deleted files

            List<DataProvider> list2 = m_Presentation.DataProviderManager.ManagedObjects.ContentsAs_ListCopy;
            index = 0;
            foreach (DataProvider dp in list2)
            {
                progress = 100 * index++ / list2.Count;
                string info = dp is FileDataProvider ? ((FileDataProvider)dp).DataFileRelativePath : "";
                //reportProgress(progress, info);

                if (RequestCancellation) return;

                if (!usedDataProviders.Contains(dp))
                {
                    //idx++;
                    //if (idx > 5)
                    //{
                    //    throw new Exception("test");
                    //}

                    try
                    {

                        if (dp is FileDataProvider)
                        {
                            ((FileDataProvider)dp).DeleteByMovingToFolder(m_FullPathToDeletedDataFolder);
                        }
                        else
                        {
                            dp.Delete();
                        }
                    }
                    catch (OutputStreamOpenException ex1)
                    {
                        Console.WriteLine("OutputStreamOpenException === " + ((FileDataProvider)dp).DataFileFullPath);
                    }
                    catch (InputStreamsOpenException ex2)
                    {
                        Console.WriteLine("InputStreamsOpenException === " + ((FileDataProvider)dp).DataFileFullPath);
                    }
                    catch (OperationNotValidException ex3)
                    {
                        Console.WriteLine("OperationNotValidException === " + ((FileDataProvider)dp).DataFileFullPath);
                    }
                }
            }
        }
    }
}
