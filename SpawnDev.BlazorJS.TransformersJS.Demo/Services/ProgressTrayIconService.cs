using SpawnDev.BlazorJS.TransformersJS.Demo.Layout.AppTray;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Services
{
    public class ProgressTrayIconService
    {
        BlazorJSRuntime JS;
        AppTrayService AppTrayService;
        public ProgressTrayIconService(BlazorJSRuntime js, AppTrayService appTrayService)
        {
            JS = js;
            AppTrayService = appTrayService;
        }
    }
}
