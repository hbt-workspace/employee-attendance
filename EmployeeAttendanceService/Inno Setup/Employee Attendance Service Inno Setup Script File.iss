[Setup]
AppName=EmployeeAttendanceService
AppVersion=1.0.0
DefaultDirName={autopf}\HBT Workspace\EmployeeAttendanceService
DefaultGroupName=HBT Workspace\EmployeeAttendanceService
OutputDir=Installer
OutputBaseFilename=EmployeeAttendanceServiceSetup
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin

[Files]
Source: "E:\HBT Workspace\Internal Project\Published\EmployeeAttendanceService\EmployeeAttendanceService_Publish_2026_10_23\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\EmployeeAttendanceService"; Filename: "{app}\EmployeeAttendanceService.exe"