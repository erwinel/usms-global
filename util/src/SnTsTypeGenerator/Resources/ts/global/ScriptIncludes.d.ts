/// <reference path="./Packages.d.ts" />
/// <reference path="./$$rhino.d.ts" />
/// <reference path="./GlideTypes.d.ts" />

declare interface IAbstractAjaxProcessor {
    readonly CALLABLE_PREFIX: 'ajaxFunction_';

    // readonly prototype: AbstractAjaxProcessor;

    readonly request?: GlideServletRequest;

    readonly responseXML?: XMLDocument2;

    readonly gc?: Packages.com.glide.script.GlideController;

    initialize(request?: GlideServletRequest, responseXML?: XMLDocument2, gc?: Packages.com.glide.script.GlideController): void;

    process(): any;

    newItem(name?: string): Packages.org.w3c.dom.Element;

    /**
     * returns value of parameter as a Java String instance
     * @param {string} name - The name of the parameter
     * @return {$$rhino.String} The value of the parameter.
     * @memberof IAbstractAjaxProcessor
     */
    getParameter(name: string): $$rhino.Nilable<$$rhino.String>;

    getDocument(): XMLDocument2 | undefined;

    getRootElement(): Packages.org.w3c.dom.Element;

    /**
     * Returns value of "sysparm_name" as a Java String instance
     * @return {$$rhino.Stringg}
     * @memberof IAbstractAjaxProcessor
     */
    getName(): $$rhino.String;

    /**
     * Returns value of "sysparm_value" as a Java String instance
     * @return {$$rhino.String}
     * @memberof IAbstractAjaxProcessor
     */
    getValue(): $$rhino.String;

    /**
     * Returns value of "sysparm_type" as a Java String instance
     * @return {$$rhino.String}
     * @memberof IAbstractAjaxProcessor
     */
    getType(): $$rhino.String;

    getChars(): $$rhino.String;

    setAnswer(value: any): void;

    setError(error: any): void;
}

declare interface AbstractAjaxProcessorConstructor extends $$class.Constructor<AbstractAjaxProcessor, IAbstractAjaxProcessor & Readonly<{ prototype: AbstractAjaxProcessor }>> {
    new(): AbstractAjaxProcessor;
    (): AbstractAjaxProcessor;
}

/**
 * Base ajax processor class that other ajax processors extend
 */
declare class AbstractAjaxProcessor implements IAbstractAjaxProcessor {
    readonly CALLABLE_PREFIX: 'ajaxFunction_';

    constructor();

    readonly prototype: AbstractAjaxProcessor;

    readonly request?: GlideServletRequest;

    readonly responseXML?: XMLDocument2;

    readonly gc?: Packages.com.glide.script.GlideController;

    initialize(request?: GlideServletRequest, responseXML?: XMLDocument2, gc?: Packages.com.glide.script.GlideController): void;

    process(): any;

    newItem(name?: string): Packages.org.w3c.dom.Element;

    /**
     * returns value of parameter as a Java String instance
     * @param {string} name - The name of the parameter
     * @return {$$rhino.String} The value of the parameter.
     * @memberof AbstractAjaxProcessor
     */
    getParameter(name: string): $$rhino.Nilable<$$rhino.String>;

    getDocument(): XMLDocument2 | undefined;

    getRootElement(): Packages.org.w3c.dom.Element;

    /**
     * Returns value of "sysparm_name" as a Java String instance
     * @return {$$rhino.Stringg}
     * @memberof AbstractAjaxProcessor
     */
    getName(): $$rhino.String;

    /**
     * Returns value of "sysparm_value" as a Java String instance
     * @return {$$rhino.String}
     * @memberof AbstractAjaxProcessor
     */
    getValue(): $$rhino.String;

    /**
     * Returns value of "sysparm_type" as a Java String instance
     * @return {$$rhino.String}
     * @memberof AbstractAjaxProcessor
     */
    getType(): $$rhino.String;

    getChars(): $$rhino.String;

    setAnswer(value: any): void;

    setError(error: any): void;
}

/**
 * Utility to check if Approvers can read records they are approving.
 * g_approval_form_request is set in the approval_summarizer_default UI Macro.
 */
declare class ApproverUtils {
    canApproversRead(): boolean;
    verify(sourceTable: string, documentId: string, userId: string): boolean;
    type: "ApproverUtils";
}

/**
 * ArrayUtil API is a script include with useful functions for working with JavaScript arrays.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/ArrayUtil/concept/c_ArrayUtilAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_ArrayUtilAPI}
 */
declare class ArrayUtil {
    /**
     * Merge two arrays.
     * @template T - The element type,
     * @param {T[]} parent - An array to merge.
     * @param {T[]} child - An array to merge.
     * @return {T[]} An array of elements from both input arrays. Duplicates are not removed.
     * @memberof ArrayUtil
     */
    concat<T>(parent: T[], child: T[]): T[];

    /**
     * Searches the array for the specified element.
     * @template T - The element type,
     * @param {T[]} array
     * @param {T} element
     * @return {boolean} True if the element exists in the array; otherwise returns false.
     * @memberof ArrayUtil
     */
    contains<T>(array: T[], element: T): boolean;

    /**
     * Converts a Java object to an array.
     * @param {*} a - Object to convert.
     * @return {any[]} Array created from the object.
     * @memberof ArrayUtil
     */
    convertArray(a: any): any[];
    /**
     * Finds the differences between two or more arrays.
     * @template T - The element type.
     * @param {T[]} a - The first array.
     * @param {T[]} b - The second array.
     * @param {...T[]} c - Zero or more additional arrays.
     * @return {T[]} - Returns an array of items from array a that were not found in any other input array. Duplicates are removed from the result.
     * @memberof ArrayUtil
     */
    diff<T>(a: T[], b: T[], ...c: T[]): T[];

    /**
     * Returns an array from the specified object.
     * @param {*} obj - Object from which to create an array.
     * @return {any[]} Array created from the object.
     * @memberof ArrayUtil
     */
    ensureArray(object: any): any[];

    /**
     * Searches the array for the element.
     * @template T - The element type.
     * @param {T} array - Array to search.
     * @param {T} element - Element to search for.
     * @param {number} [startIndex] - The optional zero-based index to start searching from.
     * @return {number} Zero-based position of the element in the array, or -1 if the element is not found.
     * @memberof ArrayUtil
     */
    indexOf<T>(array: T, element: T, startIndex?: number): number;

    /**
     * Finds the elements present in all arrays.
     * @template T - The element type.
     * @param {T[]} a - The first array.
     * @param {T[]} b - The second array.
     * @param {...T[]} c - Zero or more additional arrays.
     * @return {T[]} An array of elements from array a that were found in all of the other input arrays. Duplicates are removed.
     * @memberof ArrayUtil
     */
    intersect<T>(a: T[], b: T[], ...c: T[]): T[];
    /**
     * Merge two or more arrays.
     * @template T - The element type.
     * @param {T[]} a - The first array.
     * @param {T[]} b - The second array.
     * @param {...T[]} c - Zero or more additional arrays.
     * @return {T[]} An array of items from all the input arrays. Duplicates are removed.
     * @memberof ArrayUtil
     */
    union<T>(a: T[], b: T[], ...c: T[]): T[];

    /**
     * Removes duplicate items from an array.
     * @template T - The element type.
     * @param {T[]} a - The array to check for duplicate elements.
     * @return {T[]} An array of unique items from the input array.
     * @memberof ArrayUtil
     */
    unique<T>(a: T[]): T[];
}

/**
 * Attachment utilities
 */
declare class AttachmentUtils {
    attInptStream: GlideScriptableInputStream;

    /**
     * Creates an instance of AttachmentUtils.
     * @param {string} attachmentSysId - Attachment sys_id.
     * @memberof AttachmentUtils
     */
    constructor(attachmentSysId: string);

    /**
     * Gets MD5 checksum for the attachment identified by the attachmentSysId parameter in the class initialization.
     * @return {$$rhino.String} MD5 checksum string.
     * @memberof AttachmentUtils
     */
    getMD5ChecksumFromAttachment(): $$rhino.String;

    /**
     * Gets SHA1 checksum for the attachment identified by the attachmentSysId parameter in the class initialization.
     * @return {$$rhino.String} SHA1 checksum string
     */
    getSHA1ChecksumFromAttachment(): $$rhino.String;

    /**
     * Gets SHA256 checksum for the attachment identified by the attachmentSysId parameter in the class initialization.
     * @return {$$rhino.String} SHA256 checksum string
     */
    getSHA256ChecksumFromAttachment(): $$rhino.String;

    /**
     * Gets MD5 Hex checksum for the attachment identified by the attachmentSysId parameter in the class initialization.
     * @return {$$rhino.String} MD5 Hex checksum string
     */
    getMD5HexChecksumFromAttachment(): $$rhino.String;

    /**
     * Gets SHA1 Hex checksum for the attachment identified by the attachmentSysId parameter in the class initialization.
     * @return {$$rhino.String} SHA1 Hex checksum string
     */
    getSHA1HexChecksumFromAttachment(): $$rhino.String;

    /**
     * Gets SHA256 Hex checksum for the attachment identified by the attachmentSysId parameter in the class initialization.
     * @return {$$rhino.String} SHA256 HEx checksum string
     */
    getSHA256HexChecksumFromAttachment(): $$rhino.String;

    type: "AttachmentUtils";
}

/**
 * This script does availability calculation given a start and an end date, it considers the availability as well as maintenance commitments
 * along with the schedules attached to the offering.
 */
declare class AvailabilityCalculator {
    SCHEDULES: GlideLRUCache;
    cmdb_ci: string | null;
    commitment: string | null;
    sumCount: number;
    constructor();
    calculate(start: GlideDateTime, end: GlideDateTime, type: service_availabilityType): void;
    setCommitment(id: string): void;
    setCI(cmdb_ci: string): void;
}

