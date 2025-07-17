using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CodeEditor.Components
{
    /// <summary>
    /// This class provides syntax highlighting for C# code in a RichTextBox.
    /// </summary>
    public static partial class SyntaxHighlighter
    {
        private static readonly Brush KeywordBrush = Brushes.MediumPurple; // Brush for highlighting keywords
        private static readonly Brush StringBrush = Brushes.Blue; // Brush for highlighting string literals
        private static readonly Brush CommentBrush = Brushes.Green; // Brush for highlighting comments

        //Set of C# keywords to be highlighted
        private static readonly HashSet<string> Keywords = new() 
        {
            "public","private","protected","internal","static","class","interface",
            "enum","void","int","string","bool","return","if","else","switch",
            "case","using","namespace","new","null","this","var"
        };

        public static void Highlight(RichTextBox editor)
        {
            var document = editor.Document; // Get the document from the RichTextBox
            var textRange = new TextRange(document.ContentStart, document.ContentEnd); // Create a TextRange covering the entire document
            textRange.ClearAllProperties(); // Clear all formatting properties in the TextRange
            string text = textRange.Text; // Get the text content from the TextRange
            var commentRegex = new Regex(@"//.*?$", RegexOptions.Multiline); // Regex to match single-line comments
            foreach (Match match in commentRegex.Matches(text)) // Find all matches of the comment regex in the text
            {
                ApplyFormat(editor, match.Index, match.Length, CommentBrush); // Apply the comment brush to the matched comment text
            }
            var stringRegex = new Regex("\".*?\""); // Regex to match string literals
            foreach (Match match in stringRegex.Matches(text)) // Find all matches of the string regex in the text
            {
                ApplyFormat(editor, match.Index, match.Length, StringBrush); // Apply the string brush to the matched string literals
            }
            foreach (var keyword in Keywords) // Iterate through the set of keywords to highlight
            {
                var keywordRegex = new Regex($@"\b{Regex.Escape(keyword)}\b"); // Create a regex for the current keyword
                foreach (Match match in keywordRegex.Matches(text)) // Find all matches of the keyword regex in the text
                {
                    ApplyFormat(editor, match.Index, match.Length, KeywordBrush); // Apply the keyword brush to the matched keyword text
                }
            }
        }
        private static void ApplyFormat(RichTextBox editor, int startIndex, int length, Brush brush) // Method to apply formatting to a specific range in the RichTextBox
        {
            TextPointer start = GetTextPositionAtOffset(editor.Document.ContentStart, startIndex); // Get the starting TextPointer at the specified index
            TextPointer end = GetTextPositionAtOffset(start, length); // Get the ending TextPointer at the specified length from the start position
            var range = new TextRange(start, end); // Create a TextRange from the start and end TextPointers
            range.ApplyPropertyValue(TextElement.ForegroundProperty, brush); // Apply the specified brush to the foreground property of the TextRange
        }
        // Method to get a TextPointer at a specific offset from a starting TextPointer
        private static TextPointer GetTextPositionAtOffset(TextPointer start, int offset)
        {
            var navigator = start; // Start navigating from the provided TextPointer
            int remaining = offset; // Initialize the remaining offset to the specified offset
            while (navigator != null) // Continue navigating until there are no more TextPointers
            {
                if (navigator.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text) // Check if the current TextPointer context is text
                {
                    string textRun = navigator.GetTextInRun(LogicalDirection.Forward); // Get the text run at the current TextPointer
                    if (textRun.Length >= remaining) // If the length of the text run is greater than or equal to the remaining offset
                        return navigator.GetPositionAtOffset(remaining); // Return the TextPointer at the specified offset within the text run
                    remaining -= textRun.Length; // Otherwise, subtract the length of the text run from the remaining offset
                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward); // Move to the next TextPointer in the document
            }
            return start; // If no valid position is found, return the original start TextPointer
        }
    }
}
