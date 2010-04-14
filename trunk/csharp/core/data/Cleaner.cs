﻿using System.Collections.Generic;
using AudioLib;
using urakawa.command;
using urakawa.core;
using urakawa.ExternalFiles;
using urakawa.media.data;
using urakawa.media.data.utilities;

namespace urakawa.data
{
    public class Cleaner : DualCancellableProgressReporter
    {
        private readonly Presentation m_Presentation;
        private readonly string m_FullPathToDeletedDataFolder;
        public Cleaner(Presentation presentation, string fullPathToDeletedDataFolder)
        {
            m_Presentation = presentation;
            m_FullPathToDeletedDataFolder = fullPathToDeletedDataFolder;
        }

        public override void DoWork()
        {
            Cleanup();
        }

        /// <summary>
        /// Removes any <see cref="MediaData"/> and <see cref="DataProvider"/>s that are not used by any <see cref="TreeNode"/> in the document tree
        /// or by any <see cref="Command"/> in the <see cref="undo.UndoRedoManager"/> stacks (undo/redo/transaction).
        /// or by ExternalFileData
        /// </summary>
        public void Cleanup()
        {
            reportProgress(-1, "Analyzing multimedia objects ...");

            const int progressStep = 10;
            int progress = progressStep;

            // We collect references of MediaData used in the UndoRedoManager
            List<MediaData> usedMediaData = new List<MediaData>();
            foreach (MediaData md in m_Presentation.UndoRedoManager.UsedMediaData)
            {
                progress += progressStep;
                if (progress > 100) progress = progressStep;
                reportSubProgress(progress, "Processing undo-redo stacks");

                if (RequestCancellation) return;

                if (!usedMediaData.Contains(md))
                {
                    usedMediaData.Add(md);
                }
            }

            reportSubProgress(-1, "Walking the TreeNodes");

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
            var list3 = collectorVisitor.CollectedMedia;
            foreach (IManaged mm in list3)
            {
                index++;
                progress = 100 * index / list3.Count;
                reportSubProgress(progress, "Processing TreeNode data");

                if (RequestCancellation) return;

                if (mm.MediaData != null && !usedMediaData.Contains(mm.MediaData))
                {
                    usedMediaData.Add(mm.MediaData);
                }
            }

            progress = progressStep;

            List<DataProvider> usedDataProviders = new List<DataProvider>();

            // We eliminate MediaData registered in the MediaDataManager that is unused
            // (not in the list of collected MediaData so far)
            // and we collect references of DataProviders used by the MediaData collected so far

            index = 0;
            var list = m_Presentation.MediaDataManager.ManagedObjects.ContentsAs_ListCopy;
            foreach (MediaData md in list)
            {
                index++;
                progress = 100 * index / list.Count;
                reportSubProgress(progress, "Processing Media Datas");

                if (RequestCancellation) return;

                if (usedMediaData.Contains(md))
                {
                    if (md is media.data.audio.codec.WavAudioMediaData)
                    {
                        reportSubProgress(progress, "Creating a single data provider...");

                        ((media.data.audio.codec.WavAudioMediaData)md).ForceSingleDataProvider();
                    }
                    foreach (DataProvider dp in md.UsedDataProviders)
                    {
                        if (RequestCancellation) return;

                        if (!usedDataProviders.Contains(dp)) usedDataProviders.Add(dp);
                    }
                }
                else
                {
                    // Does not actually delete any file, just frees references.
                    md.Delete();
                }
            }

            // We collect references of DataProviders used by the registered ExternalFileData
            foreach (ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_YieldEnumerable)
            {
                foreach (DataProvider dp in efd.UsedDataProviders)
                {
                    if (RequestCancellation) return;

                    if (!usedDataProviders.Contains(dp)) usedDataProviders.Add(dp);
                }
            }

            // We eliminate DataProviders that are unused
            // (i.e. not in our list of collected used DataProviders so far)

            index = 0;
            var list2 = m_Presentation.DataProviderManager.ManagedObjects.ContentsAs_ListCopy;
            foreach (DataProvider dp in list2)
            {
                progress = 100 * index / list2.Count;
                string info = dp is FileDataProvider ? ((FileDataProvider)dp).DataFileRelativePath : "";
                reportSubProgress(progress, "Checking Data Provider: " + info);

                if (RequestCancellation) return;

                if (!usedDataProviders.Contains(dp))
                {
                    if (dp is FileDataProvider)
                    {
                        ((FileDataProvider)dp).DeleteByMovingToFolder(m_FullPathToDeletedDataFolder);
                    }
                    else
                        dp.Delete();
                }
            }
        }
    }
}