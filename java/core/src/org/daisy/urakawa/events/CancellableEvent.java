package org.daisy.urakawa.events;

/**
 *
 */
public class CancellableEvent extends Event
{
    private boolean mCancelled;

    /**
	 * 
	 */
    public CancellableEvent()
    {
        mCancelled = false;
    }

    /**
	 * 
	 */
    public void cancel()
    {
        mCancelled = true;
    }

    /**
     * @return bool
     */
    public boolean isCancelled()
    {
        return mCancelled;
    }
}
