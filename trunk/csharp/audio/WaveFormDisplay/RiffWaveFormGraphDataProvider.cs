using System;
using System.Collections.Generic;
using System.Text;

namespace WaveFormDisplay
{
	public class RiffWaveFormGraphDataProvider : IWaveFormGraphDataProvider
	{
		private Uri mRiffWaveFileUri;

		public RiffWaveFormGraphDataProvider(Uri riffUri)
		{
			if (riffUri == null) throw new ArgumentNullException();
			mRiffWaveFileUri = riffUri;
		}

		#region IWaveFormGraphDataProvider Members

		public WaveFormGraphData GetGraphData(TimeSpan clipBegin, TimeSpan clipEnd, int numberOfDataPoints)
		{
			if (numberOfDataPoints < 0) numberOfDataPoints = 0;
			WaveFormGraphData res = new WaveFormGraphData(numberOfDataPoints);
			if (res.Width > 0)
			{

			}
			return res;
		}

		#endregion
	}
}
