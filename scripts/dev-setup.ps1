#Requires -Version 7.0
[CmdletBinding()]
param([ValidateRange(0, 65535)][int]$SqlPort = 0)
. "$PSScriptRoot/dev-common.ps1"
if ($SqlPort -ne 0 -and $SqlPort -lt 1024) { throw 'SqlPort must be between 1024 and 65535.' }
Assert-DevPrerequisites
$settings = Get-DevSettings -Create -SqlPort $SqlPort
Write-Host 'Starting the development SQL Server and waiting for readiness...'
Invoke-DevCompose $settings @('up', '-d', '--wait', '--wait-timeout', '240', 'sqlserver')
Invoke-DevDotnet $settings @('tool', 'restore')
Invoke-DevDotnet $settings @('ef', 'database', 'update', '--project', 'Demo.Infrastructure', '--startup-project', 'Demo.Api')
Invoke-DevDotnet $settings @('run', '--project', 'Demo.Populator.csproj') (Join-Path $script:RepoRoot 'Demo.Populator')
Write-Host "Development database ready on 127.0.0.1:$($settings.Port). Run pwsh ./scripts/dev-run-api.ps1 to start the API."