declare class AvailabilityConstants {
	SERVICE_OFFERING_COMMITMENT: 'service_offering_commitment';
	ALERT_GROUP: 'cmdb_ci_alert_group';
	DYNAMIC_CI_GROUP: 'cmdb_ci_query_based_service';
    EXCLUDED_APPLICATION_TABLES(): ['cmdb_ci_alert_group', 'cmdb_ci_query_based_service'];
    type: "AvailabilityConstants";
}

declare class AvailabilityRecord {
    type: service_availabilityType;
    constructor(cmdb_ci: $$rhino.String, start: GlideDateTime, end: GlideDateTime);
    post(commitment: $$rhino.String, absolute: GlideDuration, scheduled: GlideDuration, absolute_avail: $$rhino.Number, scheduled_avail: $$rhino.Number, absolute_count: $$rhino.Number,
        scheduled_count: $$rhino.Number, ast: GlideDuration, mtbf: GlideDuration, mtrs: GlideDuration, allowed: GlideDuration, met: $$rhino.Boolean): void;
    setType(t: service_availabilityType): void;
}

declare class AvailabilitySummarizer {
    cmdb_ci: string | null;
    commitment: string | null;
    start: GlideDateTime;
    constructor();
    summarize(start?: string | GlideDateTime): void;
    setCommitment(id: string): void;
    setCI(cmdb_ci: string): void;
}

declare class AvailabilityUtils {
    CONSTANTS: AvailabilityConstants;
    hasCommitments(recordId: string): boolean;
    getAvailabilityCIQuery(): string;
    getApplicationServicesQuery(): string;
    getCiFromCommitment(commitmentGR: GlideRecord): string | null;
    getCiClassFromCommitment(commitmentGR: GlideRecord): string;
    type: "AvailabilityUtils";
}

/**
 * Utility for Calendar
 */
declare class CalendarUtils {
    static readonly UTC_DATE_FORMAT = "yyyy-MM-dd";
    static readonly UTC_TIME_FORMAT = "HH:mm:ss";

    log: GSLog;

    /**
     * Creates an instance of CalendarUtils.
     * @memberof CalendarUtils
     */
    constructor();

    /**
     * Get date format from user defined format or system format if not found, but converted to DHTMLX format as per spec:
     * {@link http://docs.dhtmlx.com/scheduler/settings_format.html}
     * @return {string} DHTMLX user date format or system date format.
     * @memberof CalendarUtils
     */
    getUserDateFormat(): string;

    /**
     * Get time format from user defined format or system format if not found, but converted to DHTMLX format as per spec:
     * {@link http://docs.dhtmlx.com/scheduler/settings_format.html}
     * @return {string} DHTMLX user time format or system time format.
     * @memberof CalendarUtils
     */
    getUserTimeFormat(): string;

    type: "CalendarUtils";
}

/**
 * Choice List object
 */
declare class ChoiceList {
    constructor(tableName: string, fieldName: string);
    getValue(label: string): $$rhino.String | null;
    getLabel(value: string): $$rhino.String | null;
}

/**
 * @todo Add CIData members
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/CIData/concept/c_CIDataAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_CIDataAPI}
 */
declare class CIData {
    type: "CIData";
}

/**
 * @todo Add CIRelatedList members
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/CIRelatedList/concept/c_CIRelatedListAPI.htmll}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_CIRelatedListAPI}
 */
declare class CIRelatedList {
    type: "CIRelatedList";
}

/**
 * @todo Add CIUtils members
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/CIUtils/concept/c_CIUtilsAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_CIUtilsAPI}
 */
declare class CIUtils {
    type: "CIUtils";
}

/**
 * For a given system property which lists out supported content types, this Script Inlcude checks to make sure the associated attachment stored (passed into class as a GlideRecord) has the correct content_type.
 */
declare class ContentTypeValidator {
    defaultSupportedContentTypes: "image/svg+xml";

    constructor();

    isValidType(attachment: GlideRecord, userDefinedSupportedContentTypes?: string[]): boolean;
}

declare class CurrencyConversionTableList {
    constructor();
    process(): string[];
    type: "CurrencyConversionTableList";
}

/**
 * @todo Add DateTimeUtils members
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/DateTimeUtils/concept/c_DateTimeUtilsAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_DateTimeUtilsAPI}
 */
declare class DateTimeUtils {
}

/**
 * @todo Add DateTimeUtils members
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/DurationCalculator/concept/c_DurationCalculatorAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_DurationCalculatorAPI}
 */
declare class DurationCalculator {
    type: "DurationCalculator";
}

/**
 * Core methods to get a default list of encryptable fields, meant to be leveraged by both Edge and Column Level Encryption.
 */
declare class EncryptionCommons {
    constructor();

    /**
     * Lists all fields that can be encrypted for a given table.
     * @param {string} tableName - The name of the table.
     * @param {{ (ed: GlideElementDescriptor, fieldName: $$rhino.String, ec?: EncryptionCommons): boolean }} [additionalValidator] - Validator that returns true if the field can be encrypted.
     * @return {$$rhino.String[]} Name sof encryptable fields for the given table.
     * @memberof EncryptionCommons
     */
    getEncryptableFieldsForTable(tableName: string, additionalValidator?: { (ed: GlideElementDescriptor, fieldName: $$rhino.String, ec: EncryptionCommons): boolean }): $$rhino.String[];

    addToArrayTablesWithBooleanAttributeSetToTrue(array: $$rhino.String[], attributeName: $$rhino.String): $$rhino.String[];

    /**
     * Tests whether table is marked for auditing.
     * @param {string} tableName - The name of the table.
     * @return {boolean} True if the table is marked for auditing; otherwise, false.
     * @memberof EncryptionCommons
     */
    isTableAudited(tableName: string): boolean;

    getUsableCryptoModules(): $$rhino.String[];

    type: "EncryptionCommons";
}

/**
 * Choice list for job type field based on user session and 'glide_encryption.cle_replatforming_with_kmf'
 */
declare class EncryptionJobTypeChoices {
    constructor();
    process(): GlideChoiceList;
    type: "EncryptionJobTypeChoices";
}

declare class GlideCryptoModule {
    static get(): GlideCryptoModule;
    static getModule(cryptoModuleName: $$rhino.String): GlideCryptoModule;
    CRYPTO_MODULE: sn_kmf_ns.KMFCryptoModule | null;
    constructor(cryptoModule: sn_kmf_ns.KMFCryptoModule);
    encryptData(data: $$rhino.String): $$rhino.String;
    wrapKeyFromBytes(data: $$rhino.String): $$rhino.String;
    wrapKeyFromBytesForAlgorithm(data: $$rhino.String, algorithm: $$rhino.String): $$rhino.String;
    wrapKeyFromSysId(data: $$rhino.String): $$rhino.String;
    decryptData(data: $$rhino.String): $$rhino.String;
    unwrapKey(data: $$rhino.String): $$rhino.String;
    unwrapKeyForAlgorithm(data: $$rhino.String, algorithm: $$rhino.String): $$rhino.String;
    generateMac(data: $$rhino.String): $$rhino.String;
    verifyMac(data: $$rhino.String, expectedMac: $$rhino.String): $$rhino.Boolean;
    asymmetricEncryptData(data: $$rhino.String): $$rhino.String;
    asymmetricWrapKeyFromBytes(data: $$rhino.String): $$rhino.String;
    asymmetricWrapKeyFromBytesForAlgorithm(data: $$rhino.String, algorithm: $$rhino.String): $$rhino.String;
    asymmetricWrapKeyFromSysId(data: $$rhino.String): $$rhino.String;
    asymmetricDecryptData(data: $$rhino.String): $$rhino.String;
    asymmetricUnwrapKey(data: $$rhino.String): $$rhino.String;
    asymmetricUnwrapKeyForAlgorithm(data: $$rhino.String, algorithm: $$rhino.String): $$rhino.String;
    sign(data: $$rhino.String): $$rhino.String;
    verifySignature(data: $$rhino.String, signature: $$rhino.String): $$rhino.Boolean;
    discard(): void;
    type: "GlideCryptoModule";
}

/**
 * The GlideQuery API is an alternative to GlideRecord to perform CRUD operations on record data from server-side scripts.
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server/no-namespace/GlideQueryAPI}
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideQuery/concept/GlideQueryGlobalAPI.html}
 */
declare class GlideQuery {

    /**
     * Instantiates a GlideQuery object used to build and execute record queries.
     * @param {$$rhino.String} table - Name of table to query.
     * @memberof GlideQuery
     */
    constructor(table: $$rhino.String);

    /**
     * Aggregates a field using a specified aggregation function.
     * @param {$$rhino.String} aggregateType - The type of aggregation function to perform.
     * @param {$$rhino.String} field - Field on which to perform the operation.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     * @description Use this method to build queries that aggregate against multiple fields or use multiple aggregate functions,
     * or if you must use the {@link #groupBy()} method. If you only want to aggregate against one field with one function,
     * and you don't need to use {@link #groupBy()}, then use {@link #avg()}, {@link #min()}, {@link #max()} or {@link #count()}, instead.
     */
    aggregate(aggregateType: AggregateType, field: $$rhino.String): GlideQuery;

    /**
     * Returns the aggregate average of a given numeric field.
     * @param {$$rhino.String} field - Field on which to perform the operation.
     * @return {Optional<$$rhino.Number>} Object that contains the aggregate average of the given field.
     * @memberof GlideQuery
     * @description You can only use this method on Integer, Long, Floating Point Number, Double and Currency fields.
     */
    avg(field: $$rhino.String): Optional<$$rhino.Number>;

    /**
     * Returns the number of records that match the query.
     * @return {$$rhino.Number} Number of records that match the query.
     * @memberof GlideQuery
     */
    count(): $$rhino.Number;

    /**
     * Deletes all records in the table specified by the preceding Where clauses.
     * @memberof GlideQuery
     */
    deleteMultiple(): void;

