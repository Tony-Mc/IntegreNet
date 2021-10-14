namespace IntegreNet.Exceptions
{
    public class TemplateLockedException : IntegreException
    {
        public TemplateLockedException() : base("Some other process has already recreated a PostgreSQL template database for this hash (or is currently doing it), you can just consider the template ready at this point") { }
    }
}