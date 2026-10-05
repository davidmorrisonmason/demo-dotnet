#Requires -Version 7.0
[CmdletBinding()]
param([switch]$DeleteDevelopmentData)
if (-not $DeleteDevelopmentData) {
    throw 'This permanently deletes this checkout''s Docker development database. Stop the API, then rerun with -DeleteDevelopmentData to confirm.'
}
. "$PSScriptRoot/dev-common.ps1"
Assert-DevPrerequisites
$settings = Get-DevSettings
Invoke-DevCompose $settings @('down', '--volumes')
Write-Host 'Development database deleted. Run pwsh ./scripts/dev-setup.ps1 to recreate it.'
