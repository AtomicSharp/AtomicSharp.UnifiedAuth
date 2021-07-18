using AtomicSharp.UnifiedAuth.Web.Controllers.Consent;

namespace AtomicSharp.UnifiedAuth.Web.Controllers.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}