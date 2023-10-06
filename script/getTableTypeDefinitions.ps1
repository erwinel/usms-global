Param(
    [string[]]$TableNames = @('ts_configuration', 'sp_column', 'sp_row', 'sp_container', 'ais_search_profile', 'sys_script_include', 'sys_data_policy2', 'stage_state', 'alm_stockroom_type', 'sc_category', 'sys_app_module',
        'cmn_timeline_page', 'sc_renderer', 'sc_homepage_renderer', 'sys_report'),
    #[string[]]$TableNames = @('task'),

    [Uri]$BaseUri = 'https://dev145540.service-now.com',

    [string]$RecordNamespace = '$$GlideRecord',

    [string]$ElementNamespace = '$$GlideElement',

    [string]$FieldsNamespace = '$$tableFields',

    [bool]$IsScoped = $true,

    [string]$OutputScope = 'global'
)
<#
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
#>
if ($null -eq $Script:SnCredentials) { $Script:SnCredentials = Get-Credential -Message 'SN Login' }
$Script:SysDictionaryPath = $PSScriptRoot | Join-Path -ChildPath 'sys_dictionary';
if (-not ($Script:SysDictionaryPath | Test-Path)) { New-Item -Path $Script:SysDictionaryPath -ItemType Directory -Name 'sys_dictionary' };

class ProgressInfo {
    [int]$Id = 1;
    [string]$Activity;
    [bool]$Reported;
    [ProgressInfo]$Parent;
    static [ProgressInfo]$Current;
    static [void] Push([string]$Activity) {
        if ([string]::IsNullOrWhiteSpace($Activity)) { throw 'Activity cannot be empty' }
        if ($null -eq [ProgressInfo]::Current) {
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = 1;
                Activity = $Activity;
                Reported = $false;
            };
        } else {
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = [ProgressInfo]::Current.Id + 1;
                Activity = $Activity;
                Parent = [ProgressInfo]::Current;
                Reported = $false;
            };
        }
    }
    static [void] Start([string]$Activity, [string]$Status) {
        if ($null -eq [ProgressInfo]::Current) {
            Write-Progress -Activity $Activity -Status $Status -Id 1;
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = 1;
                Activity = $Activity;
                Reported = $true;
            };
        } else {
            Write-Progress -Activity $Activity -Status $Status -Id ([ProgressInfo]::Current.Id + 1) -ParentId ([ProgressInfo]::Current.Id);
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = [ProgressInfo]::Current.Id + 1;
                Activity = $Activity;
                Parent = [ProgressInfo]::Current;
                Reported = $true;
            };
        }
    }
    static [void] Start([string]$Activity, [string]$Status, [string]$CurrentOperation) {
        if ($null -eq [ProgressInfo]::Current) {
            Write-Progress -Activity $Activity -Status $Status -CurrentOperation "$CurrentOperation,$CurrentOperation" -Id 1;
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = 1;
                Activity = $Activity;
                Reported = $true;
            };
        } else {
            Write-Progress -Activity $Activity -Status $Status -CurrentOperation $CurrentOperation -Id ([ProgressInfo]::Current.Id + 1) -ParentId ([ProgressInfo]::Current.Id);
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = [ProgressInfo]::Current.Id + 1;
                Activity = $Activity;
                Parent = [ProgressInfo]::Current;
                Reported = $true;
            };
        }
    }
    static [void] Start([string]$Activity, [string]$Status, [int]$PercentComplete) {
        if ($null -eq [ProgressInfo]::Current) {
            Write-Progress -Activity $Activity -Status $Status -PercentComplete $PercentComplete -Id 1;
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = 1;
                Activity = $Activity;
                Reported = $true;
            };
        } else {
            Write-Progress -Activity $Activity -Status $Status -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id + 1) -ParentId ([ProgressInfo]::Current.Id);
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = [ProgressInfo]::Current.Id + 1;
                Activity = $Activity;
                Parent = [ProgressInfo]::Current;
                Reported = $true;
            };
        }
    }
    static [void] Start([string]$Activity, [string]$Status, [string]$CurrentOperation, [int]$PercentComplete) {
        if ($null -eq [ProgressInfo]::Current) {
            Write-Progress -Activity $Activity -Status $Status -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete -Id 1;
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = 1;
                Activity = $Activity;
                Reported = $true;
            };
        } else {
            Write-Progress -Activity $Activity -Status $Status -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id + 1) -ParentId ([ProgressInfo]::Current.Id);
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = [ProgressInfo]::Current.Id + 1;
                Activity = $Activity;
                Parent = [ProgressInfo]::Current;
                Reported = $true;
            };
        }
    }
    static [bool] WriteFirst([string]$DefaultActivity, [string]$Status) {
        if ($null -eq [ProgressInfo]::Current) {
            Write-Progress -Activity $DefaultActivity -Status $Status -Id 1;
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = 1;
                Activity = $DefaultActivity;
                Reported = $true;
            };
            return $true;
        }
        if ($null -eq [ProgressInfo]::Current.Parent) {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -Id ([ProgressInfo]::Current.Id);
        } else {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -Id ([ProgressInfo]::Current.Id) -ParentId ([ProgressInfo]::Current.Parent.Id);
        }
        [ProgressInfo]::Current.Reported = $true;
        return $false;
    }
    static [bool] WriteFirst([string]$DefaultActivity, [string]$Status, [string]$CurrentOperation) {
        if ($null -eq [ProgressInfo]::Current) {
            Write-Progress -Activity $DefaultActivity -Status $Status -CurrentOperation $CurrentOperation -Id 1;
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = 1;
                Activity = $DefaultActivity;
                Reported = $true;
            };
            return $true;
        }
        if ($null -eq [ProgressInfo]::Current.Parent) {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -Id ([ProgressInfo]::Current.Id);
        } else {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -Id ([ProgressInfo]::Current.Id) -ParentId ([ProgressInfo]::Current.Parent.Id);
        }
        [ProgressInfo]::Current.Reported = $true;
        return $false;
    }
    static [bool] WriteFirst([string]$DefaultActivity, [string]$Status, [int]$PercentComplete) {
        if ($null -eq [ProgressInfo]::Current) {
            Write-Progress -Activity $DefaultActivity -Status $Status -PercentComplete $PercentComplete -Id 1;
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = 1;
                Activity = $DefaultActivity;
                Reported = $true;
            };
            return $true;
        }
        if ($null -eq [ProgressInfo]::Current.Parent) {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id);
        } else {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -ParentId ([ProgressInfo]::Current.Parent.Id);
        }
        [ProgressInfo]::Current.Reported = $true;
        return $false;
    }
    static [bool] WriteFirst([string]$DefaultActivity, [string]$Status, [string]$CurrentOperation, [int]$PercentComplete) {
        if ($null -eq [ProgressInfo]::Current) {
            Write-Progress -Activity $DefaultActivity -Status $Status -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete -Id 1;
            [ProgressInfo]::Current = [ProgressInfo]@{
                Id = 1;
                Activity = $DefaultActivity;
                Reported = $true;
            };
            return $true;
        }
        if ($null -eq [ProgressInfo]::Current.Parent) {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id);
        } else {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -ParentId ([ProgressInfo]::Current.Parent.Id);
        }
        [ProgressInfo]::Current.Reported = $true;
        return $false;
    }
    static [void] Write([string]$Status) {
        if ($null -eq [ProgressInfo]::Current) { return }
        if ($null -eq [ProgressInfo]::Current.Parent) {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -Id ([ProgressInfo]::Current.Id);
        } else {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -Id ([ProgressInfo]::Current.Id) -ParentId ([ProgressInfo]::Current.Parent.Id);
        }
        [ProgressInfo]::Current.Reported = $true;
    }
    static [void] Write([string]$Status, [string]$CurrentOperation) {
        if ($null -eq [ProgressInfo]::Current) { return }
        if ($null -eq [ProgressInfo]::Current.Parent) {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -Id ([ProgressInfo]::Current.Id);
        } else {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -Id ([ProgressInfo]::Current.Id) -ParentId ([ProgressInfo]::Current.Parent.Id);
        }
        [ProgressInfo]::Current.Reported = $true;
    }
    static [void] Write([string]$Status, [int]$PercentComplete) {
        if ($null -eq [ProgressInfo]::Current) { return }
        if ($null -eq [ProgressInfo]::Current.Parent) {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id);
        } else {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -ParentId ([ProgressInfo]::Current.Parent.Id);
        }
        [ProgressInfo]::Current.Reported = $true;
    }
    static [void] Write([string]$Status, [string]$CurrentOperation, [int]$PercentComplete) {
        if ($null -eq [ProgressInfo]::Current) { return }
        if ($null -eq [ProgressInfo]::Current.Parent) {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id);
        } else {
            Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -ParentId ([ProgressInfo]::Current.Parent.Id);
        }
        [ProgressInfo]::Current.Reported = $true;
    }
    static [void] Complete([string]$Status) {
        if ($null -eq [ProgressInfo]::Current) { return }
        $ProgressInfo = [ProgressInfo]::Current.Parent;
        if ([ProgressInfo]::Current.Reported) {
            if ($null -eq $ProgressInfo) {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -Id ([ProgressInfo]::Current.Id) -Completed;
            } else {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -Id ([ProgressInfo]::Current.Id) -ParentId $ProgressInfo.Id -Completed;
            }
        }
        [ProgressInfo]::Current = $ProgressInfo;
    }
    static [void] Complete() {
        if ($null -eq [ProgressInfo]::Current) { return }
        $ProgressInfo = [ProgressInfo]::Current.Parent;
        if ([ProgressInfo]::Current.Reported) {
            if ($null -eq $ProgressInfo) {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status 'Completed' -Id ([ProgressInfo]::Current.Id) -Completed;
            } else {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status 'Completed' -Id ([ProgressInfo]::Current.Id) -ParentId $ProgressInfo.Id -Completed;
            }
        }
        [ProgressInfo]::Current.Reported = $true;
        [ProgressInfo]::Current = $ProgressInfo;
    }
    static [void] Complete([int]$PercentComplete) {
        if ($null -eq [ProgressInfo]::Current) { return }
        $ProgressInfo = [ProgressInfo]::Current.Parent;
        if ([ProgressInfo]::Current.Reported) {
            if ($null -eq $ProgressInfo) {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status 'Completed' -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -Completed;
            } else {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status 'Completed' -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -ParentId $ProgressInfo.Id -Completed;
            }
        }
        [ProgressInfo]::Current = $ProgressInfo;
    }
    static [void] Complete([string]$Status, [string]$CurrentOperation) {
        if ($null -eq [ProgressInfo]::Current) { return }
        $ProgressInfo = [ProgressInfo]::Current.Parent;
        if ([ProgressInfo]::Current.Reported) {
            if ($null -eq $ProgressInfo) {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -Id ([ProgressInfo]::Current.Id) -Completed;
            } else {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -Id ([ProgressInfo]::Current.Id) -ParentId $ProgressInfo.Id -Completed;
            }
        }
        [ProgressInfo]::Current = $ProgressInfo;
    }
    static [void] Complete([string]$Status, [int]$PercentComplete) {
        if ($null -eq [ProgressInfo]::Current) { return }
        $ProgressInfo = [ProgressInfo]::Current.Parent;
        if ([ProgressInfo]::Current.Reported) {
            if ($null -eq $ProgressInfo) {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -Completed;
            } else {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -ParentId $ProgressInfo.Id -Completed;
            }
        }
        [ProgressInfo]::Current = $ProgressInfo;
    }
    static [void] Complete([string]$Status, [string]$CurrentOperation, [int]$PercentComplete) {
        if ($null -eq [ProgressInfo]::Current) { return }
        $ProgressInfo = [ProgressInfo]::Current.Parent;
        if ([ProgressInfo]::Current.Reported) {
            if ($null -eq $ProgressInfo) {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -Completed;
            } else {
                Write-Progress -Activity ([ProgressInfo]::Current.Activity) -Status $Status -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete -Id ([ProgressInfo]::Current.Id) -ParentId $ProgressInfo.Id -Completed;
            }
        }
        [ProgressInfo]::Current = $ProgressInfo;
    }
}

