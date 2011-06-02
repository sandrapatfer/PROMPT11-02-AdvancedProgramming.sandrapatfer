﻿using System;
using System.Runtime.Serialization;

namespace DIChelas.Exceptions
{
    [Serializable]
    public class DIChelasException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DIChelasException()
        {
        }

        public DIChelasException(string message) : base(message)
        {
        }

        public DIChelasException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DIChelasException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}