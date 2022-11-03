using System;
using System.Collections.Generic;

namespace VacationRental.Application.Models
{
    public class CalendarDateViewModel
    {
        public DateTime Date { get; set; }
        public List<CalendarBookingViewModel> Bookings { get; set; }
    }
}
