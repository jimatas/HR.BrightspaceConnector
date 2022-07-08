namespace HR.BrightspaceConnector.Features.Common
{
    /// <summary>
    /// Whenever you see a <c>{ &lt;composite:RichTextInput&gt; }</c> field in a JSON example, that stands in for a composite block like this.
    /// </summary>
    public class RichTextInput
    {
        public string? Content { get; set; }

        /// <summary>
        /// For the type field, you must provide either the value "Text" or "Html", depending upon the formatting of the content string you are providing to the back-end service.
        /// </summary>
        public TextContentType? Type { get; set; }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Content)
                ? $"{Type ?? TextContentType.Html}: {Content}"
                : string.Empty;
        }
    }
}
