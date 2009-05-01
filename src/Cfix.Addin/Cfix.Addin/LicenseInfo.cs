using System;
using System.Collections.Generic;
using System.Text;

namespace Cfix.Addin
{
	internal class LicenseInfo
	{
		private const uint ProductId = 1;
		private const uint SubProductId = 0;

		private readonly Native.CFIXCTL_LICENSE_INFO info;

		public LicenseInfo( Native.CFIXCTL_LICENSE_INFO info )
		{
			this.info = info;
		}

		public bool Valid
		{
			get
			{
				if ( this.info.Type == Native.CFIXCTL_LICENSE_TYPE.CfixctlLicensed )
				{
					return this.info.Valid &&
						   ( this.info.Product == ProductId ||
							 this.info.SubProduct == SubProductId );
				}
				else
				{
					return this.info.Valid;
				}
			}
		}

		public bool IsTrial
		{
			get
			{
				return this.info.Type == Native.CFIXCTL_LICENSE_TYPE.CfixctlTrial;
			}
		}

		public uint TrialDaysLeft
		{
			get { return this.info.DaysLeft; }
		}

		public string Key
		{
			get { return this.info.Key; }
		}
	}
}
