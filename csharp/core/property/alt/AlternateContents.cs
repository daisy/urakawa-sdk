using System;
using System.Collections.Generic;
using System.Text;

using urakawa.media;

using urakawa.xuk;

namespace urakawa.property.alt
{
    public class AlternateContents : XukAble
    {
        

        public AlternateContents()
        {
            m_AlternateContentObjects = new ObjectListProvider<AlternateContent>(this, true);
        }

        private ObjectListProvider<AlternateContent> m_AlternateContentObjects;
        public ObjectListProvider<AlternateContent> AlternateContentObjects
        {
            get { return m_AlternateContentObjects; } 
        }


        public override string GetTypeNameFormatted()
        {
            return XukStrings.AlternateContents;
        }

        public void AddAlternateContentItem(AlternateContent content)
        {
            if (content == null) throw new System.Exception("Null AlternateContent cannot be added");

            m_AlternateContentObjects.Insert(m_AlternateContentObjects.Count, content);
        }

        public void RemoveAlternateContentItem(AlternateContent content)
        {
            if (content == null) throw new System.Exception("AlternateContent to be removed has null value");

            if (!m_AlternateContentObjects.ContentsAs_ReadOnlyCollectionWrapper.Contains(content)) throw new System.Exception("Alternate content to be removed is not in collection");

            m_AlternateContentObjects.Remove(content);
        }

    }
}
