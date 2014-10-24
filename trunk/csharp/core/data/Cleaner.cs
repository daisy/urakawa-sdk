using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public Cleaner(Presentation presentation, string fullPathToDeletedDataFolder, double cleanAudioMaxFileMegaBytes)
        {
            m_Presentation = presentation;
            m_FullPathToDeletedDataFolder = fullPathToDeletedDataFolder;
            m_cleanAudioMaxFileMegaBytes = cleanAudioMaxFileMegaBytes;
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
                else
                {
                    Debugger.Break();
                }
#endif
            }

            progress = progressStep;

            List<DataProvider> usedDataProviders = new List<DataProvider>();

            // We eliminate MediaData registered in the MediaDataManager that is unused
            // (not in the list of collected MediaData so far)
            // and we collect references of DataProviders used by the MediaData collected so far

            index = 0;

            DataProvider curentAudioDataProvider = null;

            long nMaxBytes = (m_cleanAudioMaxFileMegaBytes <= 0 ? 0 : (long)Math.Round(m_cleanAudioMaxFileMegaBytes * 1024 * 1024));
            long currentBytes = 0;
            FileDataProvider currentFileDataProvider = null;
            PCMFormatInfo pCMFormat = null;
            ulong riffHeaderLength = 0;

            List<MediaData> list = m_Presentation.MediaDataManager.ManagedObjects.ContentsAs_ListCopy;
            foreach (MediaData md in list)
            {
                index++;
                progress = 100 * index / list.Count;
                //reportProgress(progress, "[5]...");

                if (RequestCancellation) return;

                if (usedMediaData.Contains(md))
                {
                    if (md is WavAudioMediaData)
                    {
                        reportProgress_Throttle(progress, index + " / " + list.Count);

                        WavAudioMediaData wMd = (WavAudioMediaData)md;
                        uint thisAudioByteLength = (uint)wMd.PCMFormat.Data.ConvertTimeToBytes(wMd.AudioDuration.AsLocalUnits);

                        if (nMaxBytes <= 0 || thisAudioByteLength >= nMaxBytes)
                        {
                            wMd.ForceSingleDataProvider();
                        }
                        else
                        {
                            long nextSize = currentBytes + thisAudioByteLength;

                            Stream stream = null;
                            
                            if (nextSize > nMaxBytes)
                            {
                                if (currentFileDataProvider != null)
                                {
                                    stream = currentFileDataProvider.OpenOutputStream();
                                    try
                                    {
                                        riffHeaderLength = wMd.PCMFormat.Data.RiffHeaderWrite(stream, (uint)currentBytes);
                                    }
                                    finally
                                    {
                                        stream.Close();
                                        stream = null;
                                    }
                                }

                                currentFileDataProvider = null;
                            }
                            
                            if (currentFileDataProvider == null)
                            {
                                currentBytes = 0;
                                currentFileDataProvider = (FileDataProvider) m_Presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                               
                                stream = currentFileDataProvider.OpenOutputStream();
                                try
                                {
                                    riffHeaderLength = wMd.PCMFormat.Data.RiffHeaderWrite(stream, (uint)0);
                                }
                                finally
                                {
                                    stream.Close();
                                    stream = null;
                                }

                                pCMFormat = wMd.PCMFormat;
                            }

                            bool okay = false;
                            stream = wMd.OpenPcmInputStream();

                            long availableToRead = stream.Length - stream.Position;
                            
                            //DebugFix.Assert(availableToRead == thisAudioByteLength);
                            if (thisAudioByteLength != availableToRead)
                            {
#if DEBUG
                                long diff = thisAudioByteLength - availableToRead;
                                if (Math.Abs(diff) > 2)
                                {
                                    Debugger.Break();
                                }

                                Console.WriteLine(">> audio bytes diff: " + diff);
#endif
                                thisAudioByteLength = (uint) availableToRead;
                            }

                            try
                            {
                                currentFileDataProvider.AppendData(stream, availableToRead);
                                okay = true;
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
                                        //((WavClip.PcmFormatAndTime)appData).mFormat;
                                    }
                                }

                                // TODO: to save precious I/O time, inject up-to-date RIFF header in AppendData() above.
                                stream = currentFileDataProvider.OpenOutputStream();
                                try
                                {
                                    riffHeaderLength = wMd.PCMFormat.Data.RiffHeaderWrite(stream, (uint)(currentBytes + thisAudioByteLength));
                                }
                                finally
                                {
                                    stream.Close();
                                    stream = null;
                                }

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
                                stream = currentFileDataProvider.OpenOutputStream();
                                try
                                {
                                    riffHeaderLength = pCMFormat.Data.RiffHeaderWrite(stream, (uint)currentBytes);
                                }
                                finally
                                {
                                    stream.Close();
                                    stream = null;
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
                else
                {
                    // Does not actually delete any file, just frees references.
                    md.Delete();
                }
            }
            if (currentFileDataProvider != null)
            {
                Stream stream = currentFileDataProvider.OpenOutputStream();
                try
                {
                    riffHeaderLength = pCMFormat.Data.RiffHeaderWrite(stream, (uint)currentBytes);
                }
                finally
                {
                    stream.Close();
                    stream = null;
                }
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

            index = 0;

            //int idx = 0; //to test exception handling and unmove of deleted files

            List<DataProvider> list2 = m_Presentation.DataProviderManager.ManagedObjects.ContentsAs_ListCopy;
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
