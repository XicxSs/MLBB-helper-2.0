using Microsoft.Maui.Controls;
using MlbbHelper.Models;
using MlbbHelper.Services;

namespace MlbbHelper;

public partial class HeroPage : ContentPage
{
    readonly Hero _hero;
    bool _checkedOnline;

    public HeroPage(Hero hero)
    {
        InitializeComponent();
        _hero = hero;
        BindingContext = hero;
        ShowDetails();
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
        BindableLayout.SetItemsSource(BuildBox, builds);
        NoCountersLabel.IsVisible = counters.Count == 0;
        NoBuildsLabel.IsVisible = builds.Count == 0;

        if (builds.Count > 0)
        {
            var rank = builds[0].Rank.Equals("all", StringComparison.OrdinalIgnoreCase)
                ? "всех рангов"
                : $"ранга «{GameLocalization.RankName(builds[0].Rank)}»";
            BuildNoteLabel.Text = $"Популярные варианты сборок для {rank}. Названия предметов приведены на английском языке.";
        }

        PatchLabel.Text = string.IsNullOrWhiteSpace(service.Data.Patch)
            ? "Данные сообщества MLBB"
            : $"Данные сообщества MLBB · патч {service.Data.Patch}";
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
