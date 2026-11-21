using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Maui.Storage;
using MlbbHelper.Models;

namespace MlbbHelper.Services;

public enum UpdateResult
{
    Updated,
    UpToDate,
    Offline
}

public sealed class DataService
{
    const string ApiBase = "https://mlbbdex.com/api/v1";
    const string PortraitBase = "https://mlbbdex.com/visuels/heros/";
    static readonly HttpClient Http = CreateHttpClient();
    static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static DataService Instance { get; } = new();
    public HeroData Data { get; private set; } = new();

    string CachePath => Path.Combine(FileSystem.AppDataDirectory, "heroes-cache.json");

    static HttpClient CreateHttpClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(12) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MLBBHelper", "1.0"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    public async Task LoadAsync()
    {
        HeroData? cached = null;
        try
        {
            if (File.Exists(CachePath))
                cached = ParseData(await File.ReadAllTextAsync(CachePath));
        }
        catch
        {
            // При повреждённом кэше приложение использует встроенный справочник.
        }

        if (cached is null || cached.Heroes.Count == 0)
        {
            await using var stream = await FileSystem.OpenAppPackageFileAsync("heroes.json");
            using var reader = new StreamReader(stream);
            cached = ParseData(await reader.ReadToEndAsync());
        }

        Data = cached ?? new HeroData();
        NormalizeData();
    }

    public async Task<UpdateResult> UpdateCatalogAsync()
    {
        var catalogTask = TryGetStringAsync($"{ApiBase}/heroes");
        var patchesTask = TryGetStringAsync($"{ApiBase}/patches");
        await Task.WhenAll(catalogTask, patchesTask);

        var catalogJson = await catalogTask;
        var patchesJson = await patchesTask;
        var gotResponse = catalogJson is not null || patchesJson is not null;
        var changed = false;

        if (catalogJson is not null)
            changed |= MergeCatalog(catalogJson);
        if (patchesJson is not null)
            changed |= MergeLatestPatch(patchesJson);

        if (changed)
            await SaveCacheAsync();

        if (!gotResponse)
            return UpdateResult.Offline;

        return changed ? UpdateResult.Updated : UpdateResult.UpToDate;
    }

    public async Task<bool> RefreshHeroDetailsAsync(Hero hero)
    {
        var countersTask = TryGetStringAsync($"{ApiBase}/heroes/{Uri.EscapeDataString(hero.Id)}/counters");
        var buildsTask = TryGetStringAsync($"{ApiBase}/heroes/{Uri.EscapeDataString(hero.Id)}/builds");
        await Task.WhenAll(countersTask, buildsTask);

        var countersJson = await countersTask;
        var buildsJson = await buildsTask;
        var changed = false;

        if (countersJson is not null)
            changed |= MergeCounters(hero, countersJson);
        if (buildsJson is not null)
            changed |= MergeBuilds(hero, buildsJson);

        if (changed)
            await SaveCacheAsync();

        return changed;
    }

