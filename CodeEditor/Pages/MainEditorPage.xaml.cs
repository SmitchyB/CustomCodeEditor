using CodeEditor.Components;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CodeEditor.Pages
{
    /// <summary>
    /// This is the main editor page for the code editor application.
    /// </summary>
    public partial class MainEditorPage : Page
    {
        private readonly DispatcherTimer _highlightTimer; // Timer to debounce syntax highlighting

        // Constructor for MainEditorPage
        public MainEditorPage()
        {
            InitializeComponent(); // Initialize the main editor page components

            _highlightTimer = new DispatcherTimer // Create a timer to handle syntax highlighting
            {
                Interval = TimeSpan.FromMilliseconds(200) // Set the timer interval to 200 milliseconds
            };
            _highlightTimer.Tick += HighlightTimer_Tick; // Attach the tick event handler for the timer

            EditorTextBox.TextChanged += EditorTextBox_TextChanged; // Attach the text changed event handler to the editor text box
        }
        // Event handler for text changes in the editor text box
        private void EditorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _highlightTimer.Stop(); // Stop the timer to prevent immediate highlighting
            _highlightTimer.Start(); // Restart the timer to debounce the highlighting
        }
        // Event handler for the timer tick to perform syntax highlighting
        private void HighlightTimer_Tick(object? sender, EventArgs e)
        {
            _highlightTimer.Stop(); // Stop the timer to prevent multiple highlights
            SyntaxHighlighter.Highlight(EditorTextBox); // Call the syntax highlighter to apply highlighting to the editor text box
        }

        // Word wrap setting method
        public void SetWordWrap(bool enabled)
        {
            EditorTextBox.Document.PageWidth = enabled ? 10000 : double.NaN; // Set the page width for word wrap; if enabled, set a large width, otherwise set it to NaN
            EditorTextBox.HorizontalScrollBarVisibility = enabled ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled; // Set the horizontal scrollbar visibility based on the word wrap setting
        }
    }
}

