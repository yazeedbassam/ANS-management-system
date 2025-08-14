using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class CertificateIndexViewModel
    {
        public List<CertificateViewModel> ControllerCertificates { get; set; }
        public List<CertificateViewModel> EmployeeCertificates { get; set; }

        public CertificateIndexViewModel()
        {
            ControllerCertificates = new List<CertificateViewModel>();
            EmployeeCertificates = new List<CertificateViewModel>();
        }
    }
}
