/// <reference path="$$rhino.d.ts" />
/// <reference path="Packages.d.ts" />
/// <reference path="GlideScriptable.d.ts" />

/**
 * "Action Conditions" (action_conditions) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementActionConditions)} GlideElementActionConditions
 */
declare type GlideElementActionConditions = GlideElement & Packages.com.glide.script.glide_elements.GlideElementActionConditions;

/**
 * "Audio" (audio) glide element.
 * @typedef {(GlideElement & Packages.com.glide.db_audio.GlideElementAudio)} GlideElementAudio
 */
declare type GlideElementAudio = GlideElement & Packages.com.glide.db_audio.GlideElementAudio;

/**
 * "True/False" (boolean) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementBoolean)} GlideElementBoolean
 */
declare type GlideElementBoolean = GlideElement & Packages.com.glide.script.glide_elements.GlideElementBoolean;

/**
 * "Breakdown Element" (breakdown_element) glide element.
* Length: 32
 * @typedef {(GlideElement & Packages.com.snc.pa.dc.GlideElementBreakdownElement)} GlideElementBreakdownElement
 */
declare type GlideElementBreakdownElement = GlideElement & Packages.com.snc.pa.dc.GlideElementBreakdownElement;

/**
 * "Compressed" (compressed) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementCompressed)} GlideElementCompressed
 * @todo: Make this a class in global
 */
declare type GlideElementCompressed = GlideElement & Packages.com.glide.script.glide_elements.GlideElementCompressed;

/**
 * "Conditions" (conditions) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementConditions)} GlideElementConditions
 * @todo: Make this a class in global
 */
declare type GlideElementConditions = GlideElement & Packages.com.glide.script.glide_elements.GlideElementConditions;

/**
 * "Counter" (counter) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementCounter)} GlideElementCounter
 * @todo: Make this a class in global
 */
declare type GlideElementCounter = GlideElement & Packages.com.glide.script.glide_elements.GlideElementCounter;

/**
 * "Currency" (currency) glide element.
 * @typedef {(GlideElement & Packages.com.glide.currency.GlideElementCurrency)} GlideElementCurrency
 * @todo: Make this a class in global
 */
declare type GlideElementCurrency = GlideElement & Packages.com.glide.currency.GlideElementCurrency;

/**
 * "FX Currency" (currency2) glide element.
 * @typedef {(GlideElement & Packages.com.glide.currency2.GlideElementCurrency2)} GlideElementCurrency2
 */
declare type GlideElementCurrency2 = {
    compareTo(anotherDouble: $$rhino.Number): $$rhino.Number;
    isInfinite(): $$rhino.Boolean;
    isNaN(): $$rhino.Boolean;
    byteValue(): Packages.java.lang.Byte;
    doubleValue(): Packages.java.lang.Double;
    floatValue(): Packages.java.lang.Float;
    intValue(): Packages.java.lang.Integer;
    longValue(): Packages.java.lang.Long;
    shortValue(): Packages.java.lang.Short;
    equals(obj: any): $$rhino.Boolean;
    hashCode(): $$rhino.Number;
    toString(): $$rhino.String;
} & GlideElement & Packages.com.glide.currency2.GlideElementCurrency2;

/**
 * "Data Array" (data_array) glide element.
 * @typedef {(GlideElement & Packages.com.snc.datastructure.GlideElementDataArray)} GlideElementDataArray
 * @todo: Make this a class in global
 */
declare type GlideElementDataArray = GlideElement & Packages.com.snc.datastructure.GlideElementDataArray;

/**
 * "Data Object" (data_object) glide element.
 * @typedef {(GlideElement & Packages.com.snc.datastructure.GlideElementDataObject)} GlideElementDataObject
 * @todo: Make this a class in global
 */
declare type GlideElementDataObject = GlideElement & Packages.com.snc.datastructure.GlideElementDataObject;

