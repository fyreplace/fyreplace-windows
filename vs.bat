@echo off

setlocal

for /f "delims=" %%i in ('"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -find **\vcvarsall.bat') do set devshell=%%i

if defined devshell (
    call "%devshell%" %PROCESSOR_ARCHITECTURE% > nul
) else (
    echo vcvarsall.bat not found
)

%*
exit %ERRORLEVEL%

endlocal
