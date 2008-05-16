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

		public WaveFormGraphData GetGraphData(TimeSpan clipBegin, TimeSpan clipEnd, int pixelWidth)
		{
			if (pixelWidth < 0) pixelWidth = 0;
			WaveFormGraphData res = new WaveFormGraphData(pixelWidth);
			if (res.Width > 0)
			{

			}
			return res;
		}

		#endregion
	}
}
