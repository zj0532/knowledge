using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.PacteraException
{
	[Serializable]
	public class DataValidationException : Exception
	{
		public DataValidationException(string message) : base(message) { }
		public DataValidationException(string message, Exception innerException) : base(message, innerException) { }
	}
}
