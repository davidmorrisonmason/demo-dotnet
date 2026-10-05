#Requires -Version 7.0
[CmdletBinding()]
param()
. "$PSScriptRoot/dev-common.ps1"
Assert-DevPrerequisites
$settings = Get-DevSettings
Invoke-DevCompose $settings @('down')
Write-Host 'Development SQL Server stopped. Database data and credentials have been retained.'
