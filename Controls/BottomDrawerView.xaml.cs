using System.Diagnostics;

namespace Geotracker.Controls;

[ContentProperty(nameof(Content))]
public partial class BottomDrawerView : ContentView
{
    private double _startY;
    public double DrawerTranslationY { get; set; }
    public event EventHandler? ButtonClicked;
    public BottomDrawerView()
    {
        InitializeComponent();
    }

    private void _onButtonClicked(object sender, EventArgs e)
    {
        ButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    void OnDrawerPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        // switch (e.StatusType)
        // {
        //     case GestureStatus.Started:
        //         _startY = DrawerContainer.TranslationY;
        //         DrawerTranslationY = ContentHost.Height +
        //                     ContentHost.Margin.Top +
        //                     ContentHost.Margin.Bottom; ;
        //         Debug.WriteLine($"Set max drawer translation to {DrawerTranslationY}");
        //         break;

        //     case GestureStatus.Running:
        //         var newY = _startY + e.TotalY;
        //         DrawerContainer.TranslationY = Math.Max(0, Math.Min(DrawerTranslationY, newY));
        //         Debug.WriteLine($"Drawer Y: {DrawerContainer.TranslationY} / {DrawerTranslationY}");
        //         break;

        //     case GestureStatus.Completed:
        //         // Animate open/close based on where it stopped
        //         if (DrawerContainer.TranslationY < DrawerTranslationY / 2)
        //         {
        //             OpenDrawer();
        //         }
        //         else
        //         {
        //             CloseDrawer();
        //         }
        //         break;
        // }
    }

    // private async void OpenDrawer()
    // {
    //     await DrawerContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
    // }

    // private async void CloseDrawer()
    // {
    //     await DrawerContainer.TranslateTo(0, DrawerTranslationY, 250, Easing.CubicIn);
    // }
}
