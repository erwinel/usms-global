Param(
    [Uri]$BaseUri = 'https://dev145540.service-now.com'
)

<#
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
#>

if ($null -eq $Script:SnCredentials) { $Script:SnCredentials = Get-Credential -Message 'SN Login' }

$UriBuilder = [System.UriBuilder]::new($BaseUri);
$UriBuilder.Path = '/api/now/table/sys_glide_object';
$Uri = $UriBuilder.Uri.AbsoluteUri;
$Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
    Accept = "application/json";
} -ErrorAction Stop;
if ([string]::IsNullOrWhiteSpace($Response.Content)) { Write-Error -Message "Failed to get sys_glide_object content" -ErrorAction Stop }
$Path = $PSScriptRoot | Join-Path -ChildPath '../resources/examples/sys_glide_object.json';
$Response.Content | Out-File -LiteralPath $Path -Encoding utf8 -Force;
"Saved type information to $Path" | Write-Information -InformationAction Continue;