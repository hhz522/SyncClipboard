using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SyncClipboard.Core.ViewModels;
using System;
using Windows.UI.WindowManagement;
using Application = Microsoft.UI.Xaml.Application;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SyncClipboard.WinUI3.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingWindow : Window
    {
        public TrayIcon TrayIcon => _TrayIcon;

        public SettingWindow()
        {
            this.InitializeComponent();
            this.Closed += SettingWindow_Closed;

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(_AppTitleBar.DraggableArea);
            _AppTitleBar.NavigeMenuButtonClicked += () => SplitPane.IsPaneOpen = !SplitPane.IsPaneOpen;

            AppWindow.ResizeClient(new(1200, 700));

            _MenuList.SelectedIndex = 0;
        }

        private void SettingWindow_Closed(object sender, WindowEventArgs args)
        {
            if (App.Current.AppExiting)
            {
                return;
            }
            else
            {
                this.Hide();
                args.Handled = true;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((ListView)sender).SelectedItem;
            //string pageName = "SyncClipboard.WinUI3.Views." + (((ListViewItem)selectedItem).Tag);

            string pageName = "SyncClipboard.WinUI3.Views." + (((SettingItem)selectedItem).Name + "Page");
            Type? pageType = Type.GetType(pageName);
            SettingContentFrame.Navigate(pageType ?? throw new Exception($"Page View not Found: {pageName}"));

            if (SplitPane.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                SplitPane.IsPaneOpen = false;
            }
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            if (args.Size.Width < 800)
            {
                SplitPane.IsPaneOpen = false;
                SplitPane.DisplayMode = SplitViewDisplayMode.Overlay;
                _AppTitleBar.HideNavigationButton();
            }
            else
            {
                SplitPane.DisplayMode = SplitViewDisplayMode.Inline;
                SplitPane.IsPaneOpen = true;
                _AppTitleBar.ShowNavigationButton();
            }
        }

        private void SplitPane_PaneClosed(SplitView sender, object args)
        {
            SplitPane.PaneBackground = (Brush)Application.Current.Resources["LayerOnMicaBaseAltFillColorTransparentBrush"];
        }

        private void SplitPane_PaneOpening(SplitView sender, object args)
        {
            if (SplitPane.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                SplitPane.PaneBackground = (Brush)Application.Current.Resources["AcrylicInAppFillColorDefaultBrush"];
            }
        }

        [RelayCommand]
        private void TrayIcon_DoubleClick()
        {
            this.Show();
        }
    }
}