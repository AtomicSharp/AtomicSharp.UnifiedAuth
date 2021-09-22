using Atomic.UnifiedAuth.Web.Controllers.Consent;

namespace Atomic.UnifiedAuth.Web.Controllers.Device
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}