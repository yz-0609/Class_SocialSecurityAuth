@echo off
pushd "%~dp0Build\Release"
if not exist "SocialSecurityAuth.exe" (
 echo Please build the Release configuration in Visual Studio 2015 first.
 pause
 popd
 exit /b 1
)
start "" "SocialSecurityAuth.exe"
popd
