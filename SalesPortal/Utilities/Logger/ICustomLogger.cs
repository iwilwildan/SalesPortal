namespace SalesPortal.Utilities.Logger
{
    public interface ICustomLogger
    {
        void LogException(Exception ex, string additionalInfo = "");
    }

}