    /**
     * Disables updating system fields, or fields with a name that starts with the sys prefix, such as sys_created_on, sys_updated_on, and sys_mod_count.
     * Only applies to the specified query.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    disableAutoSysFields(): GlideQuery;

    /**
     * Disables any business rules, flows, workflows, or audit records that would run or be created as the result of the query.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    disableWorkflow(): GlideQuery;

    /**
     * Forces a database update even when no record changes are made. For example, you can use this method to force a business rule to execute.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    forceUpdate(): GlideQuery;

    /**
     * Returns a single record from the query.
     * @param {$$rhino.String} key - Sys_id of the record to return.
     * @param {$$rhino.String[]} [selectedFields] - Optional additional fields to return in the result.
     * The system always returns the sys_id.
     * @return {Optional<{ [key: string]: $$rhino.String; }>} Object used to interact with a single record.
     * @memberof GlideQuery
     */
    get(key: $$rhino.String, selectedFields?: $$rhino.String[]): Optional<{ [key: string]: $$rhino.String; }>;

    /**
     * Returns an Optional object containing a single record based on a set of name-value pairs to query by.
     * Assumes the '=' operator for each name-value pair.
     * @param {{ [key: string]: any; }} keyValues - Object where the keys are the name of the fields, and the values are the values to query for.
     * @param {$$rhino.String[]} [selectedFields] - Optional additional fields to return in the result.
     * The system always returns the sys_id.
     * @return {Optional<{ [key: string]: $$rhino.String; }>} Object used to interact with a single record.
     * @memberof GlideQuery
     */
    getBy(keyValues: { [key: string]: any; }, selectedFields?: $$rhino.String[]): Optional<{ [key: string]: $$rhino.String; }>;

    /**
     * Groups the query results by a designated field or fields.
     * @param {($$rhino.String | $$rhino.String[])} fields - Field or fields to group the results by.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     * @description You must use this method with the {@link #aggregate} method.
     */
    groupBy(fields: $$rhino.String | $$rhino.String[]): GlideQuery;

    /**
     * Filters aggregate groups so that you can display only groups of results that match a specified condition.
     * @param {AggregateType} aggregateType - The type of aggregation function to perform.
     * @param {$$rhino.String} field - Field on which to perform the operation.
     * @param {NumberQueryOperator} operator - Numeric operator to use in the operation.
     * @param {$$rhino.Number} value - Number value to use in the operation.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    having(aggregateType: AggregateType, field: $$rhino.String, operator: NumberQueryOperator, value: $$rhino.Number): GlideQuery;

    /**
     * Inserts a record and returns an Optional object containing the record.
     * @param {{ [key: string]: any; }} keyValues - Object containing name-value pairs to insert into the record. Unspecified fields will be null.
     * @param {$$rhino.String[]} [selectedFields] - Optional additional fields to return in the result.
     * The system always returns the sys_id.
     * @return {Optional<{ [key: string]: $$rhino.String; }>} Object used to interact with a single record.
     * @memberof GlideQuery
     */
    insert(keyValues: { [key: string]: any; }, selectedFields?: $$rhino.String[]): Optional<{ [key: string]: $$rhino.String; }>;

    /**
     * Updates an existing record, or inserts a new record if one does not already exist.
     * @param {{ [key: string]: any; }} changes - Object containing name-value pairs to update or insert into the record.
     * @param {$$rhino.String[]} [selectedFields] - Optional additional fields to return in the result.
     * The system always returns the sys_id.
     * @return {Optional<{ [key: string]: $$rhino.String; }>} Object used to interact with a single record.
     * @memberof GlideQuery
     */
    insertOrUpdate(changes: { [key: string]: any; }, selectedFields?: $$rhino.String[]): Optional<{ [key: string]: $$rhino.String; }>;

    /**
     * Limits the number of records returned in a query.
     * @param {$$rhino.Number} limit - Number of records to return.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    limit(limit: $$rhino.Number): GlideQuery;

    /**
     * Returns the aggregate maximum of a given field.
     * @param {$$rhino.String} field - Field on which to perform the operation.
     * @return {Optional<$$rhino.String>} Object used to interact with a single record.
     * @memberof GlideQuery
     */
    max(field: $$rhino.String): Optional<$$rhino.String>;

    /**
     * Returns the aggregate minimum of a given field.
     * @param {$$rhino.String} field - Field on which to perform the operation.
     * @return {Optional<$$rhino.String>} Object used to interact with a single record.
     * @memberof GlideQuery
     */
    min(field: $$rhino.String): Optional<$$rhino.String>;

    /**
     * Orders the returned result in ascending order by a given field.
     * @param {$$rhino.String} fields - Comma-delimited fields to order the result by in ascending order.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    orderBy(fields: $$rhino.String): GlideQuery;

    /**
     * Orders the returned result in descending order by a given field.
     * @param {$$rhino.String} fieldOrAggregate - If the query does not use the aggregate() method, pass the field to order the results by.
     * If the query uses the {@link #aggregate} method, pass the type of aggregation function ({@link AggregateType}) to perform.
     * @param {$$rhino.String} field - Field on which to perform the operation.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    orderByDesc(fieldOrAggregate: $$rhino.String, field: $$rhino.String): GlideQuery;

    /**
     * Adds an OR clause to a query that returns values based on a given condition.
     * @param {($$rhino.String | GlideQuery)} fieldOrQuery - Field or another GlideQuery object used in the where clause.
     * If passing a field, you can dot-walk to a desired value.
     * @param {$$rhino.Nilable<QueryOperator>} operator - Optional operator used in the where clause.
     * If you do not pass an argument, the system uses the = operator.
     * @param {*} value - Value used in the where clause.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    orWhere(fieldOrQuery: $$rhino.String | GlideQuery, operator: $$rhino.Nilable<QueryOperator>, value: any): GlideQuery;

    /**
     * Adds an OR clause that returns records that do not contain a null value in a given field.
     * @param {$$rhino.String} field - Field on which to perform the operation.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    orWhereNotNull(field: $$rhino.String): GlideQuery;

    /**
     * Adds an OR clause to a query that returns records that contain a null value in a given field.
     * @param {$$rhino.String} field - Field on which to perform the operation.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    orWhereNull(field: $$rhino.String): GlideQuery;

    /**
     * Returns the results of the query as a Stream object containing the specified fields.
     * @param {($$rhino.String | $$rhino.String[])} fields - Fields to display in the result.
     * You can provide any number of fields as arguments, dot-walk to a desired value, or use a flag.
     * @return {Stream<{ [key: string]: $$rhino.String; }>} Object used to interact with a stream of items such as records.
     * @memberof GlideQuery
     * @description You can append a flag to a field name to return the field's metadata instead of the field's value.
     * For example, using the field name company$DISPLAY returns the display value of a company field. Possible flags include:
     * DISPLAY=Returns the display value of a field;
     * CURRENCY_CODE=Returns the currency code of a currency field;
     * CURRENCY_DISPLAY=Returns the currency display value of a currency field;
     * CURRENCY_STRING=Returns the currency string of a currency field.
     */
    select(fields: $$rhino.String | $$rhino.String[]): Stream<{ [key: string]: $$rhino.String; }>;

    /**
     * Returns the result of the query as an Optional object containing specified fields.
     * @param {$$rhino.String} [fields] - Fields to display in the result.
     * You can provide any number of fields as arguments, dot-walk to a desired value, or use a flag.
     * @return {Optional<{ [key: string]: $$rhino.String; }>} Object used to interact with a single record.
     * @memberof GlideQuery
     * @description You can append a flag to a field name to return the field's metadata instead of the field's value.
     * For example, using the field name company$DISPLAY returns the display value of a company field. Possible flags include:
     * DISPLAY=Returns the display value of a field;
     * CURRENCY_CODE=Returns the currency code of a currency field;
     * CURRENCY_DISPLAY=Returns the currency display value of a currency field;
     * CURRENCY_STRING=Returns the currency string of a currency field.
     */
    selectOne(fields?: $$rhino.String): Optional<{ [key: string]: $$rhino.String; }>;

    /**
     * Returns the aggregate sum of a given numeric field.
     * @param {$$rhino.String} field - Field on which to perform the operation.
     * @return {Optional<$$rhino.Number>} Object used to interact with a single record.
     * @memberof GlideQuery
     * @description You can only use this method on Integer, Long, Floating Point Number, Double and Currency fields.
     */
    sum(field: $$rhino.String): Optional<$$rhino.Number>;

    /**
     * Returns a GlideRecord or GlideAggregate object that represents the current query.
     * @return {(GlideRecord | GlideAggregate)} GlideRecord or GlideAggregate object that contains the query.
     * @memberof GlideQuery
     * @description After transforming the query, use the query() method in the GlideRecord or GlideAggregate classes to query the database.
     */
    toGlideRecord(): GlideRecord | GlideAggregate;

    /**
     * Updates an existing record that matches the defined conditions.
     * @param {{ [key: string]: any; }} changes - Object containing name-value pairs to update in the record. Names must match fields in the table.
     * @param {$$rhino.String[]} [selectedFields] - Optional additional fields to return in the result.
     * The system always returns the sys_id.
     * @return {Optional<{ [key: string]: $$rhino.String; }>} Object used to interact with a single record.
     * @memberof GlideQuery
     */
    update(changes: { [key: string]: any; }, selectedFields?: $$rhino.String[]): Optional<{ [key: string]: $$rhino.String; }>;

    /**
     * Updates all existing records that match the defined conditions. Returns the number of records updated.
     * @param {{ [key: string]: any; }} changes - Object containing name-value pairs to update in the record. Names must match fields in the table.
     * @return {{ rowCount: $$rhino.Number; }} Object containing the number of records that were updated.
     * @memberof GlideQuery
     */
    updateMultiple(changes: { [key: string]: any; }): { rowCount: $$rhino.Number; };

