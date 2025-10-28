using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Counter.Models;
using Microsoft.Maui.Controls;

namespace Counter.Controls;

public partial class CounterView : ContentView
{
    public CounterView()
    {
        InitializeComponent();
    }

    private void MinusBtn_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is CounterModel m)
            m.CurValue -= 1;
        SaveCounters();
    }

    private void PlusBtn_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is CounterModel m)
            m.CurValue += 1;
        SaveCounters();
    }

    private void ResetBtn_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is CounterModel m)
            m.CurValue = m.InitValue;
        SaveCounters();

    }
    // When the slider gets slided
    // --copilot
    private void Slider_Slided(object sender, ValueChangedEventArgs e)
    {
        if (BindingContext is CounterModel m)
            m.BackgroundColor = Color.FromHsla(e.NewValue, 0.7, 0.7, 1.0);
    }

    private void SaveCounters()
    {
        if (Application.Current?.MainPage?.BindingContext is MainPageViewModel vm)
            vm.Save();
    }
}