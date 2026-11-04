using DobreLapki.Domain.Common;
using DobreLapki.Domain.Models;
using DobreLapki.Domain.Validation;

namespace DobreLapki.Tests.Domain;

public sealed class ApplicationRulesTests
{
    private static ApplicationDraft ValidDraft() => new(
        1,
        "Иван Петров",
        "Ivan@Example.com",
        "+7 900 111-22-33",
        "Люблю животных, есть опыт содержания собак и кошек.");

    [Fact]
    public void Normalize_Valid_LowercasesEmail()
    {
        var result = ApplicationRules.Normalize(ValidDraft());

        Assert.Equal("ivan@example.com", result.Email);
        Assert.Equal("Иван Петров", result.FullName);
    }

    [Fact]
    public void Normalize_ZeroAnimalId_Throws()
    {
        var draft = ValidDraft() with { AnimalId = 0 };

        Assert.Throws<DomainValidationException>(() => ApplicationRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_InvalidEmail_Throws()
    {
        var draft = ValidDraft() with { Email = "not-an-email" };

        Assert.Throws<DomainValidationException>(() => ApplicationRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_ShortPhone_Throws()
    {
        var draft = ValidDraft() with { Phone = "123" };

        Assert.Throws<DomainValidationException>(() => ApplicationRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_ShortAbout_Throws()
    {
        var draft = ValidDraft() with { About = "мало" };

        Assert.Throws<DomainValidationException>(() => ApplicationRules.Normalize(draft));
    }
}