    /**
     * Adds a Where clause to the query that returns values based on a given condition.
     * @param {($$rhino.String | GlideQuery)} fieldOrQuery - Field or another GlideQuery object used in the where clause.
     * @param {$$rhino.Nilable<$$rhino.String>} operator - Optional operator used in the where clause.
     * If you do not pass an argument, the system uses the = operator.
     * @param {*} value - Value used in the where clause.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     * @description Do not precede this method with the {@link #orWhere}, {@link #orWhereNull}, or {@link #orWhereNotNull} methods.
     */
    where(fieldOrQuery: $$rhino.String | GlideQuery, operator: $$rhino.Nilable<$$rhino.String>, value: any): GlideQuery;

    /**
     * Returns records that do not contain a null value in a given field.
     * @param {$$rhino.String} field - Field used in the query.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     * @description Do not precede this method with the {@link #orWhere}, {@link #orWhereNull}, or {@link #orWhereNotNull} methods.
     */
    whereNotNull(field: $$rhino.String): GlideQuery;

    /**
     * Returns records that do not contain a null value in a given field.
     * @param {$$rhino.String} field - Field used in the query.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     * @description Do not precede this method with the {@link #orWhere}, {@link #orWhereNull}, or {@link #orWhereNotNull} methods.
     */
    whereNull(field: $$rhino.String): GlideQuery;

    /**
     * Executes the query using the GlideRecordSecure API to securely query the database while honoring ACLs.
     * @return {GlideQuery} The query object being built.
     * @memberof GlideQuery
     */
    withAcls(): GlideQuery;

    type: "GlideQuery";
}

/**
 * @todo Document global.GlideQueryActions
 * @todo Add members for global.GlideQueryActions with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=89cffabe29300010fa9b76addd33871b}
 */
declare class GlideQueryActions {
    type: "GlideQueryActions";
}

/**
 * @todo Document global.GlideQueryEvaluator
 * @todo Add members for global.GlideQueryEvaluator with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=d52b3c8a08013300fa9b4300d8d67a76}
 */
declare class GlideQueryEvaluator {
    type: "GlideQueryEvaluator";
}

/**
 * Utility class for working with GlideRecords.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideRecordUtil/concept/c_GlideRecordUtilAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_GlideRecordUtilAPI}
 */
declare class GlideRecordUtil {
    /**
     * Returns the GlideRecord object for the specified configuration item (CI) using just the sys_id of the CI.
     * Use this method to quickly obtain a specific CI without knowing its associated class/table.
     * @param {string} sys_id - Sys_id of the desired CI.
     * @return {(GlideRecord | undefined)} GlideRecord object of the specified CI.
     */
    getCIGR(sys_id: string): GlideRecord | undefined;

    /**
     * Returns an array of all the fields in the specified GlideRecord.
     * Note: If there is a field name which is the same as the table name, the getFields() method does not return the value of the field.
     * @param {GlideRecord} gr - GlideRecord instance positioned to a valid record.
     * @return {string[]} Field names for the specified GlideRecord.
     */
    getFields(gr: GlideRecord): string[];

    /**
     * Returns a GlideRecord instance for the given table, positioned to the given sys_id, and of the right class (table).
     * This method is useful when you need to load a GlideRecord from a sys_id, but you don't know what the actual table is (because it may be extended from the base table).
     * This method always returns a GlideRecord of the correct type base_table: the name of the base table that the specified sys_id is in.
     * @param {string} baseTable - The name of the base table containing the sys_id.
     * @param {string} sys_id - The sys_id of the desired record.
     * @return {(GlideRecord | null)} The GlideRecord for the specified sys_id or null it not found.
     */
    getGR(baseTable: string, sys_id: string): GlideRecord | null;

    /**
     * Returns a Java ArrayList of the ancestors of the specified table name.
     * @param {string} tableName - Name of the table.
     * @return {Packages.java.util.ArrayList<Packages.java.lang.String>} List of ancestors of the specified table.
     */
    getTables(tableName: string): Packages.java.util.ArrayList<Packages.java.lang.String>;

    /**
     * Sets the fields in the specified GlideRecord with the field values contained in the specified hashmap, unless that field name is in the ignore hashmap.
     * @param {{ [key: string]: string; }} hashMap - An Object instance (being used as a hashmap), with properties named for fields and containing the fields' value.
     * @param {GlideRecord} now_GR - The GlideRecord instance to receive the field values.
     * @param {{ [key: string]: boolean; }} [ignore] - An optional hashmap of field names to ignore.
     */
    mergeToGR(hashMap: { [key: string]: string; }, now_GR: GlideRecord, ignore?: { [key: string]: boolean; }): void;
    
    /**
     * Populates the given hashmap from the given GlideRecord instance. Each field in the GlideRecord becomes a property in the hashmap.
     * @param {{ [key: string]: string }} hashMap - An object being used as a hashmap.
     * @param {GlideRecord} now_GR - A GlideRecord instance positioned to a valid record.
     * @param {{ [key: string]: boolean; }} [ignore] - An optional hashmap of file names not to populate.
     */
    populateFromGR(hashMap: { [key: string]: string }, now_GR: GlideRecord, ignore?: { [key: string]: boolean; }): void;

    type: "GlideRecordUtil";
}

/**
 * Utility api for supporting client side Grid Canvas layouts.
 * @todo Add members for global.GridCanvasLoader with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=264ff6c1d7230200a8228f0b6e6103ff}
 */
declare class GridCanvasLoader extends AbstractAjaxProcessor {
    type: "GridCanvasLoader";
}

/**
 * Make logging and debugging from Script easier by implementing levels of log output, selectable by per-caller identified sys_properties values.
 * This implements both Log4j style logging and BSD Syslog style logging. As default, the logger will use BSD style logging.
 * PLEASE CHOOSE ONE AND STICK TO IT (within a class anyhow)
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GSLogBoth/concept/GSLogBoth.html}
 * @todo Add members for global.GSLog with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=2e59987d0a0a2c3946f7118c070c03e3}
 */
declare class GSLog {
    type: "GSLog";
}

/**
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/HostnameJS/concept/c_HostnameJSAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_HostnameJSAPI}
 * @todo Add members for HostnameJS with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=8998a2500ab3015300e6ebbf8bc77f26}
 */
declare class HostnameJS {
    type: "HostnameJS";
}

/**
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_ICalUtilAPI}
 * @todo Document global.ICalUtil
 */
declare type ICalUtil = Exclude<ICalUtilSNC, "type"> & { type: "ICalUtil"; };

/**
 * Provides functions to generate iCalendar compliant events.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/ICalUtilSNC/concept/c_ICalUtilSNCAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_ICalUtilSNCAPI}
 * @todo Add members for global.ICalUtilSNC with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=0944d627c3202200b6dcdfdc64d3aebb}
 */
declare class ICalUtilSNC {

    type: "ICalUtilSNC";
}

/**
 * Incident state constants, use these constants when determining which incident state to use.
 * @todo Add members for global.IncidentState with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=3d9e146f9fa302000391b89a442e7005}
 */
declare class IncidentState extends AbstractAjaxProcessor {
    type: "IncidentState";
}

/**
 * @todo Add members for IncidentUtils with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=87e5e796531232000600e26b88dc3478}
 */
declare class IncidentUtils {
    type: "IncidentUtils";
}

/**
 * This function contains utility functions related to Incident . This function should not be modified by the customer.=
 * @todo Add members for global.IncidentUtilsSNC with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=a1b5e796531232000600e26b88dc3472}
 */
declare class IncidentUtilsSNC {
    // * Public functions :
    // *
    // * ajaxFunction_getIncidentQueryParams
    // * ajaxFunction_makeIncidentCopy
    // * ajaxFunction_getKnowledgeGapMapping
    // * isCopyIncidentFlagValid
    // * makeRelatedTableCopy
    // * copyIncidentRelatedLists
    // * copyIncidentAttachments
    // * getCsvValue
    // * getProblemFromIncident

    type: "IncidentUtilsSNC";
}

/**
 * @todo Add members for IncidentUtils2 with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=44ebab4fc7001010c24ae122c7c26034}
 */
declare class IncidentUtils2 {
    type: "IncidentUtils2";
}

/**
 * @todo Add members for IncidentUtils2SNC with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=0ebbe74fc7001010c24ae122c7c26009}
 */
declare class IncidentUtils2SNC {
    type: "IncidentUtils2SNC";
}

/**
 * Called by processor to create a generic hierarchy diagram. May be extended to create new types of hierarchy diagrams.
 * @todo Add members for global.InheritedRoleMapProcessor with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=5b354e909f0002003d5c77a0942e7042}
 */
declare class InheritedRoleMapProcessor extends AbstractAjaxProcessor {
    type: "InheritedRoleMapProcessor";
}

/**
 * 
 * @param {*} javaObject
 * @return {*}
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/j2js/concept/c_J2jsAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_J2jsAPI}
 */
declare function j2js(javaObject: any): any;

/**
 * Provides scoped methods to create JSON objects from a string, and to turn JSON objects into strings.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/JSON/concept/c_JSONAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_JSONAPI}
 */
declare interface JSON {
    /**
     * Creates an object or primitive type from a JSON formatted string.
     * @static
     * @param {$$rhino.String} str - A JSON formatted string.
     * @param {(this: any, key: string, value: any) => any} [reviver] - A function that transforms the results. This function is called for each member of the object.
     * If a member contains nested objects, the nested objects are transformed before the parent object is.
     * @return {*} An object created from the specified string.
     * @description Proxies calls to the ES5 JSON object, named NativeJSON in the global scope.
     * @memberof JSON
     */
    parse(str: $$rhino.String, reviver?: (this: any, key: string, value: any) => any): any;

