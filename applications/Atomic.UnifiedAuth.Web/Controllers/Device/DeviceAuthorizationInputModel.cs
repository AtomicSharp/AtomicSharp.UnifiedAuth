using Atomic.UnifiedAuth.Web.Controllers.Consent;

namespace Atomic.UnifiedAuth.Web.Controllers.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}