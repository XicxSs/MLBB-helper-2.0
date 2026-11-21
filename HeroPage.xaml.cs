using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MlbbHelper.Models;
using MlbbHelper.Services;

namespace MlbbHelper;

public partial class HeroPage : ContentPage
{
    readonly Hero _hero;
    bool _checkedOnline;
    bool _showingBuilds;

    public HeroPage(Hero hero)
    {
        InitializeComponent();
        _hero = hero;
        BindingContext = hero;
        UpdateSectionStyles();
        ShowDetails();
        UpdateStatusLabel.Text = "Показываем сохранённые данные…";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_checkedOnline)
            return;

        _checkedOnline = true;
        UpdateStatusLabel.Text = "Проверяем свежие контрпики и сборки…";

        try
        {
            var updated = await DataService.Instance.RefreshHeroDetailsAsync(_hero);
            ShowDetails();
            UpdateStatusLabel.Text = updated
                ? "Данные профиля обновлены через интернет."
                : "Показаны сохранённые данные. Для проверки обновлений подключитесь к интернету.";
        }
        catch
        {
            UpdateStatusLabel.Text = "Не удалось обновить профиль. Показаны сохранённые данные.";
        }
    }

    void ShowDetails()
    {
        var service = DataService.Instance;
        var counters = service.GetCounters(_hero);
        var builds = service.GetBuilds(_hero);

        BindableLayout.SetItemsSource(CounterBox, counters);
        BindableLayout.SetItemsSource(BuildBox, service.GetBuildCards(_hero));
        NoCountersLabel.IsVisible = counters.Count == 0;
        NoBuildsLabel.IsVisible = builds.Count == 0;

        if (builds.Count > 0)
        {
            var rank = builds[0].Rank.Equals("all", StringComparison.OrdinalIgnoreCase)
                ? "всех рангов"
                : $"ранга «{GameLocalization.RankName(builds[0].Rank)}»";
            BuildNoteLabel.Text = $"Популярные варианты для {rank}. Названия предметов показаны под иконками.";
        }
        else
        {
            BuildNoteLabel.Text = "Популярные варианты по игровой статистике.";
        }

        PatchLabel.Text = string.IsNullOrWhiteSpace(service.Data.Patch)
            ? "Данные сообщества MLBB"
            : $"Данные сообщества MLBB · патч {service.Data.Patch}";
    }

    void UpdateSectionStyles()
    {
        SetTabStyle(CounterTabButton, !_showingBuilds);
        SetTabStyle(BuildTabButton, _showingBuilds);
        CounterSection.IsVisible = !_showingBuilds;
        BuildSection.IsVisible = _showingBuilds;
    }

    static void SetTabStyle(Button button, bool isSelected)
    {
        button.BackgroundColor = Color.FromArgb(isSelected ? "#315A9FD0" : "#8E16253B");
        button.BorderColor = Color.FromArgb(isSelected ? "#93D5FFFF" : "#386998C2");
        button.TextColor = Color.FromArgb(isSelected ? "#F0FAFF" : "#A8C4E0");
    }

    async void OnSectionTapped(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string section })
            return;

        _showingBuilds = section == "builds";
        UpdateSectionStyles();
        await PageScroll.ScrollToAsync(0, 0, true);
    }

    async void OnBackTapped(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    async void OnCounterTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is CounterView counter)
            await Navigation.PushAsync(new HeroPage(counter.Hero));
    }
}
