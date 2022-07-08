using HR.BrightspaceConnector.Features.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class RichTextExtensionsTests
    {
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ToRichTextInput_CalledOnRichTextWithMissingProperties_ReturnsRichTextInputWithNullProperties(string content)
        {
            // Arrange
            var richText = new RichText
            {
                Html = content,
                Text = content
            };

            // Act
            var richTextInput = richText.ToRichTextInput();

            // Assert
            Assert.IsNotNull(richTextInput);
            Assert.IsNull(richTextInput.Type);
            Assert.IsNull(richTextInput.Content);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ToRichText_CalledOnRichTextInputWithMissingProperties_ReturnsRichTextWithNullProperties(string content)
        {
            // Arrange
            var richTextInput = new RichTextInput
            {
                Content = content,
                Type = null
            };

            // Act
            var richText = richTextInput.ToRichText();

            // Assert
            Assert.IsNotNull(richText);
            Assert.IsNull(richText.Html);
            Assert.IsNull(richText.Text);
        }

        [TestMethod]
        public void ToRichTextInput_CalledOnRichTextWithHtmlPropertySet_ReturnsRichTextWithHtmlContent()
        {
            // Arrange
            var richText = new RichText
            {
                Html = "<p>Description</p>",
                Text = null
            };

            // Act
            var richTextInput = richText.ToRichTextInput();

            // Assert
            Assert.AreEqual(TextContentType.Html, richTextInput.Type);
            Assert.AreEqual(richText.Html, richTextInput.Content);
        }

        [TestMethod]
        public void ToRichTextInput_CalledOnRichTextWithTextPropertySet_ReturnsRichTextWithTextContent()
        {
            // Arrange
            var richText = new RichText
            {
                Html = null,
                Text = "Description"
            };

            // Act
            var richTextInput = richText.ToRichTextInput();

            // Assert
            Assert.AreEqual(TextContentType.Text, richTextInput.Type);
            Assert.AreEqual(richText.Text, richTextInput.Content);
        }

        [TestMethod]
        public void ToRichTextInput_CalledOnRichTextWithBothPropertiesSet_ReturnsRichTextWithHtmlContent()
        {
            // Arrange
            var richText = new RichText
            {
                Html = "<p>Description</p>",
                Text = "Description"
            };

            // Act
            var richTextInput = richText.ToRichTextInput();

            // Assert
            Assert.AreEqual(TextContentType.Html, richTextInput.Type);
            Assert.AreEqual(richText.Html, richTextInput.Content);
        }

        [TestMethod]
        public void ToRichText_CalledOnRichTextInputWithHtmlContent_ReturnsRichTextWithHtmlPropertySet()
        {
            // Arrange
            var richTextInput = new RichTextInput
            {
                Content = "<p>Description</p>",
                Type = TextContentType.Html
            };

            // Act
            var richText = richTextInput.ToRichText();

            // Assert
            Assert.AreEqual(richTextInput.Content, richText.Html);
            Assert.IsNull(richText.Text);
        }

        [TestMethod]
        public void ToRichText_CalledOnRichTextInputWithTextContent_ReturnsRichTextWithTextPropertySet()
        {
            // Arrange
            var richTextInput = new RichTextInput
            {
                Content = "Description",
                Type = TextContentType.Text
            };

            // Act
            var richText = richTextInput.ToRichText();

            // Assert
            Assert.AreEqual(richTextInput.Content, richText.Text);
            Assert.IsNull(richText.Html);
        }
    }
}
