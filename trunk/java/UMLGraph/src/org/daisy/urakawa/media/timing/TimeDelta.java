package org.daisy.urakawa.media.timing;

/**
 * Abstract Time duration (could be in milliseconds, SMPTE, etc.). This really
 * is an interface "lollypop" that should be extended. Typically, methods like
 * getTimeMilliseconds(), getTimeSMPTE(), etc. should be available to the
 * end-user of the API. Can be a 0/positive value in the current local timebase.
 * (cannot be negative)
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface TimeDelta {
	long getTimeDeltaAsMilliseconds();

	double getTimeDeltaAsMillisecondFloat();

	void setTimeDelta(long timeDeltaAsMS);

	void setTimeDelta(double timeDeltaAsMSF);

	TimeDelta addTimeDelta(TimeDelta other);
}