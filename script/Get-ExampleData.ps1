Param(
    [Uri]$BaseUri = 'https://dev243690.service-now.com',
    [string[]]$TableNames = @('sys_metadata', 'sys_scope', 'sys_package', 'task', 'sys_number', 'sys_user', 'sys_user_group', 'sys_user_role', 'sys_user_grmember',
    'sys_user_group_type', 'sys_user_role', 'sys_user_role_contains', 'sys_user_has_role', 'sys_group_has_role', 'sys_glide_object', 'sys_db_object', 'sys_dictionary',
    'incident', 'sc_request', 'sc_req_item', 'sc_task', 'cab_meeting', 'idontexist'),
    [string[]]$PackageIds = @('global', '0f6ab99a0f36060094f3c09ce1050ee8', 'x_g_doj_usmsprogop', 'sn_cmdb_ci_class', 'sn_core_ui_ponents', 'com.snc.sdlc.agile.2.0', 'apps/system', 'glidesoft')
)
$Script:SnCredentials = $null;
# 2bzF79kJkuH0QIR..9Cd*TN_A+}w7I29wqYXxx!Y?nN+#X<>Xz})M96(!p8V
if ($null -eq $Script:SnCredentials) {
    $Confirm = $null;
    do {
        $Script:SnCredentials = $null;
        $Confirm = Get-Credential -Message 'SN Login';
        if ($null -eq $Confirm -or $null -eq ($Script:SnCredentials = Get-Credential -Message 'Confirm SN Login' -UserName $Confirm.UserName)) { return }
    } while ($Script:SnCredentials.GetNetworkCredential().Password -ne $Confirm.GetNetworkCredential().Password);
}

