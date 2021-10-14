namespace IntegreNet.Exceptions
{
    public class ServiceUnavailableException : IntegreException
    {
        public ServiceUnavailableException() : base("Service unavailable, make sure IntegreSQL can communicate with PostgreSQL") { }
    }
}