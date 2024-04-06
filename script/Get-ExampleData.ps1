Param(
    [Uri]$BaseUri = 'https://dev243690.service-now.com',
    [string[]]$TableNames = @('sys_metadata', 'sys_scope', 'sys_package', 'task', 'sys_number', 'sys_user', 'sys_user_group', 'sys_user_role', 'sys_user_grmember',
    'sys_user_group_type', 'sys_user_role', 'sys_user_role_contains', 'sys_user_has_role', 'sys_group_has_role', 'sys_glide_object', 'sys_db_object', 'sys_dictionary',
    'incident', 'sc_request', 'sc_req_item', 'sc_task', 'cab_meeting', 'idontexist'),
    [string[]]$ScopeIds = @('global')
)
if ($null -eq $Script:SnCredentials) {
    $Confirm = $null;
    do {
        $Script:SnCredentials = $null;
        $Confirm = Get-Credential -Message 'SN Login';
        if ($null -eq $Confirm -or $null -eq ($Script:SnCredentials = Get-Credential -Message 'Confirm SN Login' -UserName $Confirm.UserName)) { return }
    } while ($Script:SnCredentials.GetNetworkCredential().Password -ne $Confirm.GetNetworkCredential().Password);
}

$TablesByNamePath = $PSScriptRoot | Join-Path -ChildPath "../resources/examples/tables-by-name";
$ElementsByNamePath = $PSScriptRoot | Join-Path -ChildPath "../resources/examples/elements-by-name";
$TablesByIdPath = $PSScriptRoot | Join-Path -ChildPath "../resources/examples/tables-by-id";
$ScopesByIdPath = $PSScriptRoot | Join-Path -ChildPath "../resources/examples/scopes-by-id";
$TypesByNamePath = $PSScriptRoot | Join-Path -ChildPath "../resources/examples/types-by-name";
if (-not ($TablesByNamePath | Test-Path)) {
    (New-Item -Path $TablesByNamePath -ItemType Directory) | Out-Null;
}
if (-not ($ElementsByNamePath | Test-Path)) {
    (New-Item -Path $ElementsByNamePath -ItemType Directory) | Out-Null;
}
if (-not ($TablesByIdPath | Test-Path)) {
    (New-Item -Path $TablesByIdPath -ItemType Directory) | Out-Null;
}
if (-not ($ScopesByIdPath | Test-Path)) {
    (New-Item -Path $ScopesByIdPath -ItemType Directory) | Out-Null;
}
if (-not ($TypesByNamePath | Test-Path)) {
    (New-Item -Path $TypesByNamePath -ItemType Directory) | Out-Null;
}
$SysDbObject = $null;
$TypeNames = @();
$ScopeIds = @();
for ($Index = 0; $Index -lt $TableNames.Count; $Index++) {
    $UriBuilder = [System.UriBuilder]::new($BaseUri);
    $UriBuilder.Path = '/api/now/table/sys_db_object';
    $Query = "name=$($TableNames[$Index])";
    $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))&sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    [int]$PercentComplete = ($Index / $TableNames.Count) * 100;
    Write-Progress -Activity 'Getting example data' -Status $TableNames[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) { Write-Error -Message "Failed to get sys_db_object content for $($TableNames[$Index])" -ErrorAction Stop }
    $Path = $TablesByNamePath | Join-Path -ChildPath "$($TableNames[$Index]).json";
    $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;

    $SysDbObject = $JsonText | ConvertFrom-Json;
    
    $UriBuilder.Path = '/api/now/table/sys_dictionary';
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    Write-Progress -Activity 'Getting table data' -Status $TableNames[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $Path = $ElementsByNamePath | Join-Path -ChildPath "$($TableNames[$Index])_.json";
    $JsonText = $Response.Content;
    $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    $SysDictionary = $JsonText | ConvertFrom-Json;
    $SysDictionary.result | ForEach-Object {
        $n = $_.internal_type.value;
        if ($TypeNames -notcontains $n) { $TypeNames += $n }
        $n = $_.sys_scope.value;
        if (-not ([string]::IsNullOrEmpty($n) -or $ScopeIds -contains $n)) { $ScopeIds += $n }
    }
    if ($SysDbObject.result.Length -gt 0) {
        $n = $SysDbObject.result[0].sys_scope.value;
        if (-not ([string]::IsNullOrEmpty($n) -or $ScopeIds -contains $n)) { $ScopeIds += $n }
        $UriBuilder.Path = "/api/now/table/sys_db_object/$($SysDbObject.result[0].sys_id.value)";
    } else {
        $UriBuilder.Path = "/api/now/table/sys_db_object/$($TableNames[$Index])";
    }
    $UriBuilder.Query = "sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    Write-Progress -Activity 'Getting table data' -Status $TableNames[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $Path = $TablesByIdPath | Join-Path -ChildPath "$($TableNames[$Index])_.json";
    $Response.Content | Out-File -LiteralPath $Path -Encoding utf8 -Force;
}
Write-Progress -Activity 'Getting table data' -Status 'Finished' -Completed;
for ($Index = 0; $Index -lt $TypeNames.Count; $Index++) {
    $UriBuilder = [System.UriBuilder]::new($BaseUri);
    $UriBuilder.Path = '/api/now/table/sys_glide_object';
    $Query = "name=$($TypeNames[$Index])";
    $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))&sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    [int]$PercentComplete = ($Index / $TypeNames.Count) * 100;
    Write-Progress -Activity 'Getting type data' -Status $TypeNames[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) { Write-Error -Message "Failed to get sys_glide_object content for $($TypeNames[$Index])" -ErrorAction Stop }
    $Path = $TypesByNamePath | Join-Path -ChildPath "$($TypeNames[$Index]).json";
    $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    $SysGlideObject = $JsonText | ConvertFrom-Json;
    if ($SysGlideObject.result.Length -gt 0) {
        $n = $SysGlideObject.result[0].sys_scope.value;
        if (-not ([string]::IsNullOrEmpty($n) -or $ScopeIds -contains $n)) { $ScopeIds += $n }
    }
}
Write-Progress -Activity 'Getting type data' -Status 'Finished' -Completed;
for ($Index = 0; $Index -lt $ScopeIds.Count; $Index++) {
    $UriBuilder = [System.UriBuilder]::new($BaseUri);
    $UriBuilder.Path = "/api/now/table/sys_scope/$($ScopeIds[$Index])";
    $Query = "name=$($ScopeIds[$Index])";
    $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))&sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    [int]$PercentComplete = ($Index / $ScopeIds.Count) * 100;
    Write-Progress -Activity 'Getting scope data' -Status $ScopeIds[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) { Write-Error -Message "Failed to get sys_scope content for $($ScopeIds[$Index])" -ErrorAction Stop }
    $Path = $ScopesByIdPath | Join-Path -ChildPath "$($ScopeIds[$Index]).json";
    $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
}

Write-Progress -Activity 'Getting scope data' -Status 'Finished' -Completed;


