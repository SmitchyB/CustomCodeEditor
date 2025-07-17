using CodeEditor.Pages;
using System.Windows;
using System.Windows.Input;

namespace CodeEditor
{
    /// <summary>
    /// This is the mainw for the code editor application.
    /// It initializes the main editor page and handles key events for toggling the find bar and word wrap functionality.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainEditorPage EditorPage { get; private set; } // Property to hold the main editor page instance

        public MainWindow()
        {
            InitializeComponent(); // Initialize the main window components

            EditorPage = new MainEditorPage(); // Create an instance of the main editor page
            MainFrame.Navigate(EditorPage); // Navigate the main frame to the editor page

            this.PreviewKeyDown += MainWindow_PreviewKeyDown; //Subscribe to the PreviewKeyDown event for the main window for the find bar toggle functionality

            MenuBarControl.WordWrapToggled += (enabled) => // Subscribe to the WordWrapToggled event from the MenuBarControl. Handled here to fix an issue where the WordWrap was misaligned with the toggle state
            {
                EditorPage.SetWordWrap(enabled); // Call the SetWordWrap method on the editor page to apply the word wrap state
            };

            MenuBarControl.RaiseInitialWordWrapState(); // Raise the initial word wrap state to apply it when the application starts
        }
        // vent handler for key down events in the main window
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) // Check if Ctrl+F is pressed
            {
                e.Handled = true; // Mark the event as handled to prevent further processing

                if (EditorPage.FindBar != null) // Check if the FindBar is initialized
                {
                    var findBar = EditorPage.FindBar; // Get the FindBar instance from the editor page
                     
                    if (findBar.Visibility == Visibility.Visible) // If the find bar is currently visible
                    {
                        findBar.Visibility = Visibility.Collapsed; // Hide the find bar
                        findBar.ClearHighlights(); // Clear any existing highlights in the editor
                    }
                    else // If the find bar is currently hidden
                    {
                        findBar.TargetEditor = EditorPage.EditorTextBox; // Set the target editor for the find bar to the editor text box
                        findBar.Visibility = Visibility.Visible; // Show the find bar
                        findBar.SearchBox.Focus(); // Set focus to the search box in the find bar
                    }
                }
            }
        }
    }
}
