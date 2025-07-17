using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CodeEditor.Components
{
    /// <summary>
    /// Findbar component for searching and highlighting text in a RichTextBox.
    /// </summary>
    public partial class FindBar : UserControl
    {
        //Constructor for FindBar
        public FindBar()
        {
            InitializeComponent(); // Initialize the FindBar component
            this.Visibility = Visibility.Collapsed; // Set the initial visibility to collapsed
            SearchBox.TextChanged += SearchBox_TextChanged; // Attach the TextChanged event handler to the SearchBox

        }
        public RichTextBox? TargetEditor { get; set; } // The RichTextBox where the search will be performed
        private readonly List<TextRange> _matches = new(); // List to store found text ranges
        private int _currentMatchIndex = -1; // Index of the currently selected match
        // Event handler for the Find Next button click
        private void FindNext_Click(object sender, RoutedEventArgs e)
        {
            if (_matches.Count == 0) // If no matches found
            {
                SearchAndHighlight(); // Perform search and highlight
                if (_matches.Count == 0) return; // If still no matches, exit
                _currentMatchIndex = 0; // Set the current match index to the first match
            }
            else // If matches are found
            {
                _currentMatchIndex = (_currentMatchIndex + 1) % _matches.Count; // Move to the next match, wrapping around if necessary
            }

            SelectCurrentMatch(); // Select the current match in the RichTextBox
        }
        // Event handler for the Find Previous button click
        private void FindPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_matches.Count == 0) // If no matches found
            {
                SearchAndHighlight(); // Perform search and highlight
                if (_matches.Count == 0) return; // If still no matches, exit
                _currentMatchIndex = 0; // Set the current match index to the first match
            }
            else // If matches are found
            {
                _currentMatchIndex = (_currentMatchIndex - 1 + _matches.Count) % _matches.Count; // Move to the previous match, wrapping around if necessary
            }

            SelectCurrentMatch(); // Select the current match in the RichTextBox
        }
        // Event handler for the SearchBox text changed event
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchAndHighlight(); // Call the method to search and highlight text in the RichTextBox
        }
        // Method to search for text in the RichTextBox and highlight matches
        private void SearchAndHighlight()
        {
            if (TargetEditor == null) // If the TargetEditor is not set, exit the method
                return; // Ensure that the TargetEditor is not null

            string searchText = SearchBox.Text; // Get the text to search from the SearchBox
            if (string.IsNullOrEmpty(searchText)) // If the search text is empty, clear highlights and exit
                return; // Exit if the search text is empty

            ClearHighlights(); // Clear any existing highlights in the RichTextBox

            _matches.Clear(); // Clear the list of matches
            _currentMatchIndex = -1; // Reset the current match index

            TextPointer position = TargetEditor.Document.ContentStart; // Start searching from the beginning of the document
            StringComparison comparison = CaseSensitiveCheckBox.IsChecked == true // Determine the comparison type based on the CaseSensitiveCheckBox state
                ? StringComparison.CurrentCulture // Use current culture for case-sensitive search
                : StringComparison.CurrentCultureIgnoreCase; // Use current culture ignore case for case-insensitive search

            while (position != null && position.CompareTo(TargetEditor.Document.ContentEnd) < 0) // Loop through the document until the end
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text) // Check if the current position is in a text context
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward); // Get the text in the current run
                    int indexInRun = textRun.IndexOf(searchText, comparison); // Find the index of the search text in the current run
                    if (indexInRun >= 0) // If the search text is found in the current run
                    {
                        TextPointer start = position.GetPositionAtOffset(indexInRun); // Get the start position of the found text
                        TextPointer end = start.GetPositionAtOffset(searchText.Length); // Get the end position of the found text
                        var range = new TextRange(start, end); // Create a TextRange for the found text
                        range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow); // Highlight the found text with a yellow background
                        _matches.Add(range); // Add the found range to the list of matches
                        position = end; // Move the position to the end of the found text
                        continue; // Continue searching from the end of the found text
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Forward); // Move to the next context position in the document
            }

            if (_matches.Count == 0) // If no matches were found
            {
                MessageBox.Show("Text not found.", "Find", MessageBoxButton.OK, MessageBoxImage.Information); // Show a message box indicating that no matches were found
            }
        }
        // Method to select the current match in the RichTextBox
        private void SelectCurrentMatch()
        {
            if (TargetEditor == null) // If the TargetEditor is not set, exit the method
                return; // Ensure that the TargetEditor is not null

            if (_currentMatchIndex >= 0 && _currentMatchIndex < _matches.Count) // Check if the current match index is valid
            {
                var match = _matches[_currentMatchIndex]; // Get the current match from the list of matches
                TargetEditor.Selection.Select(match.Start, match.End); // Select the text range of the current match in the RichTextBox
                TargetEditor.Focus(); // Set focus to the TargetEditor
            }
        }
        // Method to clear all highlights in the RichTextBox
        internal void ClearHighlights()
        {
            if (TargetEditor == null) // If the TargetEditor is not set, exit the method
                return; // Ensure that the TargetEditor is not null

            var textRange = new TextRange(TargetEditor.Document.ContentStart, TargetEditor.Document.ContentEnd); // Create a TextRange covering the entire document
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent); // Clear the background color of the entire document
        }
        // Method to close the FindBar
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed; // Hide the FindBar
            ClearHighlights(); // Clear any highlights in the RichTextBox
        }
    }
}
