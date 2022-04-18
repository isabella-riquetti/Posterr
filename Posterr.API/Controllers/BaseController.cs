using Microsoft.AspNetCore.Mvc;

namespace Posterr.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        // "Do not build authentication"
        public const int AuthenticatedUserId = 1;
    }
}
