using Microsoft.AspNetCore.Mvc;
using VacationRental.Application.Interfaces;
using VacationRental.Domain.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingAppService _bookingAppService;

        public BookingsController(IBookingAppService bookingAppService)
        {
            _bookingAppService = bookingAppService;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            return _bookingAppService.Get(bookingId);
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            return _bookingAppService.Post(model);
        }
    }
}