    /**
     * Creates a string from a JSON object.
     * @static
     * @param {*} jsonObject - JSON object to turn into a string.
     * @param {(this: any, key: string, value: any) => any} [replacer] - A function that transforms the results.
     * @param {(string | number)} [space] - Adds indentation, white space, and line break characters to the return-value JSON text to make it easier to read.
     * @return {string} A JSON formatted string.
     * @description Proxies calls to the ES5 JSON object, named NativeJSON in the global scope.
     * Converts a value to JSON notation using the following guidelines:
     *
     * If the value has a toJSON() method, it is responsible for defining the data that is serialized.
     *
     * Boolean, number, and string objects are converted to the corresponding primitive values during stringification; in accordance with the traditional conversion semantics.
     *
     * If a function, undefined, or a symbol is encountered during conversion, it is either omitted (when it is found in an object) or censored to null (when it is found in an array). JSON.stringify() also returns undefined when passing in "pure" values, such as JSON.stringify(function(){}) or JSON.stringify(undefined).
     *
     * All symbol-keyed properties are ignored, even when using a replacer() function.
     *
     * Instances of Date implement the toJSON() function by returning a string (the same as date.toISOString()), thus they are treated as strings.
     *
     * The numbers Infinity and NaN, as well as the value null, are all considered null.
     *
     * For all other object instances, only their enumerable properties are serialized.
     * @memberof JSON
     */
    stringify(jsonObject: any, replacer?: (this: any, key: string, value: any) => any, space?: string | number): string;

    /**
     * Creates a string from a JSON object.
     * @static
     * @param {*} jsonObject - JSON object to turn into a string.
     * @param {((number | string)[] | null)} [replacer] - An array of strings and numbers that acts as an approved list for selecting the object properties that will be stringified.
     * @param {(string | number)} [space] - Adds indentation, white space, and line break characters to the return-value JSON text to make it easier to read.
     * @return {string} A JSON formatted string.
     * @description Proxies calls to the ES5 JSON object, named NativeJSON in the global scope.
     * Converts a value to JSON notation using the following guidelines:
     *
     * If the value has a toJSON() method, it is responsible for defining the data that is serialized.
     *
     * Boolean, number, and string objects are converted to the corresponding primitive values during stringification; in accordance with the traditional conversion semantics.
     *
     * If a function, undefined, or a symbol is encountered during conversion, it is either omitted (when it is found in an object) or censored to null (when it is found in an array). JSON.stringify() also returns undefined when passing in "pure" values, such as JSON.stringify(function(){}) or JSON.stringify(undefined).
     *
     * All symbol-keyed properties are ignored, even when using a replacer() function.
     *
     * Instances of Date implement the toJSON() function by returning a string (the same as date.toISOString()), thus they are treated as strings.
     *
     * The numbers Infinity and NaN, as well as the value null, are all considered null.
     *
     * For all other object instances, only their enumerable properties are serialized.
     * @memberof JSON
     */
    stringify(jsonObject: any, replacer?: (number | string)[] | null, space?: string | number): string;

    type: "JSON";
}

/**
 * JavaScript utility functions.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/JSUtil/concept/c_JSUtilAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_JSUtilAPI}
 */
declare class JSUtil {
    /**
     * Returns a map (Object) that is the union of all the given maps.
     */
    static union(...maps: { [key: string]: any }[]): { [key: string]: any };

    /**
    * Removes entries from the given map. The second argument defines what will be removed. If it
    * is an array, it is treated as an array of names to remove. If it is an object, the names of
    * its properties are the names to remove. Otherwise, it is coerced to a string as the name of
    * the single item to remove.
    */
    static removeFromMap(map: { [key: string]: any }, names: any): void;

    /*
    *  Returns true if item is defined but has no properties or functions. (handy for associative arrays)
    */
    static isEmpty(item: any): boolean;

    /*
    * Returns true if the given item is not null and is not undefined.
    */
    static has(item: any): boolean;

    /*
    * Returns true if the given item is null or is undefined (the logical inverse of .has(), above).
    */
    static doesNotHave(item: any): boolean;

    /*
    * Returns true if the given item is null, undefined, or evaluates to the empty string.
    */
    static nil(item: any): boolean;

    /*
    * Returns true if the given item exists and is not empty (the logical inverse of .nil(), above).
    */
    static notNil(item: any): boolean;

    /*
    * Returns the Rhino global object.
    */
    static getGlobal(): any;

    /**
     * Returns true if the given item is a member of the given class.
     * For JavaScript objects, this method behaves exactly like the JavaScript operator "instanceof". However, this method (unlike the JavaScript operator) also tests Java objects.
     * @static
     * @param {*} item - The object to be tested.
     * @param {string} klass - The class to be tested (for Java objects, must be the complete class name, like "java.util.ArrayList").
     * @return {boolean}
     * @memberof JSUtil
     */
    static instance_of(item: any, klass: string): boolean;

    /**
     * Returns the type of the given value as a string, as follows.
     * @static
     * @param {*} value - The object to test.
     * @return {GlideJSType} A string value indicating the objec type.
     * @memberof JSUtil
     */
    static type_of(value: any | undefined): string;


    /**
    * Returns the type of the given value.
    *
    * If 'x' is JavaObject, then this is the class name,
    *
    * If 'x' is a JavaScript object from a JS Class (like our Script Include boilerplate)
    * then this is the value of the 'type' property which is meant to be the JavaScript
    * class name,
    *
    * If 'x' is a JavaScript Array, then this returns 'array',
    *
    * If 'x' is a JavaScript Date, this returns 'date'
    *
    * Otherwise this returns the JavaScript type: string, number, boolean or object as per
    * the type_of method (above).
    *
    * See Also: type_of
    */
    static typeOf(x: any): string;

    /*
    * Returns true if the given value is an instance of a Java object.
    */
    static isJavaObject(value: any): boolean;

    /*
    * Coerces the given item to a boolean. If the given item is a boolean, it is passed through. Non-zero numbers return true. Null or
    * undefined returns false. Strings return true only if exactly equal to 'true'.
    */
    static toBoolean(item: any): boolean;

    /*
    * Returns the value in a boolean GlideRecord field.
    */
    static getBooleanValue(gr: GlideRecord, field: string): boolean;

    /**
    * Determines whether a value exists within an object or not.
    * @param {} container The haystack to search within.
    * @param {} value The expected needle value to compare against.
    * @param boolean|undefined compareByIdentity If true, uses === for comparison; == otherwise.
    * @return True if value exists in container, False otherwise.
    */
    static contains(container: any, value: any, compareByIdentity?: boolean): boolean;

    /*
    * Returns true if the two given values are equivalent, and optionally logs any differences. The two
    * values may be any value - JavaScript primitives or objects. Objects of classes Object, Array, Date,
    * String, Boolean, and Number are all compared correctly and (as necessary) recursively. Note that
    * comparand types much match exactly - for the purposes of this comparison, 'abc' does NOT match
    * new String('abc'). If differences are logged, they may be retrieved from JSUtil.areEqualLog.
    */
    static areEqual(val1: any, val2: any, logDiff?: boolean): boolean;

    /*
    * Logs all the properties (recursively) in the given object: name, type, and value. The optional second parameter is a name for the logged object.
    */
    static logObject(obj: any, name: string): void;

    /*
    * Returns a string that recursively describes all the properties in the given object: name, type, and value.
    * The optional second parameter is a name for the logged object.
    */
    static describeObject(obj: any, name?: string): string;

    /*
    * NOTE: between this banner and the following banner, several string literals are specified in an odd way: by the contatenation of a single
    *       character ('&') and the remainder of the HTML entity (such as 'amp;'). This method was employed to avoid having the entities translated
    *       into the equivalent characters when the script include is edited in the instance.
    */
    static readonly AMP: RegExp;
    static readonly GT: RegExp;
    static readonly LT: RegExp;
    static readonly QT: RegExp;
    static readonly AMP_ENT: RegExp;
    static readonly GT_ENT: RegExp;
    static readonly LT_ENT: RegExp;
    static readonly QT_ENT: RegExp;

    static escapeText(text: string): string;

    static unescapeText(text: string): string;

    static escapeAttr(attr: string): string;

    static unescapeAttr(attr: string): string;

    /** Render an expanded/evaluted string from a string that may contain one
    *  or more Javascript expressions, each wrapped in a dolloar-braces
    *  delimiter pattern.
    *
    *     'The timeis:${newGlideDateTime()}'
    *
    *  will displaythecurrenttime.
    *
    *  When used in specific contexts, such as inside a workflow context
    *  certain global variables might be usable such as 'current' or 'workflow':
    *
    *      'WF State:${context.state},rec:${current.sys_id}'
    *
    *  and content can be substituted into data from various Javascripts:
    *
    *      <CREATED>${newGlideDateTime()}</CREATED>
    *
    *  WARNING: This is used heavily by workflows. If this is changed, then
    *           be sure to run all workflow tests. Test Log Message activity
    *           with ${workflow.variables.somevariable} and similar usages.
    */
    static strEval(str: string): string;
}

/**
 * @todo Add members for ListCis with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=84881a35c73320108c74759ab8c26087}
 */
declare class ListCis {
    type: "ListCis";
}

/**
 * @todo Add members for MakeAndModelJS with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=d7c58843ef72010098d5925495c0fb82}
 */
declare class MakeAndModelJS {
    type: "MakeAndModelJS";
}

/**
 * Script for checking if table is many to many table.
 * @todo Add members for global.ManyToManyChecker with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=8984a7c10f023300fd3e3632d4767e2a}
 */
declare class ManyToManyChecker extends AbstractAjaxProcessor {
    type: "ManyToManyChecker";
}

/**
 * @todo Add members for ModelUtils with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=e5e5e63edbdf33001f9b765f369619d9}
 */
declare class ModelUtils {
    type: "ModelUtils";
}

/**
 * @todo Document global.NiceError
 * @todo Add members for global.NiceError with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=fc70ddc629230010fa9bf7f97d737e2e}
 */
declare class NiceError {
    type: "NiceError";
}

/**
 * @todo Document global.Optional
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server/no-namespace/OptionalGlobalAPI}
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/Optional/concept/OptionalGlobalAPI.html}
 * @todo Validate with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=15bd3bd973692300bb513198caf6a7cf}
 */
