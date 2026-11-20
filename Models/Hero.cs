```csharp
using System.Text.Json.Serialization;

namespace MlbbHelper.Models;

public class HeroData
{
    public string Patch { get; set; } = "";
    public string Updated { get; set; } = "";
    public Dictionary<string, List<Build>> RoleBuilds { get; set; } = new();
    public List<Hero> Heroes { get; set; } = new();
}

public class Hero
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string NameEn { get; set; } = "";
    public string Role { get; set; } = "";
    public string Image { get; set; } = "";
    public string Weakness { get; set; } = "";
    public List<string> Counters { get; set; } = new();
    public List<Build> Builds { get; set; } = new();

    [JsonIgnore] public string? ImageUrl { get; set; }
    [JsonIgnore] public string Initial => string.IsNullOrEmpty(Name) ? "?" : Name.Substring(0, 1);
}

public class Build
{
    public string Title { get; set; } = "";
    public List<string> Items { get; set; } = new();
    public string Emblem { get; set; } = "";
    public string Spell { get; set; } = "";

    [JsonIgnore] public string Extra => $"Эмблема: {Emblem}  •  Заклинание: {Spell}";
}

public class CounterView
{
    public Hero Hero { get; set; } = null!;
    public string Reason { get; set; } = "";
}
```
