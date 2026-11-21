using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using MlbbHelper.Models;
using MlbbHelper.Services;

namespace MlbbHelper;

public partial class MainPage : ContentPage
{
    readonly ObservableCollection<Hero> _shownHeroes = new();
    bool _started;

    public MainPage()
    {
        InitializeComponent();
        HeroList.ItemsSource = _shownHeroes;
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

    void ApplyFilter()
    {
        var matches = DataService.Instance.Search(SearchEntry.Text ?? string.Empty);
        _shownHeroes.Clear();
        foreach (var hero in matches)
            _shownHeroes.Add(hero);

        var data = DataService.Instance.Data;
        PatchLabel.Text = string.IsNullOrWhiteSpace(data.Patch)
            ? "Справочник MLBB"
            : $"Актуальные данные · патч {data.Patch}";
        CountLabel.Text = HeroCount(data.Heroes.Count);
    }

    async Task CheckForUpdatesAsync()
    {
        StatusLabel.Text = "Проверяем обновления справочника…";
        var result = await DataService.Instance.UpdateCatalogAsync();
        ApplyFilter();

        StatusLabel.Text = result switch
        {
            UpdateResult.Updated => "Справочник обновлён. Новые данные сохранены на устройстве.",
            UpdateResult.UpToDate => "Справочник проверен. Данные доступны и без сети.",
            _ => "Нет подключения к сети. Используются сохранённые данные."
        };
    }

    void OnSearchChanged(object? sender, TextChangedEventArgs e) => ApplyFilter();

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
