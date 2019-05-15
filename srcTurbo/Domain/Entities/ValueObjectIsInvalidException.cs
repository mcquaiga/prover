#region

using System;

#endregion

namespace Domain.Entities
{
    public class ValueObjectIsInvalidException : Exception
    {
        public ValueObjectIsInvalidException(string message)
            : base(message)
        {
        }
    }
}