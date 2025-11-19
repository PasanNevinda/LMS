using Microsoft.AspNetCore.Mvc;

namespace LMS.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderViewAsync(ControllerContext context, string viewName, object model, bool partial = false);
    }

}
