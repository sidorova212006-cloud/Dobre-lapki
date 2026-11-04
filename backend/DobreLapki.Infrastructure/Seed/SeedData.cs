using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Enums;
using DobreLapki.Domain.Security;

namespace DobreLapki.Infrastructure.Seed;

/// <summary>Демонстрационные данные для in-memory-хранилищ.</summary>
public static class SeedData
{
    /// <summary>Email демонстрационного пользователя.</summary>
    public const string DemoEmail = "user@example.com";

    /// <summary>Пароль демонстрационного пользователя.</summary>
    public const string DemoPassword = "password123";

    /// <summary>Создаёт каталог животных.</summary>
    public static IReadOnlyList<Animal> CreateAnimals()
    {
        return new List<Animal>
        {
            Animal(1, "Мурка", AnimalType.Cat, AnimalGender.Female, 1, "Домашняя короткошёрстная",
                "Ласковая и спокойная кошка. Обожает сидеть на коленях и мурчать под любимую передачу. Отлично ладит с детьми и другими животными.",
                "cat-1", "в норме", true, true, new DateOnly(2026, 9, 28)),
            Animal(2, "Рекс", AnimalType.Dog, AnimalGender.Male, 2, "Лабрадор",
                "Активный и очень дружелюбный пёс. Любит бегать, играть с мячом и учиться новым командам. Ищет семью, готовую к прогулкам.",
                "dog-1", "в норме", true, true, new DateOnly(2026, 9, 20)),
            Animal(3, "Луна", AnimalType.Cat, AnimalGender.Female, 4, "Британская короткошёрстная",
                "Спокойная кошка с мягкой плюшевой шёрсткой. Ненавязчива, но всегда рядом. Хорошо переносит одиночество.",
                "cat-2", "в норме", true, true, new DateOnly(2026, 8, 15)),
            Animal(4, "Бим", AnimalType.Dog, AnimalGender.Male, 5, "Метис",
                "Верный и умный пёс. Знает базовые команды, спокоен на поводке. Будет идеальным компаньоном для семьи с детьми.",
                "dog-2", "в норме", true, true, new DateOnly(2026, 7, 30)),
            Animal(5, "Персик", AnimalType.Cat, AnimalGender.Male, 0.6, "Домашняя короткошёрстная",
                "Озорной котёнок-непоседа. Любит играть с бантиками и исследовать всё вокруг. Растёт ручным и общительным.",
                "cat-3", "в норме", false, true, new DateOnly(2026, 10, 1)),
            Animal(6, "Найда", AnimalType.Dog, AnimalGender.Female, 3, "Овчарка",
                "Умная и преданная собака. Отличный охранник и нежная подруга. Нужны регулярные нагрузки и внимание.",
                "dog-3", "в норме", true, true, new DateOnly(2026, 6, 11)),
            Animal(7, "Симба", AnimalType.Cat, AnimalGender.Male, 2, "Мейн-кун",
                "Крупный и величественный кот с добрым сердцем. Спокоен, ласков, любит наблюдать с высоты. Настоящий друг семьи.",
                "cat-4", "в норме", true, true, new DateOnly(2026, 9, 5)),
            Animal(8, "Белла", AnimalType.Dog, AnimalGender.Female, 1, "Джек-рассел",
                "Энергичная и весёлая девочка. Обожает игры и активные прогулки. Подойдёт активным хозяевам.",
                "dog-4", "в норме", false, true, new DateOnly(2026, 9, 30)),
            Animal(9, "Багира", AnimalType.Cat, AnimalGender.Female, 6, "Домашняя короткошёрстная",
                "Взрослая кошка с характером. Ценит личное пространство, но отвечает нежностью тем, кто заслужил доверие.",
                "cat-5", "в норме", true, true, new DateOnly(2026, 5, 2)),
            Animal(10, "Лео", AnimalType.Dog, AnimalGender.Male, 0.8, "Метис",
                "Молодой пёс, полный энтузиазма. Быстро учится, обожает людей и других собак. Ищет терпеливого хозяина.",
                "dog-5", "в норме", false, true, new DateOnly(2026, 10, 5)),
            Animal(11, "Тимка", AnimalType.Cat, AnimalGender.Male, 3, "Домашняя короткошёрстная",
                "Общительный кот-компаньон. Разговорчив, любит внимание и охотно спит рядом с хозяином.",
                "cat-6", "в норме", true, true, new DateOnly(2026, 8, 1)),
            Animal(12, "Арчи", AnimalType.Dog, AnimalGender.Male, 7, "Такса",
                "Спокойный и ласковый пёс в зрелом возрасте. Идеален для размеренной жизни. Очень привязан к человеку.",
                "dog-6", "в норме", true, true, new DateOnly(2026, 4, 18)),
            Animal(13, "Джесси", AnimalType.Dog, AnimalGender.Female, 2, "Хаски",
                "Красавица с голубыми глазами и добрым нравом. Нужны долгие прогулки и простор для активности.",
                "dog-7", "в норме", true, true, new DateOnly(2026, 9, 12)),
            Animal(14, "Пушок", AnimalType.Other, AnimalGender.Male, 1, "Декоративный кролик",
                "Мягкий и пушистый кролик. Спокойный и аккуратный, отлично подойдёт для семьи, которая ищет необычного питомца.",
                "other-1", "в норме", false, false, new DateOnly(2026, 7, 7)),
            Animal(15, "Муся", AnimalType.Cat, AnimalGender.Female, 1, "Домашняя короткошёрстная",
                "Игривая и любопытная кошка. Быстро привыкает к дому и людям, любит активные игры и ласку.",
                "cat-7", "в норме", true, true, new DateOnly(2026, 9, 25))
        };
    }

