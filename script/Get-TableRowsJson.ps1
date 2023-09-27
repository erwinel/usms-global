Param(
    [Uri]$BaseUri = 'https://dev145316.service-now.com',
    [string]$TableName = 'sys_scope',
    [string]$SysId = 'global'
)

<#
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
#>

if ($null -eq $Script:SnCredentials) { $Script:SnCredentials = Get-Credential -Message 'SN Login' }

$UriBuilder = [System.UriBuilder]::new($BaseUri);
$UriBuilder.Path = "/api/now/table/$([Uri]::EscapeDataString($TableName))/$([Uri]::EscapeDataString($SysId))";
$UriBuilder.Query = "sysparm_display_value=all";
$Uri = $UriBuilder.Uri.AbsoluteUri;
$Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
    Accept = "application/json";
} -ErrorAction Stop;
$JsonText = $Response.Content;
if ([string]::IsNullOrWhiteSpace($JsonText)) { Write-Error -Message "Failed to get $TableName content" -ErrorAction Stop }
$Path = $PSScriptRoot | Join-Path -ChildPath "../resources/examples/$TableName.json";
$JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
"Saved type information to $Path" | Write-Information -InformationAction Continue;