declare class Optional<T> {
    /**
     * Creates an instance of Optional.
     * @param {(T | null | undefined)} value - Value to be contained by Optional.
     * @param {({ (): T | null | undefined })} [lazyGetFn] - Function which returns a value.
     * Used when you want the Optional to contain data which may be costly to retrieve (like a query) and may not be necessary.
     * @param {string} [reason] - Reason given when an empty Optional is unwrapped (e.g. using `get()`)
     * @memberof Optional
     */
    constructor(value: T | null | undefined, lazyGetFn?: { (): T | null | undefined }, reason?: string);

    /**
     * Applies a predicate function, a function that takes a single value and returns true or false, to the record inside the Optional object.
     * If the function returns true, the method returns the Optional record unchanged. If the function returns false, it returns an empty Optional object.
     * @param {{ (value: T): boolean }} predicate - Predicate function to apply to the value inside the Optional object.
     * @return {Optional<U>} Object used to interact with a single record.
     * @memberof Optional
     */
    filter(predicate: { (value: T): boolean }): Optional<T>;

    /**
     * Function to apply to the results of the query that returned the Optional object.
     * @template U - The record type.
     * @param {{ (value: T): Optional<U> }} fn - Function to apply to the results of the query that returned the Optional object.
     * @return {Optional<U>} Object used to interact with a single record.
     * @memberof Optional
     */
    flatMap<U>(fn: { (value: T): Optional<U> }): Optional<U>;

    /**
     * Returns the record inside the Optional object, or throws an error if the query does not return a record.
     * @return {T} The record inside the Optional object. If the value is null or undefined, the system throws an error.
     * @memberof Optional
     */
    get(): T;

    /**
     * Applies a function to the record within an Optional object. If the Optional object does not contain a record, the function does not execute.
     * @param {{ (value: T): void }} fn - The function to apply to the record within the Optional object.
     * @memberof Optional
     */
    ifPresent(fn: { (value: T): void }): void;

    /**
     * Tests whether the current Optional contains no value.
     * @return {boolean} True if the current Optional contains no value; otherwise, false.
     * @memberof Optional
     */
    isEmpty(): boolean;

    /**
     * Tests whether the current Optional object contains a value.
     * @return {boolean} True if the current Optional contains a value; otherwise, false.
     * @memberof Optional
     */
    isPresent(): boolean;

    /**
     * Applies a function to the result of a query.
     * @template U - The mapped record type.
     * @param {{ (value: T): U }} fn - Function to apply to the result of the query.
     * @return {Optional<U>} Object used to interact with a single record.
     * @memberof Optional
     */
    map<U>(fn: { (value: T): U }): Optional<U>;

    /**
     * Provides an alternate value if the current Optional is empty.
     * @template U - The alternate value type.
     * @param {T} defaultValue - Alternate value to return if the current Optional is empty.
     * @return {(T | U)} Value within the current Optional object or the alternate value if the current Optional is empty.
     * @memberof Optional
     */
    orElse<U>(defaultValue: U): T | U;

    toString(): string;

    /**
     * Returns an empty Optional object. Use this method in an Else clause to handle a query that might not return a result.
     * @static
     * @template U - Optional value type.
     * @param {string} [reason] - Optional reason displayed in the log when Optional.get() is called on the empty Optional object.
     * @return {Optional<U>} Object used to interact with a single record.
     * @memberof Optional
     */
    static empty<U>(reason?: string): Optional<U>;

    /**
     * Returns a new Optional object. Instead of containing the record,
     * the object contains a function to get the record that is only called if and when requested in the code.
     * @static
     * @template U - The record type.
     * @param {({ (): U | null | undefined })} lazyGetFn - Function that returns a single record as a result of a query.
     * @return {Optional<U>} Object used to interact with a single record.
     * @memberof Optional
     */
    static lazy<U>(lazyGetFn: { (): U | null | undefined }): Optional<U>;

    /**
     * Wraps a given value in an Optional object. For example, you can wrap the result of a GlideRecord query in an Optional object to use the associated methods.
     * @static
     * @template U - The record type.
     * @param {U} value - Value inside the Optional object.
     * @return {Optional<U>} Object used to interact with a single record.
     * @memberof Optional
     */
    static of<U>(value: U): Optional<U>;
}

/**
 * @todo Add members for OSChoice with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=5107787137203000dcd48c00dfbe5da7}
 * @todo Add see reference to {@link $$tablefields.cmdb_ci_computer#os}
 */
declare class OSChoice {
    type: "OSChoice";
}

/**
 * @todo Add members for ProblemState with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=91d5739b875b13000e3dd61e36cb0b7f}
 */
declare class ProblemState {
    type: "ProblemState";
}

/**
 * @todo Add members for ProblemStateSNC with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=7b547b5b875b13000e3dd61e36cb0b55}
 */
declare class ProblemStateSNC {
    type: "ProblemStateSNC";
}

/**
 * @todo Add members for ProblemStateUtils with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=5428ba14872313000e3dd61e36cb0b72}
 */
declare class ProblemStateUtils {
    type: "ProblemStateUtils";
}

/**
 * @todo Add members for ProblemStateUtilsSNC with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=f8b66716871313000e3dd61e36cb0b77}
 */
declare class ProblemStateUtilsSNC {
    type: "ProblemStateUtilsSNC";
}

/**
 * @todo Add members for ProblemTaskState with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=b16748eb535b13004247ddeeff7b12c5}
 */
declare class ProblemTaskState {
    type: "ProblemTaskState";
}

/**
 * @todo Add members for ProblemTaskStateSNC with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=35988ceb535b13004247ddeeff7b12cc}
 */
declare class ProblemTaskStateSNC {
    type: "ProblemTaskStateSNC";
}

/**
 * @todo Add members for ProblemTaskStateUtils with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=d36a573e535b13004247ddeeff7b12db}
 */
declare class ProblemTaskStateUtils {
    type: "ProblemTaskStateUtils";
}

/**
 * @todo Add members for ProblemTaskStateUtilsSNC with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=d12653fa535b13004247ddeeff7b12cb}
 */
declare class ProblemTaskStateUtilsSNC {
    type: "ProblemTaskStateUtilsSNC";
}

/**
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_RecordToHTMLAPI}
 */
declare class RecordToHTML {

}
/**
 * @todo Add members for Schema with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=4e115aed73512300bb513198caf6a749}
 */
declare class Schema {
    type: "Schema";
}

/**
 * The singleton type for {@link Stream.END}.
 * @abstract
 * @hideconstructor
 */
declare abstract class STREAM_END { private constructor(); }

/**
 * Interacts with a stream of items such as records.
 * @template T The element type.
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server/no-namespace/StreamGlobalAPI}
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/Stream/concept/StreamGlobalAPI.html}
 */
declare class Stream<T> {
    /**
     * Creates an instance of Stream.
     * @param {({ (): T | STREAM_END })} nextFn - A function that retrieves the next item in the stream or returns {@link Stream.END} if there are no more items.
     * @memberof Stream
     */
    constructor(nextFn: { (): T | STREAM_END });

    /**
     * Limits the number of results returned by the stream.
     * @param {number} count - Number of records to return.
     * @return {Stream<T>} Object used to interact with a stream of items such as records.
     * @memberof Stream
     */
    limit(count: number): Stream<T>;

    /**
     * Returns results in batches of arrays, each containing the number of records passed to the method.
     * @param {number} count - Number of records in each array returned from the stream.
     * @return {Stream<T[]>} Object used to interact with a stream of items such as records.
     * @memberof Stream
     */
    chunk(count: number): Stream<T[]>;

    /**
     * Applies a function to each item in a stream and returns the updated Stream object.
     * @template U - The result element type.
     * @param {{ (obj: T): U }} fn - Function to apply to the result of the query that takes the each item in the stream as input.
     * @return {Stream<U>} Object containing the stream of records updated after applying the function.
     * @memberof Stream
     */
    map<U>(fn: { (obj: T): U }): Stream<U>;

    /**
     * Applies a function to every item in a stream. Returns another stream that you can iterate over.
     * @template U - The result stream element type.
     * @param {{ (obj: T): Stream<U> }} fn - Function to apply to the result of the query that returns a Stream object.
     * @return {Stream<U>} Object containing the stream of records updated after applying the function.
     * @memberof Stream
     */
    flatMap<U>(fn: { (obj: T): Stream<U> }): Stream<U>;

    /**
     * Applies a predicate function to each item in the Stream object.
     * If the predicate returns true, the method returns the stream.
     * If the predicate returns false, it returns an empty Stream object.
     * @param {{ (obj: T): boolean }} predicate - Predicate function to apply to every record or item inside the Stream object.
     * The function must take each item in the stream as input and return a boolean.
     * @return {Stream<T>} Object used to interact with a stream of items such as records.
     * @memberof Stream
     */
    filter(predicate: { (obj: T): boolean }): Stream<T>;

    /**
     * Returns the first record or item in the Stream object that matches the predicate function.
     * If no predicate function is provided, then the method returns the first record or item in the Stream.
     * @param {{ (obj: T): boolean }} [predicate] - Optional predicate function to apply to the items inside the Stream object.
     * The function must take each item in the stream as input and return a boolean.
     * @return {Optional<T>} Object containing the returned record.
     * @memberof Stream
     */
    find(predicate?: { (obj: T): boolean }): Optional<T>;

    /**
     * Applies a predicate function, a function that takes a single value and returns true or false, to each item in the stream.
     * If the predicate returns true for any item in the stream, the method returns true.
     * @param {{ (obj: T): boolean }} predicate - Predicate function to apply to the items inside the Stream object.
     * @return {boolean} True if the predicate function returned true for an item in the stream; otherwise, false.
     * @memberof Stream
     */
    some(predicate: { (obj: T): boolean }): boolean;

