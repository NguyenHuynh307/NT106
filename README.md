# NT106 - Practical Labs

Personal repository for selected practical work and supporting reports for NT106.

## Repository structure

```text
NT106/
|-- LAB01_NT106.R14.1/
|-- LAB02_NT106.R14.1/
|-- .gitignore
|-- LICENSE
`-- README.md
```

## Labs

| Lab | Focus | Project |
|---|---|---|
| [Lab 01](LAB01_NT106.R14.1/README.md) | C# fundamentals and Windows Forms exercises. | [Lab01.slnx](LAB01_NT106.R14.1/Lab01.slnx) |
| [Lab 02](LAB02_NT106.R14.1/README.md) | File and stream I/O, expression parsing, JSON, SQLite and file browsing. | [Lab02.slnx](LAB02_NT106.R14.1/Lab02.slnx) |

Each lab directory has its own README with an exercise summary and run instructions.

## Build

With the .NET 10 SDK installed on Windows, run from the repository root:

```powershell
dotnet build .\LAB01_NT106.R14.1\Lab01.csproj
dotnet build .\LAB02_NT106.R14.1\Lab02.csproj
```

## Repository hygiene

Generated folders such as .vs/, bin/ and obj/, along with user-specific IDE settings, should not be committed. Private SSH keys and credentials must never be added to Git. Theory materials and group-project documents are kept outside this repository.
