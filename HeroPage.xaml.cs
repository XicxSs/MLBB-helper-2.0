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