/**
 * "Data Structure" (data_structure) glide element.
 * @typedef {(GlideElement & Packages.com.snc.datastructure.GlideElementDataStructure)} GlideElementDataStructure
 * @todo: Make this a class in global
 */
declare type GlideElementDataStructure = GlideElement & Packages.com.snc.datastructure.GlideElementDataStructure;

/**
 * "Document ID" (document_id) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementDocumentId)} GlideElementDocumentId
 * @todo: Make this a class in global
 */
declare type GlideElementDocumentId = GlideElement & Packages.com.glide.script.glide_elements.GlideElementDocumentId;

/**
 * "Documentation Field" (documentation_field) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementDocumentation)} GlideElementDocumentation
 */
declare type GlideElementDocumentation = GlideElement & Packages.com.glide.script.glide_elements.GlideElementDocumentation;

/**
 * "Domain ID" (domain_id) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementDomainId)} GlideElementDomainId
 * @todo: Make this a class in global
 */
declare type GlideElementDomainId = GlideElement & Packages.com.glide.script.glide_elements.GlideElementDomainId;

/**
 * "Encrypted Text" (glide_encrypted) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementEncrypted)} GlideElementEncrypted
 * @todo: Make this a class in global
 */
declare type GlideElementEncrypted = GlideElement & Packages.com.glide.script.glide_elements.GlideElementEncrypted;

/**
 * "File Attachment" (file_attachment) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementFileAttachment)} GlideElementFileAttachment
 * @todo: Make this a class in global
 */
declare type GlideElementFileAttachment = GlideElement & Packages.com.glide.script.glide_elements.GlideElementFileAttachment;

/**
 * "String (Full UTF-8)" (string_full_utf8) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementFullUTF8)} GlideElementFullUTF8
 * @todo: Make this a class in global
 */
declare type GlideElementFullUTF8 = GlideElement & Packages.com.glide.script.glide_elements.GlideElementFullUTF8;

/**
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementGeoPoint)} GlideElementGeoPoint
 * @todo: Make this a class in global
 */
declare type GlideElementGeoPoint = GlideElement & Packages.com.glide.script.glide_elements.GlideElementGeoPoint;

/**
 * Glide object glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementGlideObject)} GlideElementGlideObject
 * @todo: Make this a class in global
 */
declare type GlideElementGlideObject = GlideElement & Packages.com.glide.script.glide_elements.GlideElementGlideObject;

/**
 * "GraphQL Schema" (graphql_schema) glide element.
 * @typedef {(GlideElement & Packages.com.glide.graphql.element.GlideElementGraphQLSchema)} GlideElementGraphQLSchema
 * @todo: Make this a class in global
 */
declare type GlideElementGraphQLSchema = GlideElement & Packages.com.glide.graphql.element.GlideElementGraphQLSchema;

/**
 * "IP Address (Validated IPV4, IPV6)" (ip_addr) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementIPAddress)} GlideElementIPAddress
 * @todo: Make this a class in global
 */
declare type GlideElementIPAddress = GlideElement & Packages.com.glide.script.glide_elements.GlideElementIPAddress;

/**
 * "Icon" (icon) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementIcon)} GlideElementIcon
 * @todo: Make this a class in global
 */
declare type GlideElementIcon = GlideElement & Packages.com.glide.script.glide_elements.GlideElementIcon;

/**
 * "Integer Date" (integer_date) glide element.
 * @typedef {(GlideElement & Packages.com.glide.glideobject.IntegerDate)} GlideIntegerDate
 * @todo: This is of type GlideElementGlideObject, not GlideIntegerDate. GlideIntegerDate is an actual type, though
 * @todo: Make this a class in global
 */
declare type GlideIntegerDate = GlideElementGlideObject & Packages.com.glide.glideobject.IntegerDate;

/**
 * "Integer Time" (integer_time) glide element.
 * @typedef {(GlideElement & Packages.com.glide.glideobject.IntegerTime)} GlideIntegerTime
 * @todo: This is of type GlideElementGlideObject, not GlideIntegerTime. GlideIntegerTime is an actual type, though
 * @todo: Make this a class in global
 */