class SysScope {
    [string]$sys_id;
    [string]$value;
    [string]$name;
    [string]$short_description;

    static [SysScope] Load([string]$Value, [PSObject]$JsonData) {
        $SysScope = [SysScope]@{
            sys_id = [GlideType]::NullIfEmpty($JsonData.sys_id);
            value = [GlideType]::NullIfEmpty($Value);
            name = [GlideType]::DefaultIfEmpty($JsonData.name, $Value);
            short_description = [GlideType]::NullIfEmpty($JsonData.short_description);
        };
        if ($null -ne $SysScope.value -and $null -ne $SysScope.name -and -not [string]::IsNullOrWhiteSpace($SysScope.sys_id)) { return $SysScope }
        return $null;
    }

    static [SysScope] Import([TypeDb]$TypeDb, [PSObject]$JsonObj) {
        $n = [GlideType]::DefaultIfEmpty($JsonObj.name, $JsonObj.scope);
        $SysScope = [SysScope]@{
            sys_id = [GlideType]::NullIfEmpty($JsonObj.sys_id);
            value = $JsonObj.scope;
            name = $n;
            short_description = [GlideType]::NullIfEmpty($JsonObj.short_description);
        };
        if ($null -eq $SysScope.sys_id -or $null -eq $SysScope.value) { return $null }
        if ($TypeDb.Scopes.ContainsKey($SysScope.value)) {
            Write-Warning -Message "Duplicate scope: $(($SysScope.value | ConvertTo-Json))";
            $TypeDb.Scopes[$SysScope.value] = $SysScope;
            if (!$TypeDb.ScopeSysIdMap.ContainsKey($SysScope.sys_id)) {
                $TypeDb.ScopeSysIdMap.Add($SysScope.sys_id, $SysScope.value);
            }
        } else {
            $TypeDb.Scopes.Add($SysScope.value, $SysScope);
            $TypeDb.ScopeSysIdMap.Add($SysScope.sys_id, $SysScope.value);
        }
        $TypeDb.HasChanges = $true;
        return $SysScope;
    }

    [PSObject]ForJsonExport() {
        $Result = [PSCustomObject]@{ sys_id = $this.sys_id; }
        if ($this.name -cne $this.value) { $Result | Add-Member -MemberType NoteProperty -Name 'name' -Value $this.name }
        if (-not [string]::IsNullOrWhiteSpace($this.short_description)) { $Result | Add-Member -MemberType NoteProperty -Name 'short_description' -Value $this.short_description }
        return $result;
    }

    [void] WriteLine([System.IO.TextWriter]$Writer) {
        if ($this.name -ceq $this.value) {
            if ([string]::IsNullOrEmpty($this.short_description) -or $this.short_description -eq $this.name) {
                $Writer.WriteLine(($this.name | ConvertTo-Json));
            } else {
                $Writer.WriteLine("$(($this.name | ConvertTo-Json)): $($this.short_description)");
            }
        } else {
            $Writer.WriteLine("$(($this.name | ConvertTo-Json)) ($($this.value))");
        }
    }
    <#
    [string] ToString() {
        if ($this.name -ceq $this.value) {
            if ([string]::IsNullOrEmpty($this.short_description) -or $this.short_description -eq $this.name) { return $this.name | ConvertTo-Json }
            return "$(($this.name | ConvertTo-Json)): $($this.short_description)";
        }
        return "$(($this.name | ConvertTo-Json)) ($($this.value))";
    }
    #>
}

$Script:BaseFields = @{
    sys_id = 'GUID';
    sys_created_on = 'glide_date_time';
    sys_created_by = 'string';
    sys_mod_count = 'integer';
    sys_updated_on = 'glide_date_time';
    sys_updated_by = 'string';
}
class TableInfo {
    [string]$sys_id;
    [string]$name;
    [string]$label;
    [bool]$is_extendable;
    [string]$scope;
    [string]$super_class;
    [string]$number_prefix;
    [string[]]$js_doc

    static [TableInfo] Load([string]$Name, [PSObject]$JsonData) {
        $TableInfo = [TableInfo]@{
            sys_id = [GlideType]::NullIfEmpty($JsonData.sys_id);
            name = [GlideType]::NullIfEmpty($Name);
            label = [GlideType]::DefaultIfEmpty($JsonData.label, $Name);
            is_extendable = [GlideType]::IsTrue($JsonData.is_extendable);
            scope = [GlideType]::NullIfEmpty($JsonData.scope);
            super_class = [GlideType]::NullIfEmpty($JsonData.super_class);
            number_prefix = [GlideType]::NullIfEmpty($JsonData.number_prefix);
        };
        if ($null -ne $JsonData.js_doc) { $TableInfo.js_doc = $JsonData.js_doc }
        if ($null -ne $TableInfo.name -and -not [string]::IsNullOrWhiteSpace($TableInfo.sys_id)) { return $TableInfo }
        return $null;
    }

    static [TableInfo] Import([TypeDb]$TypeDb, [PSObject]$JsonObj, [string]$TableName) {
        $n = [GlideType]::DefaultIfEmpty($JsonObj.name, $TableName);
        # number_ref             : @{link=https://dev145540.service-now.com/api/now/table/sys_number/3; value=3}
        $TableInfo = [TableInfo]@{
            sys_id = [GlideType]::NullIfEmpty($JsonObj.sys_id);
            name = $n;
            label = [GlideType]::DefaultIfEmpty($JsonObj.label, $n);
            is_extendable = [GlideType]::IsTrue($JsonObj.is_extendable);
            scope = [GlideType]::NullIfEmpty($JsonObj.scope);
        };
        if ($null -ne $JsonObj.number_ref) {
            [Uri]$Uri = $null;
            if ([string]::IsNullOrWhiteSpace($JsonObj.number_ref.link) -or -not [Uri]::TryCreate($JsonObj.number_ref.link, [UriKind]::Absolute, [ref]$Uri)) {
                $TableInfo.number_prefix = $TypeDb.FetchNumberRef($JsonObj.number_ref.value, $null);
            } else {
                $TableInfo.number_prefix = $TypeDb.FetchNumberRef($JsonObj.number_ref.value, $Uri);
            }
        }
        if ($null -eq $TableInfo.name -or [string]::IsNullOrWhiteSpace($TableInfo.sys_id)) { return $null }
        if ($TypeDb.TableDefinitions.ContainsKey($TableInfo.name)) {
            Write-Warning -Message "Duplicate table: $(($TableInfo.name | ConvertTo-Json))";
            $TypeDb.TableDefinitions[$TableInfo.name] = $TableInfo;
            if (!$TypeDb.TableSysIdMap.ContainsKey($TableInfo.sys_id)) {
                $TypeDb.TableSysIdMap.Add($TableInfo.sys_id, $TableInfo.name);
            }
        } else {
            $TypeDb.TableDefinitions.Add($TableInfo.name, $TableInfo);
            $TypeDb.TableSysIdMap.Add($TableInfo.sys_id, $TableInfo.name);
        }
        if ($null -ne $JsonObj.super_class -and $null -ne $JsonObj.super_class.value) {
            $sc = $TypeDb.FetchTableInfo($JsonObj.super_class.value, [GlideType]::AsAbsoluteUri($JsonObj.super_class.link));
            if ($null -ne $sc) { $TableInfo.super_class = $sc.name }
        }
        $TypeDb.HasChanges = $true;
        return $TableInfo;
    }
    
