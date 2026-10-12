# Lab 01 — NT106.R14.1

Windows Forms exercises for the NT106.R14.1 practical lab. The project is written in C# and targets **.NET 10 for Windows**.

## Project

- **Solution:** `Lab01.slnx`
- **Project:** `Lab01.csproj`
- **Application type:** Windows Forms (`net10.0-windows`)
- **Entry point:** `Program.cs`
- **Report:** `Report-NT106.R14.1-Lab01_25521233.pdf`
- **External NuGet packages:** None declared in the project file.

## Exercises

| No. | Topic | Main functionality |
|---|---|---|
| 01 | Add two integers | Validate two integer inputs and display their sum. |
| 02 | Find minimum and maximum | Find the smallest and largest among three numeric values. |
| 03 | Read an integer | Read numbers in basic mode (`0–9`) or switch to an advanced mode supporting up to 12 digits. |
| 04 | Cinema ticket booking | Select a movie, room, showtime and seats; calculate ticket prices and enforce booking rules. |
| 05 | Integer-based calculations | Generate a multiplication table from `B - A`, or calculate `(A - B)!` and the power sum `A¹ + A² + … + Aᴮ` when inputs are valid. |
| 06 | Birthday zodiac lookup | Use a selected birth date to determine and display the app's zodiac information. |
| 07 | Student score analysis | Parse a student's name and scores, validate scores, and calculate average, minimum, maximum, pass/fail counts and ranking. |
| 08 | Food and location explorer | Manage and filter food choices, select a random dish, and browse province/ward/street data provided by the application. |

## Source layout

- `Lab01_Bai01.*`–`Lab01_Bai08.*`: forms and their exercise logic.
- Matching `.Designer.cs` and `.resx` files: WinForms UI layout and resources.
- `Mainform.*`: main menu for opening each exercise.
- `Properties/Resources.resx`: project resources.

## Run the project

### Visual Studio

1. Open `Lab01.slnx` in Visual Studio on Windows.
2. Allow Visual Studio to restore the project if prompted.
3. Press **F5** to launch the main menu and choose an exercise.

### .NET CLI

From this directory, run:

```powershell
dotnet build .\Lab01.csproj
dotnet run --project .\Lab01.csproj
```

A Windows environment with the .NET 10 SDK and Windows Desktop targeting support is required to run the WinForms application.

## Repository notes

Generated output such as `.vs/`, `bin/`, `obj/`, and user-specific IDE settings should remain untracked. The lab report is kept alongside the project files.
