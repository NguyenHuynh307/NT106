# LAB02 - NT106.R14.1

Source code for Lab 02: File and Stream I/O in C#.

## Project
- .NET Windows Forms
- Target framework: `net10.0-windows`
- NuGet package: `Microsoft.Data.Sqlite` (restored automatically on build)

## Exercises
| No. | Title | Highlights |
|---|---|---|
| 01 | Read and write file | Read `input1.txt`, write upper-case content to `output1.txt` |
| 02 | File information | File name, size, path, line / word / character count |
| 03 | Expression calculator | Own parser for `+ - * /` and parentheses, no `DataTable.Compute` |
| 04 | Student manager | `JsonSerializer` (replaces obsolete `BinaryFormatter`), input validation, paging, average score |
| 05 | Cinema box office | Read `input5.txt`, revenue statistics and ranking, ProgressBar when exporting `output5.txt` |
| 06 | What to eat today? | SQLite database (`MonAn`, `NguoiDung`), ListView, random dish with image |
| 07 | Folder browser | TreeView of drives and folders, preview of text / image / binary files |

## Structure
- `Lab02.csproj`: project file
- `Program.cs`: application entry point
- `Mainform.*`: main menu with one card per exercise
- `Lab02_Bai01.*` to `Lab02_Bai07.*`: lab exercises
- `Bai06_*.cs`: SQLite database, dish catalog and image generator for exercise 06
- `Bai07_*.cs`: folder scanner, file viewer and favorites for exercise 07
- `Theme.cs`, `AnimatedHeader.cs`, `CardPanel.cs`, `ExerciseCard.cs`: shared UI styling
- `input*.txt`: sample input files

## How to run
1. Open `LAB02_25521233_NT106.R14.1.slnx` in Visual Studio.
2. Press `F5` and pick an exercise from the main menu.

Generated folders such as `bin/` and `obj/` are intentionally excluded from Git.
