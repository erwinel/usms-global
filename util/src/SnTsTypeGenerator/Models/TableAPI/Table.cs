using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SnTsTypeGenerator.Services;

namespace SnTsTypeGenerator.Models.TableAPI
{
    public record PackageRef(string Name, string SysID, string SourceFqdn);

    public record ScopeRef(string Name, string SysID, string SourceFqdn);

    public record Scope(string Name, string Value, string? ShortDescription, string SysID, string SourceFqdn);

    public record TableRef(string Label, string SysID, string SourceFqdn);

    public record GlideTypeRef(string Name, string Label, string SourceFqdn);

    public record GlideTypeRecord(string Name, string Label, string SysID, string? ScalarType, int? ScalarLength, string? ClassName, bool UseOriginalValue, bool IsVisible, PackageRef? Package, ScopeRef? Scope, string SourceFqdn) : GlideTypeRef(Name, Label, SourceFqdn);

    public record Element(string Name, string Label, string SysID, TableRef? Reference, bool IsReadOnly, GlideTypeRef? Type, int? MaxLength,
        bool IsActive, bool IsUnique, bool IsPrimary, bool IsCalculated, int? SizeClass, bool IsMandatory, bool IsArray,
        string? Comments, bool IsDisplay, string? DefaultValue, PackageRef? Package, string SourceFqdn);

    public record Table(string Name, string Label, string SysID, bool IsExtendable, string? NumberPrefix, PackageRef? Package, ScopeRef? Scope, TableRef? SuperClass, string AccessibleFrom, string? ExtensionModel, string SourceFqdn) : TableRef(Label, SysID, SourceFqdn);
}