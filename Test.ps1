$ErrorActionPreference='Stop'
& (Join-Path $PSScriptRoot 'Build.ps1') -Configuration Debug
& (Join-Path $PSScriptRoot 'Build\Tests\SocialSecurityAuth.Tests.exe')
if ($LASTEXITCODE -ne 0) { throw '集成测试失败。' }
