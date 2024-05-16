using NitesInstitution.Models;

namespace NitesInstitution.Data
{
    public interface IInstitutionRepository
    {
        Task<List<StoredInstitutionCaseDataModel>> GetCasesForPatient(PatientIdentificator patientIdentificator);
    }
}
