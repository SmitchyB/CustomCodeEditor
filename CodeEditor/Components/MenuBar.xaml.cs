using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CodeEditor.Components
{
    /// <summary>
    /// MenuBar component for the code editor application. Hanldles file operations, themes, word wrap, and productivity features.
    /// </summary>
    public partial class MenuBar : UserControl
    {
        private bool isWordWrapEnabled = true; // Default word wrap state
        public event Action<bool>? WordWrapToggled; // Event to notify when word wrap is toggled
        //Constructor for MenuBar
        public MenuBar()
        {
            InitializeComponent(); // Initialize the MenuBar component
        }
        //Event handler for the New File button click
        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow; // Get the main window instance
            mainWindow?.EditorPage.EditorTextBox.Document.Blocks.Clear(); // Clear the editor text box content
        }
        // Event handler for the Open File button click
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog // Create a new OpenFileDialog instance
            {
                Filter = "Code Files (*.cs;*.xaml;*.xml;*.json;*.html)|*.cs;*.xaml;*.xml;*.json;*.html|All Files (*.*)|*.*" // Set the filter for file types
            };
            if (dlg.ShowDialog() == true) // Show the dialog and check if a file was selected
            {
                string text = File.ReadAllText(dlg.FileName); // Read the content of the selected file
                var mainWindow = Application.Current.MainWindow as MainWindow; // Get the main window instance
                var editor = mainWindow?.EditorPage.EditorTextBox; // Get the editor text box from the main window
                if (editor != null) //Check if the editor is not null
                {
                    editor.Document.Blocks.Clear(); // Clear the existing content in the editor
                    editor.Document.Blocks.Add(new Paragraph(new Run(text))); // Add the new content as a paragraph in the editor
                }
                WordWrapToggled?.Invoke(isWordWrapEnabled); // Invoke the WordWrapToggled event to apply the current word wrap state
            }
        }
        // Event handler for the Save File button click
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog // Create a new SaveFileDialog instance
            {
                Filter = "Code Files (*.cs;*.xaml;*.xml;*.json;*.html)|*.cs;*.xaml;*.xml;*.json;*.html|All Files (*.*)|*.*" // Set the filter for file types
            };
            if (dlg.ShowDialog() == true) // Show the dialog and check if a file was selected
            {
                var mainWindow = Application.Current.MainWindow as MainWindow; // Get the main window instance
                var editor = mainWindow?.EditorPage.EditorTextBox; // Get the editor text box from the main window
                if (editor != null) // Check if the editor is not null
                {
                    string text = new TextRange(editor.Document.ContentStart, editor.Document.ContentEnd).Text; // Get the text content from the editor
                    File.WriteAllText(dlg.FileName, text); // Write the content of the editor to the selected file
                }
            }
        }
        // Event handler for the Exit button click
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Shutdown the application
        }
        // Event handler for the Find button click
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow) // Check if the current main window is of type MainWindow
            {
                if (mainWindow.EditorPage.FindBar != null) //Check if the FindBar is initialized
                {
                    var findBar = mainWindow.EditorPage.FindBar; // Get the FindBar instance from the editor page
                    findBar.TargetEditor = mainWindow.EditorPage.EditorTextBox; // Set the target editor for the find bar to the editor text box
                    findBar.Visibility = Visibility.Visible; // Show the find bar
                    findBar.SearchBox.Focus(); // Set focus to the search box in the find bar
                }
            }
        }
        // Event handler for the Word Wrap toggle button click
        private void WordWrap_Click(object sender, RoutedEventArgs e)
        {
            isWordWrapEnabled = !isWordWrapEnabled; // Toggle the word wrap state
            WordWrapToggled?.Invoke(isWordWrapEnabled); // Invoke the WordWrapToggled event to notify subscribers of the change
        }
        // Event handler for the Light Theme button click
        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("/Themes/LightTheme.xaml"); // Apply the light theme by calling the ApplyTheme method with the path to the light theme resource dictionary
        }
        // Event handler for the Dark Theme button click
        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("/Themes/DarkTheme.xaml"); // Apply the dark theme by calling the ApplyTheme method with the path to the dark theme resource dictionary
        }
        // Event handler for the Productivity button click
        private static void ApplyTheme(string themePath)
        {
            Application.Current.Resources.MergedDictionaries.Clear(); // Clear existing merged dictionaries to apply the new theme
            var dict = new ResourceDictionary // Create a new ResourceDictionary instance
            {
                Source = new Uri(themePath, UriKind.Relative) // Set the source of the resource dictionary to the specified theme path
            };
            Application.Current.Resources.MergedDictionaries.Add(dict); // Add the new resource dictionary to the application's resources
        }
        // Event handler for the Start Pomodoro button click
        private void ProductivityBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow; // Get the main window instance
            var prodTab = mainWindow?.EditorPage.ProductivityTab; // Get the ProductivityTab from the editor page

            if (prodTab != null) // Check if the ProductivityTab is not null
            {
                prodTab.Visibility = prodTab.Visibility == Visibility.Visible // If the ProductivityTab is currently visible
                    ? Visibility.Collapsed // Hide it
                    : Visibility.Visible; // Otherwise, show it
            }
        }
        // Method to raise the initial word wrap state event
        public void RaiseInitialWordWrapState()
        {
            WordWrapToggled?.Invoke(isWordWrapEnabled); // Invoke the WordWrapToggled event to apply the initial word wrap state
        }
    }
}
