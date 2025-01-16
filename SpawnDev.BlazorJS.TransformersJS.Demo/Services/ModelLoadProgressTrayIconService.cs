using Microsoft.AspNetCore.Components.Web;
using Radzen;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.TransformersJS.Demo.Layout.AppTray;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Services
{
    public class ModelLoadProgressTrayIconService : IBackgroundService
    {
        BlazorJSRuntime JS;
        AppTrayService AppTrayService;
        DepthEstimationService DepthEstimationService;
        AppTrayIcon TrayIcon;
        public ModelLoadProgressTrayIconService(BlazorJSRuntime js, AppTrayService appTrayService, DepthEstimationService depthEstimationService)
        {
            JS = js;
            AppTrayService = appTrayService;
            DepthEstimationService = depthEstimationService;
            DepthEstimationService.OnStateChange += DepthEstimationService_OnStateChange;
            if (JS.IsWindow)
            {
                using var window = JS.Get<Window>("window");
                // Theme icon
                TrayIcon = new AppTrayIcon
                {
                    ClickCallback = TrayIcon_ClickCallback,
                    ContextCallback = TrayIcon_ContextCallback,
                    Icon = "",
                    Visible = false,
                };
                AppTrayService.Add(TrayIcon);
            }
        }
        private void DepthEstimationService_OnStateChange()
        {
            TrayIcon.Visible = DepthEstimationService.Loading;
            if (DepthEstimationService.Loading)
            {
                TrayIcon.TLText = DepthEstimationService.ModelProgresses.Count().ToString();
                TrayIcon.Title = Math.Round(DepthEstimationService.OverallLoadProgress).ToString() + "%";
                if (DepthEstimationService.OverallLoadProgress < 20)
                {
                    TrayIcon.Icon = "clock_loader_10";
                }
                else if (DepthEstimationService.OverallLoadProgress < 40)
                {
                    TrayIcon.Icon = "clock_loader_20";
                }
                else if (DepthEstimationService.OverallLoadProgress < 60)
                {
                    TrayIcon.Icon = "clock_loader_40";
                }
                else if (DepthEstimationService.OverallLoadProgress < 80)
                {
                    TrayIcon.Icon = "clock_loader_60";
                }
                else if (DepthEstimationService.OverallLoadProgress < 90)
                {
                    TrayIcon.Icon = "clock_loader_80";
                }
                else
                {
                    TrayIcon.Icon = "clock_loader_90";
                }
            }
            AppTrayService.StateHasChanged();
        }
        void TrayIcon_ContextCallback(MouseEventArgs mouseEventArgs)
        {

        }
        void TrayIcon_ClickCallback(MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.ShiftKey)
            {
                //NextTheme(false);
            }
            else if (mouseEventArgs.CtrlKey)
            {
                //NextTheme(true);
            }
            else
            {
                //ToggleDark();
            }
        }
    }
}
