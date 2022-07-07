namespace HR.BrightspaceConnector.Features.Common
{
    /// <summary>
    /// Whenever you see a <c>{ &lt;composite:RichText&gt; }</c> field in a JSON example, that stands in for a composite block like this.
    /// </summary>
    /// <remarks>
    /// Note. This structure does not guarantee that you'll always get both versions of a string: with some instances of its use, you might get only a text version, or only an HTML version, or both. 
    /// Accordingly, callers should be prepared to handle results that may not always contain both formats.
    /// </remarks>
    public class RichText
    {
        public string? Text { get; set; }
        public string? Html { get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(Html))
            {
                result += $"{TextContentType.Html}: {Html}";
            }

            if (!string.IsNullOrEmpty(Text))
            {
                if (result != string.Empty)
                {
                    result += Environment.NewLine;
                }
                result += $"{TextContentType.Text}: {Text}";
            }
            return result;
        }
    }
}
