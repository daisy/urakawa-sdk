package org.daisy.urakawa.core.visitor.examples;

import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.core.visitor.ITreeNodeVisitor;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.DoesNotAcceptMediaException;
import org.daisy.urakawa.media.ExternalAudioMedia;
import org.daisy.urakawa.media.data.audio.IAudioMediaData;
import org.daisy.urakawa.media.data.audio.IManagedAudioMedia;
import org.daisy.urakawa.media.data.audio.IPCMDataInfo;
import org.daisy.urakawa.media.data.audio.IPCMFormatInfo;
import org.daisy.urakawa.media.data.audio.PCMDataInfo;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.FileStream;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.property.channel.ChannelDoesNotExistException;
import org.daisy.urakawa.property.channel.IChannel;
import org.daisy.urakawa.property.channel.IChannelsProperty;

/**
 * This is an abstract ITreeNodeVisitor that publishes IManagedAudioMedia from a
 * source IChannel to a destination IChannel as ExternalAudioMedia. In concrete
 * implementations of the abstract visitor, methods
 * treeNodeTriggersNewAudioFile() and treeNodeMustBeSkipped() must be
 * implemented to control which TreeNodes trigger the generation of a new audio
 * file and which TreeNodes are skipped. After visitation the
 * writeCurrentAudioFile() method must be called to ensure that the current
 * audio file is written to disk.
 */
