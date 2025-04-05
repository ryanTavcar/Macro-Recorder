# Macro Recorder

A WPF application that allows users to record keyboard input sequences and create macros with configurable delays.

## Features

- Record keyboard input sequences in real-time
- Set custom delay times between keystrokes
- Save and load macro sequences
- Clean modern UI with dark theme
- Console output for monitoring actions

## Getting Started

### Prerequisites

- Windows operating system
- .NET 6.0 or higher
- Visual Studio 2022 (recommended for development)

### Installation

1. Clone the repository - git clone https://github.com/ryanTavcar/Macro-Recorder.git
2. Open the solution file in Visual Studio
3. Build and run the application

## Usage

### Recording Macros

1. Click the "Record" button to start recording keyboard input
2. Press any keys you want to include in your macro
3. Click "Stop" to finish recording
4. Adjust delay times (in milliseconds) for each keystroke as needed

### Saving and Loading

- Click "Save" to save your macro to a file
- Click "Load" to load a previously saved macro
- Click "Clear" to remove all current macro commands

## Project Structure

The application follows the MVVM (Model-View-ViewModel) architecture:

- **Models**: Core data structures for macro commands and sequences
- **ViewModels**: Business logic and state management
- **Views**: XAML-based user interface
- **Services**: Handles recording, file operations, etc.

## Future Enhancements

- Ability to execute recorded macros
- Support for mouse actions (clicks, movements)
- Conditional logic in macros
- Loop functionality for repetitive tasks
- Hotkey binding for quick activation

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with WPF and C#
- Uses the MVVM architectural pattern