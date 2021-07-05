using AtomicSharp.UnifiedAuth.Controllers.Consent;

namespace AtomicSharp.UnifiedAuth.Controllers.Device
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}
