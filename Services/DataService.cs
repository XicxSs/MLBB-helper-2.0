Привет, мне нужно написать полноценное андроид приложение MLBB helper, писать на c#. Функции 1. Выб
claude-sonnet-5-5-high
# Все блоки по порядку: что называть и куда вставлять

Файлов всего **14**. Для каждого блока вы делаете одно и то же:

1. На странице репозитория нажмите **Add file → Create new file**.
2. В **верхнее узкое поле** (Name your file…) вставьте имя из пункта «Имя файла». Знак `/` создаёт папку сам.
3. В **большое поле ниже** вставьте код из пункта «Содержимое».
4. Нажмите зелёную кнопку **Commit changes…**, затем ещё раз **Commit changes** в окне.

**Блоки нужно создавать по порядку. Блок 14 идёт последним, потому что он запускает сборку.**

Я добавил в блок 1 одну строку, которой не было раньше. Она нужна, чтобы файл `heroes.json` гарантированно попал внутрь приложения. Если вы уже создали блок 1 по старой версии, откройте файл, нажмите ✏️ и замените содержимое.

## Как получится структура

```
mlbb-helper (репозиторий)
├── MlbbHelper.csproj                  ← блок 1
├── MauiProgram.cs                     ← блок 2
├── App.xaml                           ← блок 3
├── App.xaml.cs                        ← блок 4
├── MainPage.xaml                      ← блок 5
├── MainPage.xaml.cs                   ← блок 6
├── HeroPage.xaml                      ← блок 7
├── HeroPage.xaml.cs                   ← блок 8
├── Models/Hero.cs                     ← блок 9
├── Services/DataService.cs            ← блок 10 (нужна правка логина)
├── Platforms/Android/MainActivity.cs  ← блок 11
├── Platforms/Android/AndroidManifest.xml ← блок 12
├── Resources/Raw/heroes.json          ← блок 13
└── .github/workflows/build.yml        ← блок 14 (последним)
```

---

## БЛОК 1

**Имя файла:**
```
MlbbHelper.csproj
```
**Содержимое:**
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net8.0-android</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <RootNamespace>MlbbHelper</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <ApplicationTitle>MLBB Помощник</ApplicationTitle>
    <ApplicationId>com.mlbbhelper.app</ApplicationId>
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
    <ApplicationVersion>1</ApplicationVersion>
    <SupportedOSPlatformVersion>24.0</SupportedOSPlatformVersion>

    <AndroidPackageFormat>apk</AndroidPackageFormat>
    <AndroidLinkMode>None</AndroidLinkMode>
    <PublishTrimmed>false</PublishTrimmed>
  </PropertyGroup>

  <ItemGroup>
    <MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
  </ItemGroup>

</Project>
```

---

## БЛОК 2

**Имя файла:**
```
MauiProgram.cs
```
**Содержимое:**
```csharp
namespace MlbbHelper;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        return builder.Build();
    }
}
```

---

## БЛОК 3

**Имя файла:**
```
App.xaml
```
**Содержимое:**
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MlbbHelper.App">
    <Application.Resources>
        <ResourceDictionary>

            <Style x:Key="Glass" TargetType="Border">
                <Setter Property="StrokeShape" Value="RoundRectangle 22" />
                <Setter Property="StrokeThickness" Value="1" />
                <Setter Property="Stroke">
                    <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                        <GradientStop Color="#99BFE8FF" Offset="0" />
                        <GradientStop Color="#331E6BFF" Offset="1" />
                    </LinearGradientBrush>
                </Setter>
                <Setter Property="Background">
                    <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                        <GradientStop Color="#3A6FB8FF" Offset="0" />
                        <GradientStop Color="#1A1A3A8F" Offset="1" />
                    </LinearGradientBrush>
                </Setter>
            </Style>

        </ResourceDictionary>
    </Application.Resources>
</Application>
```

---

## БЛОК 4

**Имя файла:**
```
App.xaml.cs
```
**Содержимое:**
```csharp
namespace MlbbHelper;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new MainPage()));
    }
}
```

