using NitesInstitution.Models;

namespace NitesInstitution.Service
{
    public interface IInstitutionService
    {
        Task<InstitutionCaseDataModel> Execute(HttpRequest request, string patientIdentificatorType, string patientIdentificatorValue,
            DateTime? dateFrom = null, DateTime? dateTo = null);
    }
}
