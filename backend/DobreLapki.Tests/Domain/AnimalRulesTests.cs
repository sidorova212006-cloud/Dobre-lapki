using DobreLapki.Domain.Common;
using DobreLapki.Domain.Enums;
using DobreLapki.Domain.Models;
using DobreLapki.Domain.Validation;

namespace DobreLapki.Tests.Domain;

public sealed class AnimalRulesTests
{
    private static AnimalDraft ValidDraft() => new(
        "Барсик",
        AnimalType.Cat,
        AnimalGender.Male,
        2,
        "Метис",
        "Очень добрый и ласковый кот, который ищет новый дом.",
        new[] { "https://example.com/cat.jpg" },
        "в норме",
        true,
        true,
        new DateOnly(2026, 1, 1));

    [Fact]
    public void Normalize_TrimsAndRemovesHtml()
    {
        var draft = ValidDraft() with { Name = "  <b>Барсик</b>  " };

        var result = AnimalRules.Normalize(draft);

        Assert.Equal("Барсик", result.Name);
    }

    [Fact]
    public void Normalize_EmptyName_Throws()
    {
        var draft = ValidDraft() with { Name = "   " };

        Assert.Throws<DomainValidationException>(() => AnimalRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_ShortDescription_Throws()
    {
        var draft = ValidDraft() with { Description = "коротко" };

        Assert.Throws<DomainValidationException>(() => AnimalRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_InvalidAge_Throws()
    {
        var draft = ValidDraft() with { AgeYears = 51 };

        Assert.Throws<DomainValidationException>(() => AnimalRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_FutureArrival_Throws()
    {
        var draft = ValidDraft() with { ArrivedAt = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1) };

        Assert.Throws<DomainValidationException>(() => AnimalRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_InvalidPhotoUrl_Throws()
    {
        var draft = ValidDraft() with { Photos = new[] { "not-a-url" } };

        Assert.Throws<DomainValidationException>(() => AnimalRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_TooManyPhotos_Throws()
    {
        var draft = ValidDraft() with
        {
            Photos = Enumerable.Range(1, 11)
                .Select(index => $"https://example.com/{index}.jpg")
                .ToList()
        };

        Assert.Throws<DomainValidationException>(() => AnimalRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_InvalidEnum_Throws()
    {
        var draft = ValidDraft() with { Type = (AnimalType)99 };

        Assert.Throws<DomainValidationException>(() => AnimalRules.Normalize(draft));
    }

    [Fact]
    public void Normalize_Valid_ReturnsNormalizedDraft()
    {
        var result = AnimalRules.Normalize(ValidDraft());

        Assert.Equal("Барсик", result.Name);
        Assert.Equal("Метис", result.Breed);
        Assert.NotNull(result.Photos);
        Assert.Single(result.Photos!);
    }
}