---

## БЛОК 5

**Имя файла:**
```
MainPage.xaml
```
**Содержимое:**
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MlbbHelper.MainPage"
             NavigationPage.HasNavigationBar="False">

    <Grid>
        <Grid.Background>
            <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                <GradientStop Color="#050B1F" Offset="0" />
                <GradientStop Color="#0B1F4D" Offset="0.55" />
                <GradientStop Color="#061233" Offset="1" />
            </LinearGradientBrush>
        </Grid.Background>

        <Ellipse WidthRequest="320" HeightRequest="320" InputTransparent="True"
                 HorizontalOptions="Start" VerticalOptions="Start"
                 TranslationX="-130" TranslationY="-80">
            <Ellipse.Fill>
                <RadialGradientBrush Center="0.5,0.5" Radius="0.5">
                    <GradientStop Color="#552E8BFF" Offset="0" />
                    <GradientStop Color="#002E8BFF" Offset="1" />
                </RadialGradientBrush>
            </Ellipse.Fill>
        </Ellipse>

        <Ellipse WidthRequest="360" HeightRequest="360" InputTransparent="True"
                 HorizontalOptions="End" VerticalOptions="End"
                 TranslationX="140" TranslationY="120">
            <Ellipse.Fill>
                <RadialGradientBrush Center="0.5,0.5" Radius="0.5">
                    <GradientStop Color="#4400C8FF" Offset="0" />
                    <GradientStop Color="#0000C8FF" Offset="1" />
                </RadialGradientBrush>
            </Ellipse.Fill>
        </Ellipse>

        <Grid RowDefinitions="Auto,Auto,Auto,*" Padding="16,44,16,0" RowSpacing="12">

            <VerticalStackLayout Grid.Row="0" Spacing="2">
                <Label Text="MLBB Помощник" FontSize="28" FontAttributes="Bold" TextColor="#EAF6FF" />
                <Label x:Name="PatchLabel" Text="Загрузка данных…" FontSize="13" TextColor="#8FC8FF" />
            </VerticalStackLayout>

            <Border Grid.Row="1" Style="{StaticResource Glass}" HeightRequest="52" Padding="16,0">
                <Grid ColumnDefinitions="Auto,*" ColumnSpacing="10">
                    <Label Text="🔍" VerticalOptions="Center" FontSize="16" />
                    <Entry x:Name="SearchEntry" Grid.Column="1"
                           Placeholder="Введите имя героя"
                           PlaceholderColor="#7FA6D6"
                           TextColor="#EAF6FF"
                           BackgroundColor="Transparent"
                           ClearButtonVisibility="WhileEditing"
                           TextChanged="OnSearchChanged" />
                </Grid>
            </Border>

            <Label x:Name="StatusLabel" Grid.Row="2" FontSize="12" TextColor="#7FB4EE" />

            <RefreshView x:Name="Refresh" Grid.Row="3" Refreshing="OnRefreshing" RefreshColor="#4FC3FF">
                <CollectionView x:Name="HeroList" SelectionMode="None">
                    <CollectionView.ItemsLayout>
                        <GridItemsLayout Orientation="Vertical" Span="3"
                                         VerticalItemSpacing="10" HorizontalItemSpacing="4" />
                    </CollectionView.ItemsLayout>

                    <CollectionView.EmptyView>
                        <Label Text="Герой не найден. Проверьте написание имени."
                               HorizontalTextAlignment="Center" TextColor="#8FC8FF"
                               Margin="0,40,0,0" />
                    </CollectionView.EmptyView>

                    <CollectionView.ItemTemplate>
                        <DataTemplate>
                            <VerticalStackLayout Spacing="8" Padding="8,12">
                                <VerticalStackLayout.GestureRecognizers>
                                    <TapGestureRecognizer Tapped="OnHeroTapped" />
                                </VerticalStackLayout.GestureRecognizers>

                                <Border WidthRequest="80" HeightRequest="80"
                                        HorizontalOptions="Center"
                                        StrokeShape="RoundRectangle 22"
                                        Stroke="#7FD6FF" StrokeThickness="2"
                                        BackgroundColor="#15305F">
                                    <Border.Shadow>
                                        <Shadow Brush="#35C6FF" Radius="16" Opacity="0.9" Offset="0,0" />
                                    </Border.Shadow>
                                    <Grid>
                                        <Label Text="{Binding Initial}" FontSize="32" FontAttributes="Bold"
                                               TextColor="#BFE8FF"
                                               HorizontalOptions="Center" VerticalOptions="Center" />
                                        <Image Source="{Binding ImageUrl}" Aspect="AspectFill" />
                                    </Grid>
                                </Border>

                                <Label Text="{Binding Name}" FontSize="13" TextColor="#EAF6FF"
                                       HorizontalTextAlignment="Center"
                                       LineBreakMode="TailTruncation" MaxLines="1" />
                            </VerticalStackLayout>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </RefreshView>
        </Grid>
    </Grid>