    [void] WriteGlideRecordType([System.CodeDom.Compiler.IndentedTextWriter]$Writer, [TypeDb]$TypeDb, [string]$DefaultScope) {
        $Writer.WriteLine("/**");
        $Writer.WriteLine(" * $(($this.label | ConvertTo-Json)) GlideRecord.");
        if (-not [string]::IsNullOrWhiteSpace($this.number_prefix)) { $Writer.WriteLine(" * Auto-number Prefix: $($this.number_prefix)") }
        if ($this.is_extendable) { $Writer.WriteLine(" * Extendable: true") }
        if ($this.scope -ne $DefaultScope -and -not [string]::IsNullOrEmpty($this.scope)) {
            [SysScope]$SysScope = $null;
            if ($TypeDb.Scopes.TryGetValue($this.scope, [ref]$SysScope)) {
                $Writer.Write(' * Scope: ');
                $SysScope.WriteLine($Writer);
            } else {
                $Writer.WriteLine(" * Scope: $($this.scope)")
            }
        }
        if ($null -ne $this.js_doc -and $this.js_doc.Length -gt 0) {
            $this.js_doc | ForEach-Object {
                if ($null -ne $_) {
                ($_ -split '\r\n|\n') | ForEach-Object { $Writer.WriteLine((" * " + $_).TrimEnd()) } }
            }
        }
        $Writer.WriteLine(" */");
        if ([string]::IsNullOrEmpty($this.super_class)) {
            $Writer.WriteLine("export type $($this.name) = $Script:FieldsNamespace.$($this.name) & GlideRecord;");
        } else {
            $Writer.WriteLine("export type $($this.name) = $Script:FieldsNamespace.$($this.name) & $($this.super_class);");
        }
    }
    
    [void] WriteElementReferenceType([System.CodeDom.Compiler.IndentedTextWriter]$Writer, [TypeDb]$TypeDb) {
        $Writer.WriteLine("/**");
        $Writer.WriteLine(" * Reference element for the $(($this.label | ConvertTo-Json)) record type.");
        if ($null -ne $this.js_doc -and $this.js_doc.Length -gt 0) {
            $this.js_doc | ForEach-Object {
                if ($null -ne $_) {
                ($_ -split '\r\n|\n') | ForEach-Object { $Writer.WriteLine((" * " + $_).TrimEnd()) } }
            }
        }
        $Writer.WriteLine(" */");
        if ([string]::IsNullOrEmpty($this.super_class)) {
            $Writer.WriteLine("export type $($this.name) = Reference<$Script:FieldsNamespace.$($this.name), $Script:RecordNamespace.$($this.name)>;");
        } else {
            $Writer.WriteLine("export type $($this.name) = Reference<$Script:FieldsNamespace.$($this.name), $Script:RecordNamespace.$($this.name)> & $($this.super_class);");
        }
    }

    [void] WriteFieldsType([System.CodeDom.Compiler.IndentedTextWriter]$Writer, [TypeDb]$TypeDb, [string]$DefaultScope) {
        $Writer.WriteLine("/**");
        $Writer.WriteLine(" * $(($this.label | ConvertTo-Json)) GlideRecord fields.");
        [TableFields]$TableFields = $TypeDb.GetTableFields($this.name);
        if (-not [string]::IsNullOrEmpty($this.scope)) {
            if ($this.scope -ne $DefaultScope) {
                [SysScope]$SysScope = $null;
                if ($TypeDb.Scopes.TryGetValue($this.scope, [ref]$SysScope)) {
                    $Writer.Write(' * Scope: ');
                    $SysScope.WriteLine($Writer);
                } else {
                    $Writer.WriteLine(" * Scope: $($this.scope)")
                }
            }
            $DefaultScope = $this.scope;
        }
        if ($null -ne $this.js_doc -and $this.js_doc.Length -gt 0) {
            $this.js_doc | ForEach-Object {
                if ($null -ne $_) {
                ($_ -split '\r\n|\n') | ForEach-Object { $Writer.WriteLine((" * " + $_).TrimEnd()) } }
            }
        }
        $Writer.WriteLine(" * @see {@link $Script:RecordNamespace.$($this.name)}");
        $Writer.WriteLine(" * @see {@link $Script:ElementNamespace.$($this.name)}");
        $Writer.WriteLine(" */");
        [string[]]$SortedNames = @();
        $ExtendsBaseFields = $false;
        try {
        if ($null -ne $TableFields -and $TableFields.Fields.PSBase.Count -gt 0) {
            [FieldInfo]$tf = $null;
            if ([string]::IsNullOrEmpty($this.super_class)) {
                $ExtendsBaseFields = @($Script:BaseFields.Keys | Where-Object { $TableFields.Fields.PSBase.TryGetValue($_, [ref]$tf) -and $Script:BaseFields[$_] -eq $tf.type }).Count -eq $Script:BaseFields.Count;
                if ($ExtendsBaseFields) {
                    $SortedNames = @($TableFields.Fields.PSBase.Keys | Sort-Object | Where-Object { -not $Script:BaseFields.ContainsKey($_) });
                } else {
                    $SortedNames = @($TableFields.Fields.PSBase.Keys | Sort-Object);
                }
            } else {
                [TableFields]$sctf = $TypeDb.GetTableFields($this.super_class);
                if ($null -ne $sctf) {
                    $SortedNames = @($TableFields.Fields.PSBase.Keys | Sort-Object | Where-Object { -not ($sctf.Fields.TryGetValue($_, [ref]$tf) -and $tf.type -eq $TableFields.Fields[$_].type) });
                } else {
                    $SortedNames = @($TableFields.Fields.PSBase.Keys | Sort-Object);
                }
            }
        }
        } catch {
            Write-Error -Message $_;
        }
        if ($SortedNames.Count -gt 0) {
            if ($ExtendsBaseFields) {
                $Writer.WriteLine("export interface $($this.name) extends IBaseRecord {");
            } else {
                if ([string]::IsNullOrEmpty($this.super_class)) {
                    $Writer.WriteLine("export interface $($this.name) {");
                } else {
                    $Writer.WriteLine("export interface $($this.name) extends $($this.super_class) {");
                }
            }
            $Writer.Indent = ($Indent = $Writer.Indent) + 1;
            $TableFields.Fields[$SortedNames[0]].WriteProperty($Writer, $TypeDb, $DefaultScope);
            foreach ($n in ($SortedNames | Select-Object -Skip 1)) {
                $Writer.WriteLine();
                $TableFields.Fields[$n].WriteProperty($Writer, $TypeDb, $DefaultScope);
            }
            $Writer.Indent = $Indent;
            $Writer.WriteLine("}");
        } else {
            if ($ExtendsBaseFields) {
                $Writer.WriteLine("export interface $($this.name) extends IBaseRecord { }");
            } else {
                if ([string]::IsNullOrEmpty($this.super_class)) {
                    $Writer.WriteLine("export interface $($this.name) { }");
                } else {
                    $Writer.WriteLine("export interface $($this.name) extends $($this.super_class) { }");
                }
            }
        }
    }
    [PSObject]ForJsonExport() {
        $Result = [PSCustomObject]@{ sys_id = $this.sys_id; }
        if ($this.name -cne $this.label) { $Result | Add-Member -MemberType NoteProperty -Name 'label' -Value $this.label }
        if ($this.is_extendable) { $Result | Add-Member -MemberType NoteProperty -Name 'is_extendable' -Value $true }
        if (-not [string]::IsNullOrWhiteSpace($this.scope)) { $Result | Add-Member -MemberType NoteProperty -Name 'scope' -Value $this.scope }
        if (-not [string]::IsNullOrWhiteSpace($this.super_class)) { $Result | Add-Member -MemberType NoteProperty -Name 'super_class' -Value $this.super_class }
        if (-not [string]::IsNullOrWhiteSpace($this.number_prefix)) { $Result | Add-Member -MemberType NoteProperty -Name 'number_prefix' -Value $this.number_prefix }
        if ($null -ne $this.js_doc -and $this.js_doc.Length -gt 0) { $Result | Add-Member -MemberType NoteProperty -Name 'js_doc' -Value $this.js_doc }
        return $result;
    }
    
    <#
    [string] ToString() {
        if ($this.name -ceq $this.label) { return $this.name }
        return "$(($this.label | ConvertTo-Json)) ($($this.name))";
    }
    #>
}

class GlideType {
    [string]$sys_id;
    [string]$name;
    [string]$label;
    [string]$scalar_type;
    [Nullable[int]]$scalar_length;
    [string]$class_name;
    [string]$scope;
    [boolean]$use_original_value;
    [boolean]$visible;

    static [Uri] AsAbsoluteUri([string]$Value) {
        if ([string]::IsNullOrWhiteSpace($Value)) { return $null; }
        [Uri]$Uri = $null;
        if ([Uri]::TryCreate($Value, [System.UriKind]::Absolute, [ref]$Uri)) { return $Uri }
        return $null;
    }

    static [Nullable[int]] AsInt([object]$Value) {
        if ($null -eq $Value -or $Value -is [int]) { return $Value }
        $i = 0;
        if ($Value -is [string]) {
            if ([int]::TryParse($Value, [ref]$i)) { return $i }
        } else {
            if ([int]::TryParse('' + $Value, [ref]$i)) { return $i }
        }
        return $null;
    }

    static [bool] IsTrue([object]$Value) {
        if ($null -eq $Value) { return $false }
        if ($Value -is [bool]) { return $Value }
        try {
            if ($Value -is [string]) { return [System.Xml.XmlConvert]::ToBoolean($Value) }
            return [System.Xml.XmlConvert]::ToBoolean($Value.ToString());
        } catch { return $false }
    }

    static [string] NullIfEmpty([string]$Value) {
        if ([string]::IsNullOrWhiteSpace($Value)) { return $null }
        return $Value;
    }

    static [string] DefaultIfEmpty([string]$Value, [string]$DefaultValue) {
        if ([string]::IsNullOrWhiteSpace($Value)) { return $DefaultValue }
        return $Value;
    }