public abstract class AbstractPublishManagedAudioVisitor implements
        ITreeNodeVisitor
{
    protected AbstractPublishManagedAudioVisitor()
    {
        resetAudioFileNumbering();
    }

    private IChannel mSourceChannel;
    private IChannel mDestinationChannel;
    private URI mDestinationDirectory;
    private String mAudioFileBaseNameFormat = "aud{0:0}.wav";
    private int mCurrentAudioFileNumber;
    private IPCMFormatInfo mCurrentAudioFilePCMFormat = null;
    private IStream mCurrentAudioFileStream = null;

    /**
     * Gets the source IChannel from which the IManagedAudioMedia to publish is
     * retrieved
     * 
     * @return the channel
     * @throws IsNotInitializedException
     */
    public IChannel getSourceChannel() throws IsNotInitializedException
    {
        if (mSourceChannel == null)
        {
            throw new IsNotInitializedException();
        }
        return mSourceChannel;
    }

    /**
     * Sets the source IChannel from which the <see IManagedAudioMedia to
     * publish is retrieved
     * 
     * @param ch
     * @throws MethodParameterIsNullException
     */
    public void setSourceChannel(IChannel ch)
            throws MethodParameterIsNullException
    {
        if (ch == null)
            throw new MethodParameterIsNullException();
        mSourceChannel = ch;
    }

    /**
     * Gets the destination IChannel to which the published audio is added as
     * ExternalAudioMedia
     * 
     * @return the channel
     * @throws IsNotInitializedException
     */
    public IChannel getDestinationChannel() throws IsNotInitializedException
    {
        if (mDestinationChannel == null)
        {
            throw new IsNotInitializedException();
        }
        return mDestinationChannel;
    }

    /**
     * Sets the destination IChannel to which the published // audio is added as
     * ExternalAudioMedia
     * 
     * @param ch
     * @throws MethodParameterIsNullException
     */
    public void setDestinationChannel(IChannel ch)
            throws MethodParameterIsNullException
    {
        if (ch == null)
            throw new MethodParameterIsNullException();
        mDestinationChannel = ch;
    }

    /**
     * Gets the URI of the destination directory in which the published audio
     * files are created
     * 
     * @return URI
     * @throws IsNotInitializedException
     */
    public URI getDestinationDirectory() throws IsNotInitializedException
    {
        if (mDestinationChannel == null)
        {
            throw new IsNotInitializedException();
        }
        return mDestinationDirectory;
    }

    /**
     * Sets the URI of the destination directory in which the published audio
     * files are created
     * 
     * @param destDir
     * @throws MethodParameterIsNullException
     */
    public void setDestinationDirectory(URI destDir)
            throws MethodParameterIsNullException
    {
        if (destDir == null)
        {
            throw new MethodParameterIsNullException();
        }
        mDestinationDirectory = destDir;
    }

    /**
     * Gets the format of the name of the published audio files - format
     * parameter 0 is the number of the audio file (1, 2, ...)
     * 
     * @return string
     */
    public String getAudioFileNameFormat()
    {
        return mAudioFileBaseNameFormat;
    }

    /**
     * Gets the number of the current audio file
     * 
     * @return number
     */
    public int getCurrentAudioFileNumber()
    {
        return mCurrentAudioFileNumber;
    }

    /**
     * Resets the audio file numbering, setting the current audio file number to
     * 0.
     */
    public void resetAudioFileNumbering()
    {
        mCurrentAudioFileNumber = 0;
    }

    /**
     * Controls when new audio files are created. In concrete implementations,
     * if this method returns true for a given <see ITreeNode, this ITreeNode
     * triggers the creation of a new audio file
     * 
     * @param node
     * @return true or false
     */
    public abstract boolean treeNodeTriggersNewAudioFile(ITreeNode node);

    /**
     * Controls what ITreeNode are skipped during publish visitation
     * 
     * @param node
     * @return true or false
     */
    public abstract boolean treeNodeMustBeSkipped(ITreeNode node);

    /**
     * Writes the currently active audio file to disk.
     */
    public void writeCurrentAudioFile()
    {
        if (mCurrentAudioFileStream != null
                && mCurrentAudioFilePCMFormat != null)
        {
            URI file = getCurrentAudioFileUri();
            FileStream fs = new FileStream(file.getPath());
            try
            {
                IPCMDataInfo pcmData;
                try
                {
                    pcmData = new PCMDataInfo(mCurrentAudioFilePCMFormat);
                }
                catch (MethodParameterIsNullException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                pcmData.setDataLength(mCurrentAudioFileStream.getLength());
                try
                {
                    pcmData.writeRiffWaveHeader(fs);
                }
                catch (MethodParameterIsNullException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                catch (IOException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                mCurrentAudioFileStream.setPosition(0);
                byte[] data = mCurrentAudioFileStream
                        .readBytes(mCurrentAudioFileStream.getLength());
                try
                {
                    mCurrentAudioFileStream.close();
                    fs.write(data, 0, data.length);
                }
                catch (IOException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                mCurrentAudioFileStream = null;
                mCurrentAudioFilePCMFormat = null;
            }
            finally
            {
                try
                {
                    fs.close();
                }
                catch (IOException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
            }
        }
    }

    @SuppressWarnings("boxing")
    private URI getCurrentAudioFileUri()
    {
        URI res;
        try
        {
            res = getDestinationDirectory();
        }
        catch (IsNotInitializedException e1)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e1);
        }
        URI res2;
        try
        {
            res2 = new URI(String.format(getAudioFileNameFormat(),
                    getCurrentAudioFileNumber()));
        }
        catch (URISyntaxException e)
        {
            e.printStackTrace();
            return null;
        }
        return res2.relativize(res);
    }

    private void createNextAudioFile()
    {
        writeCurrentAudioFile();
        mCurrentAudioFileNumber++;
        // TODO: mCurrentAudioFileStream = new MemoryStream();
        mCurrentAudioFileStream = null;
    }

    public boolean preVisit(ITreeNode node)
            throws MethodParameterIsNullException
    {
        if (node == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (treeNodeMustBeSkipped(node))
            return false;
        if (treeNodeTriggersNewAudioFile(node))
            createNextAudioFile();
        if (node.hasProperties(IChannelsProperty.class))
        {
            IChannelsProperty chProp = node
                    .getProperty(IChannelsProperty.class);
            try
            {
                if (chProp.getMedia(getDestinationChannel()) != null)
                    chProp.setMedia(getDestinationChannel(), null);
            }
            catch (ChannelDoesNotExistException e1)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e1);
            }
            catch (IsNotInitializedException e1)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e1);
            }
            catch (DoesNotAcceptMediaException e1)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e1);
            }
            IManagedAudioMedia mam;
            try
            {
                mam = (IManagedAudioMedia) chProp.getMedia(getSourceChannel());
            }
            catch (ChannelDoesNotExistException e1)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e1);
            }
            catch (IsNotInitializedException e1)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e1);
            }
            if (mam != null)
            {
                IAudioMediaData amd = mam.getMediaData();
                if (mCurrentAudioFilePCMFormat == null)
                {
                    mCurrentAudioFilePCMFormat = amd.getPCMFormat();
                }
                if (mCurrentAudioFileStream == null
                        || !mCurrentAudioFilePCMFormat.ValueEquals(amd
                                .getPCMFormat()))
                {
                    createNextAudioFile();
                    mCurrentAudioFilePCMFormat = amd.getPCMFormat();
                }
                ITime clipBegin = new Time().getZero().addTimeDelta(
                        mCurrentAudioFilePCMFormat
                                .getDuration(mCurrentAudioFileStream
                                        .getPosition()));
                ITime clipEnd = clipBegin.addTimeDelta(amd.getAudioDuration());
                IStream st = amd.getAudioData();
                try
                {
                    byte[] data = st.readBytes(amd.getPCMLength());
                    try
                    {
                        mCurrentAudioFileStream.write(data, 0, data.length);
                    }
                    catch (IOException e)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e);
                    }
                }
                finally
                {
                    try
                    {
                        st.close();
                    }
                    catch (IOException e)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e);
                    }
                }
                ExternalAudioMedia eam;
                try
                {
                    eam = node.getPresentation().getMediaFactory()
                            .createExternalAudioMedia();
                }
                catch (IsNotInitializedException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                try
                {
                    eam.setLanguage(mam.getLanguage());
                    eam.setSrc(node.getPresentation().getRootURI().relativize(
                            getCurrentAudioFileUri()).toString());
                    eam.setClipBegin(clipBegin);
                    eam.setClipEnd(clipEnd);
                    chProp.setMedia(mDestinationChannel, eam);
                }
                catch (MethodParameterIsEmptyStringException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                catch (ChannelDoesNotExistException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                catch (DoesNotAcceptMediaException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                catch (IsNotInitializedException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
                catch (TimeOffsetIsOutOfBoundsException e)
                {
                    // Should never happen
                    throw new RuntimeException("WTF ??!", e);
                }
            }
        }
        return true;
    }

    public void postVisit(@SuppressWarnings("unused") ITreeNode node)
    {
        // Nothing is done in postVisit visit
    }
}