</ContentPage>
```

---

## БЛОК 6

**Имя файла:**
```
MainPage.xaml.cs
```
**Содержимое:**
```csharp
using System.Collections.ObjectModel;
using MlbbHelper.Models;
using MlbbHelper.Services;

namespace MlbbHelper;

public partial class MainPage : ContentPage
{
    readonly ObservableCollection<Hero> _shown = new();
    bool _started;

    public MainPage()
    {
        InitializeComponent();
        HeroList.ItemsSource = _shown;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_started) return;
        _started = true;

        await DataService.Instance.LoadAsync();
        ApplyFilter();
        await CheckUpdatesAsync();
    }

    void ApplyFilter()
    {
        var list = DataService.Instance.Search(SearchEntry.Text ?? "");
        _shown.Clear();
        foreach (var hero in list)
            _shown.Add(hero);

        var data = DataService.Instance.Data;
        PatchLabel.Text = $"Патч {data.Patch}  •  героев: {data.Heroes.Count}";
    }

    async Task CheckUpdatesAsync()
    {
        StatusLabel.Text = "Проверка обновлений…";
        var result = await DataService.Instance.UpdateAsync();
        switch (result)
        {
            case UpdateResult.Updated:
                ApplyFilter();
                StatusLabel.Text = $"Данные обновлены до патча {DataService.Instance.Data.Patch}";
                break;
            case UpdateResult.UpToDate:
                StatusLabel.Text = "Установлены актуальные данные";
                break;
            default:
                StatusLabel.Text = "Нет соединения. Используются сохранённые данные";
                break;
        }

        // Поиск портретов героев; список обновляется, если найдены новые картинки
        if (await DataService.Instance.ResolveImagesAsync())
            ApplyFilter();
    }

    void OnSearchChanged(object? sender, TextChangedEventArgs e) => ApplyFilter();

    async void OnRefreshing(object? sender, EventArgs e)
    {
        await CheckUpdatesAsync();
        Refresh.IsRefreshing = false;
    }

    async void OnHeroTapped(object? sender, TappedEventArgs e)
    {
        if (sender is BindableObject view && view.BindingContext is Hero hero)
            await Navigation.PushAsync(new HeroPage(hero));
    }
}
```

---

## БЛОК 7

**Имя файла:**
```
HeroPage.xaml
```
**Содержимое:**
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MlbbHelper.HeroPage"
             NavigationPage.HasNavigationBar="False">

    <Grid>
        <Grid.Background>
            <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                <GradientStop Color="#050B1F" Offset="0" />
                <GradientStop Color="#0B1F4D" Offset="0.55" />
                <GradientStop Color="#061233" Offset="1" />
            </LinearGradientBrush>
        </Grid.Background>

        <Ellipse WidthRequest="340" HeightRequest="340" InputTransparent="True"
                 HorizontalOptions="Center" VerticalOptions="Start" TranslationY="-60">
            <Ellipse.Fill>
                <RadialGradientBrush Center="0.5,0.5" Radius="0.5">
                    <GradientStop Color="#552E8BFF" Offset="0" />
                    <GradientStop Color="#002E8BFF" Offset="1" />
                </RadialGradientBrush>
            </Ellipse.Fill>
        </Ellipse>

        <ScrollView>
            <VerticalStackLayout Padding="16,44,16,32" Spacing="18">

                <Border Style="{StaticResource Glass}" HorizontalOptions="Start" Padding="16,8">
                    <Border.GestureRecognizers>
                        <TapGestureRecognizer Tapped="OnBackTapped" />
                    </Border.GestureRecognizers>
                    <Label Text="‹  Назад" FontSize="15" TextColor="#EAF6FF" />
                </Border>

                <VerticalStackLayout Spacing="8" Padding="0,12">
                    <Border WidthRequest="140" HeightRequest="140" HorizontalOptions="Center"
                            StrokeShape="RoundRectangle 36"
                            Stroke="#9BE3FF" StrokeThickness="2.5"
                            BackgroundColor="#15305F">
                        <Border.Shadow>
                            <Shadow Brush="#35C6FF" Radius="30" Opacity="0.95" Offset="0,0" />
                        </Border.Shadow>
                        <Grid>
                            <Label Text="{Binding Initial}" FontSize="64" FontAttributes="Bold"
                                   TextColor="#BFE8FF"
                                   HorizontalOptions="Center" VerticalOptions="Center" />
                            <Image Source="{Binding ImageUrl}" Aspect="AspectFill" />
                        </Grid>
                    </Border>

                    <Label Text="{Binding Name}" FontSize="30" FontAttributes="Bold"
                           TextColor="#EAF6FF" HorizontalTextAlignment="Center" Margin="0,14,0,0" />
                    <Label Text="{Binding NameEn}" FontSize="14" TextColor="#8FC8FF"
                           HorizontalTextAlignment="Center" />
                    <Label Text="{Binding Role}" FontSize="15" TextColor="#4FC3FF"
                           HorizontalTextAlignment="Center" />
                    <Label x:Name="PatchLabel" FontSize="12" TextColor="#7FB4EE"
                           HorizontalTextAlignment="Center" />
                </VerticalStackLayout>

                <Label Text="Контрпики" FontSize="22" FontAttributes="Bold" TextColor="#EAF6FF" />

                <Label x:Name="WeaknessLabel" FontSize="14" TextColor="#A9D3FF" IsVisible="False" />

                <Label x:Name="NoCountersLabel" Text="Для этого героя контрпики пока не добавлены."
                       TextColor="#8FC8FF" IsVisible="False" />

                <VerticalStackLayout x:Name="CounterBox" Spacing="12">
                    <BindableLayout.ItemTemplate>
                        <DataTemplate>
                            <Border Style="{StaticResource Glass}" Padding="12">
                                <Border.GestureRecognizers>
                                    <TapGestureRecognizer Tapped="OnCounterTapped" />
                                </Border.GestureRecognizers>
                                <Grid ColumnDefinitions="Auto,*" ColumnSpacing="16">
                                    <Border WidthRequest="64" HeightRequest="64"
                                            Margin="4"
                                            StrokeShape="RoundRectangle 18"
                                            Stroke="#7FD6FF" StrokeThickness="2"
                                            BackgroundColor="#15305F">
                                        <Border.Shadow>
                                            <Shadow Brush="#35C6FF" Radius="12" Opacity="0.9" Offset="0,0" />
                                        </Border.Shadow>
                                        <Grid>
                                            <Label Text="{Binding Hero.Initial}" FontSize="26" FontAttributes="Bold"
                                                   TextColor="#BFE8FF"
                                                   HorizontalOptions="Center" VerticalOptions="Center" />
                                            <Image Source="{Binding Hero.ImageUrl}" Aspect="AspectFill" />
                                        </Grid>
                                    </Border>
                                    <VerticalStackLayout Grid.Column="1" VerticalOptions="Center" Spacing="4">
                                        <Label Text="{Binding Hero.Name}" FontSize="17" FontAttributes="Bold"
                                               TextColor="#EAF6FF" />
                                        <Label Text="{Binding Reason}" FontSize="13" TextColor="#A9D3FF" />
                                    </VerticalStackLayout>
                                </Grid>
                            </Border>
                        </DataTemplate>
                    </BindableLayout.ItemTemplate>
                </VerticalStackLayout>

                <Label Text="Актуальные сборки" FontSize="22" FontAttributes="Bold"
                       TextColor="#EAF6FF" Margin="0,10,0,0" />

                <Label x:Name="BuildNoteLabel" FontSize="12" TextColor="#8FC8FF" IsVisible="False" />

                <Label x:Name="NoBuildsLabel" Text="Для этого героя сборки пока не добавлены."
                       TextColor="#8FC8FF" IsVisible="False" />

                <VerticalStackLayout x:Name="BuildBox" Spacing="12">
                    <BindableLayout.ItemTemplate>
                        <DataTemplate>
                            <Border Style="{StaticResource Glass}" Padding="16">
                                <VerticalStackLayout Spacing="10">
                                    <Label Text="{Binding Title}" FontSize="17" FontAttributes="Bold"
                                           TextColor="#EAF6FF" />
                                    <FlexLayout Wrap="Wrap" BindableLayout.ItemsSource="{Binding Items}">
                                        <BindableLayout.ItemTemplate>
                                            <DataTemplate>
                                                <Border StrokeShape="RoundRectangle 12" Stroke="#55A8E8FF"
                                                        StrokeThickness="1" Padding="10,6"
                                                        Margin="0,0,8,8" BackgroundColor="#22478CFF">
                                                    <Label Text="{Binding .}" FontSize="13" TextColor="#EAF6FF" />
                                                </Border>
                                            </DataTemplate>
                                        </BindableLayout.ItemTemplate>
                                    </FlexLayout>
                                    <Label Text="{Binding Extra}" FontSize="12" TextColor="#8FC8FF" />
                                </VerticalStackLayout>
                            </Border>
                        </DataTemplate>
                    </BindableLayout.ItemTemplate>
                </VerticalStackLayout>

            </VerticalStackLayout>
        </ScrollView>
    </Grid>
</ContentPage>
```

