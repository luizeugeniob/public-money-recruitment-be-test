using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Domain.Models;
using VacationRental.Tests.Common;

namespace VacationRental.Tests.Api;

[Collection("Integration")]
public class PutRentalTests
{
    private readonly HttpClient _client;

    public PutRentalTests(IntegrationFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRental()
    {
        var request = new RentalBindingModel
        {
            Units = 25,
            PreparationTimeInDays = 12
        };

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        request = new RentalBindingModel
        {
            Units = 10,
            PreparationTimeInDays = 3
        };

        using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", request))
        {
            Assert.True(putResponse.IsSuccessStatusCode);
            postResult = await putResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
        {
            Assert.True(getResponse.IsSuccessStatusCode);

            var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
            Assert.Equal(request.Units, getResult.Units);
            Assert.Equal(request.PreparationTimeInDays, getResult.PreparationTimeInDays);
        }
    }

    [Fact]
    public async Task GivenCompleteRequest_WhenPutRental_ThenAPutReturnsErrorWhenUpdateWillCauseOverbook()
    {
        var request = new RentalBindingModel
        {
            Units = 3,
            PreparationTimeInDays = 2
        };

        ResourceIdViewModel postRentalResult;
        using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
        {
            Assert.True(postResponse.IsSuccessStatusCode);
            postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
        }

        var postBookingRequest = new BookingBindingModel
        {
            RentalId = postRentalResult.Id,
            Nights = 3,
            Start = new DateTime(2001, 01, 01)
        };

        using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
        {
            Assert.True(postBookingResponse.IsSuccessStatusCode);
        }

        using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
        {
            Assert.True(postBookingResponse.IsSuccessStatusCode);
        }

        using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
        {
            Assert.True(postBookingResponse.IsSuccessStatusCode);
        }

        request = new RentalBindingModel
        {
            Units = 2,
            PreparationTimeInDays = 2
        };

        using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", request))
        {
            Assert.Equal(HttpStatusCode.InternalServerError, putResponse.StatusCode);
        }
    }
}