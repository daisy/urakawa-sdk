using System;
using System.Collections.Generic;
using System.Text;

namespace WaveFormDisplay
{
    /// <summary>
    /// Interface for a 
    /// </summary>
	public interface IWaveFormGraphDataProvider
	{
		WaveFormGraphData GetGraphData(TimeSpan clipBegin, TimeSpan clipEnd, int numberOfDataPoints);
	}
}
