param([ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop'
$buildTool='C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe'
if (-not (Test-Path -LiteralPath $buildTool)) { throw '未找到 VS2015 MSBuild 14.0，请检查开发环境。' }
& $buildTool (Join-Path $PSScriptRoot 'Source\SocialSecurityAuth.sln') /t:Build "/p:Configuration=$Configuration" /p:Platform=x86 /verbosity:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw '生成失败，请查看上方编译信息。' }
Write-Output (Join-Path $PSScriptRoot "Build\$Configuration\SocialSecurityAuth.exe")
