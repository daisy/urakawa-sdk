using System;
using System.Collections.Generic;
using System.Text;

namespace WaveFormDisplay
{
	public interface IWaveFormGraphDataProvider
	{
		WaveFormGraphData GetGraphData(TimeSpan clipBegin, TimeSpan clipEnd, int pixelWidth);
	}
}
