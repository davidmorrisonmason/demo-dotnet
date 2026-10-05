#Requires -Version 7.0
[CmdletBinding()]
param()
. "$PSScriptRoot/dev-common.ps1"
$settings = Get-DevSettings
Invoke-DevDotnet $settings @('run', '--project', 'Demo.Api', '--launch-profile', 'http')
