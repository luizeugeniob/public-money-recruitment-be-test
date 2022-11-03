using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VacationRental.Domain.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class BookingNotFoundException : Exception
{
    public BookingNotFoundException() : base("Booking not found")
    {
    }

    protected BookingNotFoundException(string? message) : base(message)
    {
    }

    protected BookingNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BookingNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}