    /// <summary>Создаёт демонстрационные заявки.</summary>
    public static IReadOnlyList<AdoptionApplication> CreateApplications(IReadOnlyList<Animal> animals)
    {
        return new List<AdoptionApplication>
        {
            new()
            {
                Id = 1,
                Animal = animals[0],
                Status = ApplicationStatus.Approved,
                FullName = "Анна Смирнова",
                Email = DemoEmail,
                Phone = "+7 900 000-00-00",
                About = "Опыт содержания кошек более пяти лет, живу в квартире без других животных.",
                CreatedAt = new DateTime(2026, 9, 30, 10, 12, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = 2,
                Animal = animals[4],
                Status = ApplicationStatus.Pending,
                FullName = "Анна Смирнова",
                Email = DemoEmail,
                Phone = "+7 900 000-00-00",
                About = "Готова забрать котёнка, есть всё необходимое для ухода и игр.",
                CreatedAt = new DateTime(2026, 10, 8, 16, 40, 0, DateTimeKind.Utc)
            }
        };
    }

    /// <summary>Создаёт демонстрационного пользователя.</summary>
    public static User CreateDemoUser(IPasswordHasher passwordHasher)
    {
        var (hash, salt) = passwordHasher.HashPassword(DemoPassword);
        return new User
        {
            Id = 1,
            Name = "Анна",
            Email = DemoEmail,
            Phone = "+7 900 000-00-00",
            Avatar = "https://picsum.photos/seed/dp-user/200/200",
            PasswordHash = hash,
            PasswordSalt = salt
        };
    }

    private static Animal Animal(
        int id,
        string name,
        AnimalType type,
        AnimalGender gender,
        double ageYears,
        string breed,
        string description,
        string slug,
        string health,
        bool sterilized,
        bool vaccinated,
        DateOnly arrivedAt)
    {
        return new Animal
        {
            Id = id,
            Name = name,
            Type = type,
            Gender = gender,
            AgeYears = ageYears,
            Breed = breed,
            Description = description,
            Photos = Enumerable.Range(1, 3)
                .Select(n => $"https://picsum.photos/seed/dp-{slug}-{n}/800/800")
                .ToList(),
            Health = health,
            Sterilized = sterilized,
            Vaccinated = vaccinated,
            ArrivedAt = arrivedAt
        };
    }
}
