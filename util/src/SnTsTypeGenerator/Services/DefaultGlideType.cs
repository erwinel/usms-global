using System.Collections.Immutable;

namespace SnTsTypeGenerator.Services;

/// <summary>
/// Record for default glide type definition.
/// </summary>
/// <param name="JsClass"></param>
/// <param name="Label">The display name, which corresponds to the "Label" (<see cref="SnApiConstants.JSON_KEY_LABEL" />) column.</param>
/// <param name="ScalarType">The data type, which corresponds to the "Extends" (<see cref="SnApiConstants.JSON_KEY_SCALAR_TYPE" />) column.</param>
/// <param name="ScalarLength">The data length, which corresponds to the "Length" (<see cref="SnApiConstants.JSON_KEY_SCALAR_LENGTH" />) column.</param>
/// <param name="UnderlyingType">Gets the type of object returned by the getGlideObject() method, which may be derived from the "Class name" (<see cref="SnApiConstants.JSON_KEY_CLASS_NAME" />) column.</param>
/// <param name="Visible">The visibility of the type, which corresponds to the "Visible" (<see cref="SnApiConstants.JSON_KEY_VISIBLE" />) column.</param>
/// <param name="DoNotUseOriginalValue">Gets the inverse value that corresponds to the "Use original value" (<see cref="SnApiConstants.JSON_KEY_USE_ORIGINAL_VALUE" />) column.</param>
/// <param name="CaseSensitive">Indicates whether the type is case-sensitive, which corresponds to the <see cref="SnApiConstants.JSON_KEY_CASE_SENSITIVE" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="EncodeUtf8">Indicates whether the data is UTF8-encoded, which corresponds to the <see cref="SnApiConstants.JSON_KEY_ENCODE_UTF8" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="OmitSysOriginal">The value that corresponds to the <see cref="SnApiConstants.JSON_KEY_OMIT_SYS_ORIGINAL" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="EdgeEncryptionEnabled">The value that corresponds to the <see cref="SnApiConstants.JSON_KEY_EDGE_ENCRYPTION_ENABLED" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="Serializer">The value that corresponds to the <see cref="SnApiConstants.JSON_KEY_SERIALIZER" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="IsMultiText">The value that corresponds to the <see cref="SnApiConstants.JSON_KEY_IS_MULTI_TEXT" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="PdfCellType">The value that corresponds to the <see cref="SnApiConstants.JSON_KEY_PDF_CELL_TYPE" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="NoSort">The value that corresponds to the <see cref="SnApiConstants.JSON_KEY_NO_SORT" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="NoDataReplicate">The value that corresponds to the <see cref="SnApiConstants.JSON_KEY_NO_DATA_REPLICATE" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="NoAudit">The value that corresponds to the <see cref="SnApiConstants.JSON_KEY_NO_AUDIT" /> attribute from the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
/// <param name="Attributes">Additional attributes that correspond to the "Attributes" (<see cref="SnApiConstants.JSON_KEY_ATTRIBUTES" />) column.</param>
public record DefaultGlideType(string JsClass, string Label, string? ScalarType, int? ScalarLength, string? UnderlyingType, bool? Visible, bool? DoNotUseOriginalValue, bool? CaseSensitive, bool? EncodeUtf8,
    bool? OmitSysOriginal, bool? EdgeEncryptionEnabled, string? Serializer, bool? IsMultiText, string? PdfCellType, bool? NoSort, bool? NoDataReplicate, bool? NoAudit, ImmutableArray<string> Attributes);
