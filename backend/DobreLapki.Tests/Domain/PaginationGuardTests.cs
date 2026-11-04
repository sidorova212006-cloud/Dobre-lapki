using DobreLapki.Domain.Common;
using DobreLapki.Domain.Validation;

namespace DobreLapki.Tests.Domain;

public sealed class PaginationGuardTests
{
    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public void Validate_InvalidArguments_Throws(int page, int pageSize)
    {
        Assert.Throws<DomainValidationException>(() => PaginationGuard.Validate(page, pageSize));
    }

    [Fact]
    public void Validate_ValidArguments_DoesNotThrow()
    {
        PaginationGuard.Validate(1, 20);
    }

    [Fact]
    public void Skip_CalculatesOffset()
    {
        Assert.Equal(40, PaginationGuard.Skip(3, 20));
    }
}
