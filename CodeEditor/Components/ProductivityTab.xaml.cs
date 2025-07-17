using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CodeEditor.Components
{
    /// <summary>
    /// ProductivityTab component for managing tasks and Pomodoro sprints.
    /// </summary>
    // This class provides stores for the pomodoro sprint info
    public class PomodoroSprint
    {
        public string Title { get; set; } = ""; // Title of the Pomodoro sprint
        public string Description { get; set; } = ""; // Description of the Pomodoro sprint
        public string TimeInfo { get; set; } = ""; // Time information for the Pomodoro sprint, such as start and end times
    }
    // This class represents a task item with properties for text, completion status, and indentation level
    public class TaskItem : INotifyPropertyChanged
    {
        private string _text = "New Task"; // Default text for a new task
        private bool _isCompleted; // Indicates whether the task is completed
        private int _indentLevel; // Indentation level for the task, used for visual hierarchy
        
        // Gets or sets the text of the task
        public string Text
        {
            get => _text; // Returns the current text of the task
            set // Sets the text of the task and raises the PropertyChanged event if it has changed
            {
                if (_text != value) // Check if the new value is different from the current value
                {
                    _text = value; // Update the text of the task
                    OnPropertyChanged(nameof(Text)); // Notify listeners that the Text property has changed
                }
            }
        }
        // Gets or sets whether the task is completed
        public bool IsCompleted
        {
            get => _isCompleted; // Returns the current completion status of the task
            set // Sets the completion status of the task and raises the PropertyChanged event if it has changed
            {
                if (_isCompleted != value) // Check if the new value is different from the current value
                {
                    _isCompleted = value; // Update the completion status of the task
                    OnPropertyChanged(nameof(IsCompleted)); // Notify listeners that the IsCompleted property has changed
                }
            }
        }
        // Gets or sets the indentation level of the task, used for visual hierarchy
        public int IndentLevel
        {
            get => _indentLevel; // Returns the current indentation level of the task
            set // Sets the indentation level of the task and raises the PropertyChanged event if it has changed
            {
                if (_indentLevel != value) // Check if the new value is different from the current value
                {
                    _indentLevel = value; // Update the indentation level of the task
                    OnPropertyChanged(nameof(IndentLevel)); // Notify listeners that the IndentLevel property has changed
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged; // Event to notify listeners when a property has changed
        // This method raises the PropertyChanged event for a specified property name
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Raises the PropertyChanged event for the specified property name
        }
    }
    //Main class for the ProductivityTab component
    public partial class ProductivityTab : UserControl
    {
        private readonly DispatcherTimer _timer; // Timer for managing the Pomodoro countdown
        private TimeSpan _timeRemaining; // Remaining time for the current Pomodoro session
        private bool _isWorkSession = true; // Indicates whether the current session is a work session
        private readonly ObservableCollection<TaskItem> _tasks = new(); // Collection of tasks in the productivity tab
        private readonly ObservableCollection<PomodoroSprint> _sprints = new(); // Collection of Pomodoro sprints
        private int _sprintCount = 0; // Counter for the number of Pomodoro sprints completed
        private DateTime _currentSprintStart; // Start time of the current Pomodoro sprint
        private int _completedWorkSessions = 0; // Counter for the number of completed work sessions
        private bool _isLongBreak = false; // Indicates whether the next break is a long break
        // Constructor for the ProductivityTab component
        public ProductivityTab()
        {
            InitializeComponent(); // Initialize the component and its UI elements
            this.Visibility = Visibility.Collapsed; // Set the initial visibility of the component to collapsed
            TaskList.ItemsSource = _tasks; // Set the data source for the task list to the collection of tasks
            _timer = new DispatcherTimer // Initialize the timer for managing the Pomodoro countdown
            {
                Interval = TimeSpan.FromSeconds(1) // Set the timer interval to 1 second
            };
            _timer.Tick += Timer_Tick; // Subscribe to the Timer_Tick event to handle countdown updates
        }
        // Event handler for the Start Work button click event
        private void StartWorkButton_Click(object sender, RoutedEventArgs e)
        {
            _isWorkSession = true; // Set the current session to a work session
            _timeRemaining = TimeSpan.FromMinutes(25); // Set the remaining time for the work session to 25 minutes
            PomodoroTimerText.Text = _timeRemaining.ToString(@"mm\:ss"); // Update the timer text to show the remaining time
            _timer.Start(); // Start the timer for the Pomodoro countdown
            StartWorkButton.IsEnabled = false; // Disable the Start Work button to prevent multiple clicks
            _currentSprintStart = DateTime.Now; // Record the start time of the current Pomodoro sprint
        }
        // Event handler for the Start Break button click event
        private void StartBreakButton_Click(object sender, RoutedEventArgs e)
        {
            string description = SprintDescriptionBox.Text.Trim(); // Get the description from the input box and trim any whitespace
            if (!string.IsNullOrEmpty(description)) // Check if the description is not empty
            {
                _sprintCount++; // Increment the sprint count for each completed work session
                _sprints.Add(new PomodoroSprint // Add a new Pomodoro sprint to the collection with the current sprint details
                {
                    Title = $"{_sprintCount}{GetOrdinalSuffix(_sprintCount)} Pomodoro Sprint", // Title of the sprint with an ordinal suffix
                    Description = description, // Description of the sprint
                    TimeInfo = $"Started: {_currentSprintStart:t}  Ended: {DateTime.Now:t}" // Time information for the sprint, showing start and end times
                });
                SprintList.ItemsSource = null; // Reset the ItemsSource to refresh the list
                SprintList.ItemsSource = _sprints; // Set the ItemsSource to the updated collection of Pomodoro sprints
            }
            _isWorkSession = false; // Set the current session to a break session
            _isLongBreak = _completedWorkSessions >= 4; // Determine if the next break is a long break based on the number of completed work sessions
            _timeRemaining = _isLongBreak ? TimeSpan.FromMinutes(30) : TimeSpan.FromMinutes(5); // Set the remaining time for the break session, 30 minutes for a long break or 5 minutes for a short break
            PomodoroTimerText.Text = _timeRemaining.ToString(@"mm\:ss"); // Update the timer text to show the remaining time for the break
            _timer.Start(); // Start the timer for the Pomodoro countdown
            DescriptionPanel.Visibility = Visibility.Collapsed; // Hide the description panel after starting the break
            SprintDescriptionBox.Text = ""; // Clear the description input box for the next sprint
        }
        // Event handler for the timer tick event, which updates the countdown and handles session transitions
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_timeRemaining.TotalSeconds > 0) // Check if there is remaining time in the current session
            {
                _timeRemaining = _timeRemaining.Subtract(TimeSpan.FromSeconds(1)); // Decrease the remaining time by 1 second
                PomodoroTimerText.Text = _timeRemaining.ToString(@"mm\:ss"); // Update the timer text to show the new remaining time
            }
            else // If the remaining time has reached zero, handle the end of the session
            {
                _timer.Stop(); // Stop the timer as the session has ended
                if (_isWorkSession) //If the current session is a work session
                {
                    _completedWorkSessions++; // Increment the count of completed work sessions
                    DescriptionPanel.Visibility = Visibility.Visible; // Show the description panel to allow the user to enter details for the completed sprint
                }
                else //otherwise if the current session is a break session
                {
                    if (_isLongBreak) //If the break session is a long break
                    {
                        _completedWorkSessions = 0; // Reset the count of completed work sessions after a long break
                        _isLongBreak = false; // Reset the long break flag
                    }
                    StartWorkButton.IsEnabled = true; // Re-enable the Start Work button to allow starting a new work session
                    PomodoroTimerText.Text = "25:00"; // Reset the timer text to the default work session time
                }
            }
        }
        // This method returns the ordinal suffix for a given number (e.g., "st", "nd", "rd", "th")
        private static string GetOrdinalSuffix(int number)
        {
            if (number % 100 is >= 11 and <= 13) return "th"; // Special case for numbers ending in 11, 12, or 13
            return (number % 10) switch // Determine the suffix based on the last digit of the number
            {
                1 => "st", // First
                2 => "nd", // Second
                3 => "rd", // Third
                _ => "th" // All other numbers
            };
        }
        // Event handler for the Add Task button click event, which adds a new task to the task list
        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var newTask = new TaskItem(); // Create a new task item with default properties
            _tasks.Add(newTask); // Add the new task to the collection of tasks
            Dispatcher.BeginInvoke(() => // Focus the newly added task text box after it has been added to the UI
            {
                int index = _tasks.IndexOf(newTask); // Get the index of the newly added task
                FocusTaskTextBox(index); // Focus the text box of the newly added task
            }, DispatcherPriority.Loaded); // Use Dispatcher to ensure the focus is set after the UI has loaded
        }
        //This method handles the key down event for the task text boxes, allowing for task creation and indentation changes
        private void TaskTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox textBox) return; // Ensure the sender is a TextBox before proceeding
            if (e.Key == Key.Enter) // If the Enter key is pressed
            {
                e.Handled = true; // Mark the event as handled to prevent default behavior
                if (textBox.DataContext is TaskItem currentTask) // Check if the DataContext of the TextBox is a TaskItem
                {
                    int index = _tasks.IndexOf(currentTask); // Get the index of the current task in the collection
                    if (index >= 0) // Ensure the index is valid
                    {
                        var newTask = new TaskItem(); // Create a new task item with default properties
                        _tasks.Insert(index + 1, newTask); // Insert the new task after the current task in the collection
                        Dispatcher.BeginInvoke(() => // Focus the newly added task text box after it has been added to the UI
                        {
                            int newIndex = _tasks.IndexOf(newTask); // Get the index of the newly added task
                            FocusTaskTextBox(newIndex); // Focus the text box of the newly added task
                        }, DispatcherPriority.Loaded); // Use Dispatcher to ensure the focus is set after the UI has loaded
                    }
                }
            }
            else if (e.Key == Key.Tab) // If the Tab key is pressed
            {
                e.Handled = true; // Mark the event as handled to prevent default behavior
                if (textBox.DataContext is TaskItem currentTask) // Check if the DataContext of the TextBox is a TaskItem
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) // If Shift is held down while pressing Tab
                    {
                        if (currentTask.IndentLevel > 0) // If the current task's indent level is greater than 0
                            currentTask.IndentLevel--; // Decrease the indent level of the current task
                    }
                    else // If Shift is not held down while pressing Tab
                    {
                        currentTask.IndentLevel++; // Increase the indent level of the current task
                    }
                    Dispatcher.BeginInvoke(() => // Focus the current task text box after changing the indent level
                    {
                        textBox.Focus(); // Focus the text box of the current task
                        textBox.SelectAll(); // Select all text in the text box
                    });
                }
            }
        }
        // Event handler for the GotFocus event of the task text boxes, which selects all text when the text box gains focus
        private void TaskTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb) //Ensure the sender is a TextBox before proceeding
            {
                tb.SelectAll(); // Select all text in the text box when it gains focus
            }
        }
        // Event handler for the Close button click event, which hides the ProductivityTab component
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed; // Set the visibility of the ProductivityTab component to collapsed to hide it
        }
        // Helper method to find a visual child of a specific type in the visual tree
        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) // Iterate through the children of the parent DependencyObject
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i); // Get the child at the current index
                if (child is T correctlyTyped) return correctlyTyped; // If the child is of the specified type, return it
                T? childOfChild = FindVisualChild<T>(child); // Recursively search for the specified type in the child's visual tree
                if (childOfChild != null) return childOfChild; // If a child of the specified type is found, return it
            }
            return null; // If no child of the specified type is found, return null
        }
        // This method focuses the text box of a task at a specific index in the task list
        private void FocusTaskTextBox(int index)
        {
            for (int i = 0; i < TaskList.Items.Count; i++) // Iterate through the items in the TaskList
            {
                var container = (ContentPresenter)TaskList.ItemContainerGenerator.ContainerFromIndex(i); // Get the container for the item at the current index
                if (container != null) // If the container is not null (i.e., the item is visible in the UI)
                {
                    var tb = FindVisualChild<TextBox>(container); // Find the TextBox within the container
                    if (tb != null && i == index) // If the TextBox is found and the current index matches the specified index
                    {
                        tb.Focus(); // Focus the TextBox of the task at the specified index
                        tb.SelectAll(); // Select all text in the TextBox
                        break; // Exit the loop after focusing the correct TextBox
                    }
                }
            }
        }
    }
}
