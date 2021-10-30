namespace IntegreNet.Exceptions
{
    public class TemplateNotFoundException : IntegreException
    {
        public TemplateNotFoundException() : base($"Template not found. Make sure you successfully called {nameof(IntegreSql.InitializeTemplateAsync)}") { }
    }
}