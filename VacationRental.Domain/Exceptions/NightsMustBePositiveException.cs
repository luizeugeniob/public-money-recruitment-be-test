using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VacationRental.Domain.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class NightsMustBePositiveException : Exception
{
    public NightsMustBePositiveException() : base("Nights must be positive")
    {
    }

    protected NightsMustBePositiveException(string? message) : base(message)
    {
    }

    protected NightsMustBePositiveException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected NightsMustBePositiveException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}