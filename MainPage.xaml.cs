using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MlbbHelper.Models;
using MlbbHelper.Services;

namespace MlbbHelper;

public partial class MainPage : ContentPage
{
    static readonly (string Code, string Label)[] LaneFilters =
    {
        ("", "Все линии"),
        ("Exp", "Опыт"),
        ("Jungle", "Лес"),
        ("Mid", "Центр"),
        ("Gold", "Золото"),
        ("Roam", "Роум")
    };

    string _selectedLaneCode = "";
    bool _started;

    public MainPage()
    {
        InitializeComponent();
        CreateLaneFilters();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_started)
            return;

        _started = true;
        StatusLabel.Text = "Загружаем сохранённый справочник…";

        try
        {
            await DataService.Instance.LoadAsync();
            ApplyFilter();
            await CheckForUpdatesAsync();
        }
        catch
        {
            StatusLabel.Text = "Не удалось загрузить справочник. Попробуйте перезапустить приложение.";
        }
    }

    void CreateLaneFilters()
    {
        foreach (var lane in LaneFilters)
        {
            var button = new Button
            {
                Text = lane.Label,
                CommandParameter = lane.Code,
                Padding = new Thickness(13, 7),
                CornerRadius = 18,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                MinimumHeightRequest = 36,
                BorderWidth = 1
            };
            button.Clicked += OnLaneFilterTapped;
            LaneFilterLayout.Children.Add(button);
        }

        UpdateLaneFilterStyles();
    }

    void UpdateLaneFilterStyles()
    {
        foreach (var button in LaneFilterLayout.Children.OfType<Button>())
        {
            var selected = string.Equals(button.CommandParameter as string,
                _selectedLaneCode, StringComparison.OrdinalIgnoreCase);
            button.BackgroundColor = Color.FromArgb(selected ? "#315A9FD0" : "#8E16253B");
            button.BorderColor = Color.FromArgb(selected ? "#93D5FFFF" : "#386998C2");
            button.TextColor = Color.FromArgb(selected ? "#F0FAFF" : "#A8C4E0");
        }
    }

    void ApplyFilter()
    {
        var matches = DataService.Instance.Search(SearchEntry.Text ?? string.Empty, _selectedLaneCode);
        HeroList.ItemsSource = matches;

        var data = DataService.Instance.Data;
        PatchLabel.Text = string.IsNullOrWhiteSpace(data.Patch)
            ? "Справочник Mobile Legends: Bang Bang"
            : $"Актуальный патч · {data.Patch}";
        CountLabel.Text = HeroCount(matches.Count);
    }

    async Task CheckForUpdatesAsync()
    {
        StatusLabel.Text = "Проверяем обновления справочника…";
        var result = await DataService.Instance.UpdateCatalogAsync();
        ApplyFilter();

        StatusLabel.Text = result switch
        {
            UpdateResult.Updated => "Справочник обновлён и сохранён на устройстве.",
            UpdateResult.UpToDate => "Справочник актуален. Он доступен и без сети.",
            _ => "Нет подключения к сети. Используются сохранённые данные."
        };
    }

    void OnSearchChanged(object? sender, TextChangedEventArgs e) => ApplyFilter();

    void OnLaneFilterTapped(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string laneCode })
            return;

        _selectedLaneCode = laneCode;
        UpdateLaneFilterStyles();
        ApplyFilter();
    }

    async void OnRefreshing(object? sender, EventArgs e)
    {
        try
        {
            await CheckForUpdatesAsync();
        }
        finally
        {
            Refresh.IsRefreshing = false;
        }
    }

    async void OnHeroTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is Hero hero)
            await Navigation.PushAsync(new HeroPage(hero));
    }

    async void OnAboutTapped(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AboutPage());
    }

    static string HeroCount(int count)
    {
        var lastTwo = count % 100;
        var last = count % 10;
        var noun = lastTwo is >= 11 and <= 14
            ? "героев"
            : last switch
            {
                1 => "герой",
                2 or 3 or 4 => "героя",
                _ => "героев"
            };
        return $"{count} {noun}";
    }
}
