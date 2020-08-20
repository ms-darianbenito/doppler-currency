using System.Diagnostics.CodeAnalysis;

namespace CrossCutting
{
    [ExcludeFromCodeCoverage]
    public class SlackHookSettings
    {
        public string Url { get; set; }
        public string Text { get; set; }
    }
}
