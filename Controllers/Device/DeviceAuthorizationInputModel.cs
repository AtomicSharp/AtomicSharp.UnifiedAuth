using AtomicSharp.UnifiedAuth.Controllers.Consent;

namespace AtomicSharp.UnifiedAuth.Controllers.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}
