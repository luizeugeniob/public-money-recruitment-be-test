using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VacationRental.Domain.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class RentalNotFoundException : Exception
{
    public RentalNotFoundException() : base("Rental not found")
    {
    }

    protected RentalNotFoundException(string? message) : base(message)
    {
    }

    protected RentalNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected RentalNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}