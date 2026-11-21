using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.Maui.Controls;

namespace MlbbHelper.Models;

public sealed class HeroData
{
    public string Patch { get; set; } = "";
    public string Updated { get; set; } = "";
    public string Source { get; set; } = "";
    public List<Hero> Heroes { get; set; } = new();
}

public sealed class Hero
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string NameEn { get; set; } = "";
    public List<string> Roles { get; set; } = new();
    public List<string> Lanes { get; set; } = new();
    public string Image { get; set; } = "";
    public string? ImageUrl { get; set; }
    public List<HeroCounter> Counters { get; set; } = new();
    public List<Build> Builds { get; set; } = new();

    [JsonIgnore]
    public string RoleDisplay => Roles.Count == 0
        ? "Роль не указана"
        : string.Join(" · ", Roles.Select(GameLocalization.RoleName));

    [JsonIgnore]
    public string LaneDisplay => Lanes.Count == 0
        ? "Линия не указана"
        : string.Join(" · ", Lanes.Select(GameLocalization.LaneName));

    [JsonIgnore]
    public ImageSource Portrait => !string.IsNullOrWhiteSpace(ImageUrl)
        ? ImageSource.FromUri(new Uri(ImageUrl))
        : ImageSource.FromFile(Image);

    [JsonIgnore]
    public string Initial => string.IsNullOrWhiteSpace(Name) ? "?" : Name[..1];
}

public sealed class HeroCounter
{
    public string HeroId { get; set; } = "";
    public double Advantage { get; set; }
    public string Rank { get; set; } = "all";
}

public sealed class Build
{
    public string Title { get; set; } = "";
    public string Lane { get; set; } = "";
    public string Rank { get; set; } = "all";
    public List<string> Items { get; set; } = new();
    public string Emblem { get; set; } = "";
    public List<string> Talents { get; set; } = new();
    public string Spell { get; set; } = "";
    public double? WinRate { get; set; }
    public double? PickRate { get; set; }

    [JsonIgnore]
    public string ItemsDisplay => Items.Count == 0 ? "Предметы не указаны" : string.Join("  •  ", Items);

    [JsonIgnore]
    public string Extra => string.Join("  •  ", new[]
    {
        string.IsNullOrWhiteSpace(Emblem) ? null : $"Эмблема: {Emblem}",
        string.IsNullOrWhiteSpace(Spell) ? null : $"Заклинание: {Spell}"
    }.Where(value => value is not null));

    [JsonIgnore]
    public string TalentDisplay => Talents.Count == 0 ? "" : $"Таланты: {string.Join(" · ", Talents)}";

    [JsonIgnore]
    public string StatisticsDisplay
    {
        get
        {
            var culture = CultureInfo.GetCultureInfo("ru-RU");
            var values = new List<string>();
            if (WinRate is > 0) values.Add($"Винрейт {WinRate.Value.ToString("0.#", culture)} %");
            if (PickRate is > 0) values.Add($"Выбор {PickRate.Value.ToString("0.#", culture)} %");
            return string.Join("  ·  ", values);
        }
    }
}

public sealed class CounterView
{
    public Hero Hero { get; set; } = null!;
    public double Advantage { get; set; }

    [JsonIgnore]
    public string AdvantageText => Advantage > 0
        ? $"Преимущество +{Advantage.ToString("0.#", CultureInfo.GetCultureInfo("ru-RU"))} п. п."
        : "Подходит против этого героя";
}