    static [GlideType] Import([TypeDb]$TypeDb, [PSObject]$JsonObj) {
        $GlideType = [GlideType]@{
            sys_id = [GlideType]::NullIfEmpty($JsonObj.sys_id);
            name = $JsonObj.name;
            label = [GlideType]::DefaultIfEmpty($JsonObj.label, $JsonObj.name);
            scalar_type = [GlideType]::DefaultIfEmpty($JsonObj.scalar_type, 'string');
            scalar_length = [GlideType]::AsInt($JsonObj.scalar_length);
            class_name = [GlideType]::NullIfEmpty($JsonObj.class_name);
            use_original_value = [GlideType]::IsTrue($JsonObj.use_original_value);
            visible = [GlideType]::IsTrue($JsonObj.visible);
        };
        if ($null -eq $GlideType.sys_id -or $null -eq $GlideType.name) { return $null }
        if ($TypeDb.Types.ContainsKey($GlideType.name)) {
            Write-Warning -Message "Duplicate type: $(($GlideType.name | ConvertTo-Json))";
            $TypeDb.Types[$GlideType.name] = $GlideType;
            if (!$TypeDb.TypeSysIdMap.ContainsKey($GlideType.sys_id)) {
                $TypeDb.TypeSysIdMap.Add($GlideType.sys_id, $GlideType.name);
            }
        } else {
            $TypeDb.Types.Add($GlideType.name, $GlideType);
            $TypeDb.TypeSysIdMap.Add($GlideType.sys_id, $GlideType.name);
        }
        if ($null -ne $JsonObj.sys_scope -and -not [string]::IsNullOrWhiteSpace($JsonObj.sys_scope.value)) {
            [SysScope]$s = $null;
            [Uri]$RemoteUrl = $null;
            if ((-not [string]::IsNullOrWhiteSpace($JsonObj.sys_scope.link)) -and [Uri]::TryCreate($JsonObj.sys_scope.link, [System.UriKind]::Absolute, [ref]$RemoteUrl)) {
                $s = $TypeDb.FetchScope($JsonObj.sys_scope.value, $RemoteUrl);
            } else {
                $s = $TypeDb.FetchScope($JsonObj.sys_scope.value, $null);
            }
            if ($null -ne $s) { $GlideType.scope = $s.value }
        }
        $TypeDb.HasChanges = $true;
        return $GlideType;
    }

    static [GlideType] Load([string]$Name, [PSObject]$JsonData) {
        $GlideType = [GlideType]@{
            sys_id = [GlideType]::NullIfEmpty($JsonData.sys_id);
            name = [GlideType]::NullIfEmpty($Name);
            label = [GlideType]::DefaultIfEmpty($JsonData.label, $Name);
            scalar_type = [GlideType]::DefaultIfEmpty($JsonData.scalar_type, 'string');
            scalar_length = [GlideType]::AsInt($JsonData.scalar_length);
            class_name = [GlideType]::NullIfEmpty($JsonData.class_name);
            scope = [GlideType]::NullIfEmpty($JsonData.scope);
            use_original_value = [GlideType]::IsTrue($JsonData.use_original_value);
            visible = [GlideType]::IsTrue($JsonData.visible);
        };
        if ($null -ne $GlideType.name -and $null -ne $GlideType.scalar_type -and -not [string]::IsNullOrWhiteSpace($GlideType.sys_id)) { return $GlideType }
        return $null;
    }

    [void] Write([System.IO.TextWriter]$Writer, [bool]$IncludeScalarType, [bool]$IncludeScalarLength) {
        if ($this.name -ceq $this.label) {
            if ($IncludeScalarType -and $this.name -ne $this.scalar_type) {
                if ($IncludeScalarLength -and $null -ne $this.scalar_length) {
                    $Writer.Write("$(($this.label | ConvertTo-Json)); Scalar type: $($this.scalar_type); Scalar length: $($this.scalar_length)");
                } else {
                    $Writer.Write("$(($this.label | ConvertTo-Json)); Scalar type: $($this.scalar_type)");
                }
            } else {
                if ($IncludeScalarLength -and $null -ne $this.scalar_length) {
                    $Writer.Write("$(($this.label | ConvertTo-Json)); Scalar length: $($this.scalar_length)");
                } else {
                    $Writer.Write("$(($this.label | ConvertTo-Json))");
                }
            }
        } else {
            if ($IncludeScalarType -and $this.name -ne $this.scalar_type) {
                if ($IncludeScalarLength -and $null -ne $this.scalar_length) {
                    $Writer.Write("$(($this.label | ConvertTo-Json)) ($($this.name)); Scalar type: $($this.scalar_type); Scalar length: $($this.scalar_length)");
                } else {
                    $Writer.Write("$(($this.label | ConvertTo-Json)) ($($this.name)); Scalar type: $($this.scalar_type)");
                }
            } else {
                if ($IncludeScalarLength -and $null -ne $this.scalar_length) {
                    $Writer.Write("$(($this.label | ConvertTo-Json)) ($($this.name)); Scalar length: $($this.scalar_length)");
                } else {
                    $Writer.Write("$(($this.label | ConvertTo-Json)) ($($this.name))");
                }
            }
        }
    }

    [PSObject]ForJsonExport() {
        $Result = [PSCustomObject]@{ sys_id = $this.sys_id; }
        if ($this.label -cne $this.name) { $Result | Add-Member -MemberType NoteProperty -Name 'label' -Value $this.label }
        if ($this.scalar_type -ne 'string') { $Result | Add-Member -MemberType NoteProperty -Name 'scalar_type' -Value $this.scalar_type }
        if ($null -ne $this.scalar_length) { $Result | Add-Member -MemberType NoteProperty -Name 'scalar_length' -Value $this.scalar_length }
        if (-not [string]::IsNullOrWhiteSpace($this.class_name)) { $Result | Add-Member -MemberType NoteProperty -Name 'class_name' -Value $this.class_name }
        if (-not [string]::IsNullOrWhiteSpace($this.scope)) { $Result | Add-Member -MemberType NoteProperty -Name 'scope' -Value $this.scope }
        if ($this.use_original_value) { $Result | Add-Member -MemberType NoteProperty -Name 'use_original_value' -Value $true }
        if ($this.visible) { $Result | Add-Member -MemberType NoteProperty -Name 'visible' -Value $true }
        return $result;
    }
    
    <#
    [string] ToString() {
        if ($this.name -ceq $this.label) { return $this.name }
        return "$(($this.label | ConvertTo-Json)) ($($this.name))";
    }
    #>
}

class FieldInfo {
    [string]$sys_id;
    [string]$name;
    [string]$label;
    [string]$type;
    [string]$reference;
    [string]$scope;
    [bool]$active;
    [bool]$array;
    [string]$comments;
    [string]$default_value;
    [bool]$display;
    [bool]$mandatory;
    [Nullable[int]]$max_length;
    [bool]$primary;
    [bool]$read_only;
    [bool]$unique;
    [string[]]$js_doc

    static [FieldInfo] Load([string]$Name, [PSObject]$JsonData) {
        $FieldInfo = [FieldInfo]@{
            sys_id = [GlideType]::NullIfEmpty($JsonData.sys_id);
            name = $Name;
            label = [GlideType]::DefaultIfEmpty($JsonData.label, $Name);
            type = [GlideType]::DefaultIfEmpty($JsonData.type, 'string');
            active = [GlideType]::IsTrue($JsonData.active);
            array = [GlideType]::IsTrue($JsonData.array);
            comments = [GlideType]::NullIfEmpty($JsonData.comments);
            default_value = [GlideType]::NullIfEmpty($JsonData.default_value);
            reference = [GlideType]::NullIfEmpty($JsonData.reference);
            scope = [GlideType]::NullIfEmpty($JsonData.scope);
            display = [GlideType]::IsTrue($JsonData.display);
            mandatory = [GlideType]::IsTrue($JsonData.mandatory);
            max_length = [GlideType]::AsInt($JsonData.max_length);
            primary = [GlideType]::IsTrue($JsonData.primary);
            read_only = [GlideType]::IsTrue($JsonData.read_only);
            unique = [GlideType]::IsTrue($JsonData.unique);
        }
        if ($null -ne $JsonData.js_doc) { $FieldInfo.js_doc = $JsonData.js_doc }
        if ($null -ne $FieldInfo.name -and -not [string]::IsNullOrWhiteSpace($FieldInfo.sys_id)) { return $FieldInfo }
        return $null;
    }
    
    static [void] Import([TypeDb]$TypeDb, [TableInfo]$TableInfo, [TableFields]$TableFields, $JsonObj) {
        if ($null -eq $JsonObj -or [string]::IsNullOrWhiteSpace($JsonObj.sys_id) -or [string]::IsNullOrWhiteSpace($JsonObj.element)) { return }
        $FieldInfo = [FieldInfo]@{
            sys_id = $JsonObj.sys_id;
            name = $JsonObj.element;
            label = [GlideType]::DefaultIfEmpty($JsonObj.column_label, $JsonObj.name);
            active = [GlideType]::IsTrue($JsonObj.active);
            array = [GlideType]::IsTrue($JsonObj.array);
            comments = [GlideType]::NullIfEmpty($JsonObj.comments);
            default_value = [GlideType]::NullIfEmpty($JsonObj.default_value);
            display = [GlideType]::IsTrue($JsonObj.display);
            mandatory = [GlideType]::IsTrue($JsonObj.mandatory);
            max_length = [GlideType]::AsInt($JsonObj.max_length);
            primary = [GlideType]::IsTrue($JsonObj.primary);
            read_only = [GlideType]::IsTrue($JsonObj.read_only);
            unique = [GlideType]::IsTrue($JsonObj.unique);
        };
        $TableFields.FieldIdMap.Add($FieldInfo.sys_id, $FieldInfo.name);
        $TableFields.Fields.PSBase.Add($FieldInfo.name, $FieldInfo);
        if ($null -ne $JsonObj.internal_type -and $null -ne $JsonObj.internal_type.value) {
            $TypeInfo = $TypeDb.FetchType($JsonObj.internal_type.value, [GlideType]::AsAbsoluteUri($JsonObj.internal_type.link));
            if ($null -eq $TypeInfo) { $FieldInfo.type = $JsonObj.internal_type.value } else { $FieldInfo.type = $TypeInfo.name }
        } else {
            $FieldInfo.type = 'string';
        }
        if ($null -ne $JsonObj.reference -and $null -ne $JsonObj.reference.value) {
            $TableInfo = $TypeDb.FetchTableInfo($JsonObj.reference.value, [GlideType]::AsAbsoluteUri($JsonObj.reference.link));
            if ($null -ne $TableInfo) { $FieldInfo.reference = $TableInfo.name }
        }
        if ($null -ne $JsonObj.sys_scope -and $null -ne $JsonObj.sys_scope.value) {
            $ScopeInfo = $TypeDb.FetchScope($JsonObj.sys_scope.value, [GlideType]::AsAbsoluteUri($JsonObj.sys_scope.link));
            if ($null -ne $ScopeInfo) { $FieldInfo.scope = $ScopeInfo.value }
        }
    }
    
