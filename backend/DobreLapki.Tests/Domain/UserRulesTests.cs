using DobreLapki.Domain.Common;
using DobreLapki.Domain.Models;
using DobreLapki.Domain.Validation;

namespace DobreLapki.Tests.Domain;

public sealed class UserRulesTests
{
    [Fact]
    public void NormalizeRegistration_Valid_LowercasesEmail()
    {
        var result = UserRules.NormalizeRegistration(
            new UserRegistration("  Анна  ", "Anna@Example.com", " +7 900 000-00-00 ", "secret123"));

        Assert.Equal("Анна", result.Name);
        Assert.Equal("anna@example.com", result.Email);
        Assert.Equal("+7 900 000-00-00", result.Phone);
    }

    [Fact]
    public void NormalizeRegistration_ShortPassword_Throws()
    {
        var draft = new UserRegistration("Анна", "anna@example.com", null, "123");

        Assert.Throws<DomainValidationException>(() => UserRules.NormalizeRegistration(draft));
    }

    [Fact]
    public void NormalizeRegistration_InvalidEmail_Throws()
    {
        var draft = new UserRegistration("Анна", "anna", null, "secret123");

        Assert.Throws<DomainValidationException>(() => UserRules.NormalizeRegistration(draft));
    }

    [Fact]
    public void NormalizeRegistration_InvalidPhone_Throws()
    {
        var draft = new UserRegistration("Анна", "anna@example.com", "телефон", "secret123");

        Assert.Throws<DomainValidationException>(() => UserRules.NormalizeRegistration(draft));
    }

    [Fact]
    public void NormalizeUpdate_Valid_ReturnsNormalized()
    {
        var result = UserRules.NormalizeUpdate(
            new UserUpdateDraft("  Анна  ", "Anna@Example.com", "  "));

        Assert.Equal("Анна", result.Name);
        Assert.Equal("anna@example.com", result.Email);
        Assert.Equal(string.Empty, result.Phone);
    }

    [Fact]
    public void NormalizeLogin_EmptyPassword_Throws()
    {
        var draft = new UserLogin("anna@example.com", " ");

        Assert.Throws<DomainValidationException>(() => UserRules.NormalizeLogin(draft));
    }
}
