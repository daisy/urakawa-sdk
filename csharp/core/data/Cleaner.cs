﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly bool m_enableFileDataProviderPreservation;
        public Cleaner(Presentation presentation, string fullPathToDeletedDataFolder, double cleanAudioMaxFileMegaBytes, bool enableFileDataProviderPreservation)
        {
            m_Presentation = presentation;
            m_FullPathToDeletedDataFolder = fullPathToDeletedDataFolder;
            m_cleanAudioMaxFileMegaBytes = cleanAudioMaxFileMegaBytes;
            m_enableFileDataProviderPreservation = enableFileDataProviderPreservation;
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

        private bool isFileDataProviderFullyUsed(DataProvider dataProvider, List<MediaData> usedMediaData)
        {
            ensureDataProviderWavClipHoles(usedMediaData);

            return m_fullyUsedFileDataProviders.Contains(dataProvider);
        }

        private void ensureDataProviderWavClipHoles(List<MediaData> usedMediaData)
        {
            if (m_fullyUsedFileDataProviders != null)
            {
                return;
            }

            // List of DataProviders (WAV files) that are entirely used (no unused gaps)
            m_fullyUsedFileDataProviders = new List<DataProvider>();

            // Associate each DataProvider (WAV file) with a list of potentially-overlapping WavClips (begin-end time range)
            // that represent actual file usage.
            // Note that those WavClips may belong to different WavAudioMediaData, as FileDataProviders can be reused / shared.
            m_FileDataProvidersWavClipMap = new Dictionary<DataProvider, List<WavClip>>();

            // Associate each DataProvider (WAV file) with a list of non-overlapping WavClips (begin-end time range)
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

                    foreach (WavClip clip in wMd.mWavClips)
                    {
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
#if DEBUG
                Debugger.Break();
#endif
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


            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("## CLEANUP -- fully-used WAV files:");
            foreach (DataProvider dp in m_fullyUsedFileDataProviders)
            {
                Console.Write("[" + ((FileDataProvider)dp).DataFileRelativePath + "] ");
            }
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("## CLEANUP -- WAV files with unused gaps:");
            foreach (DataProvider dp in m_FileDataProvidersHolesMap.Keys)
            {
                Console.WriteLine("[" + ((FileDataProvider)dp).DataFileRelativePath + "] ");
                foreach (WavClip hole in m_FileDataProvidersHolesMap[dp])
                {
                    Console.WriteLine(hole.ClipBegin.Format_Standard() + " - " + hole.ClipEnd.Format_Standard());
                }
                Console.WriteLine(" ");
            }
#endif
        }

        private Time m_minimumDurationToPreserveHole = new Time(AudioLibPCMFormat.MILLISECONDS_TOLERANCE * AudioLibPCMFormat.TIME_UNIT); // 5ms
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
                    if (dur1.IsGreaterThanOrEqualTo(m_minimumDurationToPreserveHole))
                    {
                        holesToAdd.Add(new WavClip(clip.DataProvider, begin1, end1));
                    }

                    Time begin2 = new Time(clip.ClipEnd);
                    Time end2 = new Time(hole.ClipEnd);
                    Time dur2 = end2.GetDifference(begin2);
                    if (dur2.IsGreaterThanOrEqualTo(m_minimumDurationToPreserveHole))
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
                }
#if DEBUG
                else if (mm.MediaData != null)
                {
                    Debugger.Break();
                }
#endif
            }

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

            DataProvider curentAudioDataProvider = null;
            long currentFileDataProviderIndex = 0;
            string prefixFormat = @"{0:D4}_";

            long nMaxBytes = (m_cleanAudioMaxFileMegaBytes <= 0 ? 0 : (long)Math.Round(m_cleanAudioMaxFileMegaBytes * 1024 * 1024));
            long currentBytes = 0;
            FileDataProvider currentFileDataProvider = null;
            Stream currentFileDataProviderOutputStream = null;
            PCMFormatInfo pCMFormat = null;
            ulong riffHeaderLength = 0;

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
                ensureDataProviderWavClipHoles(usedMediaData);
            }

            index = 0;
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

                    //if (m_enableFileDataProviderPreservation)
                    //{
                    //    bool allFullyUsed = true;
                    //    foreach (DataProvider dp in md.UsedDataProviders)
                    //    {
                    //        if (RequestCancellation) return;

                    //        if (!isFileDataProviderFullyUsed(dp, usedMediaData))
                    //        {
                    //            allFullyUsed = false;
                    //            break;
                    //        }
                    //    }

                    //    if (allFullyUsed)
                    //    {
                    //        foreach (DataProvider dp in md.UsedDataProviders)
                    //        {
                    //            if (RequestCancellation) return;

                    //            if (!usedDataProviders.Contains(dp))
                    //            {
                    //                usedDataProviders.Add(dp);
                    //            }
                    //        }

                    //        continue;
                    //    }
                    //}

                    uint thisAudioByteLength = (uint)wMd.PCMFormat.Data.ConvertTimeToBytes(wMd.AudioDuration.AsLocalUnits);

                    if (nMaxBytes <= 0 || thisAudioByteLength >= nMaxBytes)
                    {
                        wMd.ForceSingleDataProvider(true, String.Format(prefixFormat, ++currentFileDataProviderIndex));
                    }
                    else
                    {
                        //                            DataProvider dp = null;
                        //                            int count = 0;
                        //                            foreach (DataProvider dp_ in md.UsedDataProviders)
                        //                            {
                        //                                count++;

                        //                                if (dp == null)
                        //                                {
                        //                                    dp = dp_;
                        //                                }

                        //                            }
                        //                            if (count == 1)
                        //                            {
                        //                                if (usedDataProviders.Contains(dp))
                        //                                {
                        //                                    // already processed in prior iteration (previous MediaData)
                        //                                    continue;
                        //                                }

                        ////                                //long nBytes = 0;

                        ////                                Object DPappData = dp.AppData;
                        ////                                if (DPappData != null)
                        ////                                {
                        ////                                    if (DPappData is WavClip.PcmFormatAndTime)
                        ////                                    {
                        ////                                        //nBytes = ((WavClip.PcmFormatAndTime)DPappData).Bytes;

                        //////#if DEBUG
                        //////                                        long nBytes_ = ((WavClip.PcmFormatAndTime)DPappData).mFormat.ConvertTimeToBytes(((WavClip.PcmFormatAndTime)DPappData).mTime.AsLocalUnits);
                        //////                                        DebugFix.Assert(((WavClip.PcmFormatAndTime)DPappData).mFormat.BytesAreEqualWithMillisecondsTolerance(nBytes, nBytes_));
                        //////#endif
                        ////                                    }
                        ////#if DEBUG
                        ////                                    else
                        ////                                    {
                        ////                                        Debugger.Break();
                        ////                                    }
                        ////#endif
                        ////                                }
                        ////                                else
                        ////                                {
                        ////                                    Stream ism = null;
                        ////                                    try
                        ////                                    {
                        ////                                        ism = wMd.OpenInputStream();

                        ////                                        uint dataLength;
                        ////                                        AudioLibPCMFormat PCMformat = AudioLibPCMFormat.RiffHeaderParse(ism, out dataLength);

                        ////                                        DebugFix.Assert(wMd.PCMFormat.Data.Equals(PCMformat));

                        ////                                        DPappData = new WavClip.PcmFormatAndTime(PCMformat, new Time(PCMformat.ConvertBytesToTime(dataLength))
                        ////                                            //, dataLength
                        ////                                            );
                        ////                                        dp.AppData = DPappData;

                        ////                                        //nBytes = dataLength;
                        ////                                    }
                        ////                                    catch (Exception ex)
                        ////                                    {
                        ////                                        throw ex;
                        ////                                    }
                        ////                                    finally
                        ////                                    {
                        ////                                        if (ism != null)
                        ////                                        {
                        ////                                            ism.Close();
                        ////                                        }
                        ////                                    }
                        ////                                }

                        //                                //// TODO: some FDP might be smaller, yet full! (because the next MD did not fit within) => How to detect?
                        //                                //if (nBytes >= nMaxBytes)
                        //                                //{
                        //                                //    bool mediaHasSingleFileDataProviderWithLengthGreaterThanMaxThreshold = false;


                        //if (!usedDataProviders.Contains(dp))
                        //{
                        //    usedDataProviders.Add(dp);
                        //}


                        //                                //    continue;
                        //                                //}
                        //                            }


                        long nextSize = currentBytes + thisAudioByteLength;
                        if (nextSize > nMaxBytes)
                        {
                            if (currentFileDataProvider != null)
                            {
                                //Stream stream = currentFileDataProvider.OpenOutputStream();
                                currentFileDataProviderOutputStream.Position = 0;
                                try
                                {
                                    riffHeaderLength = wMd.PCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream, (uint)currentBytes);
                                }
                                finally
                                {
                                    currentFileDataProviderOutputStream.Close();
                                    currentFileDataProviderOutputStream = null;
                                }

                                currentFileDataProvider = null;
                            }
                        }

                        if (currentFileDataProvider == null)
                        {
                            currentBytes = 0;
                            currentFileDataProvider = (FileDataProvider)m_Presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                            currentFileDataProvider.SetNamePrefix(String.Format(prefixFormat, ++currentFileDataProviderIndex));

                            currentFileDataProviderOutputStream = currentFileDataProvider.OpenOutputStream();
                            try
                            {
                                riffHeaderLength = wMd.PCMFormat.Data.RiffHeaderWrite(currentFileDataProviderOutputStream, (uint)0);
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

                        bool okay = false;
                        try
                        {
                            //currentFileDataProvider.AppendData(stream, availableToRead);

#if RIFF_HEADER_INCREMENTAL_MAINTAIN
                                currentFileDataProviderOutputStream.Seek(0, SeekOrigin.End);
#endif //RIFF_HEADER_INCREMENTAL_MAINTAIN

                            const uint BUFFER_SIZE = 1024 * 300; // 300 KB MAX BUFFER
                            StreamUtils.Copy(stream, (ulong)availableToRead, currentFileDataProviderOutputStream, BUFFER_SIZE);

                            okay = true;
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
                        if (okay)
                        {
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
                                    new AudioLibPCMFormat(wMd.PCMFormat.Data.NumberOfChannels, wMd.PCMFormat.Data.SampleRate, wMd.PCMFormat.Data.BitDepth),
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
                        }
                        else
                        {
#if DEBUG
                            Debugger.Break();
#endif
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
                }

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
