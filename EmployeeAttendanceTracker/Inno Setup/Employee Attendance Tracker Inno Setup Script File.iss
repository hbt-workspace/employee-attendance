#define MyAppName "Employee Attendance Tracker"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "HBT Workspace"
#define MyAppExeName "EmployeeAttendanceTracker.exe"

[Setup]
AppId={{A2F0D38F-38C5-49A7-A7A4-2E8D8F5D96E1}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\HBT Workspace\Employee Attendance Tracker
DefaultGroupName=HBT Workspace\Employee Attendance Tracker
AllowNoIcons=no
OutputDir=Installer
OutputBaseFilename=EmployeeAttendanceTrackerSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
SetupIconFile=E:\HBT Workspace\Internal Project\Published\EmployeeAttendanceTracker\EmployeeAttendanceTracker_Published_2026_10_29\Assets\attendance_tracker.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked
Name: "startup"; Description: "Launch Employee Attendance Tracker automatically when Windows starts"; GroupDescription: "Additional options:"; Flags: unchecked

[Files]
Source: "E:\HBT Workspace\Internal Project\Published\EmployeeAttendanceTracker\EmployeeAttendanceTracker_Published_2026_10_29\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Employee Attendance Tracker"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\Employee Attendance Tracker"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userstartup}\Employee Attendance Tracker"; Filename: "{app}\{#MyAppExeName}"; Tasks: startup

[Run]
Filename: "{app}\{#MyAppExeName}";Description: "Launch Employee Attendance Tracker";Flags: nowait postinstall skipifsilent