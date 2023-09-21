$MethodSignatureRegex = [System.Text.RegularExpressions.Regex]::new('^[\r\n\s]*(?<n>[\w+.]+)\s*(\((?<a>[^)]+)?\))?');
$ParameterTableRegex = [System.Text.RegularExpressions.Regex]::new('[\r\n][ \t]*Parameters[ \t]*[\r\n]+[ \t]*Name[ \t]+Type[ \t]+Description[ \t]*[\r\n]+');
$ParameterRowRegex = [System.Text.RegularExpressions.Regex]::new('(?<n>\S+(?: \S+)*)[ \t]+(?<t>\S+(?: \S+)*)(?:[ \t]+(?:(?<d>\S+(?: \S+)*) *)?)?[\r\n]*');
$ReturnsRegex = [System.Text.RegularExpressions.Regex]::new('[\r\n]+[ \t]*Returns[ \t]*[\r\n]+[ \t]*Type[ \t]+Description[ \t]*[\r\n]+(?<t>\S+(?: \S+)*)(?:[ \t]+(?<d>\S+(?: \S+)*)?)?');
$Text = @'
concat(Array parent, Array child)
Merge two arrays.
Parameters
Name	Type	Description
parent	Array	An array to merge
child	Array	An array to merge
Returns
Type	Description
Array	An array of elements from both input arrays. Duplicates are not removed.
'@;

