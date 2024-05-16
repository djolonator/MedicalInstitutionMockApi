using MongoDB.Bson.Serialization.Attributes;

namespace NitesInstitution.Models
{
    public class InstitutionCaseDataModel
    {
        public List<PatientCaseRecords> PatientCases { get; set; } = new List<PatientCaseRecords>();
        
    }

    public class PatientCaseRecords
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CaseDate { get; set; }
        public string CaseStatus { get; set; }
        public string CaseInstitution { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionCode { get; set; }
        public string CaseNumber { get; set; }
        public string CaseID { get; set; }
        public List<PatientCaseRecordRequest> Requests { get; set; } = new List<PatientCaseRecordRequest>();
    }

    public class PatientCaseRecordRequest
    {

        public string AdmissionCabinetName { get; set; }//
        public string AdmissionCabinetCode { get; set; }
        public DateTime AdmissionDate { get; set; }//
        public string AdmissionDoctor { get; set; }//
        public string ReferralID { get; set; }//
        public string AdmissionID { get; set; }//
        public string ReferralTypeID { get; set; }//
        public string ReferralType { get; set; }//
        public string ReferralStatus { get; set; }//
        public int ReferralStatusID { get; set; }//
        public string ReferralStatusLocalID { get; set; }//
        public string AdmissionProceduresSerialized { get; set; }//
        public string ReferralImageUrl { get; set; }//
        public List<PatientCaseRecordResponse> Responses { get; set; } = new List<PatientCaseRecordResponse>();
    }

    public class PatientCaseRecordResponse
    {
        public string ReferralResponseTypeName { get; set; }//
        public string ReferralResponseStatusCode { get; set; }//
        public string ReferralResponseStatusName { get; set; }//
        public int ReferralResponseStatusID { get; set; }//
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ReferralResponseDate { get; set; }//
        public string ReferralResponseID { get; set; }//
        public string ReferralID { get; set; }//
        public string ReferralResponseUrl { get; set; }//
    }

    public class PatientIdentificator
    {
        public string PassedIdentificatorType { get; set; }
        public string Npi { get; set; }
        public string Ssn { get; set; }
        public int PageNumber { get; set; } = 1;
        public string InstitutionCode { get; } = "66699969";
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public string SortBy { get; set; }
        public string RequestingInstitution { get; set; }
        public int? CaseLimit { get; set; }
        public PatientIdentificator() { }

        public PatientIdentificator(string type, string value)
        {
            if (type.ToUpper() == "LBO" || type.ToUpper() == "SSN")
            {
                Ssn = value;
                PassedIdentificatorType = "LBO";
            }
            else
            {
                Npi = value;
                PassedIdentificatorType = "NPI";
            }
        }

        public string ReturnIdentificatorValue()
        {
            string res = string.Empty;
            if (PassedIdentificatorType.ToUpper() == "LBO" || PassedIdentificatorType.ToUpper() == "SSN")
            {
                res = Ssn;
            }
            else
            {
                res = Npi;
            }
            return res;
        }
    }
}