    /**
     * Applies a predicate function to every item in the Stream object.
     * If the predicate returns true for every item in the stream, the method returns true.
     * If the predicate returns false for any item in the stream, the method returns false.
     * @param {{ (obj: T): boolean }} predicate - Predicate function to apply to every record or item inside the Stream object.
     * The function must take each item in the stream as input and return a boolean.
     * @return {boolean} True if the predicate function returns true for every item in the stream; otherwise, false.
     * @memberof Stream
     */
    every(predicate: { (obj: T): boolean }): boolean;

    /**
     * Returns an array containing the given number of items from the stream.
     * @param {number} count - The maximum number of items from the stream to return in the array.
     * @return {T[]} Array containing the given number of items from the stream.
     * @memberof Stream
     */
    toArray(count: number): T[];

    /**
     * Executes a reducer function on each item in the stream, resulting in single output value.
     * @template U - The accumulated value type.
     * @param {{ (acc: U, cur: T): U }} reducerFn - Function to apply to each item in the stream that reduces the stream to a single value.
     * @param {U} initialValue - Value passed to the function as the initial value.
     * @return {U} Accumulated total of all items returned by the reducer function.
     * @memberof Stream
     */
    reduce<U>(reducerFn: { (acc: U, cur: T): U }, initialValue: U): U;

    /**
     * Applies the specified function to each record or item in the stream.
     * @param {{ (obj: T): void }} fn - Function to apply to each item in the stream.
     * @memberof Stream
     */
    forEach(fn: { (obj: T): void }): void;

    toString(): "Stream";

    /**
     * Returns a Stream object that contains the values from the provided array.
     * @static
     * @template U - The element type.
     * @param {U[]} arr - Array of values to create the stream from.
     * @return {Stream<U>} Object used to interact with a stream of items such as records.
     * @memberof Stream
     */
    static fromArray<U>(arr: U[]): Stream<U>;

    /**
     * Combines multiple Streams using a combiner function
     * @static
     * @template U - The input stream element type.
     * @template V - The result stream element type.
     * @param {{ (...values: U[]): V}} combinerFn - Function which has a N number parameters, one for each Stream, and return a combined value.
     * @param {...Stream<U>[]} streams - The streams to combine.
     * @return {Stream<V>} The merged stream.
     * @memberof Stream
     */
    static zip<U, V>(combinerFn: { (...values: U[]): V }, ...streams: Stream<U>[]): Stream<V>;

    static readonly END: STREAM_END;
}

/**
 * Locates the list that should be used for a table and view and parent (related list).
 * @description 1. Search for this table, this view, this related list
 * 2. Search for parents of this table, this view, this related list
 * 3. Search for this table, this view
 * 4. Search for parents of this table, this view
 * 5. Search for this table, default view
 * 6. Search for parents of this table, default view
 * 7. Construct and return the default list
 * @todo Add members for global.SysList with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=7d8f32c3c0a8016400e609be97b96d89}
 */
declare class SysList extends AbstractAjaxProcessor {

    type: "SysList";
}

/**
 * @todo Document global.SysRelatedList
 * @todo Add members for global.SysRelatedList with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=89171ef2c0a8016800449d9d0407bb27}
 */
declare class SysRelatedList {

    type: "SysRelatedList";
}

/**
 * Locate the list associated with the current user for the table and view.
 * @todo Add members for global.SysUserList with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=7dc9266bc0a80164003bff257b3ff216}
 */
declare class SysUserList extends AbstractAjaxProcessor {

    type: "SysUserList";
}

/**
 * Task State Management Utility
 * @deprecated Helper functions for working with task-type table state attributes, primarily used by the Task Active State Management business rule to set the active field based on state changes
 * Can be called by any server script to determine inactive states, default work, or default close states for a given table
 * Configurations are define in the task.state dictionary element, usually using dictionary overrides since state values vary by table.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/TaskStateUtil/concept/c_TaskStateUtil.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_TaskStateUtil}
 * @todo Add members for global.TaskStateUtil with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=96e1ade7c0a80a6d381ba0c6aeb4ad61}
 */
declare class TaskStateUtil {

    type: "TaskStateUtil";
}

/**
 * @todo Document global.TaskUtils
 * @todo Add members for global.TaskUtils with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=584d558687c010100e3dd61e36cb0b8f}
 */
declare class TaskUtils {

    type: "TaskUtils";
}

/**
 * @todo Document global.TaskUtilsSNC
 * @todo Add members for global.TaskUtilsSNC with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=1cf5818a878010100e3dd61e36cb0b4a}
 */
declare class TaskUtilsSNC {

    type: "TaskUtilsSNC";
}

/**
 * @todo Document global.UnifiedConversationUtil
 * @todo Add members for global.UnifiedConversationUtil with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=72ef5784e7431010748b42d6c2f6a9d1}
 */
declare class UnifiedConversationUtil {

    type: "UnifiedConversationUtil";
}

/**
 * @todo Add members for UserProfileService with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=0efb5d910f09230096f9d6892f767e6f}
 */
declare class UserProfileService {
    type: "UserProfileService";
}

/**
 * Analyzes user skills against skills required to complete work items.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/UserSkillAnalyzer/concept/UserSkillAnalyzerAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/UserSkillAnalyzerAPI}
 */
declare class UserSkillAnalyzer {
    // TODO: Add members for global.UserSkillAnalyzer

    type: "UserSkillAnalyzer";
}

/**
 * Used to configure options for choosing the right user and getting a sorted list of qualified users based on number of matching skills and skill-level gap.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/UserSkillRanking/concept/UserSkillRankingAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/UserSkillRankingAPI}
 */
declare class UserSkillRanking {
    // TODO: Add members for global.UserSkillRanking

    type: "UserSkillRanking";
}

/**
 * Validates field input values.
 * @todo Add members for global.VAFieldValidator with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=1e2e643a5bd200101f254d3ba881c7d9}
 */
declare class VAFieldValidator {

    type: "VAFieldValidator";
}

/**
 * Checks if schedule spans start and end dates are valid and that they don't overlap with any other rotation schdedule
 * @todo Add members for global.ValidateSchedule with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=ecae190bbf1211003f07e2c1ac0739fb}
 */
declare class ValidateSchedule extends AbstractAjaxProcessor {

    type: "ValidateSchedule";
}

/**
 * Used to parse the query and then evaluate the values of the variables starting with 'javascript:'.
 * Values are evaluated within the sandbox environment and doesn't throw an error in case of an exception.
 * @todo Add members for global.VariableQueryParser with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=dadc4af05fa023008e6b1f9f2f731340}
 */
declare class VariableQueryParser extends AbstractAjaxProcessor {

    type: "VariableQueryParser";
}

/**
 * @todo Document global.VariableUtil
 * @todo Add members for global.VariableUtil with {@link https://usmskdev2.servicenowservices.com//nav_to.do?uri=sys_script_include.do?sys_id=15f2285773011300f49d0690fdf6a721}
 */
declare class VariableUtil {
    type: "VariableUtil";
}

/**
 * Walks the workflow and gets the list of successors for each activity along with a shortest path order that indicates the activity depth in the workflow.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/Walkworkflow/concept/c_WalkWorkflowAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_WalkWorkflowAPI}
 * @todo: Add members for WalkWorkflow with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=441f22d40a2581027b72c63a41c520ff}
 */
declare class WalkWorkflow {
    type: "WalkWorkflow";
}

/**
 * The base class for all workflow activities.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/WFACtivityHandler/concept/c_WFACtivityHandlerAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_WFActivityHandlerAPI}
 * @todo: Add members for WFActivityHandler with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=365eca63c0a8016600069528aee5affb}
 */
declare class WFActivityHandler {
    type: "WFActivityHandler";
}

/**
 * Handles the formatting of Windows OS names.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/WindowsOSNameHelper/concept/c_WindowsOSNameHelperAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_WindowsOSNameHelperAPI}
 * @todo: Add members for WindowsOSNameHelper with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=9083d4690a0a0b8277155ea2ab428f1f}
 */
declare class WindowsOSNameHelper {owsOSNameHelper

    type: "WindowsOSNameHelper";
}

/**
 * Workflow scripts provide an interface with the workflow engine.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/Workflow/concept/c_Workflow_api.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_WorkflowAPI}
 * @todo Validate with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=d3669766c0a8016901828e92d1202ac5}
 */
declare class Workflow {
    /**
     * Creates an instance of the Workflow class.
     */
    public constructor();

    /**
     * Typical use of this method is to enable activities that wait for some action to occur before proceeding.
     * For additional information on using broadcastEvent, refer to Workflow event-specific functions ({@link https://docs.servicenow.com/bundle/utah-build-workflows/page/administer/workflow-administration/reference/r_WorkflowEventSpecificFunctions.html}).
     * See {@link BaseSystemWorkflowEventNames} for available OOB events.
     *
     * @param {string} contextId - The context ID.
     * @param {string} eventName - The name of the event.
     * @summary Sends the specified event (message) into the workflow context to pass along to the executing activities.
     */
    broadcastEvent(contextId: string, eventName: string): void;

    /**
     * Cancels all running workflows on this record by broadcasting the cancel event to activities in all running workflows on this record.
     * @param {GlideRecord} record - GlideRecord on any table. All workflows running on this record will be cancelled.
     */
    cancel(record: GlideRecord): void;

    /**
     * Cancels this running context by broadcasting a cancel event to activities in this workflow context.
     * @param {GlideRecord} context - GlideRecord of the running context to cancel.
     */
    cancelContext(context: GlideRecord): void;

    /**
     * Deletes all the workflows on the record.
     * @param {GlideRecord} current - GlideRecord for which the caller wants to delete all workflows. This can be any record on any table.
     */
    deleteWorkflow(current: GlideRecord): void;

    /**
     * Fires the named event on the input record.
     *
     * Used in ActivitiesApproval Coordinator, Timer, Lock, and some others.
     * @param {GlideRecord} eventRecord - The event record.
     * @param {string} eventName - The name of the event to send to the executing workflow.
     */
    fireEvent(eventRecord: GlideRecord, eventName: string): void;