declare type GlideIntegerTime = GlideElementGlideObject & Packages.com.glide.glideobject.IntegerTime;

/**
 * "Internal Type" (internal_type) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementInternalType)} GlideElementInternalType
 * @todo: Make this a class in global
 */
declare type GlideElementInternalType = GlideElement & Packages.com.glide.script.glide_elements.GlideElementInternalType;

/**
 * Metric (metric) glide element.
 * @typedef {(GlideElement & Packages.com.snc.clotho.script.GlideElementMetric)} GlideElementMetric
 * @todo: Make sure this is not a class in global
 */
declare type GlideElementMetric = GlideElement & Packages.com.snc.clotho.script.GlideElementMetric;

/**
 * "Name/Values" (name_values) glide element.
 * @typedef {(GlideElement & Packages.com.glide.glideobject.GlideElementNameValue)} GlideElementNameValue
 * @todo: Make this a class in global
 */
declare type GlideElementNameValue = GlideElement & Packages.com.glide.glideobject.GlideElementNameValue;

/**
 * "Integer" (integer), "Floating Point Number" (float), "Order Index" (order_index), "Percent Complete" (percent_complete), "Repeat Count" (repeat_count), or "Decimal" (decimal) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementNumeric)} GlideElementNumeric
 * @todo: Make this a class in global
 */
declare type GlideElementNumeric = GlideElement & Packages.com.glide.script.glide_elements.GlideElementNumeric;

/**
 * "Password (1 Way Encrypted)" (password) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementPassword)} GlideElementPassword
 * @todo: Make this a class in global
 */
declare type GlideElementPassword = GlideElement & Packages.com.glide.script.glide_elements.GlideElementPassword;

/**
 * "Password (2 Way Encrypted)" (password2) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementPassword2)} GlideElementPassword2
 * @todo: Make this a class in global
 */
declare type GlideElementPassword2 = GlideElement & Packages.com.glide.script.glide_elements.GlideElementPassword2;

/**
 * "Phone Number (E164)" (phone_number_e164) glide element.
 * @typedef {(GlideElement & Packages.com.glide.PhoneNumber.GlideElementPhoneNumber)} GlideElementPhoneNumber
 * @todo: Make this a class in global
 */
declare type GlideElementPhoneNumber = GlideElement & Packages.com.glide.PhoneNumber.GlideElementPhoneNumber;

/**
 * "Price" (price) glide element.
 * @typedef {(GlideElement & Packages.com.glide.currency.GlideElementPrice)} GlideElementPrice
 * @todo: Make this a class in global
 */
declare type GlideElementPrice = GlideElement & Packages.com.glide.currency.GlideElementPrice;

declare type JournalGlideElement = {
    /**
     * Returns either the most recent journal entry or all journal entries.
     * @param {number} mostRecent - If 1, returns the most recent entry. If -1, returns all journal entries.
     * @returns {string} For the most recent entry, returns a string that contains the field label, timestamp, and user display name of the journal entry.For all journal entries, returns the same information for all journal entries ever entered as a single string with each entry delimited by "\n\n".
     */
    getJournalEntry(mostRecent: number): string;

} & GlideElement;

declare type GlideDateTimeElement = {
    /**
     * Returns the number of milliseconds since January 1, 1970, 00:00:00 GMT for a duration field. Does not require the creation of a GlideDateTime object because the duration field is already a GlideDateTime object.
     * @returns {number} Number of milliseconds since January 1, 1970, 00:00:00 GMT.
     */
    dateNumericValue(): number;

    /**
     * Sets the value of a date/time element to the specified number of milliseconds since January 1, 1970 00:00:00 GMT.
     * @param {number} milliseconds - Number of milliseconds since 1/1/1970
     */
    setDateNumericValue(milliseconds: number): void;
} & GlideElementGlideObject;

/**
 * "Reference" (reference) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementReference)} GlideElementReference
 * @todo: Make this a class in global
 */
