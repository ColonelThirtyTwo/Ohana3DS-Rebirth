using System;

namespace Ohana3DS_Rebirth.Ohana
{
    /// <summary>
    /// Exception thrown when invalid data is encountered when parsing a file
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message)
        {
        }
    }
}