---

## БЛОК 8

**Имя файла:**
```
HeroPage.xaml.cs
```
**Содержимое:**
```csharp
using MlbbHelper.Models;
using MlbbHelper.Services;

namespace MlbbHelper;

public partial class HeroPage : ContentPage
{
    public HeroPage(Hero hero)
    {
        InitializeComponent();
        BindingContext = hero;

        var counters = DataService.Instance.GetCounters(hero);
        BindableLayout.SetItemsSource(CounterBox, counters);
        NoCountersLabel.IsVisible = counters.Count == 0;

        if (!string.IsNullOrWhiteSpace(hero.Weakness))
        {
            WeaknessLabel.Text = $"Слабые стороны героя: {hero.Weakness}.";
            WeaknessLabel.IsVisible = true;
        }

        var (builds, isGeneric) = DataService.Instance.GetBuilds(hero);
        BindableLayout.SetItemsSource(BuildBox, builds);
        NoBuildsLabel.IsVisible = builds.Count == 0;

        if (isGeneric && builds.Count > 0)
        {
            BuildNoteLabel.Text = $"Это типовая сборка для роли «{hero.Role}», а не личная сборка героя.";
            BuildNoteLabel.IsVisible = true;
        }

        PatchLabel.Text = $"Актуально для патча {DataService.Instance.Data.Patch}";
    }

    async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    async void OnCounterTapped(object? sender, TappedEventArgs e)
    {
        if (sender is BindableObject view && view.BindingContext is CounterView counter)
            await Navigation.PushAsync(new HeroPage(counter.Hero));
    }
}
```

