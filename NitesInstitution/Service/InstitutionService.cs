using Newtonsoft.Json;
using NitesInstitution.Data;
using NitesInstitution.Models;
using NitesInstitution.Utils.ExceptionHelper.CustomExceptions;

namespace NitesInstitution.Service
{
    public class InstitutionService : IInstitutionService
    {
        private readonly IInstitutionRepository _database;
        private readonly string _authBaseURL;

        public InstitutionService(IInstitutionRepository database, IConfiguration configuration)
        {
            _database = database;
            _authBaseURL = configuration.GetSection("TokenSettings").GetSection("BaseURL").Value!;
        }

        public async Task<InstitutionCaseDataModel> Execute(HttpRequest request, string patientIdentificatorType, string patientIdentificatorValue,
            DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            if (!await ValidateToken(request))
                throw new NotAuthenticatedException();

            var identificator = MapParamsToModel(patientIdentificatorType, patientIdentificatorValue, dateFrom, dateTo);
            var cases = await GetCases(identificator);
            if (cases.Count == 0)
                throw new FileNotFoundException();

            var vendorLikeCases = TransformDataToVendorLike(cases);
            return vendorLikeCases;
        }

        private async Task<List<StoredInstitutionCaseDataModel>> GetCases(PatientIdentificator identificator)
        {
            return await _database.GetCasesForPatient(identificator);
        }

        private PatientIdentificator MapParamsToModel(string patientIdentificatorType, string patientIdentificatorValue,
            DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var identificator = new PatientIdentificator(patientIdentificatorType, patientIdentificatorValue);
            identificator.dateFrom = dateFrom;
            identificator.dateTo = dateTo;
            return identificator;
        }

        private InstitutionCaseDataModel TransformDataToVendorLike(List<StoredInstitutionCaseDataModel> mongoCases)
        {
            var vendorDataLike = new InstitutionCaseDataModel();

            if (mongoCases != null && mongoCases.Count > 0)
            {
                foreach (var storedPatientCase in mongoCases)
                {
                    var patientCase = new PatientCaseRecords
                    {
                        CaseDate = storedPatientCase.CaseDate,
                        CaseID = storedPatientCase.CaseID,
                        CaseInstitution = storedPatientCase.CaseInstitution,
                        CaseNumber = storedPatientCase.CaseNumber,
                        CaseStatus = storedPatientCase.CaseStatus,
                        InstitutionId = storedPatientCase.InstitutionId,
                        InstitutionCode = storedPatientCase.InstitutionCode
                    };

                    var request = new PatientCaseRecordRequest
                    {
                        AdmissionCabinetCode = storedPatientCase.Request.AdmissionCabinetCode,
                        AdmissionCabinetName = storedPatientCase.Request.AdmissionCabinetName,
                        AdmissionDate = storedPatientCase.Request.AdmissionDate,
                        AdmissionDoctor = storedPatientCase.Request.AdmissionDoctor,
                        AdmissionID = storedPatientCase.Request.AdmissionID,
                        AdmissionProceduresSerialized = storedPatientCase.Request.AdmissionProceduresSerialized,
                        ReferralID = storedPatientCase.Request.ReferralID,
                        ReferralImageUrl = storedPatientCase.Request.ReferralImageUrl,
                        ReferralStatus = storedPatientCase.Request.ReferralStatus,
                        ReferralStatusID = storedPatientCase.Request.ReferralStatusID,
                        ReferralStatusLocalID = storedPatientCase.Request.ReferralStatusLocalID,
                        ReferralType = storedPatientCase.Request.ReferralType,
                        ReferralTypeID = storedPatientCase.Request.ReferralTypeID
                    };

                    var response = new PatientCaseRecordResponse
                    {
                        ReferralID = storedPatientCase.Request.Response.ReferralID,
                        ReferralResponseDate = storedPatientCase.Request.Response.ReferralResponseDate,
                        ReferralResponseID = storedPatientCase.Request.Response.ReferralResponseID,
                        ReferralResponseStatusCode = storedPatientCase.Request.Response.ReferralResponseStatusCode,
                        ReferralResponseStatusID = storedPatientCase.Request.Response.ReferralResponseStatusID,
                        ReferralResponseStatusName = storedPatientCase.Request.Response.ReferralResponseStatusName,
                        ReferralResponseTypeName = storedPatientCase.Request.Response.ReferralResponseTypeName,
                        ReferralResponseUrl = storedPatientCase.Request.Response.ReferralResponseUrl
                    };

                    request.Responses.Add(response);
                    patientCase.Requests.Add(request);
                    vendorDataLike.PatientCases.Add(patientCase);
                }
            }
            return vendorDataLike;
        }

        private async Task<bool> ValidateToken(HttpRequest request)
        {
            var isValid = false;

            if (Utils.Utils.ValidateNitesToken(request.Headers["NitesToken"].ToString()))
                isValid = true;
            else
            {
                var token = request.Headers["Authorization"].ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    var trimedToken = token.Substring(7);
                    isValid = await ValidateBearerToken(trimedToken); 
                }
            }
            return isValid; 
        }

        private async Task<bool> ValidateBearerToken(string token)
        {
            var isValid = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_authBaseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                
                var validationResponse = await client.GetAsync("/api/oauth/ValidateToken?authKey=" + token);
                if (validationResponse.IsSuccessStatusCode)
                {
                    var tokenString = await validationResponse.Content.ReadAsStringAsync();
                    var validationModel = JsonConvert.DeserializeObject<TokenValidation>(tokenString);
                    isValid =  validationModel!.Valid;
                }
            }

            return isValid;
        }
    }
}
