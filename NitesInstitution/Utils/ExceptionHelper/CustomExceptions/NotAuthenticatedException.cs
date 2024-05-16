namespace NitesInstitution.Utils.ExceptionHelper.CustomExceptions
{
    public class NotAuthenticatedException : Exception
    {
        public NotAuthenticatedException() : base() { }
        public NotAuthenticatedException(string msg) : base(msg) { }
    }
}