    /**
     * Fires the named event on the record specified by record ID.
     *
     * Used in Activities Approval Coordinator, Timer, Lock, and some others.
     * @param {string} eventRecordId - The sys_id of the glide record.
     * @param {string} eventName - The name of the event to send to the executing workflow.
     */
    fireEventById(eventRecordId: string, eventName: string): void;

    /**
     * Returns all workflow contexts for a specified record.
     * @param {GlideRecord} record - GlideRecord for which the caller wants a list of all workflow contexts. This can be any record on any table for which the caller wants the running workflow contexts.
     * @returns {GlideRecord} GlideRecord in the Workflow context [wf_context] table filtered for all workflow contexts for the specified record (in any state, such as running, cancelled, finished).
     */
    getContexts(record: GlideRecord): GlideRecord;

    /**
     * Gets the estimated time for a workflow to complete.
     * @param {string} workflowId - Sys_id of the workflow (table wf_workflow) to get the estimated run time.
     * @returns {string} Display value from a GlideDuration (e.g., 3 days), or blank if unknown.
     */
    getEstimatedDeliveryTime(workflowId: string): string;

    /**
     * Get the estimated elapsed execution time for the workflow version.
     * @param {GlideRecord} wfVersion - GlideRecord on table wf_workflow_version of a specific workflow version for which the caller wants the estimated during of executing.
     * @returns {string}
     */
    getEstimatedDeliveryTimeFromWFVersion(wfVersion: GlideRecord): string;

    /**
     * Gets the appropriate workflow return value for the input workflow ID.
     *
     * This is either the workflow checked out by the current user or the published workflow with the most recent date.
     * @param {string} workflowID - The sys_id of the workflow (table wf_workflow)
     * @param {number} amount - amount
     * @param {boolean} result - True, if true
     * @returns {*} The return value of the workflow as specified by the Return Value activity. Workflows without a Return Value activity return a null value.
     */
    getReturnValue(workflowID: string, amount: number, result: boolean): any;

    /**
     * Gets all the currently running workflow contexts for the input record.
     *
     * The input record is any record on any table for which the caller wants the running workflow contexts.
     * @param {GlideRecord} record - GlideRecord of the record for which the caller wants a list of all running workflows.
     * @returns {GlideRecord} GlideRecord on table wf_context and filtered for all executing workflow contexts.
     */
    getRunningFlows(record: GlideRecord): GlideRecord;

    /**
     * Gets the appropriate workflow version for the input workflow ID.
     * 
     * This is either the workflow checked out by the current user or the published workflow with the most recent date.
     * @param {string} workflowID - The sys_id of the workflow (table wf_workflow)
     * @returns {string} The workflow version.
     */
    getVersion(workflowID: string): string;

    /**
     * Returns the appropriate workflow version for the input workflow name.
     * @param {string} workflowName - Name of the workflow (table wf_workflow)
     * @returns {string} The workflow version.
     */
    getVersionFromName(workflowName: string): string;

    /**
     * Returns the sys_id of the workflow associated with the specified workflow name.
     * @param {string} workflowName - Name of the workflow.
     * @returns {string}The sys_id of the workflow associated with the passed in name.
     */
    getWorkflowFromName(workflowName: string): string;

    /**
     * Determines if a specified record has any workflow contexts associated to it.
     *
     * This includes running and completed workflow contexts.
     * @param {GlideRecord} record - GlideRecord under scrutiny. This GlideRecord can be from any table.
     * @returns {boolean} True, if record has associated workflow; otherwise, returns False.
     */
    hasWorkflow(record: GlideRecord): boolean;

    /**
     * Recalculates the approvals and tasks for a workflow by adding new approvals and tasks, while not resetting current approvals and tasks.
     *
     * You can use this method to perform such tasks as adding a company to a change request, without resetting the current approvals for companies already in the workflow.
     * @param {GlideRecord} current - GlideRecord of the record this workflow is executing. This can by any record on any table.
     * @param {boolean} maintainStateFlag - Flag that indicates whether to maintain all approvals and tasks in their current state.
     *                                      true: Maintain all approvals and tasks in their current state; false: Update all approval and task states.
     */
    restartWorkflow(current: GlideRecord, maintainStateFlag: boolean): void;

    /**
     * Runs all workflows for a given record in a given table and its descendant tables.
     * @param {GlideRecord} record - GlideRecord to run workflows against.
     * @param {WorkflowOperationType} operation - Database operation.
     */
    runFlows(record: GlideRecord, operation: WorkflowOperationType): void;

    /**
     * Starts a specified workflow.
     * @param {string} workflowId - The sys_id of the workflow to start. This sys_id refers to table wf_workflow.
     * @param {GlideRecord} current - The record to use as current in this workflow. This is normally from the Table field of the workflow properties for this workflow.
     * @param {WorkflowOperationType} operation - The operation to perform on current.
     * @param {Array<any>} vars - Collection of variables to add to the workflow.
     * @returns {GlideRecord} GlideRecord on table wf_context for the newly created workflow context.
     */
    startFlow(workflowId: string, current: GlideRecord, operation: WorkflowOperationType, vars: Array<any>): GlideRecord;

    /**
     * Helper method for business rule Auto start on context.
     * @param {GlideRecord} context - GlideRecord on table wf_context of a new record (the "current" record in the business rule).
     * @param {WorkflowOperationType} operation - Database operation being performed.
     */
    startFlowFromContextInsert(context: GlideRecord, operation: WorkflowOperationType): void;
    
    /**
     * Used by business rule Start Workflow on table task_sla.
     * 
     * This starts a workflow and the extra arguments to this method are used by activity "Timer" to pause the execution of the workflow for some duration.
     * @param {string} workflowId - The sys_id of the workflow to start. This sys_id refers to table wf_workflow.
     * @param {number} retroactiveMSecs - Delay in milliseconds used by Activity Timer.
     * @param {GlideRecord} current - GlideRecord of the record to use as current in this workflow. This is normally from the Table field of the workflow properties for this workflow.
     * @param {WorkflowOperationType} operation - Database operation being performed.
     * @param {Array<any>} vars - Collection of variables to add to the workflow.
     * @param {*} [withSchedule] - Schedule used by Activity Timer
     * @returns {GlideRecord}
     */
    startFlowRetroactive(workflowId: string, retroactiveMSecs: number, current: GlideRecord, operation: WorkflowOperationType, vars: Array<any>, withSchedule?: any): GlideRecord;
}

/**
 * This class calculates the duration (in seconds) based on the variables of a workflow activity.
 * It is an interface between Workflow Timer() and DurationCalculator().
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/WorkflowDuration/concept/c_WorkflowDurationAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_WorkflowDurationAPI}
 * @todo: Add members for WorkflowDuration with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=8eb21271c0a8006666a23d675ebb20bb}
 */
declare class WorkflowDuration {
    type: "WorkflowDuration";
}

/**
 * Provides a way to query the workflow model, to step backwards and forwards between specified wf_history items,
 * and to query the history for activity and transition-specific information.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/WorkflowModelManager/context/c_WorkflowModelManagerAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_WorkflowModelManagerAPI}
 * @todo: Add members for WorkflowModelManager with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=3d68dd633b130000dada82c09ccf3d28}
 */
declare class WorkflowModelManager {
    type: "WorkflowModelManager";
}

/**
 * Provides an AjaxProcessor wrapper for the WorkflowModelManager.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/WorkflowModelManagerAjax/concept/c_WorkflowModelManagerAjaxAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_WorkflowModelManagerAjaxAPI}
 * @todo: Add members for WorkflowModelManagerAjax with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=fb6bd23d1f230000dada1c883d8b7037}
 */
declare class WorkflowModelManagerAjax extends AbstractAjaxProcessor {
    type: "WorkflowModelManagerAjax";
}

/**
 * This code is executed in business rule Set workflow scheduler script on table wf_workflow_schedule.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/WorkflowScheduler/concept/c_WorkflowSchedulerAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_WorkflowSchedulerAPI}
 * @todo: Add members for WorkflowScheduler with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=dd93c0940a00010c00714ff05fa5a5df}
 */
declare class WorkflowScheduler {
    type: "WorkflowScheduler";
}

/**
 * Generates a view that shows a workflow context on the timeline.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/WorkflowTimeLine/concept/c_WorkflowTimelineAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_WorkflowTimelineAPI}
 * @todo: Add members for WorkflowTimeline with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=313d4ff207130000dada43c0d1021e90}
 */
declare class WorkflowTimeline {
    type: "WorkflowTimeline";
}

/**
 * @todo Validate XMLDocument with {@link https://usmskdev2.servicenowservices.com/nav_to.do?uri=sys_script_include.do?sys_id=6a2b73370a0a0b240037050e42a58f5b}
 */
declare class XMLDocument {
    constructor(xml: $$rhino.String, nsAware?: $$rhino.Boolean);
    createElement(name: string, value?: any): Packages.org.w3c.dom.Element;
    setCurrent(el: Packages.org.w3c.dom.Element): void;
    setAttribute(name: string, value?: any): void;
    getChildTextByTagName(parent: Packages.org.w3c.dom.Element, tagName: string): Packages.java.lang.String;
    getElementByTagName(ptagName: string): Packages.org.w3c.dom.Element;
    getElementValueByTagName(tagName: string): Packages.java.lang.String;
    getDocument(): GlideXMLDocument;
    getDocumentElement(): Packages.org.w3c.dom.Element;
    getNodeText(xpath: string): Packages.java.lang.String;
    getNodeInt(xpath: string): Packages.java.lang.Integer;
    getNode(xpath: string): Packages.org.w3c.dom.Node;
    getNodeName(xpath: string): Packages.java.lang.String;
    getNodeType(xpath: string): Packages.java.lang.String;
    getNodes(xpath): Packages.org.w3c.dom.NodeList;
    getAttribute(xpath: string, attributeName: string): Packages.java.lang.String;
    isValid(): Packages.java.lang.Boolean;
    load(f: string): void;
    toIndentedString(): Packages.java.lang.String;
    toString(): Packages.java.lang.String;
}
