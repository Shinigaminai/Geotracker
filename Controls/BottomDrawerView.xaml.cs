using System.Diagnostics;

namespace Geotracker.Controls;

[ContentProperty(nameof(Content))]
public partial class BottomDrawerView : ContentView
{
    private Grid _drawerContainer;
    private Button _floatingBtn;
    private View _contentPresenter;

    private double _startY;
    public double DrawerTranslationY { get; set; }
    public event EventHandler? ButtonClicked;
    public BottomDrawerView()
    {
        InitializeComponent();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _drawerContainer = GetTemplateChild("DrawerContainer") as Grid;
        _floatingBtn = GetTemplateChild("FloatingBtn") as Button;
        _contentPresenter = GetTemplateChild("ContentPresenter") as View;

        if (_floatingBtn != null)
        {
            _floatingBtn.Clicked += _onButtonClicked;
        }

        // Optional: Debug to verify it's working
        System.Diagnostics.Debug.WriteLine($"DrawerContainer found: {_drawerContainer != null}");
    }

    private void _onButtonClicked(object sender, EventArgs e)
    {
        ButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    void OnDrawerPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _startY = _drawerContainer.TranslationY;
                DrawerTranslationY = _contentPresenter.Height +
                            _contentPresenter.Margin.Top +
                            _contentPresenter.Margin.Bottom; ;
                Debug.WriteLine($"Set max drawer translation to {DrawerTranslationY}");
                break;

            case GestureStatus.Running:
                var newY = _startY + e.TotalY;
                _drawerContainer.TranslationY = Math.Max(0, Math.Min(DrawerTranslationY, newY));
                Debug.WriteLine($"Drawer Y: {_drawerContainer.TranslationY} / {DrawerTranslationY}");
                break;

            case GestureStatus.Completed:
                // Animate open/close based on where it stopped
                if (_drawerContainer.TranslationY < DrawerTranslationY / 2)
                {
                    OpenDrawer();
                }
                else
                {
                    CloseDrawer();
                }
                break;
        }
    }

    private async void OpenDrawer()
    {
        await _drawerContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
    }

    private async void CloseDrawer()
    {
        await _drawerContainer.TranslateTo(0, DrawerTranslationY, 250, Easing.CubicIn);
    }
}
