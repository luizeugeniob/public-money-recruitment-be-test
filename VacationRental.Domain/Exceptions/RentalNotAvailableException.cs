using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VacationRental.Domain.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class RentalNotAvailableException : Exception
{
    public RentalNotAvailableException() : base("Rental not available")
    {
    }

    protected RentalNotAvailableException(string? message) : base(message)
    {
    }

    protected RentalNotAvailableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected RentalNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}