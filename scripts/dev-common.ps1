# Shared by the development commands. Credentials are never placed in CLI arguments.
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$script:RepoRoot = Split-Path -Parent $PSScriptRoot
$script:SettingsPath = Join-Path $script:RepoRoot '.dev/sqlserver.env'

function Assert-DevPrerequisites {
    if ($PSVersionTable.PSVersion.Major -lt 7) { throw 'Install PowerShell 7 and run these commands with pwsh.' }
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Install the .NET 10 SDK.' }
    $sdks = & dotnet --list-sdks
    if ($LASTEXITCODE -ne 0 -or -not ($sdks -match '^10\.')) { throw 'Install the .NET 10 SDK.' }
    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        throw 'Install Docker Desktop (Linux containers) or Docker Engine with Compose, then start Docker.'
    }
    $server = & docker info --format '{{.OSType}}/{{.Architecture}}' 2>$null
    if ($LASTEXITCODE -ne 0) { throw 'Docker is unavailable. Start Docker and retry.' }
    if ($server -notmatch '^linux/(x86_64|amd64)$') {
        throw 'SQL Server requires a Linux x86-64 Docker engine. On Windows, switch Docker Desktop to Linux containers.'
    }
    $composeVersion = & docker compose version --short 2>$null
    if ($LASTEXITCODE -ne 0 -or $composeVersion -notmatch '^v?(\d+)\.(\d+)\.(\d+)') {
        throw 'Install Docker Compose v2.20.0 or newer.'
    }
    if ([version]($composeVersion.TrimStart('v') -replace '-.*$', '') -lt [version]'2.20.0') {
        throw 'Update Docker Compose to v2.20.0 or newer.'
    }
}

function Get-DevSettings {
    param([switch]$Create, [int]$SqlPort = 0)
    $hashBytes = [Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($script:RepoRoot))
    $projectName = 'demo-dotnet-' + [Convert]::ToHexString($hashBytes).Substring(0, 12).ToLowerInvariant()
    if (-not (Test-Path -LiteralPath $script:SettingsPath)) {
        if (-not $Create) { throw 'Development credentials are missing. Run scripts/dev-setup.ps1 first.' }
        & docker volume inspect "${projectName}_sqlserver-data" *> $null
        if ($LASTEXITCODE -eq 0) {
            throw 'The database volume exists but its credentials are missing. Restore .dev/sqlserver.env before running setup.'
        }
        if ($SqlPort -eq 0) { $SqlPort = 14333 }
        $password = 'Dev1!' + [Convert]::ToHexString([Security.Cryptography.RandomNumberGenerator]::GetBytes(24))
        New-Item -ItemType Directory -Path (Split-Path -Parent $script:SettingsPath) -Force | Out-Null
        "MSSQL_SA_PASSWORD=$password`nDEMO_SQL_PORT=$SqlPort" | Set-Content -LiteralPath $script:SettingsPath -Encoding utf8NoBOM
    }
    $values = @{}
    foreach ($line in Get-Content -LiteralPath $script:SettingsPath) {
        if ($line -match '^(MSSQL_SA_PASSWORD|DEMO_SQL_PORT)=(.+)$') { $values[$Matches[1]] = $Matches[2] }
    }
    if (-not $values.ContainsKey('MSSQL_SA_PASSWORD') -or $values['MSSQL_SA_PASSWORD'] -notmatch '^Dev1![A-F0-9]{48}$' -or
        -not $values.ContainsKey('DEMO_SQL_PORT') -or $values['DEMO_SQL_PORT'] -notmatch '^\d+$') {
        throw 'Development settings are invalid. Restore the generated .dev/sqlserver.env file; do not rotate the password without resetting the volume.'
    }
    $savedPort = [int]$values['DEMO_SQL_PORT']
    if ($savedPort -lt 1024 -or $savedPort -gt 65535) { throw 'Development SQL port must be between 1024 and 65535.' }
    if ($SqlPort -ne 0 -and $SqlPort -ne $savedPort) { throw 'The saved port differs. Stop the database and edit DEMO_SQL_PORT in .dev/sqlserver.env first.' }
    [pscustomobject]@{
        ProjectName = $projectName
        Password = $values['MSSQL_SA_PASSWORD']
        Port = $savedPort
        ConnectionString = "Server=127.0.0.1,$savedPort;Database=Demo;User ID=sa;Password=$($values['MSSQL_SA_PASSWORD']);Encrypt=true;TrustServerCertificate=true"
    }
}

function Invoke-DevCompose {
    param($Settings, [string[]]$ComposeArguments)
    $previousPassword = $env:MSSQL_SA_PASSWORD
    $previousPort = $env:DEMO_SQL_PORT
    try {
        # Pin the environment to this checkout even if the caller has other SQL settings.
        $env:MSSQL_SA_PASSWORD = $Settings.Password
        $env:DEMO_SQL_PORT = [string]$Settings.Port
        & docker compose --project-name $Settings.ProjectName --file (Join-Path $script:RepoRoot 'compose.yaml') --env-file $script:SettingsPath @ComposeArguments
        if ($LASTEXITCODE -ne 0) { throw 'Docker Compose failed. Check Docker, available memory and the configured port.' }
    }
    finally {
        $env:MSSQL_SA_PASSWORD = $previousPassword
        $env:DEMO_SQL_PORT = $previousPort
    }
}

function Invoke-DevDotnet {
    param($Settings, [string[]]$DotnetArguments, [string]$WorkingDirectory = $script:RepoRoot)
    $previousConnection = $env:ConnectionStrings__DefaultConnection
    $previousAspnetEnvironment = $env:ASPNETCORE_ENVIRONMENT
    $previousDotnetEnvironment = $env:DOTNET_ENVIRONMENT
    try {
        $env:ConnectionStrings__DefaultConnection = $Settings.ConnectionString
        $env:ASPNETCORE_ENVIRONMENT = 'Development'
        $env:DOTNET_ENVIRONMENT = 'Development'
        Push-Location -LiteralPath $WorkingDirectory
        try {
            & dotnet @DotnetArguments
            if ($LASTEXITCODE -ne 0) { throw 'The dotnet command failed; development setup has not completed.' }
        }
        finally { Pop-Location }
    }
    finally {
        $env:ConnectionStrings__DefaultConnection = $previousConnection
        $env:ASPNETCORE_ENVIRONMENT = $previousAspnetEnvironment
        $env:DOTNET_ENVIRONMENT = $previousDotnetEnvironment
    }
}
