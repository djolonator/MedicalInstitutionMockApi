using MongoDB.Driver;
using NitesInstitution.Models;

namespace NitesInstitution.Data
{
    public class InstitutionRepository : IInstitutionRepository
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly IMongoCollection<StoredInstitutionCaseDataModel> _collectionPatientCasesV3;

        public InstitutionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value!;
            _databaseName = configuration.GetSection("DatabaseSettings:DatabaseName").Value!;
            _collectionName = configuration.GetSection("DatabaseSettings:CollectionName").Value!;
            _collectionPatientCasesV3 = GetCollection();
        }

        private IMongoCollection<StoredInstitutionCaseDataModel> GetCollection()
        {
            var mongoClient = new MongoClient(_connectionString);
            var database = mongoClient.GetDatabase(_databaseName);
            var collection = database.GetCollection<StoredInstitutionCaseDataModel>(_collectionName);
            return collection;
        }

        public async Task<List<StoredInstitutionCaseDataModel>> GetCasesForPatient(PatientIdentificator patientIdentificator)
        {
            var cases = new List<StoredInstitutionCaseDataModel>();
            var caseLimit = 10;
            var builder = Builders<StoredInstitutionCaseDataModel>.Filter;
            var filter = builder.Empty;

            if (patientIdentificator.PassedIdentificatorType.Equals("LBO"))
            {
                var patientIdentificatorSsn = builder.Eq(x => x.PatientSsn, patientIdentificator.Ssn);
                filter &= patientIdentificatorSsn;
            }
            else
            {
                var patientIdentificatorNpi = builder.Eq(x => x.PatientNpi, patientIdentificator.Npi);
                filter &= patientIdentificatorNpi;
            }

            if (!string.IsNullOrEmpty(patientIdentificator.InstitutionCode))
            {
                var istitutionCodeFilter = builder.Eq(x => x.InstitutionCode, patientIdentificator.InstitutionCode);
                filter &= istitutionCodeFilter;
            }

            if (!string.IsNullOrEmpty(patientIdentificator.RequestingInstitution))
            {
                var requestingInstitutionFilter = builder.Ne(x => x.InstitutionCode, patientIdentificator.RequestingInstitution);
                filter &= requestingInstitutionFilter;
            }


            if (patientIdentificator.dateFrom.HasValue)
            {
                var caseDateFilter = builder.Gt(x => x.CaseDate, patientIdentificator.dateFrom);
                filter &= caseDateFilter;
            }

            if (patientIdentificator.dateTo.HasValue)
            {
                var caseDateFilter = builder.Lt(x => x.CaseDate, patientIdentificator.dateTo);
                filter &= caseDateFilter;
            }

            cases = await _collectionPatientCasesV3.Find(filter)
                        .Skip((patientIdentificator.PageNumber - 1) * caseLimit)
                        .Limit(caseLimit)
                        .SortByDescending(x => x.CaseDate)
                        .ToListAsync();

            return cases;
        }
    }
}