    [void] WriteProperty([System.CodeDom.Compiler.IndentedTextWriter]$Writer, [TypeDb]$TypeDb, [string]$DefaultScope) {
        $Writer.WriteLine("/**");
        $Writer.WriteLine(" * $(($this.label | ConvertTo-Json)) column element.");
        [GlideType]$GlideType = $null;
        $jsType = 'GlideElement';
        $scalarType = 'string';
        if ($Script:IsScoped) {
            switch ($this.type) {
                "journal" {
                    $jsType = 'JournalGlideElement';
                    break;
                }
                "glide_date_time" {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) { $scalarType = $GlideType.scalar_type }
                    $jsType = 'GlideDateTimeElement';
                    break;
                }
                { $_ -eq "glide_list" -or $_ -eq "glide_action_list" -or $_ -eq "user_input" -or $_ -eq "journal_input" -or $_ -eq "journal_list" } {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'string'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    $jsType = 'JournalGlideElement';
                    break;
                }
                { $_ -eq "glide_date" -or $_ -eq "glide_time" -or $_ -eq "timer" -or $_ -eq "glide_duration" -or $_ -eq "glide_utc_time" -or $_ -eq "due_date" -or $_ -eq "glide_precise_time" -or $_ -eq "calendar_date_time" } {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'datetime'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    $jsType = 'GlideDateTimeElement';
                    break;
                }
                "reference" {
                    $jsType = 'GlideElementReference';
                    break;
                }
                { $_ -eq "currency2" -or $_ -eq "domain_id" -or $_ -eq "document_id" -or $_ -eq "source_id" } {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'GUID'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    $jsType = 'GlideElementReference';
                    break;
                }
                "string" { break; }
                default {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'string'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    break;
                }
            }
        } else {
            switch ($this.type) {
                "boolean" {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) { $scalarType = $GlideType.scalar_type }
                    $jsType = 'GlideElementBoolean';
                    break;
                }
                { $_ -eq "integer" -or $_ -eq "decimal" -or $_ -eq "float" -or $_ -eq "percent_complete" -or $_ -eq "order_index" -or $_ -eq "longint" } {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'integer'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    $jsType = 'GlideElementNumeric';
                    break;
                }
                "sys_class_name" {
                    $jsType = 'GlideElementSysClassName';
                    break;
                }
                "document_id" {
                    $jsType = 'GlideElementDocumentId';
                    break;
                }
                "domain_id" {
                    $jsType = 'GlideElementDomainId';
                    break;
                }
                "related_tags" {
                    $jsType = 'GlideElementRelatedTags';
                    break;
                }
                "translated_field" {
                    $jsType = 'GlideElementTranslatedField';
                    break;
                }
                "documentation_field" {
                    $jsType = 'GlideElementDocumentation';
                    break;
                }
                "script" {
                    $jsType = 'GlideElementScript';
                    break;
                }
                { $_ -eq "script_plain" -or $_ -eq "xml" } {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'string'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    $jsType = 'GlideElementScript';
                    break;
                }
                "conditions" {
                    $jsType = 'GlideElementConditions';
                    break;
                }
                "variables" {
                    $jsType = 'GlideElementVariables';
                    break;
                }
                "password" {
                    $jsType = 'GlideElementPassword';
                    break;
                }
                "user_image" {
                    $jsType = 'GlideElementUserImage';
                    break;
                }
                "translated_text" {
                    $jsType = 'GlideElementTranslatedText';
                    break;
                }
                "counter" {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) { $scalarType = $GlideType.scalar_type }
                    $jsType = 'GlideElementCounter';
                    break;
                }
                "currency" {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) { $scalarType = $GlideType.scalar_type }
                    $jsType = 'GlideElementCurrency';
                    break;
                }
                "price" {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) { $scalarType = $GlideType.scalar_type }
                    $jsType = 'GlideElementPrice';
                    break;
                }
                "short_field_name" {
                    $jsType = 'GlideElementShortFieldName';
                    break;
                }
                "audio" {
                    $jsType = 'GlideElementAudio';
                    break;
                }
                "replication_payload" {
                    $jsType = 'GlideElementReplicationPayload';
                    break;
                }
                "breakdown_element" {
                    $jsType = 'GlideElementBreakdownElement';
                    break;
                }
                "compressed" {
                    $jsType = 'GlideElementCompressed';
                    break;
                }
                "translated_html" {
                    $jsType = 'GlideElementTranslatedHTML';
                    break;
                }
                "url" {
                    $jsType = 'GlideElementURL';
                    break;
                }
                "template_value" {
                    $jsType = 'GlideElementWorkflowConditions';
                    break;
                }
                "short_table_name" {
                    $jsType = 'GlideElementShortTableName';
                    break;
                }
                "data_object" {
                    $jsType = 'GlideElementDataObject';
                    break;
                }
                "string_full_utf8" {
                    $jsType = 'GlideElementFullUTF8';
                    break;
                }
                "icon" {
                    $jsType = 'GlideElementIcon';
                    break;
                }
                "glide_var" {
                    $jsType = 'GlideElementGlideVar';
                    break;
                }
                "internal_type" {
                    $jsType = 'GlideElementInternalType';
                    break;
                }
                "simple_name_values" {
                    $jsType = 'GlideElementSimpleNameValue';
                    break;
                }
                "name_values" {
                    $jsType = 'GlideElementNameValue';
                    break;
                }
                "source_name" {
                    $jsType = 'GlideElementSourceName';
                    break;
                }
                "source_table" {
                    $jsType = 'GlideElementSourceTable';
                    break;
                }
                "password2" {
                    $jsType = 'GlideElementPassword2';
                    break;
                }
                "reference" {
                    $jsType = 'GlideElementReference';
                    break;
                }
                "wiki_text" {
                    $jsType = 'GlideElementWikiText';
                    break;
                }
                "wiki_text" {
                    $jsType = 'GlideElementWikiText';
                    break;
                }
                "workflow" {
                    $jsType = 'GlideElementWorkflow';
                    break;
                }
                { $_ -eq "glide_date_time" -or $_ -eq "glide_date" -or $_ -eq "glide_time" -or $_ -eq "timer" -or $_ -eq "glide_duration" -or $_ -eq "glide_utc_time" -or $_ -eq "due_date" -or $_ -eq "glide_precise_time" -or
                    $_ -eq "calendar_date_time" -or $_ -eq "user_input" -or $_ -eq "journal_input" -or $_ -eq "journal_list" -or $_ -eq "html" -or $_ -eq "glide_list" -or $_ -eq "journal" -or $_ -eq "glide_action_list" -or
                    $_ -eq "date" -or $_ -eq "day_of_week" -or $_ -eq "month_of_year" -or $_ -eq "week_of_month" } {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'datetime'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    $jsType = 'GlideElementGlideObject';
                    break;
                }
                "reference" {
                    $jsType = 'GlideElementReference';
                    break;
                }
                { $_ -eq "phone_number" -or $_ -eq "caller_phone_number" -or $_ -eq "phone_number_e164" } {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'string'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    $jsType = 'GlideElementPhoneNumber';
                    break;
                }
                "ip_addr" {
                    $jsType = "GlideElementIPAddress";
                    break;
                }
                "string" { break; }
                { $_ -eq "choice" -or $_ -eq "field_name" -or $_ -eq "color" -or $_ -eq "user_roles" -or $_ -eq "image" -or $_ -eq "json" -or $_ -eq "char" -or $_ -eq "email" -or $_ -eq "ph_number" -or $_ -eq "multi_two_lines" -or $_ -eq "table_name" -or $_ -eq "external_names" -or $_ -eq "expression" -or $_ -eq "glyphicon" -or $_ -eq "field_list" -or $_ -eq "datetime" -or $_ -eq "slushbucket" -or $_ -eq "GUID" -or $_ -eq "domain_path" -or $_ -eq "composite_field" -or $_ -eq "radio" -or $_ -eq "script_server" -or $_ -eq "decoration" -or $_ -eq "sys_class_code" -or $_ -eq "wide_text" -or $_ -eq "version" -or $_ -eq "sys_class_path" -or $_ -eq "catalog_preview" -or $_ -eq "properties" -or $_ -eq "bootstrap_color" -or $_ -eq "css" -or $_ -eq "html_template" -or $_ -eq "color_display" -or $_ -eq "composite_name" -or $_ -eq "condtion_string" } {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'string'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    break;
                }
                default {
                    if ($TypeDb.Types.TryGetValue($this.type, [ref]$GlideType)) {
                        $scalarType = $GlideType.scalar_type;
                        $Writer.Write(' * Type: ');
                        $GlideType.Write($Writer, ($scalarType -ne 'string'), ($null -eq $this.max_length));
                        $Writer.WriteLine('.');
                    } else {
                        $Writer.WriteLine(" * Type: $($this.type).");
                    }
                    Write-Warning -Message "Unknown $_ on $($this.name)" -WarningAction Continue;
                    break;
                }
            }
        }
        if ($this.active) {
            if ($this.mandatory) {
                if ($this.unique) {
                    if ($this.array) {
                        if ($this.display) {
                            if ($this.primary) {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Unique: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Unique: true; Array: true; Display: true.");
                                }
                            } else {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Unique: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Unique: true; Array: true; Display: true.");
                                }
                            }
                        } else {
                            if ($this.primary) {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Unique: true; Array: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Unique: true; Array: true.");
                                }
                            } else {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Unique: true; Array: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Unique: true; Array: true.");
                                }
                            }
                        }
                    } else {
                        if ($this.display) {
                            if ($this.primary) {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Unique: true; Display: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Unique: true; Display: true.");
                                }
                            } else {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Unique: true; Display: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Unique: true; Display: true.");
                                }
                            }
                        } else {
                            if ($this.primary) {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Unique: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Unique: true.");
                                }
                            } else {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Unique: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Unique: true.");
                                }
                            }
                        }
                    }
                } else {
                    if ($this.array) {
                        if ($this.display) {
                            if ($this.primary) {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Array: true; Display: true.");
                                }
                            } else {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Array: true; Display: true.");
                                }
                            }
                        } else {
                            if ($this.primary) {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Array: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Array: true.");
                                }
                            } else {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Array: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Array: true.");
                                }
                            }
                        }
                    } else {
                        if ($this.display) {
                            if ($this.primary) {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Display: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Display: true.");
                                }
                            } else {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Display: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Display: true.");
                                }
                            }
                        } else {
                            if ($this.primary) {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true; Primary: true.");
                                }
                            } else {
                                if ($null -ne $this.max_length) {
                                    $Writer.WriteLine(" * Mandatory: true; Max Length: $($this.max_length).");
                                } else {
                                    $Writer.WriteLine(" * Mandatory: true.");
                                }
                            }
                        }
                    }
                }
            } else {
                if ($this.read_only) {
                    if ($this.unique) {
                        if ($this.array) {
                            if ($this.display) {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Primary: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Primary: true; Array: true; Display: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Array: true; Display: true.");
                                    }
                                }
                            } else {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Primary: true; Array: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Primary: true; Array: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Array: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Array: true.");
                                    }
                                }
                            }
                        } else {
                            if ($this.display) {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Primary: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Primary: true; Display: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Display: true.");
                                    }
                                }
                            } else {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Primary: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Primary: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Read-only: true.");
                                    }
                                }
                            }
                        }
                    } else {
                        if ($this.array) {
                            if ($this.display) {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Read-only: true; Primary: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Read-only: true; Primary: true; Array: true; Display: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Read-only: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Read-only: true; Array: true; Display: true.");
                                    }
                                }
                            } else {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Read-only: true; Primary: true; Array: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Read-only: true; Primary: true; Array: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Read-only: true; Array: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Read-only: true; Array: true.");
                                    }
                                }
                            }
                        } else {
                            if ($this.display) {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Read-only: true; Primary: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Read-only: true; Primary: true; Display: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Read-only: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Read-only: true; Display: true.");
                                    }
                                }
                            } else {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Read-only: true; Primary: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Read-only: true; Primary: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Read-only: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Read-only: true.");
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if ($this.unique) {
                        if ($this.array) {
                            if ($this.display) {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Primary: true; Unique: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Primary: true; Unique: true; Array: true; Display: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Array: true; Display: true.");
                                    }
                                }
                            } else {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Primary: true; Unique: true; Array: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Primary: true; Unique: true; Array: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Array: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Array: true.");
                                    }
                                }
                            }
                        } else {
                            if ($this.display) {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Primary: true; Unique: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Primary: true; Unique: true; Display: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true; Display: true.");
                                    }
                                }
                            } else {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Primary: true; Unique: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Primary: true; Unique: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Unique: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Unique: true.");
                                    }
                                }
                            }
                        }
                    } else {
                        if ($this.array) {
                            if ($this.display) {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Primary: true; Array: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Primary: true; Array: true; Display: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Array: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Array: true; Display: true.");
                                    }
                                }
                            } else {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Primary: true; Array: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Primary: true; Array: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Array: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Array: true.");
                                    }
                                }
                            }
                        } else {
                            if ($this.display) {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Primary: true; Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Primary: true; Display: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Display: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Display: true.");
                                    }
                                }
                            } else {
                                if ($this.primary) {
                                    if ($null -ne $this.max_length) {
                                        $Writer.WriteLine(" * Primary: true; Max Length: $($this.max_length).");
                                    } else {
                                        $Writer.WriteLine(" * Primary: true.");
                                    }
                                } else {
                                    if ($null -ne $this.max_length) { $Writer.WriteLine(" * Max Length: $($this.max_length).") }
                                }
                            }
                        }
                    }
                }
            }
        } else {
            if ($this.unique) {
                if ($this.read_only) {
                    if ($this.array) {
                        if ($null -ne $this.max_length) {
                            $Writer.WriteLine(" * Active: false; Unique: true; Read-only: true; Array: true; Max Length: $($this.max_length).");
                        } else {
                            $Writer.WriteLine(" * Active: false; Unique: true; Read-only: true; Array: true.");
                        }
                    } else {
                        if ($null -ne $this.max_length) {
                            $Writer.WriteLine(" * Active: false; Unique: true; Read-only: true; Max Length: $($this.max_length).");
                        } else {
                            $Writer.WriteLine(" * Active: false; Unique: true; Read-only: true.");
                        }
                    }
                } else {
                    if ($this.array) {
                        if ($null -ne $this.max_length) {
                            $Writer.WriteLine(" * Active: false; Unique: true; Array: true; Max Length: $($this.max_length).");
                        } else {
                            $Writer.WriteLine(" * Active: false; Unique: true; Array: true.");
                        }
                    } else {
                        if ($null -ne $this.max_length) {
                            $Writer.WriteLine(" * Active: false; Unique: true; Max Length: $($this.max_length).");
                        } else {
                            $Writer.WriteLine(" * Active: false; Unique: true.");
                        }
                    }
                }
            } else {
                if ($this.read_only) {
                    if ($this.array) {
                        if ($null -ne $this.max_length) {
                            $Writer.WriteLine(" * Active: false; Read-only: true; Array: true; Max Length: $($this.max_length).");
                        } else {
                            $Writer.WriteLine(" * Active: false; Read-only: true; Array: true.");
                        }
                    } else {
                        if ($null -ne $this.max_length) {
                            $Writer.WriteLine(" * Active: false; Read-only: true; Max Length: $($this.max_length).");
                        } else {
                            $Writer.WriteLine(" * Active: false; Read-only: true.");
                        }
                    }
                } else {
                    if ($this.array) {
                        if ($null -ne $this.max_length) {
                            $Writer.WriteLine(" * Active: false; Array: true; Max Length: $($this.max_length).");
                        } else {
                            $Writer.WriteLine(" * Active: false; Array: true.");
                        }
                    } else {
                        if ($null -ne $this.max_length) {
                            $Writer.WriteLine(" * Active: false; Max Length: $($this.max_length).");
                        } else {
                            $Writer.WriteLine(" * Active: false.");
                        }
                    }
                }
            }
        }
        if (-not [string]::IsNullOrEmpty($this.default_value)) {
            switch ($scalarType) {
                'boolean' {
                    switch ($this.default_value.Trim()) {
                        'true' {
                            $Writer.WriteLine(" * Default Value: true.");
                            break;
                        }
                        'false' {
                            $Writer.WriteLine(" * Default Value: false.");
                            break;
                        }
                        default {
                            if ($this.default_value -match '^\s*\d+(\.\d+)?\s*$') {
                                $Writer.WriteLine(" * Default Value: $($this.default_value).");
                            } else {
                                $Writer.WriteLine(" * Default Value: $(($this.default_value | ConvertTo-Json)).");
                            }
                            break;
                        }
                    }
                    break;
                }
                { $_ -eq 'decimal' -or $_ -eq 'float' -or $_ -eq 'integer' -or $_ -eq 'longint' } {
                    if ($this.default_value -match '^\s*\d+(\.\d+)?\s*$') {
                        $Writer.WriteLine(" * Default Value: $($this.default_value).");
                    } else {
                        $Writer.WriteLine(" * Default Value: $(($this.default_value | ConvertTo-Json)).");
                    }
                    break;
                }
                default {
                    $Writer.WriteLine(" * Default Value: $(($this.default_value | ConvertTo-Json)).");
                    break;
                }
            }
        }
        if ($this.scope -ne $DefaultScope -and -not [string]::IsNullOrEmpty($this.scope)) {
            [SysScope]$SysScope = $null;
            if ($TypeDb.Scopes.TryGetValue($this.scope, [ref]$SysScope)) {
                $Writer.Write(' * Scope: ');
                $SysScope.WriteLine($Writer);
            } else {
                $Writer.WriteLine(" * Scope: $($this.scope)")
            }
        }
        if (-not [string]::IsNullOrWhiteSpace($this.comments)) {
            $Writer.WriteLine(" *");
            foreach ($Line in @($this.comments.Trim() -split '\r\n|\n')) {
                $Writer.WriteLine((" *" + $Line).TrimEnd());
            }
        }
        if ($null -ne $this.js_doc -and $this.js_doc.Length -gt 0) {
            $this.js_doc | ForEach-Object {
                if ($null -ne $_) {
                ($_ -split '\r\n|\n') | ForEach-Object { $Writer.WriteLine((" * " + $_).TrimEnd()) } }
            }
        }
        $Writer.WriteLine(" */");
        if ([string]::IsNullOrEmpty($this.reference)) {
            $Writer.WriteLine("$($this.name): $jsType;");
        } else {
            $Writer.WriteLine("$($this.name): $Script:ElementNamespace.$($this.reference);");
        }
    }
    
    [PSObject]ForJsonExport() {
        $Result = [PSCustomObject]@{ sys_id = $this.sys_id; }
        if ($this.label -cne $this.name) { $Result | Add-Member -MemberType NoteProperty -Name 'label' -Value $this.label }
        if ($this.type -ne 'string') { $Result | Add-Member -MemberType NoteProperty -Name 'type' -Value $this.type }
        if (-not [string]::IsNullOrWhiteSpace($this.reference)) { $Result | Add-Member -MemberType NoteProperty -Name 'reference' -Value $this.reference }
        if (-not [string]::IsNullOrWhiteSpace($this.scope)) { $Result | Add-Member -MemberType NoteProperty -Name 'scope' -Value $this.scope }
        if ($this.active) { $Result | Add-Member -MemberType NoteProperty -Name 'active' -Value $true }
        if ($this.array) { $Result | Add-Member -MemberType NoteProperty -Name 'array' -Value $true }
        if (-not [string]::IsNullOrWhiteSpace($this.comments)) { $Result | Add-Member -MemberType NoteProperty -Name 'comments' -Value $this.comments }
        if (-not [string]::IsNullOrWhiteSpace($this.default_value)) { $Result | Add-Member -MemberType NoteProperty -Name 'default_value' -Value $this.default_value }
        if ($this.display) { $Result | Add-Member -MemberType NoteProperty -Name 'display' -Value $true }
        if ($this.mandatory) { $Result | Add-Member -MemberType NoteProperty -Name 'mandatory' -Value $true }
        if ($null -ne $this.max_length) { $Result | Add-Member -MemberType NoteProperty -Name 'max_length' -Value $this.max_length }
        if ($this.primary) { $Result | Add-Member -MemberType NoteProperty -Name 'primary' -Value $true }
        if ($this.read_only) { $Result | Add-Member -MemberType NoteProperty -Name 'read_only' -Value $true }
        if ($this.unique) { $Result | Add-Member -MemberType NoteProperty -Name 'unique' -Value $true }
        if ($null -ne $this.js_doc -and $this.js_doc.Length -gt 0) { $Result | Add-Member -MemberType NoteProperty -Name 'js_doc' -Value $this.js_doc }
        return $result;
    }
}

