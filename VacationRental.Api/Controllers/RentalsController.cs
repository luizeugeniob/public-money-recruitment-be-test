using Microsoft.AspNetCore.Mvc;
using VacationRental.Application.Interfaces;
using VacationRental.Application.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalAppService _rentalAppService;

        public RentalsController(IRentalAppService rentalAppService)
        {
            _rentalAppService = rentalAppService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            return _rentalAppService.Get(rentalId);
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            return _rentalAppService.Post(model);
        }
    }
}