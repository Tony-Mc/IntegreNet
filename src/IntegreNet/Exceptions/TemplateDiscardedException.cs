namespace IntegreNet.Exceptions
{
    public class TemplateDiscardedException : IntegreException
    {
        public TemplateDiscardedException() : base($"There was an error during test setup with our fixtures, someone called {nameof(IntegreSql.DiscardTemplateAsync)}, thus this template cannot be used") { }
    }
}