declare type GlideElementReference = {
    /**
     * Gets the table name for a reference element.
     * @return {string}
     */
    getReferenceTable(): string;
} & GlideElement & Packages.com.glide.script.glide_elements.GlideElementReference;

/**
 * "Related Tags" (related_tags) glide element.
 * @typedef {(GlideElement & Packages.com.glide.labels.GlideElementRelatedTags)} GlideElementRelatedTags
 * @todo: Make this a class in global
 */
declare type GlideElementRelatedTags = GlideElement & Packages.com.glide.labels.GlideElementRelatedTags;

/**
 * "Replication Payload" (replication_payload) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementReplicationPayload)} GlideElementReplicationPayload
 * @todo: Make this a class in global
 */
declare type GlideElementReplicationPayload = GlideElement & Packages.com.glide.script.glide_elements.GlideElementReplicationPayload;

/**
 * "Script" (script), "Script (Plain)" (script_plain), or "XML" (xml) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementScript)} GlideElementScript
 * @todo: Make this a class in global
 */
declare type GlideElementScript = GlideElement & Packages.com.glide.script.glide_elements.GlideElementScript;

/**
 * "Short Field Name" (short_field_name) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementShortFieldName)} GlideElementShortFieldName
 * @todo: Make this a class in global
 */
declare type GlideElementShortFieldName = GlideElement & Packages.com.glide.script.glide_elements.GlideElementShortFieldName;

/**
 * "Short Table Name" (short_table_name) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementShortTableName)} GlideElementShortTableName
 * @todo: Make this a class in global
 */
declare type GlideElementShortTableName = GlideElement & Packages.com.glide.script.glide_elements.GlideElementShortTableName;

/**
 * "Name-Value Pairs" (simple_name_values) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementSimpleNameValue)} GlideElementSimpleNameValue
 * @todo: Make this a class in global
 */
declare type GlideElementSimpleNameValue = GlideElement & Packages.com.glide.script.glide_elements.GlideElementSimpleNameValue;

/**
 * "Snapshot Template Value" (snapshot_template_value) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementSnapshotTemplateValue)} GlideElementSnapshotTemplateValue
 */
declare type GlideElementSnapshotTemplateValue = GlideElement & Packages.com.glide.script.glide_elements.GlideElementSnapshotTemplateValue;

/**
 * "Source ID" (source_id) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementSourceId)} GlideElementSourceId
 * @todo: Make this a class in global
 */
declare type GlideElementSourceId = GlideElement & Packages.com.glide.script.glide_elements.GlideElementSourceId;

/**
 * "Source Name" (source_name) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementSourceName)} GlideElementSourceName
 * @todo: Make this a class in global
 */
declare type GlideElementSourceName = GlideElement & Packages.com.glide.script.glide_elements.GlideElementSourceName;

/**
 * "Source Table" (source_table) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementSourceTable)} GlideElementSourceTable
 * @todo: Make this a class in global
 */
declare type GlideElementSourceTable = GlideElement & Packages.com.glide.script.glide_elements.GlideElementSourceTable;

/**
 * "System Class Name" (sys_class_name) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementSysClassName)} GlideElementSysClassName
 * @todo: Make this a class in global
 */
declare type GlideElementSysClassName = GlideElement & Packages.com.glide.script.glide_elements.GlideElementSysClassName;

/**
 * "Translated Field" (translated_field) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementTranslatedField)} GlideElementTranslatedField
 * @todo: Make this a class in global
 */
declare type GlideElementTranslatedField = GlideElement & Packages.com.glide.script.glide_elements.GlideElementTranslatedField;

/**
 * "Translated HTML" (translated_html) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementTranslatedHTML)} GlideElementTranslatedHTML
 * @todo: Make this a class in global
 */
declare type GlideElementTranslatedHTML = GlideElement & Packages.com.glide.script.glide_elements.GlideElementTranslatedHTML;

