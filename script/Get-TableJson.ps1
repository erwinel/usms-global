Param(
    [Uri]$BaseUri = 'https://dev145540.service-now.com',
    [string[]]$TableName = @('sys_metadata', 'sys_scope', 'sys_package', 'task', 'sys_number', 'sys_user', 'sys_user_group', 'sys_user_role', 'sys_user_grmember',
        'sys_user_group_type', 'sys_user_role', 'sys_user_role_contains', 'sys_user_has_role', 'sys_group_has_role', 'sys_glide_object', 'sys_db_object', 'sys_dictionary')
    # [string[]]$TableName = @('sys_filter_option_dynamic', 'gsw_content', 'license_details', 'cmn_cost_center', 'cmn_schedule', 'core_company', 'cmn_building', 'cmn_department',
    # 'sys_user_grmember', 'sys_perspective', 'cmn_location', 'ldap_server_config', 'sc_cat_item_delivery_plan', 'sys_group_has_role', 'sys_glide_object', 'sys_db_object', 'sys_dictionary')
)

<#
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
#>

if ($null -eq $Script:SnCredentials) { $Script:SnCredentials = Get-Credential -Message 'SN Login' }

$TableName | ForEach-Object {
    $UriBuilder = [System.UriBuilder]::new($BaseUri);
    $UriBuilder.Path = '/api/now/table/sys_db_object';
    $Query = "name=$_";
    $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))&sysparm_display_value=all";
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $JsonText = $Response.Content;
    if ([string]::IsNullOrWhiteSpace($JsonText)) { Write-Error -Message "Failed to get sys_db_object content for $_" -ErrorAction Stop }
    $Path = $PSScriptRoot | Join-Path -ChildPath "../resources/examples/sys_db_object/$_.json";
    $JsonText | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    "Saved type information to $Path" | Write-Information -InformationAction Continue;
    $JsonText | Write-Output;

    $UriBuilder.Path = '/api/now/table/sys_dictionary';
    $Uri = $UriBuilder.Uri.AbsoluteUri;
    $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
        Accept = "application/json";
    } -ErrorAction Stop;
    $Path = $PSScriptRoot | Join-Path -ChildPath "../resources/examples/sys_dictionary/$_.json";
    $Response.Content | Out-File -LiteralPath $Path -Encoding utf8 -Force;
    "Saved type information to $Path" | Write-Information -InformationAction Continue;
}