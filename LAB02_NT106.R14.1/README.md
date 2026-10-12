# Lab 02 — NT106.R14.1

C# Windows Forms exercises focused on **file and stream I/O**, with additional practice in calculation, JSON serialization, SQLite data access and file browsing. The project targets **.NET 10 for Windows**.

## Project

- **Solution:** `Lab02.slnx`
- **Project:** `Lab02.csproj`
- **Application type:** Windows Forms (`net10.0-windows`)
- **Entry point:** `Program.cs`
- **NuGet packages:** `Microsoft.Data.Sqlite` 10.0.12 and `SQLitePCLRaw.bundle_e_sqlite3` 2.1.13 (restored during build)
- **Report:** `Report-NT106_R14_1-Lab02_25521233.pdf`

## Exercises

| No. | Topic | Main functionality |
|---|---|---|
| 01 | Read and write files | Read `input1.txt`, convert its content to uppercase and write the result to `output1.txt`. |
| 02 | File information | Display file name, size, path, and line, word and character counts. |
| 03 | Expression calculator | Parse arithmetic expressions using `+`, `-`, `*`, `/` and parentheses without `DataTable.Compute`. |
| 04 | Student manager | Serialize student data as JSON, validate input, page through records and calculate average scores. |
| 05 | Cinema box office | Read `input5.txt`, calculate revenue statistics and rankings, and export results to `output5.txt` with progress feedback. |
| 06 | What to eat today? | Use SQLite tables `MonAn` and `NguoiDung`, display food items, choose a random dish and show food imagery. |
| 07 | Folder browser | Browse drives and folders in a `TreeView`, preview text/image/binary files and manage favorites. |

## Source layout

- `Lab02_Bai01.*`–`Lab02_Bai07.*`: exercise forms and their UI resources.
- `Mainform.*`: main menu with a card for each exercise.
- `Bai06_*.cs`: SQLite database access, food catalog and image helpers for exercise 06.
- `Bai07_*.cs`: folder scanning, file viewing, file catalog and favorites for exercise 07.
- `Theme.cs`, `AnimatedHeader.cs`, `CardPanel.cs`, `ExerciseCard.cs`: shared UI styling and reusable controls.
- `input*.txt`, `output*.txt`: sample data used by selected exercises.

## Run the project

### Visual Studio

1. Open `Lab02.slnx` in Visual Studio on Windows.
2. Restore NuGet packages when prompted.
3. Press **F5** to launch the main menu and select an exercise.

### .NET CLI

From this directory, run:

```powershell
dotnet restore .\Lab02.csproj
dotnet build .\Lab02.csproj
dotnet run --project .\Lab02.csproj
```

The application requires Windows and the .NET 10 SDK with Windows Desktop targeting support. Exercise 06 uses `Microsoft.Data.Sqlite`; its package is declared in `Lab02.csproj`.

## Sample files and generated output

The `input*.txt` and `output*.txt` files are sample/test data for the corresponding tasks. If an exercise writes an output file, check the application's working directory and avoid overwriting any personal files with the same name.

## Repository notes

Generated folders such as `.vs/`, `bin/` and `obj/`, along with user-specific IDE settings, should remain untracked. The lab report and the exercise sample files are included with the project.