    public List<Hero> Search(string query)
    {
        var normalizedQuery = Normalize(query);
        var heroes = Data.Heroes
            .OrderBy(hero => hero.Name, StringComparer.CurrentCultureIgnoreCase);

        if (normalizedQuery.Length == 0)
            return heroes.ToList();

        return heroes
            .Where(hero => Normalize(hero.Name).Contains(normalizedQuery)
                           || Normalize(hero.NameEn).Contains(normalizedQuery)
                           || Normalize(hero.Id).Replace('-', ' ').Contains(normalizedQuery)
                           || Normalize(hero.RoleDisplay).Contains(normalizedQuery)
                           || Normalize(hero.LaneDisplay).Contains(normalizedQuery))
            .OrderBy(hero => IsPrefix(hero, normalizedQuery) ? 0 : 1)
            .ThenBy(hero => hero.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    public Hero? Find(string id) => Data.Heroes.FirstOrDefault(hero =>
        string.Equals(hero.Id, id, StringComparison.OrdinalIgnoreCase));

    public List<CounterView> GetCounters(Hero hero)
    {
        return hero.Counters
            .Where(counter => counter.Advantage > 0)
            .OrderByDescending(counter => counter.Advantage)
            .Select(counter => new CounterView
            {
                Hero = Find(counter.HeroId)!,
                Advantage = counter.Advantage
            })
            .Where(counter => counter.Hero is not null)
            .ToList();
    }

    public List<Build> GetBuilds(Hero hero) => hero.Builds;

    static bool IsPrefix(Hero hero, string query) =>
        Normalize(hero.Name).StartsWith(query, StringComparison.Ordinal)
        || Normalize(hero.NameEn).StartsWith(query, StringComparison.Ordinal);

    async Task<string?> TryGetStringAsync(string url)
    {
        try
        {
            using var response = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return null;
        }
    }

    bool MergeCatalog(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!TryGetData(document.RootElement, out var payload) || payload.ValueKind != JsonValueKind.Array)
                return false;

            var changed = false;
            foreach (var item in payload.EnumerateArray())
            {
                var id = ReadString(item, "slug");
                var englishName = ReadString(item, "name");
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(englishName))
                    continue;

                var roles = ReadStrings(item, "roles");
                var lanes = ReadStrings(item, "lanes");
                var hero = Find(id);
                if (hero is null)
                {
                    hero = new Hero
                    {
                        Id = id,
                        Name = GameLocalization.HeroName(id, englishName),
                        NameEn = englishName,
                        Roles = roles,
                        Lanes = lanes,
                        ImageUrl = PortraitBase + Uri.EscapeDataString(id) + "/portrait.webp"
                    };
                    Data.Heroes.Add(hero);
                    changed = true;
                    continue;
                }

                var localizedName = GameLocalization.HeroName(id, englishName);
                if (hero.NameEn != englishName || hero.Name != localizedName)
                {
                    hero.NameEn = englishName;
                    hero.Name = localizedName;
                    changed = true;
                }

                if (!hero.Roles.SequenceEqual(roles, StringComparer.OrdinalIgnoreCase))
                {
                    hero.Roles = roles;
                    changed = true;
                }

                if (!hero.Lanes.SequenceEqual(lanes, StringComparer.OrdinalIgnoreCase))
                {
                    hero.Lanes = lanes;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(hero.Image) && string.IsNullOrWhiteSpace(hero.ImageUrl))
                {
                    hero.ImageUrl = PortraitBase + Uri.EscapeDataString(id) + "/portrait.webp";
                    changed = true;
                }
            }

            return changed;
        }
        catch
        {
            return false;
        }
    }

    bool MergeLatestPatch(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!TryGetData(document.RootElement, out var payload))
                return false;

            JsonElement patch;
            if (payload.ValueKind == JsonValueKind.Array)
            {
                if (payload.GetArrayLength() == 0)
                    return false;
                patch = payload[0];
            }
            else if (payload.ValueKind == JsonValueKind.Object)
            {
                if (payload.TryGetProperty("patches", out var patches) && patches.ValueKind == JsonValueKind.Array)
                {
                    if (patches.GetArrayLength() == 0)
                        return false;
                    patch = patches[0];
                }
                else
                {
                    patch = payload;
                }
            }
            else
            {
                return false;
            }

            var version = ReadString(patch, "version");
            if (string.IsNullOrWhiteSpace(version) || Data.Patch == version)
                return false;

            Data.Patch = version;
            return true;
        }
        catch
        {
            return false;
        }
    }

    static bool MergeCounters(Hero hero, string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!TryGetData(document.RootElement, out var payload) || payload.ValueKind != JsonValueKind.Object)
                return false;

            var allRanks = TryGetRank(payload, out var rankData) ? rankData : default;
            if (allRanks.ValueKind != JsonValueKind.Object
                || !allRanks.TryGetProperty("weak", out var weak)
                || weak.ValueKind != JsonValueKind.Array)
                return false;

            var counters = new List<HeroCounter>();
            foreach (var item in weak.EnumerateArray().Take(8))
            {
                var id = ReadString(item, "slug");
                if (string.IsNullOrWhiteSpace(id) || id == hero.Id)
                    continue;

                var advantage = ReadDouble(item, "advantage");
                if (advantage is null)
                    continue;

                counters.Add(new HeroCounter
                {
                    HeroId = id,
                    Advantage = Math.Round(Math.Abs(advantage.Value), 1),
                    Rank = "all"
                });
            }

            hero.Counters = counters;
            return true;
        }
        catch
        {
            return false;
        }
    }

    static bool MergeBuilds(Hero hero, string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!TryGetData(document.RootElement, out var payload) || payload.ValueKind != JsonValueKind.Object)
                return false;

            var builds = new List<Build>();
            foreach (var lane in hero.Lanes)
            {
                if (!payload.TryGetProperty(lane, out var laneData) || laneData.ValueKind != JsonValueKind.Object)
                    continue;

                if (!TryGetRank(laneData, out var rankData) || rankData.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var item in rankData.EnumerateArray().Take(3))
                {
                    var items = ReadStrings(item, "items");
                    if (items.Count == 0)
                        continue;

                    var rank = ReadRankName(laneData);
                    var build = new Build
                    {
                        Title = $"{GameLocalization.LaneName(lane)} · {GameLocalization.RankName(rank)}",
                        Lane = lane,
                        Rank = rank,
                        Items = items,
                        Emblem = GameLocalization.EmblemName(ReadString(item, "emblem")),
                        Talents = ReadStrings(item, "talents").Select(GameLocalization.TalentName).ToList(),
                        Spell = GameLocalization.SpellName(ReadString(item, "spell")),
                        WinRate = ReadDouble(item, "winRate"),
                        PickRate = ReadDouble(item, "pickRate")
                    };
                    builds.Add(build);
                }
            }

            if (builds.Count == 0)
                return false;

            hero.Builds = builds;
            return true;
        }
        catch
        {
            return false;
        }
    }

    static string ReadRankName(JsonElement laneData)
    {
        foreach (var rank in new[] { "all", "mythic", "honor", "glory", "legend", "epic" })
            if (laneData.TryGetProperty(rank, out var value) && value.ValueKind == JsonValueKind.Array && value.GetArrayLength() > 0)
                return rank;

        foreach (var property in laneData.EnumerateObject())
            if (property.Value.ValueKind == JsonValueKind.Array && property.Value.GetArrayLength() > 0)
                return property.Name;

        return "all";
    }

    static bool TryGetRank(JsonElement ranks, out JsonElement selected)
    {
        foreach (var rank in new[] { "all", "mythic", "honor", "glory", "legend", "epic" })
        {
            if (ranks.TryGetProperty(rank, out selected)
                && selected.ValueKind is JsonValueKind.Object or JsonValueKind.Array
                && (selected.ValueKind != JsonValueKind.Array || selected.GetArrayLength() > 0))
                return true;
        }

        foreach (var property in ranks.EnumerateObject())
        {
            if (property.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Array
                && (property.Value.ValueKind != JsonValueKind.Array || property.Value.GetArrayLength() > 0))
            {
                selected = property.Value;
                return true;
            }
        }

        selected = default;
        return false;
    }

    static bool TryGetData(JsonElement root, out JsonElement payload)
    {
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out payload))
            return true;
        payload = default;
        return false;
    }

    static string ReadString(JsonElement element, string property)
    {
        if (element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(property, out var value)
            && value.ValueKind == JsonValueKind.String)
            return value.GetString()?.Trim() ?? "";
        return "";
    }

    static List<string> ReadStrings(JsonElement element, string property)
    {
        var values = new List<string>();
        if (element.ValueKind != JsonValueKind.Object
            || !element.TryGetProperty(property, out var array)
            || array.ValueKind != JsonValueKind.Array)
            return values;

        foreach (var value in array.EnumerateArray())
            if (value.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(value.GetString()))
                values.Add(value.GetString()!.Trim());
        return values;
    }

    static double? ReadDouble(JsonElement element, string property)
    {
        if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(property, out var value))
            return null;
        if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var number))
            return number;
        if (value.ValueKind == JsonValueKind.String
            && double.TryParse(value.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out number))
            return number;
        return null;
    }

    static HeroData? ParseData(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<HeroData>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    void NormalizeData()
    {
        Data.Heroes ??= new List<Hero>();
        foreach (var hero in Data.Heroes)
        {
            hero.Roles ??= new List<string>();
            hero.Lanes ??= new List<string>();
            hero.Counters ??= new List<HeroCounter>();
            hero.Builds ??= new List<Build>();
        }
    }

    async Task SaveCacheAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(Data, JsonOptions);
            await File.WriteAllTextAsync(CachePath, json);
        }
        catch
        {
            // Ошибка записи кэша не должна мешать просмотру справочника.
        }
    }

    static string Normalize(string value) => value.Trim().ToLowerInvariant().Replace('ё', 'е');
}
