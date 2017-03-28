using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.PacteraException
{
    /// <summary>
    /// Represents errors that occur in the framework.
    /// </summary>
    [Serializable]
    public class RepeatRegistrationException : System.Exception
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <c>PacteraException</c> class.
        /// </summary>
        public RepeatRegistrationException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <c>PacteraException</c> class with the specified
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RepeatRegistrationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <c>PacteraException</c> class with the specified
        /// error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception.</param>
        public RepeatRegistrationException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }
        #endregion

    }
}