$ExamplesPath = $PSScriptRoot | Join-Path -ChildPath "../resources/examples";
if (-not ($ExamplesPath | Test-Path)) {
    (New-Item -Path $ExamplesPath -ItemType Directory) | Out-Null;
}
$TableRecordByNamePath = $ExamplesPath | Join-Path -ChildPath "TableRecordByName";
if (-not ($TableRecordByNamePath | Test-Path)) {
    (New-Item -Path $TableRecordByNamePath -ItemType Directory) | Out-Null;
}
$TableRecordBySysIdPath = $ExamplesPath | Join-Path -ChildPath "TableRecordBySysId";;
if (-not ($TableRecordBySysIdPath | Test-Path)) {
    (New-Item -Path $TableRecordBySysIdPath -ItemType Directory) | Out-Null;
}
$DictionaryEntryRecordsByTableNamePath = $ExamplesPath | Join-Path -ChildPath "DictionaryEntryRecordsByTableName";
if (-not ($DictionaryEntryRecordsByTableNamePath | Test-Path)) {
    (New-Item -Path $DictionaryEntryRecordsByTableNamePath -ItemType Directory) | Out-Null;
}
$SysPluginsByIdPath = $ExamplesPath | Join-Path -ChildPath "SysPluginsById";
if (-not ($SysPluginsByIdPath | Test-Path)) {
    (New-Item -Path $SysPluginsByIdPath -ItemType Directory) | Out-Null;
}
$SysStoreAppByIdPath = $ExamplesPath | Join-Path -ChildPath "SysStoreAppById";
if (-not ($SysStoreAppByIdPath | Test-Path)) {
    (New-Item -Path $SysStoreAppByIdPath -ItemType Directory) | Out-Null;
}
$SysAppByIdPath = $ExamplesPath | Join-Path -ChildPath "SysAppById";
if (-not ($SysAppByIdPath | Test-Path)) {
    (New-Item -Path $SysAppByIdPath -ItemType Directory) | Out-Null;
}
$SysScopeByIdPath = $ExamplesPath | Join-Path -ChildPath "SysScopeById";
if (-not ($SysScopeByIdPath | Test-Path)) {
    (New-Item -Path $SysScopeByIdPath -ItemType Directory) | Out-Null;
}
$SysPackageByIdPath = $ExamplesPath | Join-Path -ChildPath "SysPackageById";
if (-not ($SysPackageByIdPath | Test-Path)) {
    (New-Item -Path $SysPackageByIdPath -ItemType Directory) | Out-Null;
}
$SysPluginsByNamePath = $ExamplesPath | Join-Path -ChildPath "SysPluginsByName";
$SysStoreAppByNamePath = $ExamplesPath | Join-Path -ChildPath "SysStoreAppByName";
$SysAppByNamePath = $ExamplesPath | Join-Path -ChildPath "SysAppByName";
$SysScopeByNamePath = $ExamplesPath | Join-Path -ChildPath "SysScopeByName";
$SysPackageByNamePath = $ExamplesPath | Join-Path -ChildPath "SysPackageByName";
$SysPluginsBySysIdPath = $ExamplesPath | Join-Path -ChildPath "SysPluginsBySysId";
$SysStoreAppBySysIdPath = $ExamplesPath | Join-Path -ChildPath "SysStoreAppBySysId";
$SysAppBySysIdPath = $ExamplesPath | Join-Path -ChildPath "SysAppBySysId";
$SysScopeBySysIdPath = $ExamplesPath | Join-Path -ChildPath "SysScopeBySysId";
$SysPackageBySysIdPath = $ExamplesPath | Join-Path -ChildPath "SysPackageBySysId";
$FieldClassRecordByNamePath = $ExamplesPath | Join-Path -ChildPath "FieldClassRecordByName";
if (-not ($FieldClassRecordByNamePath | Test-Path)) {
    (New-Item -Path $FieldClassRecordByNamePath -ItemType Directory) | Out-Null;
}
# 
$UriBuilder = [System.UriBuilder]::new($BaseUri);
$UriBuilder.Path = '/oauth_token.do';
for ($Index = 0; $Index -lt $PackageIds.Length; $Index++) {
    $UriBuilder = [System.UriBuilder]::new($BaseUri);
    $UriBuilder.Path = '/api/now/table/sys_plugins';
    $Query = "source=$($PackageIds[$Index])";
    $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))&sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    [int]$PercentComplete = ($Index / $PackageIds.Count) * 100;
    Write-Progress -Activity 'Getting package data' -Status $PackageIds[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
        access_token = $access_token;
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) {
        Write-Error -Message "Failed to get sys_plugins content for $($PackageIds[$Index])" -ErrorAction Stop;
    } else {
        $Path = $SysPluginsByIdPath | Join-Path -ChildPath "$($PackageIds[$Index]).json";
        $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    }

    $UriBuilder = [System.UriBuilder]::new($BaseUri);
    $UriBuilder.Path = '/api/now/table/sys_store_app';
    $Query = "source=$($PackageIds[$Index])";
    $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))&sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    Write-Progress -Activity 'Getting package data' -Status $PackageIds[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) {
        Write-Error -Message "Failed to get sys_store_app content for $($PackageIds[$Index])" -ErrorAction Stop;
    } else {
        $Path = $SysStoreAppByIdPath | Join-Path -ChildPath "$($PackageIds[$Index]).json";
        $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    }

    $UriBuilder = [System.UriBuilder]::new($BaseUri);
    $UriBuilder.Path = '/api/now/table/sys_app';
    $Query = "source=$($PackageIds[$Index])";
    $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))&sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    Write-Progress -Activity 'Getting package data' -Status $PackageIds[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) {
        Write-Error -Message "Failed to get sys_app content for $($PackageIds[$Index])" -ErrorAction Stop;
    } else {
        $Path = $SysAppByIdPath | Join-Path -ChildPath "$($PackageIds[$Index]).json";
        $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    }

    $UriBuilder = [System.UriBuilder]::new($BaseUri);
    $UriBuilder.Path = '/api/now/table/sys_scope';
    $Query = "source=$($PackageIds[$Index])";
    $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))&sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    Write-Progress -Activity 'Getting package data' -Status $PackageIds[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) {
        Write-Error -Message "Failed to get sys_scope content for $($PackageIds[$Index])" -ErrorAction Stop;
    } else {
        $Path = $SysScopeByIdPath | Join-Path -ChildPath "$($PackageIds[$Index]).json";
        $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    }

    $UriBuilder = [System.UriBuilder]::new($BaseUri);
    $UriBuilder.Path = '/api/now/table/sys_package';
    $Query = "source=$($PackageIds[$Index])";
    $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))&sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    Write-Progress -Activity 'Getting package data' -Status $PackageIds[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) {
        Write-Error -Message "Failed to get sys_package content for $($PackageIds[$Index])" -ErrorAction Stop;
    } else {
        $Path = $SysPackageByIdPath | Join-Path -ChildPath "$($PackageIds[$Index]).json";
        $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    }
}
Write-Progress -Activity 'Getting package data' -Status 'Finished' -Completed;
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
    Write-Progress -Activity 'Getting table data' -Status $TableNames[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) {
        Write-Error -Message "Failed to get sys_db_object content for $($TableNames[$Index])" -ErrorAction Stop;
    } else {
        $Path = $TableRecordByNamePath | Join-Path -ChildPath "$($TableNames[$Index]).json";
        $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
        $SysDbObject = $JsonText | ConvertFrom-Json;
        if ($null -ne $SysDbObject -and $null -ne $SysDbObject.result -and $SysDbObject.result.Length -gt 0) {
            $n = $SysDbObject.result[0].sys_scope.value;
            if (-not ([string]::IsNullOrEmpty($n) -or $ScopeIds -contains $n)) { $ScopeIds += $n }
            $UriBuilder.Path = "/api/now/table/sys_db_object/$($SysDbObject.result[0].sys_id.value)";
            $UriBuilder.Query = "sysparm_display_value=all";
            $Uri = $UriBuilder.Uri.AbsoluteUri;
            Write-Progress -Activity 'Getting table data' -Status $TableNames[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
            $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
                Accept = "application/json";
            } -ErrorAction Stop;
            $Path = $TableRecordBySysIdPath | Join-Path -ChildPath "$($TableNames[$Index])_.json";
            $Response.Content | Out-File -LiteralPath $Path -Encoding utf8 -Force;
        } else {
            Write-Warning -Message "Table named `"$($TableNames[$Index])`" not found.";
        }
    }

    
    $UriBuilder.Path = '/api/now/table/sys_dictionary';
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    Write-Progress -Activity 'Getting table data' -Status $TableNames[$Index] -CurrentOperation $Uri -PercentComplete $PercentComplete;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $Path = $DictionaryEntryRecordsByTableNamePath | Join-Path -ChildPath "$($TableNames[$Index])_.json";
    $JsonText = $Response.Content;
    $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    $SysDictionary = $JsonText | ConvertFrom-Json;
    $SysDictionary.result | ForEach-Object {
        $n = $_.internal_type.value;
        if ($TypeNames -notcontains $n) { $TypeNames += $n }
        $n = $_.sys_scope.value;
    }
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
    $Path = $FieldClassRecordByNamePath | Join-Path -ChildPath "$($TypeNames[$Index]).json";
    $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    $SysGlideObject = $JsonText | ConvertFrom-Json;
    if ($SysGlideObject.result.Length -gt 0) {
        $n = $SysGlideObject.result[0].sys_scope.value;
    }
}
Write-Progress -Activity 'Getting type data' -Status 'Finished' -Completed;