$Script:TypeMap = @{
    'Array' = 'any[]';
    'String' = 'string';
};
Function Get-JsType {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Value
    )
    
    switch ($Value) {
        'Array' { return 'any[]' }
        'String' { return 'string' }
        'Void' { return 'void' }
        'Boolean' { return 'boolean' }
        'GlideRecord' { return 'GlideRecord' }
        default {
            Write-Warning -Message "Unknown type: $Value";
            return $Value;
        }
    }
}
$m = $MethodSignatureRegex.Match($Text);
if ($m.Success) {
    $MethodName = $m.Groups['n'].Value;
    $g = $Match.Groups['a'];
    $MethodArgs = @();
    $ParamTableRows = @();
    if ($g.Success) {
        $MethodArgs = @(($g.Value.Trim() -split ',\s*') | Where-Object { -not [string]::IsNullOrEmpty($_) } | ForEach-Object {
            $a = $_.Trim() -split '\s+';
            if ($a.Count -gt 2) {
                [PSCustomObject]@{
                    type = (Get-JsType -Value (($a | Select-Object -SkipLast 1) -join ' '));
                    name = $a | Select-Object -Last 1;
                }
            } else {
                if ($a.Count -eq 2) {
                    [PSCustomObject]@{
                        type = Get-JsType -Value $a[0];
                        name = $a[1];
                    }
                } else {
                    [PSCustomObject]@{
                        name = $a[0];
                    }
                }
            }
        });
    }
    $Text = $Text.Substring($m.Length);
    $ReturnsType = 'void';
    [System.Text.RegularExpressions.Match]$m2 = $null;
    $Description = '';
    if (($m = $ParameterTableRegex.Match($Text)).Success) {
        if ($m.Index -gt 0) { $Description = $Text.Substring(0, $m.Index).Trim() }
        $t = $Text.Substring($m.Index + $m.Length);
        $t | ConvertTo-Json
        $m2 = $ReturnsRegex.Match($t);
        if ($m2.Success) {
            $t = $t.Substring(0, $m2.Index);
        $t | ConvertTo-Json
        }
        $mc = $ParameterRowRegex.Matches($t);
        Write-Host -Object $mc.Count -ForegroundColor Cyan
        if ($null -ne $mc -and $mc.Count -gt 0) {
            $ParamTableRows = @(@($mc) | ForEach-Object {
                if ($_.Groups['d'].Success) {
                    [PSCustomObject]@{
                        type = Get-JsType -Value $_.Groups['t'].Value;
                        name = $_.Groups['n'].Value;
                        description = $_.Groups['d'].Value;
                    }
                } else {
                    [PSCustomObject]@{
                        type = Get-JsType -Value $_.Groups['t'].Value;
                        name = $_.Groups['n'].Value;
                    }
                }
            });
            #$ParamTableRows
        }
        $m = $m2;
    } else {
        if (($m = $ReturnsRegex.Match($Text)).Success -and $m.Index -gt 0) {
            $Description = $Text.Substring(0, $m.Index).Trim();
        }
    }
    $Returns = $null;
    if ($m.Success) {
        if ($m.Groups['d'].Success) {
            $Returns = [PSCustomObject]@{
                type = Get-JsType -Value $m.Groups['t'].Value;
                description = $m.Groups['d'].Value;
            };
        } else {
            $Returns = [PSCustomObject]@{
                type = Get-JsType -Value $m.Groups['t'].Value;
            };
        }
    } else {
        $Returns = [PSCustomObject]@{
            type = 'void';
        };
    }
    
    $StringWriter = [System.IO.StringWriter]::new();
    $StringWriter.WriteLine('/**');
    $StringWriter.WriteLine((' * ' + $Description).TrimEnd());
    ((@($MethodArgs | ForEach-Object { $_.name }) + @($ParamTableRows | ForEach-Object { $_.name })) | Select-Object -Unique) | ForEach-Object {
        $name = $_;
        $pa = $ParamTableRows | Where-Object { $_.name -eq $name } | Select-Object -First 1;
        $ma = $MethodArgs | Where-Object { $_.name -eq $name } | Select-Object -First 1;
        if ($null -eq $pa) {
            if ($null -eq $ma.type -or $ma.type -eq 'any') {
                $StringWriter.WriteLine(" * @param {*} $($ma.name) - ");
            } else {
                $StringWriter.WriteLine(" * @param {$($ma.type)} $($ma.name) - ");
            }
        } else {
            if ($null -eq $ma -or $null -eq $ma.type -or $ma.type -eq 'any' -or $ma.type -eq $pa.type) {
                if ($pa.type -eq 'any') {
                    $StringWriter.WriteLine(" * @param {*} $($pa.name) - $($pa.description)");
                } else {
                    $StringWriter.WriteLine(" * @param {$($pa.type)} $($pa.name) - $($pa.description)");
                }
            } else {
                if ($pa.type -eq 'any') {
                    $StringWriter.WriteLine(" * @param {$($ma.type)} $($pa.name) - $($pa.description)");
                } else {
                    $StringWriter.WriteLine(" * @param {($($pa.type) | $($ma.type))} $($pa.name) - $($pa.description)");
                }
            }
        }
    }
    if ($Returns.type -ne 'void') {
        if ([string]::IsNullOrWhiteSpace($Returns.description)) {
            $StringWriter.WriteLine(" * @returns {$($Returns.type)}");
        } else {
            $StringWriter.WriteLine(" * @returns {$($Returns.type)} $($Returns.description)");
        }
    }
    $StringWriter.WriteLine(' */');
    $StringWriter.Write($MethodName);
    $StringWriter.Write('(');
    $StringWriter.Write((((@($MethodArgs | ForEach-Object { $_.name }) + @($ParamTableRows | ForEach-Object { $_.name })) | Select-Object -Unique) | ForEach-Object {
        $name = $_;
        $pa = $ParamTableRows | Where-Object { $_.name -eq $name } | Select-Object -First 1;
        $ma = $MethodArgs | Where-Object { $_.name -eq $name } | Select-Object -First 1;
        if ($null -eq $pa) {
            if ($null -eq $ma.type) {
                "$($ma.name): any";
            } else {
                "$($ma.name): $($ma.type)";
            }
        } else {
            if ($null -eq $ma -or $null -eq $ma.type -or $ma.type -eq $pa.type) {
                "$($pa.name): $($pa.type)";
            } else {
                "$($pa.name): $($pa.type) | $($ma.type)";
            }
        }
    }) -join ', ');
    $StringWriter.Write('): ');
    $StringWriter.Write($Returns.type);
    $StringWriter.WriteLine(';');
    $StringWriter.ToString();
} else {
    Write-Warning -Message 'Did not detect method signature';
}
