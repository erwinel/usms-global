using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SnTsTypeGenerator.Services;

namespace SnTsTypeGenerator.Models.TableAPI
{
    public record PackageRef(string Name, string SysID, string SourceFqdn)
    {
        internal async Task<SysPackage?> ToDbEntityAsync(TypingsDbContext dbContext, Dictionary<string, string> packageIdMap, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public record ScopeRef(string Name, string SysID, string SourceFqdn)
    {
        internal async Task<SysScope> ToDbEntityAsync(TypingsDbContext? dbContext, Dictionary<string, string> scopeIdMap, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public record Scope(string Name, string Value, string? ShortDescription, string SysID, string SourceFqdn)
    {
        internal async Task<SysScope> ToDbEntityAsync(TypingsDbContext? dbContext, Dictionary<string, string> scopeIdMap, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public record TableRef(string Label, string SysID, string SourceFqdn);

    public record GlideTypeRef(string Name, string Label, string SourceFqdn);

    public record GlideTypeRecord(string Name, string Label, string SysID, string? ScalarType, int? ScalarLength, string? ClassName, bool UseOriginalValue, bool IsVisible, PackageRef? Package, ScopeRef? Scope, string SourceFqdn) : GlideTypeRef(Name, Label, SourceFqdn)
    {
        internal async Task<GlideType> ToDbEntityAsync(TypingsDbContext dbContext, Dictionary<string, string> scopeIdMap, Dictionary<string, string> packageIdMap, CancellationToken cancellationToken)
        {
            GlideType? result = await dbContext.Types.FirstOrDefaultAsync(t => t.Name == Name, cancellationToken);
            if (result is null)
            {
                result = new()
                {
                    Name = Name,
                    Label = Label,
                    ScalarType = ScalarType,
                    ScalarLength = ScalarLength,
                    ClassName = ClassName,
                    UseOriginalValue = UseOriginalValue,
                    IsVisible = IsVisible,
                    LastUpdated = DateTime.Now, SourceFqdn = SourceFqdn, SysID = SysID
                };
                if (Scope is not null)
                    result.Scope = await Scope.ToDbEntityAsync(dbContext, scopeIdMap, cancellationToken);
                if (Package is not null)
                    result.Package = await Package.ToDbEntityAsync(dbContext, packageIdMap, cancellationToken);
                await dbContext.Types.AddAsync(result, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            return result;
        }
    }

    public record Element(string Name, string Label, string SysID, TableRef? Reference, bool IsReadOnly, GlideTypeRef? Type, int? MaxLength,
        bool IsActive, bool IsUnique, bool IsPrimary, bool IsCalculated, int? SizeClass, bool IsMandatory, bool IsArray,
        string? Comments, bool IsDisplay, string? DefaultValue, PackageRef? Package, string SourceFqdn);

    public record Table(string Name, string Label, string SysID, bool IsExtendable, string? NumberPrefix, PackageRef? Package, ScopeRef? Scope, TableRef? SuperClass, string AccessibleFrom, string? ExtensionModel, string SourceFqdn) : TableRef(Label, SysID, SourceFqdn);
}