namespace HR.BrightspaceConnector.Features.Common
{
    public static class RichTextExtensions
    {
        /// <summary>
        /// Converts the <see cref="RichText"/> object to the equivalent <see cref="RichTextInput"/> object.
        /// </summary>
        /// <param name="richText"></param>
        /// <returns></returns>
        public static RichTextInput ToRichTextInput(this RichText richText)
        {
            if (!string.IsNullOrEmpty(richText.Html))
            {
                return new RichTextInput
                {
                    Content = richText.Html,
                    Type = TextContentType.Html
                };
            }

            if (!string.IsNullOrEmpty(richText.Text))
            {
                return new RichTextInput
                {
                    Content = richText.Text,
                    Type = TextContentType.Text
                };
            }

            return new();
        }

        /// <summary>
        /// Converts the <see cref="RichTextInput"/> object to the equivalent <see cref="RichText"/> object.
        /// </summary>
        /// <param name="richTextInput"></param>
        /// <returns></returns>
        public static RichText ToRichText(this RichTextInput richTextInput)
        {
            return richTextInput.Type switch
            {
                TextContentType.Html => new RichText { Html = richTextInput.Content },
                TextContentType.Text => new RichText { Text = richTextInput.Content },
                _ => new()
            };
        }
    }
}