/**
 * "Translated Text" (translated_text) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementTranslatedText)} GlideElementTranslatedText
 * @todo: Make this a class in global
 */
declare type GlideElementTranslatedText = GlideElement & Packages.com.glide.script.glide_elements.GlideElementTranslatedText;

/**
 * "URL" (url) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementURL)} GlideElementURL
 * @todo: Make this a class in global
 */
declare type GlideElementURL = GlideElement & Packages.com.glide.script.glide_elements.GlideElementURL;

/**
 * "Image" (user_image) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementUserImage)} GlideElementUserImage
 * @todo: Make this a class in global
 */
declare type GlideElementUserImage = GlideElement & Packages.com.glide.script.glide_elements.GlideElementUserImage;

/**
 * "Variable Conditions" (variable_conditions) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementVariableConditions)} GlideElementVariableConditions
 * @todo: Make this a class in global
 */
declare type GlideElementVariableConditions = GlideElement & Packages.com.glide.script.glide_elements.GlideElementVariableConditions;

/**
 * "Variable template value" (variable_template_value) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementVariableTemplateValue)} GlideElementVariableTemplateValue
 */
declare type GlideElementVariableTemplateValue = GlideElement & Packages.com.glide.script.glide_elements.GlideElementVariableTemplateValue;

/**
 * "Video" (video) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementVideo)} GlideElementVideo
 * @todo: Make this a class in global
 */
declare type GlideElementVideo = GlideElement & Packages.com.glide.script.glide_elements.GlideElementVideo;

/**
 * "Wiki" (wiki_text) glide element.
 * @typedef {(GlideElement & Packages.com.glide.wiki.GlideElementWikiText)} GlideElementWikiText
 */
declare type GlideElementWikiText = GlideElement & Packages.com.glide.wiki.GlideElementWikiText;

/**
 * "Workflow" (workflow) glide element.
 * @typedef {(GlideElement & Packages.com.glide.stages.GlideElementWorkflow)} GlideElementWorkflow
 * @todo: Make this a class in global
 */
declare type GlideElementWorkflow = GlideElement & Packages.com.glide.stages.GlideElementWorkflow;

/**
 * "Workflow Conditions" (workflow_conditions) or "Template Value" (template_value) glide element.
 * @typedef {(GlideElement & Packages.com.glide.script.glide_elements.GlideElementWorkflowConditions)} GlideElementWorkflowConditions
 * @todo: Make this a class in global
 */
declare type GlideElementWorkflowConditions = GlideElement & Packages.com.glide.script.glide_elements.GlideElementWorkflowConditions;

/**
 * "Variables" (variables) glide element.
 * @typedef {(GlideElement & Packages.com.glide.vars2.GlideElementVariables)} GlideElementVariables
 */
declare type GlideElementVariables = GlideElement & Packages.com.glide.vars2.GlideElementVariables;

/**
 * "Glide Var" (glide_var) glide element.
 * @typedef {(GlideElement & Packages.com.glide.vars2.GlideElementGlideVar)} GlideElementGlideVar
 */
declare type GlideElementGlideVar = GlideElement & Packages.com.glide.vars2.GlideElementGlideVar;

// TODO: Create type definition for "String": internal_type='string'; scalar_type: 'string'; (confirmed to be of type GlideElement)
// TODO: Create type definition for "Choice": internal_type: 'choice'; scalar_type: 'string'; (confirmed to be of type GlideElement)