---

## БЛОК 9

**Имя файла** (папка `Models` создастся сама):
```
Models/Hero.cs
```
**Содержимое:**
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

---

## БЛОК 10 ⚠️ здесь нужна одна правка

**Имя файла** (папка `Services` создастся сама):
```
Services/DataService.cs
```
**Что исправить после вставки:** в четвёртой строке кода найдите `ВАШ_ЛОГИН` и замените эти слова на **ваше имя пользователя GitHub**. Например, если вы `ivan2025`, получится `"ivan2025/mlbb-helper"`. Остальное не трогайте.

**Содержимое:**
```csharp
using System.Text.Json;
using MlbbHelper.Models;

namespace MlbbHelper.Services;

public static class Constants
{
    // ЗАМЕНИТЕ слова ВАШ_ЛОГИН на ваше имя пользователя GitHub
    public const string Repo = "ВАШ_ЛОГИН/mlbb-helper";
    public const string RawBase = "https://raw.githubusercontent.com/" + Repo + "/main/";
    public const string DataUrl = RawBase + "Resources/Raw/heroes.json";

    // Вики, откуда автоматически берутся портреты героев
    public const string WikiApi = "https://mobile-legends.fandom.com/api.php";
}

public enum UpdateResult { Updated, UpToDate, Offline }

public class DataService
{
    public static DataService Instance { get; } = new();

    static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };
    static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    static DataService()
    {
        Http.DefaultRequestHeaders.UserAgent.ParseAdd("MlbbHelper/1.0 (Android)");
    }

    string CachePath => Path.Combine(FileSystem.AppDataDirectory, "heroes_cache.json");
    string ImageCachePath => Path.Combine(FileSystem.AppDataDirectory, "image_cache.json");

    string _rawJson = "";
    Dictionary<string, string> _images = new();

    public HeroData Data { get; private set; } = new();

    public async Task LoadAsync()
    {
        LoadImageCache();

        HeroData? parsed = null;
        var raw = "";

        try
        {
            if (File.Exists(CachePath))
            {
                var cached = File.ReadAllText(CachePath);
                var p = Parse(cached);
                if (p != null && p.Heroes.Count > 0)
                {
                    parsed = p;
                    raw = cached.Trim();
                }
            }
        }
        catch { /* используем встроенные данные */ }

        if (parsed == null)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("heroes.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            parsed = Parse(json) ?? new HeroData();
            raw = json.Trim();
        }

        Data = parsed;
        _rawJson = raw;
        ApplyImages();
    }

    public async Task<UpdateResult> UpdateAsync()
    {
        try
        {
            var url = Constants.DataUrl + "?t=" + DateTime.UtcNow.Ticks;
            var json = (await Http.GetStringAsync(url)).Trim();
            var parsed = Parse(json);
            if (parsed == null || parsed.Heroes.Count == 0)
                return UpdateResult.Offline;

            if (json == _rawJson)
                return UpdateResult.UpToDate;

            File.WriteAllText(CachePath, json);
            Data = parsed;
            _rawJson = json;
            ApplyImages();
            return UpdateResult.Updated;
        }
        catch
        {
            return UpdateResult.Offline;
        }
    }

    static HeroData? Parse(string json)
    {
        try
        {
            var data = JsonSerializer.Deserialize<HeroData>(json, Options);
            if (data == null) return null;
            foreach (var hero in data.Heroes)
            {
                if (string.IsNullOrWhiteSpace(hero.Image)) continue;
                hero.ImageUrl = hero.Image.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                    ? hero.Image
                    : Constants.RawBase + hero.Image;
            }
            return data;
        }
        catch
        {
            return null;
        }
    }

    // ---------- Картинки ----------

    void LoadImageCache()
    {
        try
        {
            if (File.Exists(ImageCachePath))
                _images = JsonSerializer.Deserialize<Dictionary<string, string>>(
                              File.ReadAllText(ImageCachePath)) ?? new();
        }
        catch
        {
            _images = new();
        }
    }

    void ApplyImages()
    {
        foreach (var hero in Data.Heroes)
        {
            if (string.IsNullOrEmpty(hero.ImageUrl) && _images.TryGetValue(hero.Id, out var url))
                hero.ImageUrl = url;
        }
    }

    // Ищет портреты героев на вики. Возвращает true, если найдены новые картинки.
    public async Task<bool> ResolveImagesAsync()
    {
        var missing = Data.Heroes.Where(h => string.IsNullOrEmpty(h.ImageUrl)).ToList();
        if (missing.Count == 0) return false;

        var changed = false;

        foreach (var batch in missing.Chunk(40))
        {
            try
            {
                var titles = string.Join("|", batch.Select(h => h.NameEn));
                var url = Constants.WikiApi +
                          "?action=query&format=json&prop=pageimages&piprop=thumbnail" +
                          "&pithumbsize=300&redirects=1&titles=" + Uri.EscapeDataString(titles);

                var json = await Http.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("query", out var query)) continue;

                var normalized = ReadAliases(query, "normalized");
                var redirects = ReadAliases(query, "redirects");

                var thumbs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (query.TryGetProperty("pages", out var pages))
                {
                    foreach (var page in pages.EnumerateObject())
                    {
                        if (page.Value.TryGetProperty("title", out var title) &&
                            page.Value.TryGetProperty("thumbnail", out var thumb) &&
                            thumb.TryGetProperty("source", out var source))
                        {
                            var t = title.GetString();
                            var s = source.GetString();
                            if (!string.IsNullOrEmpty(t) && !string.IsNullOrEmpty(s))
                                thumbs[t] = s;
                        }
                    }
                }

                foreach (var hero in batch)
                {
                    var name = hero.NameEn;
                    if (normalized.TryGetValue(name, out var n)) name = n;
                    if (redirects.TryGetValue(name, out var r)) name = r;

                    if (thumbs.TryGetValue(name, out var imageUrl))
                    {
                        hero.ImageUrl = imageUrl;
                        _images[hero.Id] = imageUrl;
                        changed = true;
                    }
                }
            }
            catch
            {
                // нет сети или вики не ответила: остаются буквы вместо картинок
            }
        }

        if (changed)
        {
            try { File.WriteAllText(ImageCachePath, JsonSerializer.Serialize(_images)); }
            catch { }
        }

        return changed;
    }

    static Dictionary<string, string> ReadAliases(JsonElement query, string key)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (query.TryGetProperty(key, out var array) && array.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in array.EnumerateArray())
            {
                var from = item.GetProperty("from").GetString();
                var to = item.GetProperty("to").GetString();
                if (from != null && to != null) result[from] = to;
            }
        }
        return result;
    }

    // ---------- Поиск и данные ----------

    static string Normalize(string s) => s.Trim().ToLowerInvariant().Replace('ё', 'е');

    public List<Hero> Search(string query)
    {
        var q = Normalize(query);
        var all = Data.Heroes.OrderBy(h => h.Name).ToList();
        if (q.Length == 0) return all;

        return all
            .Where(h => Normalize(h.Name).Contains(q)
                     || Normalize(h.NameEn).Contains(q)
                     || Normalize(h.Role).Contains(q))
            .OrderBy(h => Normalize(h.Name).StartsWith(q) || Normalize(h.NameEn).StartsWith(q) ? 0 : 1)
            .ThenBy(h => h.Name)
            .ToList();
    }

    public Hero? Find(string id) =>
        Data.Heroes.FirstOrDefault(h => string.Equals(h.Id, id, StringComparison.OrdinalIgnoreCase));

    public List<CounterView> GetCounters(Hero hero)
    {
        var result = new List<CounterView>();
        foreach (var id in hero.Counters)
        {
            var target = Find(id);
            if (target != null)
                result.Add(new CounterView { Hero = target, Reason = $"Роль: {target.Role}" });
        }
        return result;
    }

    // Возвращает личные сборки героя, а если их нет, то типовые сборки для его роли
    public (List<Build> Builds, bool IsGeneric) GetBuilds(Hero hero)
    {
        if (hero.Builds.Count > 0)
            return (hero.Builds, false);

        if (Data.RoleBuilds.TryGetValue(hero.Role, out var roleBuilds))
            return (roleBuilds, true);

        return (new List<Build>(), false);
    }
}
```
