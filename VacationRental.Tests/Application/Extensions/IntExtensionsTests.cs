﻿using System;
using System.Collections.Generic;

namespace VacationRental.Tests.Application.Extensions;

public class IntExtensionsTests
{
    [Theory]
    [InlineData(5, 1)]
    [InlineData(5, 2)]
    [InlineData(5, 3)]
    [InlineData(5, 4)]
    [InlineData(5, 5)]
    public void Given_(int maxNumber, int expectedResult)
    {
        // Arrange
        var except = new List<int> { expectedResult };

        // Act
        var result = maxNumber.RandomIntExcept(except);

        // Assert
        Assert.NotEqual(expectedResult, result);
    }
}