// TODO: Create type definition for "Approval Rules": internal_type: 'approval_rules'; scalar_type: 'string'; scalar_length: 4000;
// TODO: Create type definition for "Auto Increment": internal_type: 'auto_increment'; scalar_type: 'longint';
// TODO: Create type definition for "Auto Number": internal_type: 'auto_number'; scalar_type: 'string';
// TODO: Create type definition for "Bootstrap color": internal_type: 'bootstrap_color'; scalar_type: 'string';
// TODO: Create type definition for "Catalog Preview": internal_type: 'catalog_preview'; scalar_type: 'string';
// TODO: Create type definition for "Char": internal_type: 'char'; scalar_type: 'GUID'; scalar_length: 32;
// TODO: Create type definition for "Choice": internal_type: 'choice'; scalar_type: 'string';
// TODO: Create type definition for "Collection": internal_type: 'collection'; scalar_type: 'string';
// TODO: Create type definition for "Color": internal_type: 'color'; scalar_type: 'string';
// TODO: Create type definition for "Color Display": internal_type: 'color_display'; scalar_type: 'string';
// TODO: Create type definition for "Composite Field": internal_type: 'composite_field'; scalar_type: 'string'; scalar_length: 300;
// TODO: Create type definition for "Composite Name": internal_type: 'composite_name'; scalar_type: 'string';
// TODO: Create type definition for "Condition String": internal_type: 'condition_string'; scalar_type: 'string';
// TODO: Create type definition for "CSS": internal_type: 'css'; scalar_type: 'string'; scalar_length: 4000;
// TODO: Create type definition for "Basic Date/Time": internal_type: 'datetime'; scalar_type: 'datetime';
// TODO: Create type definition for "Days of Week": internal_type: 'days_of_week'; scalar_type: 'string';
// TODO: Create type definition for "Decimal": internal_type: 'decimal'; scalar_type: 'decimal'; scalar_length: 15;
// TODO: Create type definition for "Decoration": internal_type: 'decoration'; scalar_type: 'string'; scalar_length: 255;
// TODO: Create type definition for "Domain Path": internal_type: 'domain_path'; scalar_type: 'string'; scalar_length: 255;
// TODO: Create type definition for "Email": internal_type: 'email'; scalar_type: 'string';
// TODO: Create type definition for "Email Script": internal_type: 'email_script'; scalar_type: 'string';
// TODO: Create type definition for "External Names": internal_type: 'external_names'; scalar_type: 'string';
// TODO: Create type definition for "Field List": internal_type: 'field_list'; scalar_type: 'string';
// TODO: Create type definition for "Field Name": internal_type: 'field_name'; scalar_type: 'string'; scalar_length: 80;
// TODO: Create type definition for "Floating Point Number": internal_type: 'float'; scalar_type: 'float';
// TODO: Create type definition for "Formula": internal_type: 'formula'; scalar_type: 'string';
// TODO: Create type definition for "Glyph Icon (Bootstrap)": internal_type: 'glyphicon'; scalar_type: 'string';
// TODO: Create type definition for "Sys ID (GUID)": internal_type: 'GUID'; scalar_type: 'string'; scalar_length: 32;
// TODO: Create type definition for "HTML Script": internal_type: 'html_script'; scalar_type: 'string';
// TODO: Create type definition for "HTML Template": internal_type: 'html_template'; scalar_type: 'string'; scalar_length: 65000;
// TODO: Create type definition for "Basic Image": internal_type: 'image'; scalar_type: 'string';
// TODO: Create type definition for "Index Name": internal_type: 'index_name'; scalar_type: 'string';
// TODO: Create type definition for "Integer String": internal_type: 'int'; scalar_type: 'string';
// TODO: Create type definition for "IP Address": internal_type: 'ip_address'; scalar_type: 'string';
// TODO: Create type definition for "JSON": internal_type: 'json'; scalar_type: 'string'; scalar_length: 4000;
// TODO: Create type definition for "JSON Translations": internal_type: 'json_translations'; scalar_type: 'string';
// TODO: Create type definition for "Long Integer String": internal_type: 'long'; scalar_type: 'string';
// TODO: Create type definition for "Long": internal_type: 'longint'; scalar_type: 'longint';
// TODO: Create type definition for "Metric Absolute": internal_type: 'metric_absolute'; scalar_type: 'float';
// TODO: Create type definition for "Metric Counter": internal_type: 'metric_counter'; scalar_type: 'float';
// TODO: Create type definition for "Metric Derive": internal_type: 'metric_derive'; scalar_type: 'float';
// TODO: Create type definition for "Metric Gauge": internal_type: 'metric_gauge'; scalar_type: 'float';
// TODO: Create type definition for "MID Server Configuration": internal_type: 'mid_config'; scalar_type: 'string';
// TODO: Create type definition for "Multiple Line Small Text Area": internal_type: 'multi_small'; scalar_type: 'string';
// TODO: Create type definition for "Two Line Text Area": internal_type: 'multi_two_lines'; scalar_type: 'string';
// TODO: Create type definition for "NDS Icon": internal_type: 'nds_icon'; scalar_type: 'string';
// TODO: Create type definition for "NL Task Integer 1": internal_type: 'nl_task_int1'; scalar_type: 'integer';
// TODO: Create type definition for "Order Index": internal_type: 'order_index'; scalar_type: 'integer';
// TODO: Create type definition for "Percent Complete": internal_type: 'percent_complete'; scalar_type: 'decimal'; scalar_length: 15;
// TODO: Create type definition for "Phone Number (Unused)": internal_type: 'phone_number'; scalar_type: 'string';
// TODO: Create type definition for "Phone Number": internal_type: 'ph_number'; scalar_type: 'string';
// TODO: Create type definition for "Properties": internal_type: 'properties'; scalar_type: 'string'; scalar_length: 4000;
// TODO: Create type definition for "Radio Button Choice": internal_type: 'radio'; scalar_type: 'string';
// TODO: Create type definition for "Records": internal_type: 'records'; scalar_type: 'string'; scalar_length: 1024;
// TODO: Create type definition for "Reference Name": internal_type: 'reference_name'; scalar_type: 'string';
// TODO: Create type definition for "Reminder Field Name": internal_type: 'reminder_field_name'; scalar_type: 'string';
// TODO: Create type definition for "Repeat Count": internal_type: 'repeat_count'; scalar_type: 'integer';
// TODO: Create type definition for "Repeat Type": internal_type: 'repeat_type'; scalar_type: 'string';
// TODO: Create type definition for "Script": internal_type: 'script'; scalar_type: 'string'; scalar_length: 4000;
// TODO: Create type definition for "Script (Client)": internal_type: 'script_client'; scalar_type: 'string'; scalar_length: 4000;
// TODO: Create type definition for "Script (server side)": internal_type: 'script_server'; scalar_type: 'string'; scalar_length: 4000;
// TODO: Create type definition for "Slush Bucket": internal_type: 'slushbucket'; scalar_type: 'string'; scalar_length: 4000;
// TODO: Create type definition for "System Event Name": internal_type: 'sysevent_name'; scalar_type: 'string';
// TODO: Create type definition for "System Rule Field Name": internal_type: 'sysrule_field_name'; scalar_type: 'string';
// TODO: Create type definition for "System Class path": internal_type: 'sys_class_path'; scalar_type: 'string';
// TODO: Create type definition for "Table Name": internal_type: 'table_name'; scalar_type: 'string'; scalar_length: 80;
// TODO: Create type definition for "Template Value": internal_type: 'template_value'; scalar_type: 'string'; scalar_length: 65000; class_name='Conditions'
// TODO: Create type definition for "Basic Time": internal_type: 'time'; scalar_type: 'time';
// TODO: Create type definition for "Tree Code": internal_type: 'tree_code'; scalar_type: 'string';
// TODO: Create type definition for "Tree Path": internal_type: 'tree_path'; scalar_type: 'string';
// TODO: Create type definition for "User Roles": internal_type: 'user_roles'; scalar_type: 'string'; scalar_length: 255;
// TODO: Create type definition for "Version": internal_type: 'version'; scalar_type: 'string';
// TODO: Create type definition for "Wide Text": internal_type: 'wide_text'; scalar_type: 'string';
// TODO: Create type definition for "Workflow": internal_type: 'workflow'; scalar_type: 'string'; scalar_length: 80;
// TODO: Create type definition for "XML": internal_type: 'xml'; scalar_type: 'string';