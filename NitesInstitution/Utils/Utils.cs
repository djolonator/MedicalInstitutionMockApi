namespace NitesInstitution.Utils
{
    public static class Utils
    {
        public static bool ValidateNitesToken(string nitesToken)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(nitesToken))
            {
                result = nitesToken == "";
            }
           
            return result;
        }
    }
}