class TableFields {
    [System.Collections.Generic.Dictionary[string,string]]$FieldIdMap = [System.Collections.Generic.Dictionary[string,string]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
    [System.Collections.Generic.Dictionary[string,FieldInfo]]$Fields = [System.Collections.Generic.Dictionary[string,FieldInfo]]::new([System.StringComparer]::InvariantCultureIgnoreCase);

    static [TableFields] Import([TypeDb]$TypeDb, [TableInfo]$TableInfo, [PSObject[]]$JsonObjArr) {
        $TableFields = [TableFields]::new();
        foreach ($JsonObj in $JsonObjArr) {
            [FieldInfo]::Import($TypeDb, $TableInfo, $TableFields, $JsonObj);
        }
        $TypeDb.TableFields[$TableInfo.name] = $TableFields;
        $Path = $Script:SysDictionaryPath | Join-Path -ChildPath "$($TableInfo.name).json";
        Set-Content -Value (ConvertTo-Json -InputObject $TableFields.ForJsonExport()) -LiteralPath $Path -Encoding utf8 -Force -ErrorAction Stop;
        return $TableFields;
    }

    [PSObject]ForJsonExport() {
        $Result = [PSObject]::new();
        $this.Fields.Keys | ForEach-Object {
            $Result | Add-Member -MemberType NoteProperty -Name $_ -Value $this.Fields[$_].ForJsonExport();
        }
        return $result;
    }
}

class TypeDb {
    [Uri]$BaseUri;
    [string]$DbPath;
    [bool]$HasChanges = $false;
    [System.Collections.Generic.Dictionary[string,string]]$NumberRefMap = [System.Collections.Generic.Dictionary[string,string]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
    [System.Collections.Generic.Dictionary[string,string]]$TypeSysIdMap = [System.Collections.Generic.Dictionary[string,string]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
    [System.Collections.Generic.Dictionary[string,string]]$TableSysIdMap = [System.Collections.Generic.Dictionary[string,string]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
    [System.Collections.Generic.Dictionary[string,string]]$ScopeSysIdMap = [System.Collections.Generic.Dictionary[string,string]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
    [System.Collections.Generic.Dictionary[string,SysScope]]$Scopes = [System.Collections.Generic.Dictionary[string,SysScope]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
    [System.Collections.Generic.Dictionary[string,GlideType]]$Types = [System.Collections.Generic.Dictionary[string,GlideType]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
    [System.Collections.Generic.Dictionary[string,TableInfo]]$TableDefinitions = [System.Collections.Generic.Dictionary[string,TableInfo]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
    [System.Collections.Generic.Dictionary[string,TableFields]]$TableFields = [System.Collections.Generic.Dictionary[string,TableFields]]::new([System.StringComparer]::InvariantCultureIgnoreCase);

    TypeDb([Uri]$BaseUri, [string]$DbPath) {
        if ($null -eq $BaseUri -or -not $BaseUri.IsAbsoluteUri) { throw 'Invalid base URI'; }
        if ([string]::IsNullOrWhiteSpace($DbPath) -or ($DbPath | Test-Path -PathType Container) -or -not (($DbPath | Split-Path -Parent) | Test-Path -PathType Container)) { throw 'Invalid DB Path' }
        $this.BaseUri = $BaseUri;
        $this.DbPath = $DbPath;
        try {
            if ($DbPath | Test-Path) {
                [PSObject]$JsonData = (Get-Content -LiteralPath $DbPath -Encoding utf8 -Force -ErrorAction Stop) | ConvertFrom-Json;
                [PSObject]$TypesJson = $JsonData.types;
                if ($null -ne $TypesJson) {
                    ($TypesJson | Get-Member -MemberType NoteProperty) | ForEach-Object {
                        $GlideType = [GlideType]::Load($_.Name, $TypesJson.($_.Name));
                        if ($null -ne $GlideType) {
                            $this.TypeSysIdMap.Add($GlideType.sys_id, $GlideType.name);
                            $this.Types.Add($GlideType.name, $GlideType);
                        }
                    }
                }
                
                [PSObject]$ScopesJson = $JsonData.scopes;
                if ($null -ne $ScopesJson) {
                    ($ScopesJson | Get-Member -MemberType NoteProperty) | ForEach-Object {
                        $SysScope = [SysScope]::Load($_.Name, $ScopesJson.($_.Name));
                        if ($null -ne $SysScope) {
                            $this.ScopeSysIdMap.Add($SysScope.sys_id, $SysScope.value);
                            $this.Scopes.Add($SysScope.value, $SysScope);
                        }
                    }
                }
                [PSObject]$TablesJson = $JsonData.tables;
                if ($null -ne $TablesJson) {
                    ($TablesJson | Get-Member -MemberType NoteProperty) | ForEach-Object {
                        $TableInfo = [TableInfo]::Load($_.Name, $TablesJson.($_.Name));
                        if ($null -ne $TableInfo) {
                            $this.TypeSysIdMap.Add($TableInfo.sys_id, $TableInfo.name);
                            $this.TableDefinitions.Add($TableInfo.name, $TableInfo);
                        }
                    }
                }
            } else {
                $UriBuilder = [System.UriBuilder]::new($BaseUri);
                $UriBuilder.Path = '/api/now/table/sys_glide_object';
                $Uri = $UriBuilder.Uri.AbsoluteUri;
                [ProgressInfo]::Start('Initializing database', 'Getting type definitions');
                $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
                    Accept = "application/json";
                } -ErrorAction Stop;
                if ([string]::IsNullOrWhiteSpace($Response.Content)) { Write-Error -Message "Failed to get sys_glide_object content" -ErrorAction Stop }
                $Content = $Response.Content | ConvertFrom-Json -ErrorAction Stop;
                if ($null -eq $Content -or $null -eq $Content.result -or $Content.result.Count -isnot [int]) { Write-Error -Message "Failed to get sys_glide_object content" -ErrorAction Stop }
                $Content.result | ForEach-Object { [GlideType]::Import($this, $_) }
            }
        } finally {
            [ProgressInfo]::Complete();
        }
    }

    [void] SaveChanges() {
        $IsActivityInitiator = [ProgressInfo]::WriteFirst('Saving DB Changes', 'Writing to file', $this.DbPath);
        try {
            $t = @{};
            $this.Types.Keys | ForEach-Object { $t[$_] = $this.Types[$_].ForJsonExport() }
            $JsonObj = [PSCustomObject]@{ types = [PSCustomObject]$t };
            if ($this.Scopes.Count -gt 0) {
                $s = @{};
                $this.Scopes.Keys | ForEach-Object { $s[$_] = $this.Scopes[$_].ForJsonExport() }
                $JsonObj | Add-Member -MemberType NoteProperty -Name 'scopes' -Value ([PSCustomObject]$s);
            }
            if ($this.TableDefinitions.Count -gt 0) {
                $o = @{};
                $this.TableDefinitions.Keys | ForEach-Object { $o[$_] = $this.TableDefinitions[$_].ForJsonExport() }
                $JsonObj | Add-Member -MemberType NoteProperty -Name 'tables' -Value ([PSCustomObject]$o);
            }
            Set-Content -Value (ConvertTo-Json -InputObject $JsonObj) -LiteralPath $this.DbPath -Encoding utf8 -Force -ErrorAction Stop;
            $this.HasChanges = $false
        } finally {
            if ($IsActivityInitiator) { [ProgressInfo]::Complete() }
        }
    }

    [string] FetchNumberRef([string]$id, [Uri]$Url) {
        [string]$Result = $null;
        if ($this.NumberRefMap.TryGetValue($id, [ref]$Result)) { return $Result }
        if ($null -eq $Url) { return $null }
        $IsActivityInitiator = [ProgressInfo]::WriteFirst('Remote Lookup', 'Getting Number Reference');
        try {
            $Response = Invoke-WebRequest -Uri $Url -Method Get -Credential $Script:SnCredentials -Headers @{
                Accept = "application/json";
            } -ErrorAction Stop;
            if ([string]::IsNullOrWhiteSpace($Response.Content)) { return $null }
            $JsonObj = $Response.Content | ConvertFrom-Json -ErrorAction Stop;
            if ($null -eq $JsonObj -or $null -eq $JsonObj.result -or [string]::IsNullOrWhiteSpace($JsonObj.result.prefix)) { return $null }
            $this.NumberRefMap[$id] = $JsonObj.result.prefix;
            return $JsonObj.result.prefix;
        } finally {
            if ($IsActivityInitiator) { [ProgressInfo]::Complete() }
        }
    }

    [GlideType] FetchType([string]$id, [Uri]$Url) {
        [GlideType]$Result = $null;
        if ($this.Types.TryGetValue($id, [ref]$Result) -or ($this.TypeSysIdMap.TryGetValue($id, [ref]$id) -and $this.Types.TryGetValue($id, [ref]$Result))) { return $Result }
        if ($null -eq $Url) { return $null }
        $IsActivityInitiator = [ProgressInfo]::WriteFirst('Remote Lookup', 'Getting Type Information');
        try {
            $Response = Invoke-WebRequest -Uri $Url -Method Get -Credential $Script:SnCredentials -Headers @{
                Accept = "application/json";
            } -ErrorAction Stop;
            if ([string]::IsNullOrWhiteSpace($Response.Content)) { return $null }
            $JsonObj = $Response.Content | ConvertFrom-Json -ErrorAction Stop;
            if ($null -eq $JsonObj -or $null -eq $JsonObj.result) { return $null }
            return [GlideType]::Import($this, $JsonObj.result);
        } finally {
            if ($IsActivityInitiator) { [ProgressInfo]::Complete() }
        }
    }

    [SysScope] FetchScope([string]$Value, [Uri]$Url) {
        [SysScope]$Result = $null;
        if ($this.Scopes.TryGetValue($Value, [ref]$Result) -or ($this.ScopeSysIdMap.TryGetValue($Value, [ref]$Value) -and $this.Scopes.TryGetValue($Value, [ref]$Result))) { return $Result }
        if ($null -eq $Url) { return $null }
        $IsActivityInitiator = [ProgressInfo]::WriteFirst('Remote Lookup', 'Getting Scope Information');
        try {
            $Response = Invoke-WebRequest -Uri $Url -Method Get -Credential $Script:SnCredentials -Headers @{
                Accept = "application/json";
            } -ErrorAction Stop;
            if ([string]::IsNullOrWhiteSpace($Response.Content)) { return $null }
            $JsonObj = $Response.Content | ConvertFrom-Json -ErrorAction Stop;
            if ($null -eq $JsonObj -or $null -eq $JsonObj.result) { return $null }
            return [SysScope]::Import($this, $JsonObj.result);
        } finally {
            if ($IsActivityInitiator) { [ProgressInfo]::Complete() }
        }
    }

    [TableInfo] FetchTableInfo([string]$SysId, [Uri]$Url) {
        $sc = '';
        if ($this.TableSysIdMap.TryGetValue($SysId, [ref]$sc)) { return $this.TableDefinitions[$sc] }
        [TableInfo]$TableInfo = $null;
        if ($this.TableDefinitions.TryGetValue($SysId, [ref]$TableInfo)) { return $TableInfo }
        if ($null -eq $Url) { return $null }
        $IsActivityInitiator = [ProgressInfo]::WriteFirst('Remote Lookup', 'Getting Table information');
        $JsonObj = $null
        try {
            $Response = Invoke-WebRequest -Uri $Url -Method Get -Credential $Script:SnCredentials -Headers @{
                Accept = "application/json";
            } -ErrorAction Stop;
            if ([string]::IsNullOrWhiteSpace($Response.Content)) { return $null }
            $JsonObj = $Response.Content | ConvertFrom-Json -ErrorAction Continue;
        } finally {
            if ($IsActivityInitiator) { [ProgressInfo]::Complete() }
        }
        if ($null -ne $JsonObj -and $null -ne $JsonObj.result) { return [TableInfo]::Import($this, $JsonObj.result, $null) }
        return $null;
    }
    
    [TableInfo] GetTableInfo([string]$TableName) {
        [TableInfo]$TableInfo = $null;
        if ($this.TableDefinitions.TryGetValue($TableName, [ref]$TableInfo)) { return $TableInfo }
        $IsActivityInitiator = [ProgressInfo]::WriteFirst('Remote Lookup', "Looking up table $TableName");
        $JsonObj = $null
        try {
            $UriBuilder = [System.UriBuilder]::new($this.BaseUri);
            $UriBuilder.Path = '/api/now/table/sys_db_object';
            $Query = "name=$TableName";
            $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))";
            $Uri = $UriBuilder.Uri.AbsoluteUri;
            $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
                Accept = "application/json";
            } -ErrorAction Stop;
            if ([string]::IsNullOrWhiteSpace($Response.Content)) { return $null }
            $JsonObj = $Response.Content | ConvertFrom-Json -ErrorAction Continue;
        } finally {
            if ($IsActivityInitiator) { [ProgressInfo]::Complete() }
        }
        if ($null -ne $JsonObj -and $null -ne $JsonObj.result) { return [TableInfo]::Import($this, $JsonObj.result, $TableName) }
        return $null;
    }

    [TableFields] GetTableFields([string]$TableName) {
        [TableInfo]$TableInfo = $this.GetTableInfo($TableName);
        if ($null -eq $TableInfo) { return $null }
        [TableFields]$Result = $null;
        if ($this.TableFields.TryGetValue($TableInfo.name, [ref]$Result)) { return $Result }
        $IsActivityInitiator = $false;
        try {
            $Path = $Script:SysDictionaryPath | Join-Path -ChildPath "$($TableInfo.name).json";
            if ($Path | Test-Path) {
                $IsActivityInitiator = [ProgressInfo]::WriteFirst('Remote Lookup', "Loading columns for $TableName", $Path);
                [PSObject]$JsonData = (Get-Content -LiteralPath $Path -Encoding utf8 -Force -ErrorAction Stop) | ConvertFrom-Json;
                $Result = [TableFields]::new();
                ($JsonData | Get-Member -MemberType NoteProperty) | ForEach-Object {
                    $f = [FieldInfo]::Load($_.Name, $JsonData.($_.Name));
                    if ($null -ne $f) {
                        $Result.Fields.Add($_.Name, $f);
                        $Result.FieldIdMap.Add($f.sys_id, $_.Name);
                    }
                }
                $this.TableFields.Add($TableInfo.name, $Result);
            } else {
                $UriBuilder = [System.UriBuilder]::new($this.BaseUri);
                $UriBuilder.Path = '/api/now/table/sys_dictionary';
                $Query = "name=$TableName";
                $UriBuilder.Query = "sysparm_query=$([Uri]::EscapeDataString($Query))";
                $Uri = $UriBuilder.Uri.AbsoluteUri;
                $IsActivityInitiator = [ProgressInfo]::WriteFirst('Remote Lookup', "Looking up columns for $TableName");
                $Response = Invoke-WebRequest -Uri $Uri -Method Get -Credential $Script:SnCredentials -Headers @{
                    Accept = "application/json";
                } -ErrorAction Stop;
                if ([string]::IsNullOrWhiteSpace($Response.Content)) { return $null }
                $JsonObj = $Response.Content | ConvertFrom-Json -ErrorAction Stop;
                if ($null -ne $JsonObj -and $null -ne $JsonObj.result -and $JsonObj.result.Count -is [int]) { $Result = [TableFields]::Import($this, $TableInfo, $JsonObj.result) }
            }
        } finally {
            if ($IsActivityInitiator) { [ProgressInfo]::Complete() }
        }
        return $Result;
    }
}

$Script:TypeDb = [TypeDb]::new($BaseUri, ($PSScriptRoot | Join-Path -ChildPath 'type_db.json'));

$TableInfos = [System.Collections.ObjectModel.Collection[TableInfo]]::new();
($TableNames | Select-Object -Unique) | ForEach-Object {
    $ti = $Script:TypeDb.GetTableInfo($_);
    if ($null -ne $ti) {
        $TableInfos.Add($ti);
    } else {
        Write-Warning -Message "Table $(($_ | ConvertTo-JSON)) not found.";
    }
}
if ($Script:TypeDb.HasChanges) { $Script:TypeDb.SaveChanges() }

$StringWriter = [System.IO.StringWriter]::new();
$Writer = [System.CodeDom.Compiler.IndentedTextWriter]::new($StringWriter, '    ');
if ($TableInfos.Count -gt 0) {
    $Writer.WriteLine("declare namespace $RecordNamespace {");
    $Writer.Indent = 1;
    $TableInfos[0].WriteGlideRecordType($Writer, $Script:TypeDb, $OutputScope);
    ($TableInfos | Select-Object -Skip 1) | ForEach-Object {
        $Writer.WriteLine();
        $_.WriteGlideRecordType($Writer, $Script:TypeDb, $OutputScope);
    }
    $Writer.Indent = 0;
    $Writer.WriteLine("}");
    $Writer.WriteLine("declare namespace $ElementNamespace {");
    $Writer.Indent = 1;
    $TableInfos[0].WriteElementReferenceType($Writer, $Script:TypeDb);
    ($TableInfos | Select-Object -Skip 1) | ForEach-Object {
        $Writer.WriteLine();
        $_.WriteElementReferenceType($Writer, $Script:TypeDb);
    }
    $Writer.Indent = 0;
    $Writer.WriteLine("}");
    $Writer.WriteLine("declare namespace $FieldsNamespace {");
    $Writer.Indent = 1;
    $TableInfos[0].WriteFieldsType($Writer, $Script:TypeDb, $OutputScope);
    ($TableInfos | Select-Object -Skip 1) | ForEach-Object {
        $Writer.WriteLine();
        $_.WriteFieldsType($Writer, $Script:TypeDb, $OutputScope);
    }
    $Writer.Indent = 0;
    $Writer.WriteLine("}");
}
$Writer.Flush();
[System.Windows.Clipboard]::SetText($StringWriter.ToString());
