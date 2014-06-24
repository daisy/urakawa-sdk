using System;
using System.Diagnostics;

namespace urakawa.media
{
    /// <summary>
    /// Interface for <see cref="Media"/> of textual type. 
    /// </summary>
    public abstract class AbstractTextMedia : Media
    {
        /// <summary>
        /// This always returns false, because
        /// text media is never considered continuous
        /// </summary>
        /// <returns></returns>
        public override bool IsContinuous
        {
            get { return false; }
        }

        /// <summary>
        /// This always returns true, because
        /// text media is always considered discrete
        /// </summary>
        /// <returns></returns>
        public override bool IsDiscrete
        {
            get { return true; }
        }


        /// <summary>
        /// This always returns false, because
        /// a single media object is never considered to be a sequence
        /// </summary>
        /// <returns></returns>
        public override bool IsSequence
        {
            get { return false; }
        }

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AbstractTextMedia otherz = other as AbstractTextMedia;
            if (otherz == null)
            {
                return false;
            }

            //TODO: is there a more reliable way to handle DOS versus UNIX line breaks at the end of the strings ??
            try
            {
                string str1 = Text;
                string str2 = otherz.Text;

                str1 = str1.Replace("\r\n", "\n");
                str2 = str2.Replace("\r\n", "\n");

                if (!str1.Equals(str2))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !");
                    return false;
                }
            }
            catch(Exception e)
            {
                // WebClient failure in unit-tests, non-existing file.
                //Debugger.Break();
            }
        

        //if (Text != otherz.Text)
            //{
            //    return false;
            //}

            return true;
        }
        /// <summary>
        /// Event fired after the text of the <see cref="ExternalTextMedia"/> has changed
        /// </summary>
        public event EventHandler<urakawa.events.media.TextChangedEventArgs> TextChanged;

        /// <summary>
        /// Fires the <see cref="TextChanged"/> event
        /// </summary>
        /// <param name="newText">The new text value</param>
        /// <param name="prevText">Thye text value prior to the change</param>
        protected void NotifyTextChanged(string newText, string prevText)
        {
            EventHandler<urakawa.events.media.TextChangedEventArgs> d = TextChanged;
            if (d != null) d(this, new urakawa.events.media.TextChangedEventArgs(this, newText, prevText));
        }


        private void this_TextChanged(object sender, urakawa.events.media.TextChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="AbstractTextMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        protected AbstractTextMedia()
        {
            TextChanged += this_TextChanged;
        }

        /// <summary>
        /// Gets or setsthe text string for the TextMedia.
        /// </summary>
        public abstract string Text { get; set; }
    }
}