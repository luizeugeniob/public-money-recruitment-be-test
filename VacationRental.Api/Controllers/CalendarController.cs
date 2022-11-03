using Microsoft.AspNetCore.Mvc;
using System;
using VacationRental.Application.Interfaces;
using VacationRental.Domain.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarAppService _calendarAppService;

        public CalendarController(ICalendarAppService calendarAppService)
        {
            _calendarAppService = calendarAppService;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            return _calendarAppService.Get(rentalId, start, nights);
        }
    }
}