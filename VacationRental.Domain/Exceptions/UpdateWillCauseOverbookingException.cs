using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VacationRental.Domain.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class UpdateWillCauseOverbookingException : Exception
{
    public UpdateWillCauseOverbookingException() : base("Rental cannot be updated as it would cause an overbooking")
    {
    }

    protected UpdateWillCauseOverbookingException(string message) : base(message)
    {
    }

    protected UpdateWillCauseOverbookingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected UpdateWillCauseOverbookingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}