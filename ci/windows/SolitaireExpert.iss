#define MyAppName "Solitaire Expert"
#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif
#ifndef SourceDir
  #error SourceDir define is required
#endif
#ifndef OutputDir
  #error OutputDir define is required
#endif

[Setup]
AppId={{89E594BF-6F7E-4D2A-96F7-7C1E0FBAC5DF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=Solitaire Expert
DefaultDirName={autopf}\Solitaire Expert
DefaultGroupName=Solitaire Expert
DisableProgramGroupPage=yes
Compression=lzma
SolidCompression=yes
OutputDir={#OutputDir}
OutputBaseFilename=SolitaireExpertSetup
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\Solitaire Expert"; Filename: "{app}\SolitaireExpert.exe"
Name: "{autodesktop}\Solitaire Expert"; Filename: "{app}\SolitaireExpert.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\SolitaireExpert.exe"; Description: "{cm:LaunchProgram,Solitaire Expert}"; Flags: nowait postinstall skipifsilent
