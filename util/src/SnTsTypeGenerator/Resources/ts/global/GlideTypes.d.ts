/// <reference path="./Packages.d.ts" />
/// <reference path="./$$rhino.d.ts" />

//#region TypeDefs

declare type EncryptionAlgorithmHashType = "MD-2" | "MD-5" | "SHA-1" | "SHA-224" | "SHA-256" | "SHA-384" | "SHA-512";

declare type EncryptionAlgorithmSignType = "NONEwithRSA" | "MD2withRSA" | "MD5withRSA" | "SHA1withRSA" | "SHA224withRSA" | "SHA256withRSA" | "SHA384withRSA" |
    "SHA512withRSA" | "NONEwithDSA" | "SHA1withDSA" | "SHA224withDSA" | "SHA256withDSA" | "NONEwithECDSA" | "SHA1withECDSA" | "SHA224withECDSA" | "SHA256withECDSA" |
    "SHA384withECDSA" | "SHA512withECDSA";

declare type TaskAppproval = "not requested" | "cancelled" | "requested" | "duplicate" | "not_required" | "approved" | "rejected";

/**
 * email=Email; endpoint_security=Endpoint Security; ids_ips=IDS/IPS; network_monitoring=Network Monitoring; phone=Phone; self-service=Self-service; siem=SIEM; virtual_agent=Virtual Agent; vulnerability_response=Vulnerability Response; walk-in=Walk-in
 */
declare type TaskContactType = "email" | "endpoint_security" | "ids_ips" | "network_monitoring" | "phone" | "self-service" | "siem" | "virtual_agent" | "vulnerability_response" | "walk-in";

declare type TaskEscalationNumber = 0 | 1 | 2 | 3;
declare type TaskEscalationString = "0" | "1" | "2" | "3";
declare type TaskEscalationValue = TaskEscalationNumber | TaskEscalationString;

/**
 *
 * @description 1="1 - High"; 2="2 - Medium"; 3="3 - Low"
 */
declare type Task3ScaleNumber = 1 | 2 | 3;
declare type Task3ScaleString = "1" | "2" | "3";
declare type Task3ScaleValue = Task3ScaleNumber | Task3ScaleString;

declare type TaskPriorityNumber = 1 | 2 | 3 | 4 | 5;
declare type TaskPriorityString = "1" | "2" | "3" | "4" | "5";
declare type TaskPriorityValue = TaskPriorityNumber | TaskPriorityString;

/**
 *
 * @description -5="Pending"; 1="Open"; 2="Work in Progress"; 3="Closed Complete"; 4="Closed Incomplete"; 7="Closed Skipped"
 */
declare type TaskStateNumber = -5 | 1 | 2 | 3 | 4 | 7;
declare type TaskStateString = "-5" | "1" | "2" | "3" | "4" | "7";
declare type TaskStateValue = TaskStateNumber | TaskStateString;

declare type IncidentCloseCode = "Solved (Work Around)" | "Solved (Permanently)" | "Solved Remotely (Work Around)" | "Solved Remotely (Permanently)" | "Not Solved (Not Reproducible)" | "Not Solved (Too Costly)" | "Closed/Resolved by Caller";

/**
 *
 * @description 1="Awaiting Caller"; 5="Awaiting Change"; 3="Awaiting Problem"; 4="Awaiting Vendor"
 */
declare type IncidentHoldReasonNumber = 1 | 5 | 3 | 4;
declare type IncidentHoldReasonString = "1" | "5" | "3" | "4";
declare type IncidentHoldReasonValue = IncidentHoldReasonNumber | IncidentHoldReasonString;

/**
 *
 * @description 1="New"; 2="In Progress"; 3="On Hold"; 6="Resolved"; 7="Closed"; 8="Canceled"
 */
declare type IncidentStateNumber = 1 | 2 | 3 | 6 | 7 | 8;
declare type IncidentStateString = "1" | "2" | "3" | "6" | "7" | "8";
declare type IncidentStateValue = IncidentStateNumber | IncidentStateString;

declare type IncidentNotifyNumber = 1 | 2 | 3;
declare type IncidentNotifyString = "1" | "2" | "3";
declare type IncidentNotifyValue = IncidentNotifyNumber | IncidentNotifyString;

/**
 *
 * @description 1="Success"; 2="Fail"
 */
declare type ChangeReviewStatusNumber = 1 | 2;
declare type ChangeReviewStatusString = "1" | "2";
declare type ChangeReviewStatusValue = ChangeReviewStatusNumber | ChangeReviewStatusString;

/**
 *
 * @description 2="High"; 3="Moderate"; 4="Low"
 */
declare type ChangeRiskNumber = 2 | 3 | 4;
declare type ChangeRiskString = "2" | "3" | "4";
declare type ChangeRiskValue = ChangeRiskNumber | ChangeRiskString;

/**
 *
 * @description 1="Massive"; 2="Large"; 3="Medium"; 4="Small"; 5="Tiny"
 */
declare type ChangeScopeNumber = 1 | 2 | 3 | 4 | 5;
declare type ChangeScopeString = "1" | "2" | "3" | "4" | "5";
declare type ChangeScopeValue = ChangeScopeNumber | ChangeScopeString;

declare type ChangeType = "standard" | "normal" | "emergency";

/**
 *
 * @description "successful"="Successful"; "successful_issues"="Successful with issues"; "unsuccessful"="Unsuccessful"
 */
declare type ChangeCloseCode = "successful" | "successful_issues" | "unsuccessful";

/**
 *
 * @description 1="Open"; 3="Pending Change"; 2="Known Error"; 4="Closed/Resolved"
 */
declare type ProblemStateNumber = 1 | 3 | 2 | 4;
declare type ProblemStateString = "1" | "3" | "2" | "4";
declare type ProblemStateValue = ProblemStateNumber | ProblemStateString;

/**
 *
 * @description "requested"="Pending Approval"; "in_process"="Approved"; "closed_complete"="Closed Complete"; "closed_incomplete"="Closed Incomplete"; "closed_cancelled"="Closed Cancelled"; "closed_rejected"="Closed Rejected"; "closed_skipped"="Closed Skipped"
 */
declare type IRequestState = "requested" | "in_process" | "closed_complete" | "closed_incomplete" | "closed_cancelled" | "closed_rejected" | "closed_skipped";

/**
 * Query operator values that can be used for string value comparisons.
 */
declare type StringQueryOperator = "=" | "!=" | "IN" | "NOT IN" | "STARTSWITH" | "ENDSWITH" | "CONTAINS" | "DOES NOT CONTAIN" | "INSTANCEOF";

/**
 * Query operator values that can be used for numerical operations.
 */
declare type NumberQueryOperator = "=" | "!=" | ">" | ">=" | "<" | "<=";

/**
 * Query operator values.
 */
declare type QueryOperator = StringQueryOperator | NumberQueryOperator;

/**
 * Type values for the "Type" field of the "Service Availibility" table (service_availility.type).
 */
declare type service_availabilityType = "daily" | "weekly" | "monthly" | "annually" | "last7days" | "last30days" | "last12months";

declare type AggregateType = "min" | "max" | "sum" | "avg" | "count";

declare type GlideProgressWorkerState = "starting" | "running" | "complete" | "cancelled" | "unknown";

declare type GlideProgressWorkerCompletionCode = "success" | "cancelled" | "error";

/**
 * Represents an object type.
 */
type GlideJSType = "null" | "string" | "number" | "boolean" | "function" | "object";

/**
 * Names of workflow events availablein the base system.
 * 
 * This was created by referencing {@link https://docs.servicenow.com/bundle/utah-servicenow-platform/page/administer/workflow-administration/reference/r_WorkflowEventsInTheBaseSystem.html}
 * @enum {string}
 */
declare enum BaseSystemWorkflowEventNames {
    /**
     * Informs records in the Workflow Executing Activity [wf_executing] table about the completion of other activities in the same workflow context.
     * If the activity is allowed to set the boolean value for wf_executing.notify_termination, then set the value to true (activity.notify_termination = true) during the onExecute event.
     * Source: Workflow Engine, Process Terminations; Thread: Current thread, current mutex; Listeners: Join activity
     * @summary String value used by activity definitions to respond to the onActivityComplete event handler.
     */
    activityComplete = "activityComplete",
    /**
     * From within activity definitions:
     * 
     * Informs all wf_executing records in a context that the workflow is being canceled.
     * The End activity uses the global workflow.broadcastEvent('cancel') to interrupt the currently running wf_executing records. This changes the state of those records to Cancelled.
     * Source: End activity; Thread: Current thread, current mutex; Listeners: All activities, onCancel event handler
     * 
     * Outside current context:
     * 
     * This event is the same as the cancel event above and handled the same. However, the management of this event is subtly different.
     * This event informs all wf_executing records in a context that the workflow is being canceled.
     * The event is managed via the onCancel event handler of each executing activity definition, but the event is called differently.
     * In particular, the call to cancel from outside an activity definition is blocked by the current mutex.
     * This is a significant difference in that the event does not interrupt a currently executing activity that is still operating within the parameters of the current mutex.
     * Any script can call cancel on a known executing context via the workflow script include.
     * Source: Any script include, scheduled job, UI action, or other source; Thread: Current thread, private mutex; Listeners:All activities, onCancel event handler
     * @summary String value used by workflow activities to respond to a request for cancellation.
     */
    cancel = "cancel",
    /**
     * Informs wf_executing records for approval activities about an approval that completed and triggered the timer event.
     * Approval Coordinator both registers for the event and triggers the event. The child approvals have listeners that determine their approval state.
     * Source: Approval Coordinator triggers the event during its onExecute; Thread: Current thread, current mutex;
     * Listeners: Approval Coordinator, Approval - User, and Approval - Group all have onDetermineApprovalState event handlers.
     * If the state is anything but Requested, the activity is considered finished, and the approval state (Approved, Rejected, Cancelled) is set to the wf_executing.result column
     * @summary String value used by approval activities to respond to a change in the overall approval status of the current record.
     */
    determineApprovalState = "determineApprovalState",
    /**
     * Informs a record in the wf_executing table with the initial state of Executing to proceed with its primary work.
     * The workflow engine, for each transition executed, creates an executing record with a state of Executing. Once created, the executing record is put in a queue for processing.
     * For each item in the queue, the Rhino globals are established, the activity definition that drives the executing record is instantiated, and the run() function is called.
     * When the state of a record is Executing, this function always calls onExecute.
     * Source: Workflow engine, via the WFActivity Handler; Thread: Current thread, current mutex; Listeners: All activities, onExecute event handler
     * 
     * Lock Activities
     * 
     * This is also the string value used by the Lock activity to respond to a waiting lock that is ready to make another attempt to obtain a specific lock.
     * For Lock activities, this informs a wf_executing record waiting to execute that the specified wait interval has passed and that it should attempt to get the lock again.
     * The execute event for a Lock activity is different because it is called on a separate thread, at specified intervals, and is treated as an outside event.
     * Source: Lock activity via a scheduled job; Thread: Worker thread, private mutex; Listeners: Lock activity, onExecute event handler
     * @summary String value used by workflow activities to respond to a Timer activity that has expired.
     */
    execute = "execute",
    /**
     * When a main workflow calls a subflow, the workflow keeps the ID of the subflow's context in the scratchpad. When the subflow is complete, it triggers the listener event via a business rule.
     * The listener event is passed to the parent context on completion of a subflow and is managed by the onListener action of the workflow activity.
     * Source: Business rule that is triggered by the update of a workflow that has a parent; Thread: Current thread, current mutex;
     * Listeners: This event is used by a subflow to inform it's parent flow that it is complete. The parent workflow will react to this event and continue.
     * @summary String value that the workflow (subflow) activity triggers as an event.
     */
    listener = "listener",
    /**
     * When an SLA is paused, the SLA workflows must be paused if there is a timer running.
     * Use is exclusive to the SLA timer.
     * Source: SLA; Thread: Business rule thread, private mutex; Listeners: Timer activity
     * @summary String value sent to a workflow from an SLA to pause the Timer activity.
     */
    pause = "pause",
    /**
     * String value triggered in the workflow by an Orchestration activity indicating that the MID Server has completed a task.
     * The onProbe_complete event handler is in the WebServiceActivityHandler and is used by most Orchestration activities.
     * Source: Event used to restate a workflow that is waiting for the MID Server to process a task or activity; Thread: Worker thread, private mutex; Listeners: Orchestration activities
     * @summary String value triggered in the workflow by an Orchestration activity indicating that the MID Server has completed a task.
     */
    probe_complete = "probe_complete",
    /**
     * When an SLA is resumed, the SLA workflows must be resumed as well.
     * Use is exclusive to the SLA timer.
     * Source: SLA; Thread: Business rule thread, private mutex; Listeners: Timer activity
     * @summary String value used by the Timer activity to resume a paused timer (see {@link BaseSystemWorkflowEventNames#pause}).
     */
    resume = "resume",
    /**
     * The End activity checks for this event. This is only in the End activity.
     * Source: Any script can trigger or broadcast the stop event via a script include or workflow Run Script activity; Thread: Current thread, current mutex;
     * Listeners: This event is used by the End activity to exclude the Cancel activity and allow a workflow to end, even if canceled.
     * @summary The End activity checks for this event.
     */
    stop = "stop",
    /**
     * Allows wf_executing records to be informed about a timer activity that has completed and has fired the timer event.
     * The Timer activity schedules a job that calls a script. The script calls fireEvent (wf_executing, timer).
     * Source: Timer activity via a scheduled job; Thread: Worker thread, private mutex; Listeners: Timer activity, onTimer event handler
     * @summary String value used by workflow activities to respond to a Timer activity that has expired.
     */
    timer = "timer",
    /**
     * Informs records in the Workflow Executing Activity [wf_executing] table about an otherEvent that has completed.
     * The Join activity transitions from n number of preceding activities. These preceding activities all create a wf_executing record, which causes a check to see if the record already exists.
     * If the Join already exists, then the Join created by the executing transition sets the wf_executing record for deletion.
     * Source: Join activity; Thread: Current thread, current mutex; Listeners: Join activity, onOtherEvent event handler
     * @summary String value used by the Join activity to respond to an otherEvent.
     */
    otherEvent = "otherEvent"
}

declare type WorkflowOperationType = "insert" | "update" | "delete";

/**
 * Date format value
 * 
 * full: returns the date in the format of <weekday, month, day, and year> according to the user's locale (for example, Monday, October 24, 2022)
 * long: returns the date in the format of <month, day, and year> according to the user's locale (for example, October 24, 2022).
 * medium: similar to long but returns the date with an abbreviated month (for example, Oct 24, 2022).
 * short: returns the date in the format of MM/DD/YY or DD/MM/YY according to the user's locale (for example, 10/24/2022 or 24/10/2022)
 */
declare type DateDislayValueStyle = "full" | "long" | "medium" | "short";

//#endregion

/**
 * Defines members that are common to both GlideRecord and GlideElement objects
 */
interface IDbObject {
    /**
     * Determines if the user's role permits the creation of new records in this field.
     * @returns {boolean} True if the field can be created, false otherwise.
     */
    canCreate(): boolean;
    /**
     * Indicates whether the user's role permits them to read the associated GlideRecord.
     * @returns {boolean} True if the field can be read, false otherwise.
     */
    canRead(): boolean;
    /**
     * Determines whether the user's role permits them to write to the associated GlideRecord.
     * @returns {boolean} True if the user can write to the field, false otherwise.
     */
    canWrite(): boolean;
    /**
     * Returns the value of the specified attribute from the dictionary.
     * @param {string} attributeName - Attribute name
     * @returns {string} Attribute value or null if attribute is not found.
     */
    getAttribute(attributeName: string): string | null;
    /**
     * Returns the element's descriptor.
     * @returns {GlideElementDescriptor} Element's descriptor.
     */
    getED(): GlideElementDescriptor;
    /**
     * Returns the object label.
     * @returns {string} Object label.
     */
    getLabel(): string;
    /**
     * Returns for Element objects, returns the name of the table on which the field resides; Otherwise, retrieves the name of the table associated with the GlideRecord.
     * @returns {string} Name of the table. The returned value may be different from the table Class that the record is in. See Tables and Classes in the product documentation.
     */
    getTableName(): string;
}

/**
 * The Scoped GlideElement API provides a number of convenient script methods for dealing with fields and their values. Scoped GlideElement methods are available for the fields of the current GlideRecord.
 */
declare interface IGlideElement extends IDbObject {
    /**
     * Determines if the current field has been modified. This functionality is available for all available data types, except Journal fields.
     * @returns {boolean} True if the fields have been changed, false if the field has not.
     */
    changes(): boolean;
    /**
     * Determines if the previous value of the current field matches the specified object.
     * @param {*} o - An object value to check against the previous value of the current field.
     * @returns {boolean} True if the previous value matches, false if it does not.
     */
    changesFrom(o: any): boolean;
    /**
     * Determines if the new value of a field, after a change, matches the specified object.
     * @param {*} o - An object value to check against the new value of the current field.
     * @returns {boolean} True if the previous value matches, false if it does not.
     */
    changesTo(o: any): boolean;
    /**
     * Returns the number of milliseconds since January 1, 1970, 00:00:00 GMT for a duration field. Does not require the creation of a GlideDateTime object because the duration field is already a GlideDateTime object.
     * @returns {number} Number of milliseconds since January 1, 1970, 00:00:00 GMT.
     */
    dateNumericValue(): number;
    /**
     * Returns the Boolean value of the specified attribute from the dictionary.
     * @param {string} attributeName - Attribute name
     * @returns {boolean} Boolean value of the attribute. Returns false if the attribute does not exist.
     */
    getBooleanAttribute(attributeName: string): boolean;
    /**
     * Generates a choice list for a field.
     * @param {string} [dependent] - A dependent value
     * @returns {Array<*>} An array list of choices.
     */
    getChoices(dependent?: string): any[];
    /**
     * Returns the choice label for the current choice.
     * @returns {string} The selected choice's label.
     */
    getChoiceValue(): string;
    /**
     * Returns the clear text value for Password (2 way encrypted) fields in applications.
     * @returns {string} The clear text password.
     */
    getDecryptedValue(): string;
    /**
     * Gets the formatted display value of the field.
     * @param {number} [maxCharacters] - Maximum characters desired
     * @returns {string} The display value of the field.
     */
    getDisplayValue(maxCharacters?: number): string;
    getError(): string;
    /**
     * Returns the phone number in international format.
     * @returns {string} The phone number in international format.
     */
    getGlobalDisplayValue(): string;
    /**
     * Returns the HTML value of a field.
     * @param {number} [maxChars] - Maximum number of characters to return.
     * @returns {string} HTML value for the field.
     */
    getHTMLValue(maxChars?: number): string;
    /**
     * Returns either the most recent journal entry or all journal entries.
     * @param {number} mostRecent - If 1, returns the most recent entry. If -1, returns all journal entries.
     * @returns {string} For the most recent entry, returns a string that contains the field label, timestamp, and user display name of the journal entry.For all journal entries, returns the same information for all journal entries ever entered as a single string with each entry delimited by "\n\n".
     */
    getJournalEntry(mostRecent: number): string;
    /**
     * Returns the name of the field.
     * @returns {string} Field name.
     */
    getName(): string;
    /**
     * Determines if a field is null.
     * @returns {boolean} True if the field is null or an empty string, false if not.
     */
    nil(): boolean;
    /**
     * Sets the value of a date/time element to the specified number of milliseconds since January 1, 1970 00:00:00 GMT.
     * @param {number} milliseconds - Number of milliseconds since 1/1/1970
     */
    setDateNumericValue(milliseconds: number): void;
    /**
     * Sets the display value of the field.
     * @param {*} value - The value to set for the field.
     */
    setDisplayValue(value: any): void;
    /**
     * Adds an error message. Available in Fuji patch 3.
     * @param {string} errorMessage - The error message.
     */
    setError(errorMessage: string): void;
    /**
     * Sets the field to the specified phone number.
     * @param {*} phoneNumber - The phone number to set. This can be in either the international or local format.
     * @param {boolean} strict - When true, specifies that the number specified must match the correct format. When false, the system attempts to correct an improperly formatted phone number.
     * @returns {boolean} True if the value was set.
     */
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    /**
     * Sets the value of a field.
     * @param {*} value - Object value to set the field to.
     */
    setValue(value: any): void;
}

declare class Choice {
    constructor(value: $$rhino.String, label: $$rhino.String, sys_id?: $$rhino.String);
    getLabel(): Packages.java.lang.String;
    setLabel(n: $$rhino.String): void;
    getRawLabel(): Packages.java.lang.String;
    getValue(): Packages.java.lang.String;
    setValue(n: $$rhino.String): void;
    getId(): Packages.java.lang.String;
    setId(n: $$rhino.String): void;
    getImage(): Packages.java.lang.String;
    setImage(n: $$rhino.String): void;
    getED(): GlideElementDescriptor;
    getSelected(): $$rhino.Boolean;
    setSelected(bln: $$rhino.Boolean): void;
    getUsed(): $$rhino.Boolean;
    getRefFileName(): Packages.java.lang.String;
    setRefFileName(n: $$rhino.String): void;
    getParameter(): any;
    getParameters(): Packages.java.util.HashMap<$$rhino.String, any>;
    compareTo(o: any): $$rhino.Boolean;
    isReference(): $$rhino.Boolean;
    isMissing(): $$rhino.Boolean;
}

/**
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/CMDBQueryBuilderAPI/concept/CMDBQueryBuilderAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/CMDBQueryBuilderAPI}
 * @todo Add CMDBQueryBuilderAPI members
 */
declare class CMDBQueryBuilderAPI {
}

/**
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/CMDBUtil/concept/c_CMDBUtilAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_CMDBUtilAPI}
 * @todo Add CMDBUtil members
 */
declare class CMDBUtil {
}

/**
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/ColorBoth/concept/ColorBothAPI.html}
 * @todo Add Color members
 */
declare class Color {
}

declare class EmailWatermark {
    getWatermark(): string;
}

/**
 * The EmailOutbound class implements the email object for applications.
* @description You can use the EmailOutbound methods with the email global object available in mail scripts.The email object behaves identically for global and applications.
 */
declare class EmailOutbound extends GlideEmail {
    constructor(actionType_OR_action?: string | GlideRecord, m?: EmailWatermark);
    save(): void;
}

/**
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/Event/concept/EventAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/EventAPI}
 * @todo Add Event members
 */
declare class Event {
}

/**
 * The action API enables handling data for URLs in a UI action script.
 * @see {@link https://docs.servicenow.com/bundle/vancouver-api-reference/page/app-store/dev_portal/API_reference/Action/concept/ActionAPIBoth.html}
 */
declare class GlideActionURL {
    /**
     * Gets a GlideURI object to determine the user view.
     * @return {GlideURI} GlideURI object representing the URI parameter of the user view.
     */
    getGlideURI(): GlideURI;

    /**
     * Gets the URL of the return page in view after a UI action is complete.
     * @return {string} URL of the return page in view after a UI action is complete.
     */
    getReturnURL(): string;

    /**
     * Gets the value of a URL parameter.
     * @param {string} parameterName - Name of the URL parameter name to be queried for the URL parameter value.
     * @return {string} URL parameter value or undefined if not found.
     * @example
     * action.getURLParameter('sysparm_query');
     */
    getURLParameter(parameterName: string): string | undefined;

    /**
     * Opens a page with a GlideRecord in the user view.
     * @param {GlideRecord} gr - GlideRecord of the page to be opened in the user view.
     */
    openGlideRecord(gr: GlideRecord): void;

    /**
     * Indicates whether to enable or disable pop-up windows on the page in the current view.
     * @param {boolean} noPop - Flag indicating whether to enable or disable pop-up windows on the page, where true disables pop-up windows.
     */
    setNoPop(noPop: boolean): void;

    /**
     * Sets the redirect URI for this transaction, which determines the next page the user sees.
     * @param {(string | GlideRecord)} url - URL or GlideRecord to set as the redirect.
     */
    setRedirectURL(url: string | GlideRecord): void;

    /**
     * Sets the return URI for this transaction after a UI action is complete. You can use this method to determine what page the user has in view when they return from submit.
     * @param {(string | GlideRecord)} url - URI to set as the return location after a UI action is complete.
     */
    setReturnURL(url: string | GlideRecord): void;

    /**
     * Sets a URL parameter name and value.
     * @param {string} parameterName - Name of the URL parameter.
     * @param {string} parameterValue - Value of the parameter.
     * @example
     * action.setURLParameter('sysparm_query', 'priority=2^active=true');
     */
    setURLParameter(parameterName: string, parameterValue: string): void;
}

/**
 * GlideAggregate enables you to easily create database aggregation queries.
 * @description The GlideAggregate class is an extension of GlideRecord and provides database aggregation (COUNT, SUM, MIN, MAX, AVG) queries. This functionality can be helpful when creating customized reports or in calculations for calculated fields. The GlideAggregate class works only on number fields.When you use GlideAggregate on currency or price fields, you are working with the reference currency value. Be sure to convert the aggregate values to the user's session currency for display. Because the conversion rate between the currency or price value (displayed value) and its reference currency value (aggregation value) might change, the result may not be what the user expects.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideAggregate/concept/c_GlideAggregateAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_GlideAggregateAPI}
 */
declare class GlideAggregate extends GlideRecord {
    /**
     * Creates a GlideAggregate object on the specified table.
     * @constructor
     * @param {string} tableName - Name of the table.
     */
    constructor(tableName: string);
    /**
     * Adds an aggregate.
     * @param {string} agg - Name of the aggregate to add, for example, COUNT, MIN, or MAX
     * @param {string} [name] - Name of the column to aggregate. Null is the default.
     */
    addAggregate(agg: string, name?: string): void;
    /**
     * Adds a trend for a field.
     * @param {string} fieldName - The name of the field for which trending should occur.
     * @param {string} timeInterval - The time interval for the trend. The following choices are available: Year, Quarter, Date, Week, DayOfWeek, Hour, Value.
     */
    addTrend(fieldName: string, timeInterval: string): void;
    /**
     * Gets the value of an aggregate from the current record.
     * @param {string} agg - The type of the aggregate, for example, SUM or Count.
     * @param {string} [name] - Name of the field to get the aggregate from.
     * @returns {string} The value of the aggregate.
     */
    getAggregate(agg: string, name?: string): string;
    /**
     * Gets the query necessary to return the current aggregate.
     * @returns {string} The encoded query to get the aggregate.
     */
    getAggregateEncodedQuery(): string;
    /**
     * Provides the name of a field to use in grouping the aggregates.
     * @param {string} name - Name of the field.
     */
    groupBy(name: string): void;
    /**
     * Orders the aggregates based on the specified aggregate and field.
     * @param {string} agg - Type of aggregation.
     * @param {string} fieldName - Name of the field to aggregate.
     */
    orderByAggregate(agg: string, fieldName: string): void;
    /**
     * Sets whether the results are to be grouped.
     * @param {boolean} b - When true the results are grouped.
     */
    setGroup(b: boolean): void;
}

declare class GlideChoiceList implements Packages.java.util.ArrayList<Choice> {
    add(e: Choice): $$rhino.Boolean;
    add(index: $$rhino.Number, element: Choice): void;
    addAll(c: Packages.java.util.Collection<Choice>): $$rhino.Boolean;
    addAll(index: $$rhino.Number, c: Packages.java.util.Collection<Choice>): $$rhino.Boolean;
    clear(): void;
    clone(): Packages.java.lang.Object;
    contains(o: any): $$rhino.Boolean;
    ensureCapacity(minCapacity: $$rhino.Number): void;
    get(index: $$rhino.Number): Choice;
    isEmpty(): $$rhino.Boolean;
    iterator(): Packages.java.util.Iterator<Choice>;
    lastIndexOf(o: any): $$rhino.Number;
    listIterator(): Packages.java.util.ListIterator<Choice>;
    listIterator(index: $$rhino.Number): Packages.java.util.ListIterator<Choice>;
    remove(index: $$rhino.Number): Choice;
    remove(o: any): $$rhino.Boolean;
    removeAll(c: Packages.java.util.Collection<any>): $$rhino.Boolean;
    retainAll(c: Packages.java.util.Collection<any>): $$rhino.Boolean;
    set(index: $$rhino.Number, element: Choice): Choice;
    size(): $$rhino.Number;
    subList(fromIndex: $$rhino.Number, toIndex: $$rhino.Number): Packages.java.util.List<Choice>;
    toArray();
    trimToSize(): void;
    hashCode(): $$rhino.Number;
    containsAll(c: Packages.java.util.Collection<any>): $$rhino.Boolean;
    toString(): $$rhino.String;
    equals(obj: any): $$rhino.Boolean;
    sort(): void;
    getChoice(i: $$rhino.Number | $$rhino.String): Choice;
    getChoiceNoTrim(i: $$rhino.String): Choice;
    getSelectedIndex(): Packages.java.lang.Integer;
    indexOf(o: any): Packages.java.lang.Integer;
    indexOfNoTrim(o: any): Packages.java.lang.Integer;
    addFirst(c: Choice): void;
    addNone(): Choice;
    removeNone(): void;
    removeChoice(i: $$rhino.Number | $$rhino.String): Choice;
    removeChoice(c: Choice): void;
    getLabelOf(value: $$rhino.String): Packages.java.lang.String;
    getValueOf(label: $$rhino.String): Packages.java.lang.String;
    has(value: $$rhino.String): $$rhino.Boolean;
    isReferenceList(): $$rhino.Boolean;
    getSize(): Packages.java.lang.Integer;
    static getChoiceList(tableName: $$rhino.String, fieldName: $$rhino.String): ChoiceList;
}

declare class GlideChoiceListSet {
    constructor();
    getColumns(): GlideChoiceList;
    getSelected(): GlideChoiceList;
    setColumns(clColumns: GlideChoiceList): void;
    setSelected(clSelected: GlideChoiceList): void;
    sortColumns(): void;
    toXML(): any;
}

/**
 * The GlideDate class provides methods for performing operations on GlideDate objects, such as instantiating GlideDate objects or working with GlideDate fields.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideDate/concept/GlideDateAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_GlideDateGlobalAPI}
 */
declare class GlideDate {
    /**
     * Creates an instance of GlideDate with the current date time.
     */
    constructor();

    /**
     * Adds a specified number of days to the current GlideDate object.
     * A negative parameter subtracts days.
     * @param {number} days - The number of days to add. Use a negative value to subtract.
     */
    addDays(days: number): void;

    /**
     * Adds a specified number of days to the current GlideDate object.
     * A negative parameter subtracts days.
     * The method determines the local date equivalent to the value stored by the GlideDate object,
     * then adds or subtracts days using the local date values.
     * @param {number} days - The number of days to add. Use a negative value to subtract.
     */
    addDaysLocalTime(days: number): void;

    /**
     * Adds a specified number of days to the current GlideDate object.
     * A negative parameter subtracts days. The method determines the UTC date equivalent to the value stored by the GlideDate object,
     * then adds or subtracts days using the UTC date values.
     * @param {number} days - The number of days to add. Use a negative number to subtract.
     */
    addDaysUTC(days: number): void;

    /**
     * Adds a specified number of months to the current GlideDate object.
     * A negative parameter subtracts months. The method determines the local date equivalent to the value stored by the GlideDate object,
     * then adds or subtracts months using the local date values.
     * @param {number} months - The number of months to add. use a negative value to subtract.
     */
    addMonthsLocalTime(months: number): void;

    /**
     * Adds a specified number of months to the current GlideDate object.
     * A negative parameter subtracts months. The method determines the UTC date equivalent to the value stored by the GlideDate object,
     * then adds or subtracts months using the UTC date values.
     * @param {number} months - The number of months to add. use a negative value to subtract.
     */
    addMonthsUTC(months: number): void;

    /**
     * Adds a specified number of weeks to the current GlideDate object.
     * A negative parameter subtracts weeks. The method determines the local date equivalent to the value stored by the GlideDate object,
     * then adds or subtracts weeks using the local date values.
     * @param {number} weeks - The number of weeks to add. Use a negative value to subtract.
     */
    addWeeksLocalTime(weeks: number): void;

    /**
     * Adds a specified number of weeks to the current GlideDate object.
     * A negative parameter subtracts weeks. The method determines the UTC date equivalent to the value stored by the GlideDate object,
     * then adds or subtracts weeks using the UTC date values.
     * @param {number} weeks - The number of weeks to add. Use a negative value to subtract.
     */
    addWeeksUTC(weeks: number): void;

    /**
     * Adds a specified number of years to the current GlideDate object.
     * A negative parameter subtracts years. The method determines the local date equivalent to the value stored by the GlideDate object,
     * then adds or subtracts years using the local date values.
     * @param {number} years - The number of weeks to add. Use a negative value to subtract.
     */
    addYearsLocalTime(years: number): void;

    /**
     * Adds a specified number of years to the current GlideDate object.
     * A negative parameter subtracts years. The date value stored by GlideDate object is interpreted as being in the UTC time zone.
     * @param {number} years - The number of weeks to add. Use a negative value to subtract.
     */
    addYearsUTC(years: number): void;

    /**
     * Determines if the GlideDate object's date occurs after the specified object's date.
     * @param {(GlideDateTime | GlideDate)} gdt - Date to check against.
     * @return {boolean} True if the current GlideDate object date is after the specified object's date;
     * otherwise, false if the current GlideDate object date is before or equal to the specified object's date.
     */
    after(gdt: GlideDateTime | GlideDate): boolean;

    /**
     * Determines if the GlideDate object's date occurs before the specified GlideDate object's date.
     * @param {(GlideDateTime | GlideDate)} gdt - Date to check against.
     * @return {boolean} True if the current GlideDate object date is before the specified object's date;
     * otherwise, false if the current GlideDate object date is after or equal to the specified object's date.
     */
    before(gdt: GlideDateTime | GlideDate): boolean;

    equals(obj: GlideDate): boolean;
    
    /**
     * Gets the date in the specified date format.
     * @param {string} format - Desired date format using Java SimpleDateFormat. For example, "dd-MM-yyyy" to get the day, month, and year, or "EEEE" to get the day of the week.
     * @return {number}
     * @see {@link https://docs.oracle.com/javase/10/docs/api/java/text/SimpleDateFormat.html}
     */
    getByFormat(format: string): number;

    /**
     * Gets the day of the month stored by the GlideDate object, expressed in the current user's time zone.
     * @return {number} The day of the month in the user's time zone, from 1 to 31.
     */
    getDayOfMonthLocalTime(): number;

    /**
     * Gets the day of the month stored by the GlideDate object, expressed in the UTC time zone.
     * @return {number}
     */
    getDayOfMonthNoTZ(): number;

    /**
     * Gets the day of the month stored by the GlideDate object, expressed in the UTC time zone.
     * @return {number} The day of the month in the UTC time zone, from 1 to 31.
     */
    getDayOfMonthUTC(): number;

    /**
     * Gets the day of the week stored by the GlideDate object, expressed in the user's time zone.
     * @return {number} The day of the month in the UTC time zone, from 1 to 31.
     */
    getDayOfWeekLocalTime(): number;

    /**
     * Gets the day of the week stored by the GlideDate object, expressed in the UTC time zone.
     * @return {number} The day of week value from 1 to 7. Monday equals 1, Sunday equals 7.
     */
    getDayOfWeekUTC(): number;

    /**
     * Gets the number of days in the month stored by the GlideDate object, expressed in the current user's time zone.
     * @return {number} The number of days in the current month in the user's time zone.
     */
    getDaysInMonthLocalTime(): number;

    /**
     * Gets the date in the current user's display format and time zone.
     * @return {string}
     */
    getDisplayValue(): string;

    /**
     * Gets the date in the internal format (yyyy-MM-dd) and current user's timezone.
     * @return {string}
     */
    getDisplayValueInternal(): string;

    /**
     * Gets the month stored by the GlideDate object, expressed in the current user's time zone.
     * @return {number} The numerical value of the month.
     */
    getMonthLocalTime(): number;

    /**
     * Gets the month stored by the GlideDate object, expressed in the UTC time zone.
     * @return {number}
     */
    getMonthNoTZ(): number;

    /**
     * Gets the month stored by the GlideDate object, expressed in the UTC time zone.
     * @return {number} The numerical value of the month.
     */
    getMonthUTC(): number;

    /**
     * Gets the number of the week stored by the GlideDate object, expressed in the current user's time zone.
     * All weeks begin on Sunday. The first week of the year is the week that contains at least one day of the new year.
     * The week beginning Sunday 2015-12-27 is considered the first week of 2016 as that week contains January 1 and 2.
     * @return {number} The number of the current week in local time. The highest week number in a year is either 52 or 53.
     */
    getWeekOfYearLocalTime(): number;

    /**
     * Gets the number of the week stored by the GlideDate object, expressed in the UTC time zone.
     * All weeks begin on Sunday. The first week of the year is the week that contains at least one day of the new year.
     * The week beginning Sunday 2015-12-27 is considered the first week of 2016 as that week contains January 1 and 2.
     * @return {number} The number of the current week in UTC time. The highest week number in a year is either 52 or 53.
     */
    getWeekOfYearUTC(): number;

    /**
     * Gets the year stored by the GlideDate object, expressed in the current user's time zone.
     * @return {number} Four-digit year value in the user's time zone.
     */
    getYearLocalTime(): number;

    /**
     * Gets the year stored by the GlideDate object, expressed in the UTC time zone.
     * @return {number} 4-digit year value in the UTC time zone.
     */
    getYearUTC(): number;

    /**
     * Gets the date in the internal format (yyyy-MM-dd) and the system time zone (UTC by default).
     * @return {string}
     */
    getValue(): string;

    /**
     * Gets the year stored by the GlideDate object, expressed in the UTC time zone.
     * @return {number}
     */
    getYearNoTZ(): number;

    /**
     * Determines if the GlideDate object's date occurs on or after the specified GlideDate object's date.
     * @param {(GlideDateTime | GlideDate)} gd - Date to check against.
     * @return {boolean} True if the current GlideDate object date is on or after the specified object's date; otherwise, false.
     */
    onOrAfter(gd: GlideDateTime | GlideDate): boolean;

    /**
     * Determines if the GlideDate object's date occurs on or before the specified GlideDate object's date.
     * @param {(GlideDateTime | GlideDate)} gd - Date to check against.
     * @return {boolean} True if the current GlideDate object date is on or before the specified object's date; otherwise, false.
     */
    onOrBefore(gd: GlideDateTime | GlideDate): boolean;

    /**
     * Sets the day of the month to a specified value in the current user's time zone.
     * @param {number} day - The day of month to change to, from 1 to 31.
     * If this value is greater than the maximum number of days in the month, the value is set to the last day of the month.
     */
    setDayOfMonthLocalTime(day: number): void;

    /**
     * Sets the day of the month to a specified value in the UTC time zone.
     * @param {number} day - The day of month to change to, from 1 to 31.
     * If this value is greater than the maximum number of days in the month, the value is set to the last day of the month.
     */
    setDayOfMonthUTC(day: number): void;

    /**
     * Sets a date value using the current user's display format and time zone.
     * @param {string} asDisplayed -
     */
    setDisplayValue(asDisplayed: string): void;

    /**
     * Sets the month stored by the GlideDate object to the specified value using the current user's time zone.
     * @param {number} month - The month to change to.
     */
    setMonthLocalTime(month: number): void;

    /**
     * Sets the month stored by the GlideDate object to the specified value using the UTC time zone.
     * @param {number} month - The month to change to.
     */
    setMonthUTC(month: number): void;

    /**
     * Sets the date of the current GlideDate object in internal format (yyyy-MM-dd) and the system time zone (UTC by default).
     * @param {string} o -
     */
    setValue(o: string): void;

    /**
     * Sets the year stored by the GlideDate object to the specified value using the current user's time zone.
     * @param {number} year - The year to change to.
     */
    setYearLocalTime(year: number): void;

    /**
     * Sets the year stored by the GlideDate object to the specified value using the UTC time zone.
     * @param {number} year - The year to change to.
     */
    setYearUTC(year: number): void;

    /**
     * Gets the duration difference between two GlideDate values.
     * @static
     * @param {GlideDate} start - 
     * @param {GlideDate} end - 
     * @return {GlideDuration}
     */
    static subtract(start: GlideDate, end: GlideDate): GlideDuration;

    toString(): string;
    
    hashCode(): $$rhino.Number;
}

/**
 * The GlideDateTime class provides methods for performing operations on GlideDateTime objects, such as instantiating GlideDateTime objects or working with glide_date_time fields.
 * @description Use the GlideDateTime methods to perform date-time operations, such as instantiating a GlideDateTime object, performing date-time calculations, formatting a date-time, or converting between date-time formats.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideDateTime/concept/c_GlideDateTimeAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_GlideDateTimeAPI}
 */
declare class GlideDateTime {
    /**
     * Creates an instance of GlideDateTime with the current date time time.
     */
    constructor();

    /**
     * Creates an instance of GlideDateTime set to the time of the GlideDateTime object passed in the parameter.
     * @param {GlideDateTime} g - The GlideDateTime object to use for setting the time of the new object.
     */
    constructor(g: GlideDateTime);

    /**
     * Creates an instance of GlideDateTimefrom a date and time value in the UTC time zone specified with the format yyyy-MM-dd HH:mm:ss.
     * @param {string} value - A UTC date and time using the internal format yyyy-MM-dd HH:mm:ss.
     */
    constructor(value: string);

    /**
     * Adds a GlideTime object to the current GlideDateTime object.
     * @param {GlideTime} gd - GlideTime object whose time value to add to the specified GlideDateTime object.
     */
    add(gd: GlideTime): void;

    /**
     * Adds the specified number of milliseconds to the current GlideDateTime object.
     * @param {$$rhino.Number} milliseconds - The number of milliseconds to add.
     */
    add(milliseconds: number): void;
    
    /**
     * Adds a specified number of days to the current GlideDate object.
     * A negative parameter subtracts days.
     * @param {number} days - The number of days to add. Use a negative value to subtract.
     */
    addDays(days: number): void;

    /**
     * Adds a specified number of days to the current GlideDateTime object.
     * A negative parameter subtracts days.
     * The method determines the local date and time equivalent to the value stored by the GlideDateTime object,
     * then adds or subtracts days using the local date and time values.
     * @param {number} days - The number of days to add. Use a negative value to subtract.
     */
    addDaysLocalTime(days: number): void;

    /**
     * Adds a specified number of days to the current GlideDateTime object.
     * A negative parameter subtracts days. The method determines the UTC date and time equivalent to the value stored by the GlideDateTime object,
     * then adds or subtracts days using the UTC date and time values.
     * @param {number} days - The number of days to add. Use a negative number to subtract.
     */
    addDaysUTC(days: number): void;

    /**
     * Adds a specified number of months to the current GlideDateTime object.
     * A negative parameter subtracts months. The method determines the local date and time equivalent to the value stored by the GlideDateTime object,
     * then adds or subtracts months using the local date and time values.
     * @param {number} months - The number of months to add. use a negative value to subtract.
     */
    addMonthsLocalTime(months: number): void;

    /**
     * Adds a specified number of months to the current GlideDateTime object.
     * A negative parameter subtracts months. The method determines the UTC date and time equivalent to the value stored by the GlideDateTime object,
     * then adds or subtracts months using the UTC date and time values.
     * @param {number} months - The number of months to add. use a negative value to subtract.
     */
    addMonthsUTC(months: number): void;

    /**
     * Adds the specified number of seconds to the current GlideDateTime object.
     * @param {number} seconds - The number of seconds to add.
     */
    addSeconds(seconds: number): void;

    /**
     * Adds a specified number of weeks to the current GlideDateTime object.
     * A negative parameter subtracts weeks. The method determines the local date and time equivalent to the value stored by the GlideDateTime object,
     * then adds or subtracts weeks using the local date and time values.
     * @param {number} weeks - The number of weeks to add. Use a negative value to subtract.
     */
    addWeeksLocalTime(weeks: number): void;

    /**
     * Adds a specified number of weeks to the current GlideDateTime object.
     * A negative parameter subtracts weeks. The method determines the UTC date and time equivalent to the value stored by the GlideDateTime object,
     * then adds or subtracts weeks using the UTC date and time values.
     * @param {number} weeks - The number of weeks to add. Use a negative value to subtract.
     */
    addWeeksUTC(weeks: number): void;

    /**
     * Adds a specified number of years to the current GlideDateTime object.
     * A negative parameter subtracts years. The method determines the local date and time equivalent to the value stored by the GlideDateTime object,
     * then adds or subtracts years using the local date and time values.
     * @param {number} years - The number of weeks to add. Use a negative value to subtract.
     */
    addYearsLocalTime(years: number): void;

    /**
     * Adds a specified number of years to the current GlideDateTime object.
     * A negative parameter subtracts years. The date and time value stored by GlideDateTime object is interpreted as being in the UTC time zone.
     * @param {number} years - The number of weeks to add. Use a negative value to subtract.
     */
    addYearsUTC(years: number): void;

    /**
     * Determines if the GlideDateTime object's date and time occurs after the specified object's date and time.
     * @param {(GlideDateTime | GlideDate)} gdt - Date and time to check against.
     * @return {boolean} True if the current GlideDateTime object date and time is after the specified object's date and time;
     * otherwise, false if the current GlideDateTime object date and time is before or equal to the specified object's date and time.
     */
    after(gdt: GlideDateTime | GlideDate): boolean;

    /**
     * Determines if the GlideDateTime object's date and time occurs before the specified GlideDateTime object's date and time.
     * @param {(GlideDateTime | GlideDate)} gdt - Date and time to check against.
     * @return {boolean} True if the current GlideDateTime object date and time is before the specified object's date and time;
     * otherwise, false if the current GlideDateTime object date and time is after or equal to the specified object's date and time.
     */
    before(gdt: GlideDateTime | GlideDate): boolean;

    /**
     * Compares two date and time objects to determine whether they are equivalent or one occurs before or after the other.
     * @param {(GlideDateTime | GlideDate | string)} o - Date and time object in GlideDateTime format.
     * @return {number} Less than 0 if the current GlideDateTime object is less than the date specified in the parameter;
     * Greater than 0 if the current GlideDateTime is greater than the date specified in the parameter;
     * otherwise, 0 if the current GlideDateTime is equal to the date specified in the parameter.
     */
    compareTo(o: GlideDateTime | GlideDate | string): number;

    /**
     * Compares a datetime with an existing value for equality.
     * @param {(GlideDateTime | GlideDate | string)} dateTime - 
     * @return {boolean} True if the current GlideDateTime object equal to the date specified in the parameter; otherwise, false;
     */
    equals(dateTime: GlideDateTime | GlideDate | string): boolean;

    /**
     * Returns the date stored by the GlideDateTime object. Expressed in the standard format, yyyy-MM-dd, and in the system time zone, GMT by default.
     * @return {string} Date in the system time zone.
     */
    getDate(): string;

    /**
     * Gets the day of the month stored by the GlideDateTime object, expressed in the current user's time zone.
     * @return {number} The day of the month in the user's time zone, from 1 to 31.
     */
    getDayOfMonthLocalTime(): number;

    /**
     * Gets the day of the month stored by the GlideDateTime object, expressed in the UTC time zone.
     * @return {number} The day of the month in the UTC time zone, from 1 to 31.
     */
    getDayOfMonthUTC(): number;

    /**
     * Gets the day of the week stored by the GlideDateTime object, expressed in the user's time zone.
     * @return {number} The day of the month in the UTC time zone, from 1 to 31.
     */
    getDayOfWeekLocalTime(): number;

    /**
     * Gets the day of the week stored by the GlideDateTime object, expressed in the UTC time zone.
     * @return {number} The day of week value from 1 to 7. Monday equals 1, Sunday equals 7.
     */
    getDayOfWeekUTC(): number;

    /**
     * Gets the number of days in the month stored by the GlideDateTime object, expressed in the current user's time zone.
     * @return {number} The number of days in the current month in the user's time zone.
     */
    getDaysInMonthLocalTime(): number;

    /**
     * Gets the number of days in the month stored by the GlideDateTime object, expressed in the UTC time zone.
     * @return {number} The number of days in the month stored by the GlideDateTime object, expressed in the UTC time zone.
     */
    getDaysInMonthUTC(): number;

    /**
     * Gets the date and time value in the current user's display format and time zone.
     * @return {string} The date and time in the user's format and time zone.
     * Keep in mind when designing business rules or script includes that this method may return values in different formats for different users.
     */
    getDisplayValue(): string;

    /**
     * Gets the display value in the internal format (yyyy-MM-dd HH:mm:ss).
     * @return {string} The date and time values for the GlideDateTime object in the current user's time zone and the internal date and time format of yyyy-MM-dd HH:mm:ss.
     */
    getDisplayValueInternal(): string;

    // TODO: Implement getDisplayValueWithoutTZ      from com.glide.glideobject.GlideDateTime          under com.glide.script.fencing.ScopedGlideDateTime (category: datetime utilities)

    /**
     * Gets the amount of time that daylight saving time is offset.
     * @return {number} Amount of time, in milliseconds, that daylight saving is offset.
     * Returns 0 if there is no offset or if the time is not during daylight saving time.
     */
    getDSTOffset(): number;

    /**
     * Gets the current error message.
     * @return {string} The error message.
     */
    getErrorMsg(): string;

    /**
     * Returns the object's time in the local time zone and in the internal format.
     * @return {string} The object's time in the local time zone and the internal format.
     */
    getInternalFormattedLocalTime(): string;

    /**
     * Gets the date stored by the GlideDateTime object, expressed in the standard format, yyyy-MM-dd, and the current user's time zone.
     * @return {GlideDate} The date in the user's time zone.
     */
    getLocalDate(): GlideDate;

    /**
     * Returns a GlideTime object that represents the time portion of the GlideDateTime object in the user's time zone.
     * @return {GlideTime} The time in the user's time zone.
     */
    getLocalTime(): GlideTime;

    /**
     * Gets the month stored by the GlideDateTime object, expressed in the current user's time zone.
     * @return {number} The numerical value of the month.
     */
    getMonthLocalTime(): number;

    /**
     * Gets the month stored by the GlideDateTime object, expressed in the UTC time zone.
     * @return {number} The numerical value of the month.
     */
    getMonthUTC(): number;

    /**
     * Gets the number of milliseconds since January 1, 1970, 00:00:00 GMT.
     * @return {number} The number of milliseconds since January 1, 1970, 00:00:00 GMT.
     */
    getNumericValue(): number;

    /**
     * Returns a GlideTime object that represents the time portion of the GlideDateTime object.
     * @return {GlideTime} The Unix duration stamp in system format based on GMT time.
     */
    getTime(): GlideTime;

    /**
     * Gets the time zone offset in milliseconds.
     * @return {number} The number of milliseconds of time zone offset.
     */
    getTZOffset(): number;

    /**
     * Returns the object's time in the local time zone and in the user's format.
     * @return {string} The object's time in the local time zone and in the user's format.
     */
    getUserFormattedLocalTime(): string;

    // TODO: Implement getUTCValue                   from com.glide.glideobject.GlideDateTime          under com.glide.script.fencing.ScopedGlideDateTime (category: datetime utilities)

    /**
     * Gets the date and time value stored by the GlideDateTime object in the internal format, yyyy-MM-dd HH:mm:ss, and the system time zone, UTC by default.
     * @return {string} The date and time value in the internal format and system time zone.
     */
    getValue(): string;

    /**
     * Gets the number of the week stored by the GlideDateTime object, expressed in the current user's time zone.
     * All weeks begin on Sunday. The first week of the year is the week that contains at least one day of the new year.
     * The week beginning Sunday 2015-12-27 is considered the first week of 2016 as that week contains January 1 and 2.
     * @return {number} The number of the current week in local time. The highest week number in a year is either 52 or 53.
     */
    getWeekOfYearLocalTime(): number;

    /**
     * Gets the number of the week stored by the GlideDateTime object, expressed in the UTC time zone.
     * All weeks begin on Sunday. The first week of the year is the week that contains at least one day of the new year.
     * The week beginning Sunday 2015-12-27 is considered the first week of 2016 as that week contains January 1 and 2.
     * @return {number} The number of the current week in UTC time. The highest week number in a year is either 52 or 53.
     */
    getWeekOfYearUTC(): number;

    /**
     * Gets the year stored by the GlideDateTime object, expressed in the current user's time zone.
     * @return {number} Four-digit year value in the user's time zone.
     */
    getYearLocalTime(): number;

    /**
     * Gets the year stored by the GlideDateTime object, expressed in the UTC time zone.
     * @return {number} 4-digit year value in the UTC time zone.
     */
    getYearUTC(): number;

    /**
     * Determines if an object's date is set.
     * @return {boolean} True if the object date is set; otherwise, returns false.
     */
    hasDate(): boolean;

    /**
     * Determines if an object's time uses a daylight saving offset.
     * @return {boolean} True if the time is daylight saving; otherwise, returns false.
     */
    isDST(): boolean;

    /**
     * Determines if a value is a valid date and time.
     * @return {boolean} True if value is valid; otherwise, returns false.
     */
    isValid(): boolean;

    /**
     * Determines if the GlideDateTime object's date and time occurs on or after the specified GlideDateTime object's date and time.
     * @param {(GlideDateTime | GlideDate)} gdt - Date and time to check against.
     * @return {boolean} True if the current GlideDateTime object date and time is on or after the specified object's date and time; otherwise, false.
     */
    onOrAfter(gdt: GlideDateTime | GlideDate): boolean;

    /**
     * Determines if the GlideDateTime object's date and time occurs on or before the specified GlideDateTime object's date and time.
     * @param {(GlideDateTime | GlideDate)} gdt - Date and time to check against.
     * @return {boolean} True if the current GlideDateTime object date and time is on or before the specified object's date and time; otherwise, false.
     */
    onOrBefore(gdt: GlideDateTime | GlideDate): boolean;

    /**
     * Sets the day of the month to a specified value in the current user's time zone.
     * @param {number} day - The day of month to change to, from 1 to 31.
     * If this value is greater than the maximum number of days in the month, the value is set to the last day of the month.
     */
    setDayOfMonthLocalTime(day: number): void;

    /**
     * Sets the day of the month to a specified value in the UTC time zone.
     * @param {number} day - The day of month to change to, from 1 to 31.
     * If this value is greater than the maximum number of days in the month, the value is set to the last day of the month.
     */
    setDayOfMonthUTC(day: number): void;

    /**
     * Sets a date and time value using the current user's display format and time zone.
     * @param {string} asDisplayed - The date and time in the current user's display format and time zone.
     * The parameter must be formatted using the current user's preferred display format, such as MM-dd-yyyy HH:mm:ss.
     * To assign the current date and time to a variable in a workflow script, use variable.setDisplayValue(gs.nowDateTime);.
     */
    setDisplayValue(asDisplayed: string): void;

    /**
     * Sets a date and time value using the current user's time zone and the specified date and time format.
     * This method throws a runtime exception if the date and time format used in the value parameter does not match the format parameter.
     * You can retrieve the error message by calling getErrorMsg() on the GlideDateTime object after the exception is caught.
     * @param {string} value - Date and time in the current user's time zone.
     * @param {string} format - Date and time format to use to parse the value parameter.
     * Use the following values to describe the value parameter:
     * dd=Day of the month;
     * MM=Month of the year;
     * yyyy=Year;
     * HH=Hour;
     * mm=Minutes;
     * ss=Seconds.
     * For example: "dd-MM-yyyy HH:mm:ss" or "MM-dd-yyyy HH:mm".
     */
    setDisplayValue(value: string, format: string): void;

    /**
     * Sets a date and time value using the internal format (yyyy-MM-dd HH:mm:ss) and the current user's time zone.
     * @param {string} value - The date and time in internal format.
     */
    setDisplayValueInternal(value: string): void;

    /**
     * Sets the date and time of the current object using an existing GlideDateTime object. This method is equivalent to instantiating a new object with a GlideDateTime parameter.
     * @param {GlideDateTime} g - The object to use for setting the datetime value.
     */
    setGlideDateTime(g: GlideDateTime): void;

    /**
     * Sets the month stored by the GlideDateTime object to the specified value using the current user's time zone.
     * @param {number} month - The month to change to.
     */
    setMonthLocalTime(month: number): void;

    /**
     * Sets the month stored by the GlideDateTime object to the specified value using the UTC time zone.
     * @param {number} month - The month to change to.
     */
    setMonthUTC(month: number): void;

    // TODO: Implement setNumericValue               from com.glide.script.fencing.ScopedGlideDateTime under com.glide.script.fencing.ScopedGlideDateTime (category: datetime utilities)

    /**
     * Sets the date and time of the GlideDateTime object.
     * @param {number} o -The Number value as milliseconds past January 1, 1970 00:00:00 GMT.
     */
    setValue(o: number): void;

    /**
     * Sets the date and time of the GlideDateTime object.
     * @param {GlideDateTime} o - The new date and time.
     */
    setValue(o: GlideDateTime): void;

    /**
     * Sets the date and time of the GlideDateTime object.
     * @param {string} o - A string in the UTC time zone and the internal format of yyyy-MM-dd HH:mm:ss.
     * Sets the value of the object to the specified date and time.
     * Using the method this way is equivalent to instantiating a new GlideDateTime object using the GlideDateTime(String value) constructor.
     * If the date and time format used does not match the internal format, the method attempts to set the date and time using other available formats.
     * Resolving the date and time this way can lead to inaccurate data due to ambiguity in the day and month values.
     * When using a non-standard date and time format, use setValueUTC(String dt, String format) instead.
     */
    setValue(o: string): void;

    /**
     * Sets a date and time value using the UTC time zone and the specified date and time format.
     * This method throws a runtime exception if the date and time format used in the dt parameter does not match the format parameter.
     * You can retrieve the error message by calling getErrorMsg() on the GlideDateTime object after the exception is caught.
     * @param {string} dt - The date and time to use.
     * @param {string} format - The date and time format to use.
     */
    setValueUTC(dt: string, format: string): void;

    /**
     * Sets the year stored by the GlideDateTime object to the specified value using the current user's time zone.
     * @param {number} year - The year to change to.
     */
    setYearLocalTime(year: number): void;

    /**
     * Sets the year stored by the GlideDateTime object to the specified value using the UTC time zone.
     * @param {number} year - The year to change to.
     */
    setYearUTC(year: number): void;

    /**
     * Returns the duration difference between two specified GlideDateTime objects.
     * @static
     * @param {GlideDateTime} start - Start date object.
     * @param {GlideDateTime} end - End date object.
     * @return {GlideDuration} Duration difference between the two specified dates.
     */
    static subtract(start: GlideDateTime, end: GlideDateTime): GlideDuration;

    /**
     * Subtracts a specified amount of time from the current GlideDateTime object.
     * @param {GlideTime} time - The time value to subtract.
     */
    subtract(time: GlideTime): void;

    /**
     * Subtracts the specified number of milliseconds from the GlideDateTime object.
     * @param {number} milliseconds - Number of milliseconds to subtract.
     */
    subtract(milliseconds: number): void;

    /**
     * Gets the date and time value stored by the GlideDateTime object in the internal format, yyyy-MM-dd HH:mm:ss, and the system time zone, UTC by default.
     * This method is equivalent to getValue().
     * @return {string} The date and time stored by the GlideDateTime object in the system time zone and format.
     */
    toString(): string;

    hashCode(): $$rhino.Number;
}

/**
 * The GlideDuration class provides methods for working with spans of time or durations.
 * @description GlideDuration objects store the duration as a date and time from January 1, 1970, 00:00:00. As a result, setValue() and getValue() use the GlideDateTime object for parameters and return values.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/glideDurationScoped/concept/c_GlideDurationScopedAPI.html}
 */
declare class GlideDuration {
    /**
     * Creates an instance of GlideDuration.
     */
    constructor();

    /**
     * Creates an instance of GlideDuration.
     * @param {GlideDuration} another - GlideDuration object.
     */
    constructor(another: GlideDuration);

    /**
     * Creates an instance of GlideDuration.
     * @param {number} milliseconds - Duration value in milliseconds.
     */
    constructor(milliseconds: number);

    /**
     * Creates an instance of GlideDuration.
     * @param {string} displayValue - Duration value.
     * Format: d HH:mm:ss where "d" is number of days.
     */
    constructor(displayValue: string);

    /**
     * Adds the duration of the specified GlideDuration object to the current GlideDuration object.
     * @param {GlideDuration} duration - GlideDuration object that contains the duration value to add to the current GlideDuration object.
     * @return {GlideDuration} New GlideDuration object whose duration is the sum of the durations of the two GlideDuration objects.
     */
    add(duration: GlideDuration): GlideDuration;

    equals(obj: GlideDuration): boolean;

    /**
     * Returns the duration value in the specified format.
     * @param {string} format - Duration format.
     * Format: Global date and time field format
     * @return {string} Current duration in the specified format.
     * @see {@link https://docs.servicenow.com/bundle/rome-platform-administration/page/administer/time/reference/r_FormatDateAndTimeFields.html}
     */
    getByFormat(format: string): string;

    /**
     * Returns the number of days.
     * @return {number} Number of days in the duration.
     */
    getDayPart(): number;

    /**
     * Returns the display value of the duration in number of days, hours, and minutes.
     * @return {string} Number of days, hours, and minutes, such as 2 Days 10 Hours 36 Minutes.
     * Format: Display value: "n" Days "n" Hours "n" Minutes
     */
    getDisplayValue(): string;

    /**
     * Returns the duration value in "d HH:mm:ss" format.
     * @return {string} Duration value.
     * Format: d HH:mm:ss where "d" is number of days.
     */
    getDurationValue(): string;

    /**
     * Returns the rounded number of days. If the time part is more than 12 hours, the return value is rounded up. Otherwise, it is rounded down.
     * @return {number} Day value of the display value rounded.
     */
    getRoundedDayPart(): number;

    /**
     * Returns the internal date/time value of the current GlideDuration object.
     * GlideDuration objects store the duration as a date and time from January 1, 1970, 00:00:00.
     * @return {string} Current duration within the GlideDuration object.
     * Format: YYYY-MM-DD HH:mm:ss
     */
    getValue(): string;

    /**
     * Sets the duration display value.
     * @param {string} asDisplayed - Display duration value to set.
     * Format: d HH:mm:ss where "d" is number of days
     */
    setDisplayValue(asDisplayed: string): void;

    /**
     * Sets the internal date/time value of the GlideDuration object.
     * The method sets the duration value to the difference of the passed in date/time the base date/time value of January 1, 1970, 00:00:00.
     * The passed in date/time object (string) is parsed into a GlideDateTime object.
     * @param {*} o - Date and time to use as the endpoint for the calculated duration time.
     * Format: YYYY-MM-DD HH:mm:ss
     */
    setValue(o: any): void;

    /**
     * Subtracts the duration of the specified GlideDuration object to the current GlideDuration object.
     * @param {GlideDuration} duration - GlideDuration object that contains the duration value to subtract from the current GlideDuration object.
     * @return {GlideDuration} New GlideDuration object whose duration contains the result of the subtraction of the duration of the two GlideDuration objects.
     */
    subtract(duration: GlideDuration): GlideDuration;
    hashCode(): $$rhino.Number;
    toString(): string;
}
declare interface IGlideElementGlideObject extends IGlideElement {
    
}

declare interface IGlideElementVariable extends IGlideVariableComponent {
    getQuestion(): Question;
}

declare interface IGlideElementVariables extends IGlideElement {
    getVariablePoolQuestionSet(): IVariablePoolQuestionSet;
    getAllIds(): IJavaArray<any>;
}

/**
 * The Scoped GlideElement API provides a number of convenient script methods for dealing with fields and their values. Scoped GlideElement methods are available for the fields of the current GlideRecord.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideElement_global/concept/c_GlideElementAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_GlideElementAPI}
 */
declare class GlideElement implements IGlideElement { // sys_dictionary.comments
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementAudio implements IGlideElement { // db_audio.audio
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementBoolean implements IGlideElement { // sys_dictionary.audit
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementBreakdownElement implements IGlideElement { // pa_widgets.element
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementCompressed implements IGlideElement { // wf_workflow_version.activity_stages
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementConditions implements IGlideElement { // sys_dictionary.reference_qual_condition
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementCounter implements IGlideElement { // sys_update_xml.sys_recorded_at
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementCurrency implements IGlideElement { // sc_cat_item.list_price
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementDataObject implements IGlideElement { // wf_activity.input    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

/**
 * The GlideElementDescriptor API provides information about individual fields.
 * @description There is no constructor for this class. Use the GlideElement getED() method to obtain a ElementDescriptor object.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/glideElementDescriptor/concept/c_GlideElementDescriptorScopedAPI.html}
 */
declare class GlideElementDescriptor {
    protected constructor();
    /**
     * Returns the encryption type used for attachments on the element's table.
     * @returns {string | Packages.java.lang.String} The encryption type used on attachments. Returns null if attachments on the element's table are not being encrypted.
     */
    getAttachmentEncryptionType(): string | Packages.java.lang.String;
    /**
     * Returns the element's encryption type.
     * @returns {string | Packages.java.lang.String} The element's encryption type. Returns null if the element is not encrypted.
     */
    getEncryptionType(): string | Packages.java.lang.String;
    /**
     * Returns the element's internal data type.
     * @returns {string | Packages.java.lang.String} The element's internal data type.
     */
    getInternalType(): string | Packages.java.lang.String;
    /**
     * Returns the element's label.
     * @returns {string | Packages.java.lang.String} The element's label.
     */
    getLabel(): string | Packages.java.lang.String;
    /**
     * Returns the element's length.
     * @returns {number} The element's size.
     */
    getLength(): number;
    /**
     * Returns the element's name.
     * @returns {string} The element's name.
     */
    getName(): string | Packages.java.lang.String;
    /**
     * Returns the element's plural label.
     * @returns {string} The element's plural label.
     */
    getPlural(): string | Packages.java.lang.String;
    /**
     * Returns true if an encrypted attachment has been added to the table.
     * @returns {boolean} Returns true if an encrypted attachment has been added to the table.
     */
    hasAttachmentsEncrypted(): boolean;
    /**
     * Returns true if the element is an automatically generated or system field.
     * @returns {boolean} True if the element is automatically generated or a system field.
     */
    isAutoOrSysID(): boolean;
    /**
     * Returns true if the element is defined as a dropdown choice in its dictionary definition.
     * @returns {boolean} Returns true if the element is defined as a dropdown choice. Returns true even if there are no entries defined in the choice table. The last choice type, suggestion, does not return true.
     */
    isChoiceTable(): boolean;
    /**
     * Returns true if an element is encrypted.
     * @returns {boolean} Returns true if the element is encrypted, false otherwise.
     */
    isEdgeEncrypted(): boolean;
    /**
     * Returns true if the element is a virtual element.
     * @returns {boolean} Returns true if the element is a virtual element.
     */
    isVirtual(): boolean;
}

declare class GlideElementDocumentation implements IGlideElement { // sys_dictionary.column_label
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementDocumentId implements IGlideElement { // incident.origin_id
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementDomainId implements IGlideElement { // task.sys_domain
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementFullUTF8 implements IGlideElement { // sys_email.subject    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementGlideObject implements IGlideElement { // incident.sys_created_on
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
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
}

declare class GlideElementGlideVar implements IGlideElement { // wf_activity.vars    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare type GlideElementHierarchicalVariables = GlideElement;

declare class GlideElementIcon implements IGlideElement { // sys_run_level.icon    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementInternalType implements IGlideElement { // sys_script_validator.internal_type    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementIPAddress implements IGlideElement {
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementNameValue implements IGlideElement { // wf_context.return_value    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementNumeric implements IGlideElement { // sys_dictionary.defaultsort
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementPassword implements IGlideElement { // sys_user.user_password
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementPassword2 implements IGlideElement { // sys_data_source.jdbc_password    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementPhoneNumber implements IGlideElement {
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementPrice implements IGlideElement { // sc_cat_item.recurring_price
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

/**
 * A Glide element that references another GlideRecord.
 * @todo Verify whether Packages.com.glide.script.glide_elements.GlideReference exists
 */
declare class GlideElementReference implements IGlideElement { // sys_dictionary.internal_type
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
    /**
     * Gets the table name for a reference element.
     * @returns {string} The table name of the reference.
     */
    getReferenceTable(): string;
    /**
     * Returns a GlideRecord object for a given reference element.
     * @returns {GlideRecord} A GlideRecord object.
     */
    getRefRecord(): GlideRecord | null | undefined;
}

declare class GlideElementRelatedTags implements IGlideElement { // incident.sys_tags
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementReplicationPayload implements IGlideElement { // sys_replication_queue1.payload
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementScript implements IGlideElement { // sys_dictionary.calculation
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementShortFieldName implements IGlideElement { // sys_history_line.field
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementShortTableName implements IGlideElement { // sys_email.target_table    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementSimpleNameValue implements IGlideElement { // syslog.context_map    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementSourceId implements IGlideElement {
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementSourceName implements IGlideElement { // sys_update_version.record_name    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementSourceTable implements IGlideElement { // sys_update_log.set_table    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementSysClassName implements IGlideElement { // task.sys_class_name
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementTranslatedField implements IGlideElement { // sys_glide_object.label
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementTranslatedHTML implements IGlideElement { // sys_script.message
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementTranslatedText implements IGlideElement { // sys_ux_event.description
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementUserImage implements IGlideElement { // sys_user.photo
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementURL implements IGlideElement { // v_plugin.help
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementVariableConditions implements IGlideElement {
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementVariable implements IGlideElement {
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementVariables implements IGlideElement { // task.variables
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementWikiText implements IGlideElement { // sys_ui_help.wiki_text    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementWorkflow implements IGlideElement { // sc_req_item.stage    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideElementWorkflowConditions implements IGlideElement { // sys_template.template    
    protected constructor();
    changes(): boolean;
    changesFrom(o: any): boolean;
    changesTo(o: any): boolean;
    dateNumericValue(): number;
    getBooleanAttribute(attributeName: string): boolean;
    getChoices(dependent?: string | undefined): any[];
    getChoiceValue(): string;
    getDecryptedValue(): string;
    getDisplayValue(maxCharacters?: number | undefined): string;
    getError(): string;
    getGlobalDisplayValue(): string;
    getHTMLValue(maxChars?: number | undefined): string;
    getJournalEntry(mostRecent: number): string;
    getName(): string;
    nil(): boolean;
    setDateNumericValue(milliseconds: number): void;
    setDisplayValue(value: any): void;
    setError(errorMessage: string): void;
    setPhoneNumber(phoneNumber: any, strict: boolean): boolean;
    setValue(value: any): void;
    canCreate(): boolean;
    canRead(): boolean;
    canWrite(): boolean;
    getAttribute(attributeName: string): string;
    getED(): GlideElementDescriptor;
    getLabel(): string;
    getTableName(): string;
}

declare class GlideEmail {
    /**
     * Adds the address to either the cc or bcc list.
     * @param {"cc"|"bcc"} type - Either cc or bcc, determines the list to which the address is added.
     * @param {string} address - The recipient's email address.
     */
    addAddress(type: string, address: string): void;
    /**
     * Adds the recipient to either the cc or bcc list, but uses the display name instead of the address when showing the recipient.
     * @param {"cc"|"bcc"} type - Either cc or bcc, determines the list to which the address is added.
     * @param {string} address - The recipient's email address.
     * @param {string} displayName - The name to be shown instead of the email address.
     */
    addAddress(type: string, address: string, displayName: string): void;
    /**
     * 
     * @param address 
     */
    addRecipient(address: string): void;
    /**
     * Instantiates a GlideEmail object.
     * @constructor
     * @param {string} [body] Body of message
     */
    constructor(body?: string);
    getSMSText(): string;
    /**
     * Returns the email's subject line.
     * @returns {string} The email's subject line.
     */
    getSubject(): string;
    getTextBody(): string;
    /**
     * Returns the email's watermark.
     * @returns {string} The email's watermark.
     */
    getWatermark(): string;
    /**
     * Sets the body of the email.
     * @param {string} bodyText - The body of the email.
     */
    setBody(bodyText: string): void;
    /**
     * Sets the sender's address.
     * @param {string} address - The sender's email address.
     */
    setFrom(address: string): void;
    setRecipients(recipients: string): void;
    /**
     * Sets the reply to address.
     * @param {string} address - The reply to email address.
     */
    setReplyTo(address: string): void;
    /**
     * Sets the email's subject line.
     * @param {string} subject - Text for the subject line.
     */
    setSubject(subject: string): void;
}

/**
 * @todo Add GlideLRUCache members
 */
declare class GlideLRUCache {
}

/**
 * GlideQueryCondition object.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideQueryCondition/concept/c_GlideQueryConditionAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_GlideQueryConditionAPI}
 */
declare class GlideQueryCondition {
    protected constructor();
    /**
     * Adds an AND condition to the current condition.
     * @param {string} name The name of a field.
     * @param {QueryOperator} oper The operator for the query (=,!=,>,>=,<,<=,IN,NOT IN,STARTSWITH,ENDSWITH,CONTAINS,DOES NOT CONTAIN,INSTANCEOF).
     * @param {*} value The value to query on.
     * @returns {GlideQueryCondition} A reference to a GlideQueryConditon that was added to the GlideRecord.
     */
    addCondition(name: string, oper: QueryOperator, value: any): GlideQueryCondition;
    /**
     * Adds an AND condition to the current condition. Assumes the equals operator.
     * @param {string} name The name of a field.
     * @param {*} value The value of a field.
     * @returns {GlideQueryCondition} A reference to a GlideQueryConditon that was added to the GlideRecord.
     */
    addCondition(name: string, value: any): GlideQueryCondition;
    /**
     * Adds an AND condition to the current condition.
     * @param {GlideQueryCondition} queryCondition Condition to add.
     * @returns {GlideQueryCondition} A reference to a GlideQueryConditon that was added to the GlideRecord.
     */
    addCondition(queryCondition: GlideQueryCondition): GlideQueryCondition
    /**
     * Adds an OR condition to the current condition.
     * @param {string} name The name of a field.
     * @param {QueryOperator} oper The operator for the query (=,!=,>,>=,<,<=,IN,NOT IN,STARTSWITH,ENDSWITH,CONTAINS,DOES NOT CONTAIN,INSTANCEOF).
     * @param {*} value The value to query on.
     * @returns {GlideQueryCondition} A reference to a GlideQueryConditon that was added to the GlideRecord.
     */
    addOrCondition(name: string, oper: QueryOperator, value: any): GlideQueryCondition;
    /**
     * Adds an OR condition to the current condition. Assumes the equals operator.
     * @param {string} name The name of a field.
     * @param {*} value The value to query on.
     * @returns {GlideQueryCondition} A reference to a GlideQueryConditon that was added to the GlideRecord.
     */
    addOrCondition(name: string, value: any): GlideQueryCondition;
    /**
     * Adds an OR condition to the current condition. Assumes the equals operator.
     * @param {GlideQueryCondition} queryCondition Condition to add.
     * @returns {GlideQueryCondition} A reference to a GlideQueryConditon that was added to the GlideRecord.
     */
    addOrCondition(queryCondition: GlideQueryCondition): GlideQueryCondition;
}

/**
 * Scoped GlideRecord is used for database operations.
 * @description A GlideRecord contains both records and fields. For information about GlideRecordSecure, which is a class inherited from GlideRecord that performs the same functions as GlideRecord, and also enforces ACLs, see the GlideServer APIs .Always test queries on a sub-production instance prior to deploying them on a production instance. An incorrectly constructed encoded query, such as including an invalid field name, produces an invalid query. When the invalid query is run, the invalid part of the query condition is dropped, and the results are based on the valid part of the query, which may return all records from the table. Using an insert(), update(), deleteRecord(), or deleteMultiple() method on bad query results can result in data loss.You can set the glide.invalid_query.returns_no_rows system property to true to have queries with invalid encoded queries return no records.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideRecord/concept/c_GlideRecordAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_GlideRecordAPI}
 */
declare class GlideRecord implements IDbObject {
    /**
     * Adds a filter to return active records.
     * @returns {GlideQueryCondition} Filter to return active records.
     */
    addActiveQuery(): GlideQueryCondition;
    /**
     * Adds an encoded query to other queries that may have been set.
     * @param {string} query - An encoded query string.
     */
    addEncodedQuery(query: string): void;
    /**
     * Applies a pre-defined GlideDBFunctionBuilder object to a record.
     * @param {*} function - A GlideDBFunctionBuilder object that defines a SQL operation.
     */
    addFunction(functionBuilder: any): void;
    /**
     * Creates an instance of the GlideRecord class for the specified table.
     * @constructor
     * @param {string} tableName - The table to be used.
     */
    constructor(tableName: string);
    /**
     * Adds a filter to return records based on a relationship in a related table.
     * @param {string} joinTable - Table name
     * @param {*} [primaryField] - If other than sys_id, the primary field
     * @param {*} [joinTableField] - If other than sys_id, the field that joins the tables.
     * @returns {GlideQueryCondition} A filter that lists records where the relationships match.
     */
    addJoinQuery(joinTable: string, primaryField?: any, joinTableField?: any): GlideQueryCondition;
    /**
     * A filter that specifies records where the value of the field passed in the parameter is not null.
     * @param {string} fieldName - Name of the field to check.
     * @returns {GlideQueryCondition} A filter that specifies records where the value of the field passed in the parameter is not null.
     */
    addNotNullQuery(fieldName: string): GlideQueryCondition;
    /**
     * Adds a filter to return records where the value of the specified field is null.
     * @param {string} fieldName - Name of the field to check.
     * @returns {GlideQueryCondition} Query condition added to the GlideRecord.
     */
    addNullQuery(fieldName: string): GlideQueryCondition;
    /**
     * Provides the ability to build a request, which when executed, returns the rows from the specified table, that match the request.
     */
    addQuery(): GlideQueryCondition;
    /**
     * Provides the ability to build a request, which when executed, returns the rows from the specified table, that match the request.
     * @param {string} name - Table field name.
     * @param {string} operator - Query operator. The available values are dependent on the data type of the value parameter.Numbers:=!=&gt;&gt;=&lt;&lt;=Strings (must be in upper case):=!=INNOT INSTARTSWITHENDSWITHCONTAINSDOES NOT CONTAININSTANCEOF
     * @param {*} value - Value on which to query (not case-sensitive).
     * @returns {GlideQueryCondition} The query condition that was added to the GlideRecord.
     */
    addQuery(name: string, operator: string, value: any): GlideQueryCondition;
    /**
     * Provides the ability to build a request, which when executed, returns the rows from the specified table, that match the request.
     * @param {string} name - Table field name.
     * @param {*} value - Value on which to query (not case-sensitive).
     * @returns {GlideQueryCondition} The query condition that was added to the GlideRecord.
     */
    addQuery(name: string, value: any): GlideQueryCondition;
    /**
     * Adds a filter to return records using an encoded query string.
     * @param {string} encodedQuery - Encoded query string.
     * @returns {GlideQueryCondition} The query condition added to the GlideRecord.
     */
    addQuery(encodedQuery: string): GlideQueryCondition;
    /**
     * Determines if the Access Control Rules, which include the user's roles, permit inserting new records in this table.
     * @returns {boolean} True if the user's roles permit creation of new records in this table.
     */
    canCreate(): boolean;
    /**
     * Determines if the Access Control Rules, which include the user's roles, permit deleting records in this table.
     * @returns {boolean} Flag that indicates whether the user's roles permit deletions of records in this table.Valid values:true: Deletions permittedfalse: Deletions are not permitted.
     */
    canDelete(): boolean;
    /**
     * Determines if the Access Control Rules, which include the user's roles, permit reading records in this table.
     * @returns {boolean} Flag that indicates whether the user's roles permit reading of records in this table.Valid values:true: Reading permittedfalse: Reading is not permitted.
     */
    canRead(): boolean;
    /**
     * Determines if the Access Control Rules, which include the user's roles, permit editing records in this table.
     * @returns {boolean} Flag that indicates whether the user's roles permit writing of records in this table.Valid values:true: Writing permittedfalse: Writing is not permitted.
     */
    canWrite(): boolean;
    /**
     * Sets a range of rows to be returned by subsequent queries.
     * @param {number} firstRow - The first row to include. Because the index starts at 0, a value of 0 returns the first row.
     * @param {number} lastRow - The last row to include in the range. Because the index starts at 0, use the value n - 1, in which n equals the actual row number.
     * @param {boolean} forceCount - If true, the getRowCount() method will return all possible records.
     */
    chooseWindow(firstRow: number, lastRow: number, forceCount: boolean): void;
    /**
     * Deletes multiple records that satisfy the query condition.
     */
    deleteMultiple(): void;
    /**
     * Deletes the current record.
     * @returns {boolean} Flag that indicates whether the record was successfully deleted.Valid values:true: Record was deleted.false: No record was found to delete.
     */
    deleteRecord(): boolean;
    /**
     * Returns the specified record in an instantiated GlideRecord object.
     * @param {*} sys_id - sys_id to match.
     * @returns {boolean} Flag that indicates whether the requested record was located - true: Record was found; false: Record was not found.
     */
    get(sys_id: any): boolean;
    /**
     * Returns the specified record in an instantiated GlideRecord object.
     * @param {GLIDE.String} name - Name of the instantiated GlideRecord column to search for the specified value parameter.
     * @param {*} value - Value to match.
     * @returns {boolean} Flag that indicates whether the requested record was located.true: Record was foundfalse: Record was not found.
     */
    get(name: $$rhino.String, value: any): boolean;
    /**
     * Returns the dictionary attributes for the specified field.
     * @param {string} fieldName - Field name for which to return the dictionary attributes
     * @returns {(string | null)} Dictionary attributes or null if field is not found.
     */
    getAttribute(fieldName: string): string | null;
    /**
     * Returns the table's label.
     * @returns {string} Table's label.
     */
    getClassDisplayValue(): string;
    /**
     * Retrieves the display value for the current record.
     * @returns {string} The display value for the current record.
     */
    getDisplayValue(): string;
    /**
     * Returns the element's descriptor.
     * @returns {GlideElementDescriptor} Element's descriptor.
     */
    getED(): GlideElementDescriptor;
    /**
     * Retrieves the GlideElement object for the specified field.
     * @param {string} columnName - Name of the column to get the element from.
     * @returns {GlideElement} The GlideElement for the specified column of the current record or null if column is not found.
     */
    getElement(columnName: string): GlideElement | null;
    /**
     * Retrieves the query condition of the current result set as an encoded query string.
     * @returns {string} The encoded query as a string.
     */
    getEncodedQuery(): string;
    /**
     * Returns the field's label.
     * @returns {string} Field's label.
     */
    getLabel(): string;
    /**
     * Retrieves the last error message. If there is no last error message, null is returned.
     * @returns {string} The last error message as a string.
     */
    getLastErrorMessage(): string;
    /**
     * Retrieves a link to the current record.
     * @param {boolean} [noStack] - If true, the sysparm_stack parameter is not appended to the link. The parameter sysparm_stack specifies the page to visit after closing the current link.
     * @returns {string} A link to the current record as a string.
     */
    getLink(noStack?: boolean): string;
    /**
     * Retrieves the class name for the current record.
     * @returns {string} The class name.
     */
    getRecordClassName(): string;
    /**
     * Retrieves the number of rows in the query result.
     * @returns {number} Number of rows.
     */
    getRowCount(): number;
    /**
     * Retrieves the name of the table associated with the GlideRecord.
     * @returns {string} The table name.
     */
    getTableName(): string;
    /**
     * Gets the primary key of the record, which is usually the sys_id unless otherwise specified.
     * @returns {string} The unique primary key as a String, or null if the key is null.
     */
    getUniqueValue(): string;
    /**
     * Retrieves the string value of an underlying element in a field.
     * @param {string} name - The name of the field to get the value from.
     * @returns {string} The value of the field or null if field is not found.
     */
    getValue(name: string): string | null;
    /**
     * Determines if there are any more records in the GlideRecord object.
     * @returns {boolean} True if there are more records in the query result set.
     */
    hasNext(): boolean;
    /**
     * Creates an empty record suitable for population before an insert.
     */
    initialize(): void;
    /**
     * Inserts a new record using the field values that have been set for the current record.
     * @returns {string} Unique ID of the inserted record, or null if the record is not inserted.
     */
    insert(): string;
    /**
     * Checks to see if the current database action is to be aborted.
     * @returns {boolean} Flag that indicates whether the current database action is to be aborted.Valid values:true: Action is to be aborted.false: Action is not to be aborted.
     */
    isActionAborted(): boolean;
    /**
     * Checks if the current record is a new record that has not yet been inserted into the database.
     * @returns {boolean} True if the record is new and has not been inserted into the database.
     */
    isNewRecord(): boolean;
    /**
     * Determines if the table exists.
     * @returns {boolean} True if table is valid or if record was successfully retrieved. False if table is invalid or record was not successfully retrieved.
     */
    isValid(): boolean;
    /**
     * Determines if the specified field is defined in the current table.
     * @param {string} columnName - The name of the the field.
     * @returns {boolean} True if the field is defined for the current table.
     */
    isValidField(columnName: string): boolean;
    /**
     * Determines if a record was actually returned by the query/get record operation.
     * @returns {boolean} Flag that indicates whether a record was actually returned by the query/get operation.Valid values:true: Record returned by query/get operation.false: End of record set, no record returned.
     */
    isValidRecord(): boolean;
    /**
     * Creates a new GlideRecord record, sets the default values for the fields, and assigns a unique ID to the record.
     */
    newRecord(): void;
    /**
     * Moves to the next record in the GlideRecord object.
     * @returns {boolean} Flag that indicates if there is a "next" record in the GlideRecord.Valid values:true: Move to the next record was successful.false: No more records in the result set.
     */
    next(): boolean;
    /**
     * Retrieves the current operation being performed, such as insert, update, or delete.
     * @returns {string} The current operation.
     */
    operation(): string;
    /**
     * Specifies an orderBy column.
     * @param {string} name - The column name used to order the records in this GlideRecord object.
     */
    orderBy(name: string): void;
    /**
     * Specifies a decending orderBy column.
     * @param {string} name - The column name to be used to order the records in a GlideRecord object.
     */
    orderByDesc(name: string): void;
    /**
     * Runs the query against the table based on the filters specified by addQuery, addEncodedQuery, etc.
     * @param {*} field - Column name to query on.
     * @param {*} value - Value to query for.
     */
    query(field: any, value: any): void;
    /**
     * Runs the query against the table based on the filters specified by addQuery, addEncodedQuery, etc.
     */
    query(): void;
    /**
     * Sets a flag to indicate if the next database action (insert, update, delete) is to be aborted. This is often used in business rules.
     * @param {boolean} b - True to abort the next action. False if the action is to be allowed.
     */
    setAbortAction(b: boolean): void;
    /**
     * Sets the limit for number of records are fetched by the GlideRecord query.
     * @param {number} maxNumRecords - The maximum number of records to fetch.
     */
    setLimit(maxNumRecords: number): void;
    /**
     * Sets sys_id value for the current record.
     * @param {string} guid - The GUID to be assigned to the current record.
     */
    setNewGuidValue(guid: string): void;
    /**
     * Sets the value of the field with the specified name to the specified value.
     * @param {string} name - Name of the field.
     * @param {*} value - The value to assign to the field.
     */
    setValue(name: string, value: any): void;
    /**
     * Enables or disables the running of business rules, script engines, and audit.
     * @param {boolean} enable - If true (default), enables business rules. If false, disables business rules.
     */
    setWorkflow(enable: boolean): void;
    /**
     * Updates the GlideRecord with any changes that have been made. If the record does not already exist, it is inserted.
     * @param {string} [reason] - Reason for the update. The reason appears in the audit record.
     * @returns {string} The sys_id of the new or updated record. Returns null if the update fails.
     */
    update(reason?: string): string;
    /**
     * Updates each GlideRecord in a stated query with a specified set of changes.
     */
    updateMultiple(): void;
    /**
     * Moves to the next record in the GlideRecord. Provides the same functionality as�next(), it is intended to be used in cases where the GlideRecord has a column named next.
     * @returns {boolean} True if there are more records in the query set.
     */
    _next(): boolean;
    /**
     * Identical to�query(). This method is intended to be used on tables where there is a column named query, which would interfere with using the�query()�method.
     * @param {*} name - Column name on which to query
     * @param {*} value - Value for which to query
     */
    _query(name: any, value: any): void;
}

/**
 * @todo Add GlideScriptableInputStream members
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideScriptableInputStreamScoped/concept/c_GlideScriptableInpStrmScopedAPI.html}
 */
declare class GlideScriptableInputStream {
}

/**
 * @todo Add GlideServletRequest members
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideServletRequestScoped/concept/c_GlideServletRequestScopedAPI.html}
 * @todo See if this exists on global
 */
declare class GlideServletRequest {
}

declare class GlideSysForm {
    // constructor(tableName: string);
    // constructor(tableName: string, viewName: string);
    // constructor();
    constructor(tableName?: string, viewName?: string);
    createDefaultBaselineVersion(record: GlideRecord): void;
    static generateDefaultForm(tableName: string): string;
    static getRelatedTables(tableName: string): GlideChoiceList;
    getSuggestedFields(): Array<string>;
    getTableName(): string;
    save(): void;
    setAvailable(available: string): void;
    setCollection(s: string): void;
    setForm(form: string): void;
    setName(name: string): void;
    setPackageID(packageID: string): void;
    setScopeID(scopeID: string): void;
    setSelected(selected: string): void;
    setTablePackageID(): void;
    setTableScopeID(): void;
    setView(viewName: string): void;
    setViewName(viewName: string): void;
}

declare class GlideSysList extends GlideSysForm {
    InsertListElements(fields: Array<any>): void;
    // constructor(tableName: string);
    // constructor(tableName: string, parentName: string);
    constructor(tableName: string, parentName?: string);
    createDefaultBaseline(): void;
    getAccessList(collectionKey: string): Array<string>;
    getListColumns(): GlideChoiceList;
    getListRecords(): Array<string>;
    getListSet(): GlideChoiceListSet;
    getRecordSet(): GlideRecord;
    getStandardListID(): string;
    isUserList(): boolean;
    // save(): void;
    // save(fields: string): void;
    save(fields?: string): void;
    saveForUser(fields: string): void;
    setIncludeFormatting(b: boolean): void;
    setReconcileList(b: boolean): void;
    setRelatedParentID(parentID: string): void;
    setRelatedParentName(parentName: string): void;
    setRelationshipID(relationshipID: string): void;
    setUserApplies(b: boolean): void;
}

/**
 * The GlideSystem (referred to by the variable name 'gs' in any server-side JavaScript) API provides a number of convenient methods to get information about the system, the current logged in user, etc.
 * @description Many of the GlideSystem methods facilitate the easy inclusion of dates in query ranges, and are most often used in filters and reporting.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideSystem/concept/c_GlideSystemAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/c_GlideSystemAPI}
 */
declare class GlideSystem {
    protected constructor();
    /**
     * Adds an error message for the current session.
     * @param {*} message - The message to add.
     */
    addErrorMessage(message: any): void;
    
    /**
     * Adds an info message for the current session. This method is not supported for asynchronous business rules.
     * @param {*} message - An info message object.
     */
    addInfoMessage(message: any): void;
    
    /**
     * Returns an ASCII string from the specified base64 string.
     * @param {string} source - A base64 encoded string.
     * @returns {string} The decoded string..
     */
    base64Decode(source: string): string;
    
    /**
     * Creates a base64 string from the specified string.
     * @param {string} source - The string to be encoded.
     * @returns {string} The base64 string..
     */
    base64Encode(source: string): string;
    
    /**
     * Returns the date and time for the beginning of last month in GMT.
     * @returns {string} GMT beginning of last month, in the format yyyy-mm-dd hh:mm:ss.
     */
    beginningOfLastMonth(): string;
    
    /**
     * Returns the date and time for the beginning of last week in GMT.
     * @returns {string} GMT beginning of last week, in the format yyyy-mm-dd hh:mm:ss.
     */
    beginningOfLastWeek(): string;
    
    /**
     * Returns the date and time for the beginning of next month in GMT.
     * @returns {string} GMT beginning of next month, in the format yyyy-mm-dd hh:mm:ss.
     */
    beginningOfNextMonth(): string;
    
    /**
     * Returns the date and time for the beginning of next week in GMT.
     * @returns {string} The GMT beginning of next week, in the format yyyy-mm-dd hh:mm:ss..
     */
    beginningOfNextWeek(): string;
    
    /**
     * Returns the date and time for the beginning of next year in GMT.
     * @returns {string} GMT beginning of next year, in the format yyyy-mm-dd hh:mm:ss.
     */
    beginningOfNextYear(): string;
    
    /**
     * Returns the date and time for the beginning of this month in GMT.
     * @returns {string} GMT beginning of this month, in the format yyyy-mm-dd hh:mm:ss.
     */
    beginningOfThisMonth(): string;
    
    /**
     * Returns the date and time for the beginning of this quarter in GMT.
     * @returns {string} GMT beginning of this quarter, in the format yyyy-mm-dd hh:mm:ss.
     */
    beginningOfThisQuarter(): string;
    
    /**
     * Returns the date and time for the beginning of this week in GMT.
     * @returns {string} GMT beginning of this week, in the format yyyy-mm-dd hh:mm:ss.
     */
    beginningOfThisWeek(): string;
    
    /**
     * Returns the date and time for the beginning of this year in GMT.
     * @returns {string} GMT beginning of this year, in the format yyyy-mm-dd hh:mm:ss.
     */
    beginningOfThisYear(): string;
    
    /**
     * Generates a date and time for the specified date in GMT.
     * @param {string} date - Format: yyyy-mm-dd
     * @param {string} range - Start, end, or a time in the 24 hour format hh:mm:ss.
     * @returns {string} A date and time in the format yyyy-mm-dd hh:mm:ss. If range is start, the returned value is yyyy-mm-dd 00:00:00; If range is end the return value is yyyy-mm-dd 23:59:59..
     */
    dateGenerate(date: string, range: string): string;
    
    /**
     * Returns the date and time for a specified number of days ago.
     * @param {number} days - Integer number of days
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    daysAgo(days: number): string;
    
    /**
     * Returns the date and time for the end of the day a specified number of days ago.
     * @param {number} days - Integer number of days
     * @returns {string} GMT end of the day in the format yyyy-mm-dd hh:mm:ss.
     */
    daysAgoEnd(days: number): string;
    
    /**
     * Returns the date and time for the beginning of the day a specified number of days ago.
     * @param {string} days - Integer number of days
     * @returns {string} GMT start of the day in the format yyyy-mm-dd hh:mm:ss.
     */
    
    daysAgoStart(days: string): string;
    
    /**
     * Writes a debug message to the system log.
     * @param {string} message - The log message with place holders for any variable arguments.
     * @param {*} [param1] - First variable argument.
     * @param {*} [param2] - Second variable argument.
     * @param {*} [param3] - Third variable argument.
     * @param {*} [param4] - Fourth variable argument.
     * @param {*} [param5] - Fifth variable argument.
     */
    debug(message: string, param1?: any, param2?: any, param3?: any, param4?: any, param5?: any): void;
    
    /**
     * Returns the date and time for the end of last month in GMT.
     * @returns {string} GMT end of last month, in the format yyyy-mm-dd hh:mm:ss.
     */
    endOfLastMonth(): string;
    
    /**
     * Returns the date and time for the end of last week in GMT.
     * @returns {string} GMT end of last week, in the format yyyy-mm-dd hh:mm:ss.
     */
    endOfLastWeek(): string;
    
    /**
     * Returns the date and time for the end of last year in GMT.
     * @returns {string} GMT in format yyyy-mm-dd hh:mm:ss.
     */
    endOfLastYear(): string;
    
    /**
     * Returns the date and time for the end of next month in GMT.
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    endOfNextMonth(): string;
    
    /**
     * Returns the date and time for the end of next week in GMT.
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    endOfNextWeek(): string;
    
    /**
     * Returns the date and time for the end of next year in GMT.
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    endOfNextYear(): string;
    
    /**
     * Returns the date and time for the end of this month in GMT.
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    endOfThisMonth(): string;
    
    /**
     * Returns the date and time for the end of this quarter in GMT.
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    endOfThisQuarter(): string;
    
    /**
     * Returns the date and time for the end of this week in GMT.
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    
    endOfThisWeek(): string;
    /**
     * Returns the date and time for the end of this year in GMT.
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    
    endOfThisYear(): string;
    
    /**
     * Writes an error message to the system log.
     * @param {string} message - The log message with place holders for any variable arguments.
     * @param {*} [param1] - First variable argument.
     * @param {*} [param2] - Second variable argument.
     * @param {*} [param3] - Third variable argument.
     * @param {*} [param4] - Fourth variable argument.
     * @param {*} [param5] - Fifth variable argument.
     */
    error(message: string, param1?: any, param2?: any, param3?: any, param4?: any, param5?: any): void;
    
    /**
     * Queues an event for the event manager.
     * @param {string} name - Name of the event being queued.
     * @param {*} instance - GlideRecord object, such as "current".
     * @param {string} [parm1] - Saved with the instance if specified.
     * @param {string} [parm2] - Saved with the instance if specified.
     * @param {string} [queue] - Name of the queue.
     */
    eventQueue(name: string, instance: any, parm1?: string, parm2?: string, queue?: string): void;
    
    /**
     * Queues an event for the event manager at a specified date and time.
     * @param {string} name - Name of the event being queued.
     * @param {*} instance - GlideRecord object, such as "current".
     * @param {string} [parm1] - Saved with the instance if specified.
     * @param {string} [parm2] - Saved with the instance if specified.
     * @param {*} [expiration] - Date and time to process this event..
     */
    eventQueueScheduled(name: string, instance: any, parm1?: string, parm2?: string, expiration?: any): void;
    
    /**
     * Executes a job for an application.
     * @param {GlideRecord} job - The job to be run.
     * @returns {string} Returns the sysID of the scheduled job. Returns null if the job is global..
     */
    executeNow(job: GlideRecord): string;
    
    /**
     * Generates a GUID that can be used when a unique identifier is required.
     * @returns {string} A 32-character hexadecimal GUID..
     */
    generateGUID(): string;
    
    /**
     * Gets the caller scope name; returns null if there is no caller.
     * @returns {string} The caller's scope name, or null if there is no caller..
     */
    getCallerScopeName(): string;
    
    /**
     * Gets a string representing the cache version for a CSS file.
     * @returns {string} The CSS cache version..
     */
    getCssCacheVersionString(): string;
    
    /**
     * Gets the ID of the current application as set using the Application Picker.
     * @returns {string} The current application's sys_id, or global in none is set..
     */
    getCurrentApplicationId(): string;
    
    /**
     * Gets the name of the current scope.
     * @returns {string} The current scope name..
     */
    getCurrentScopeName(): string;
    
    /**
     * Returns the list of error messages for the session that were added by�addErrorMessage().
     * @returns {string} List of error messages.
     */
    getErrorMessages(): string;
    
    /**
     * Retrieves a message from UI messages that has HTML special characters, and replaces them with escape sequences. For example,�&amp;�becomes�&amp;amp;.
     * @param {string} id - ID of the message.
     * @param {any[]} [args] - List of strings or other values defined by java.text.MessageFormat, which allows you to produce language-neutral messages for display to users.
     * @returns {string} The UI message with HTML special characters replaced with escape sequences..
     */
    getEscapedMessage(id: string, args?: any[]): string;
    
    /**
     * Retrieves a message from UI messages.
     * @param {string} id - The ID of the message.
     * @param {any[]} [args] - A list of strings or other values defined by java.text.MessageFormat, which allows you to produce language-neutral messages for display to users.
     * @returns {string} The UI message..
     */
    getMessage(id: string, args?: any[]): string;
    
    /**
     * Returns the specified user preference.
     * @param {string} key - Key for the preference.
     * @return {(string | null)} The preference value or null if the preference is not found.
     */
    getPreference(key: string): string | null;

    /**
     * Returns the specified user preference.
     * @template T - The default value type.
     * @param {string} key - Key for the preference.
     * @param {T} defaultValue - Default value to use if the specified preference is not found.
     */
    getPreference<T>(key: string, defaultValue: T): string | T;

    /**
     * Gets the value of a Glide property.
     * @param {string} key - The key for the property.
     * @return {(string | undefined)} The property value or undefined if the property is not found.
     */
    getProperty(key: string): string | undefined;

    /**
     * Gets the value of a Glide property.
     * @template T - The default avlue type.
     * @param {string} key - The key for the property.
     * @param {T} alt - Default value to use if the specified property is not found.
     * @return {(string | T)} The Property value or the specified default value if the property is not found.
     */
    getProperty<T>(key: string, alt: T): string | T;
    
    /**
     * Gets a reference to the current Glide session.
     * @returns {string} A reference for the current session..
     */
    getSession(): string;
    
    /**
     * Retrieves the GlideSession session ID.
     * @returns {string} The session ID..
     */
    getSessionID(): string;
    
    /**
     * This method is no longer available. Instead, use�gs.getSession().getSessionToken().
     * @returns {string} The session token..
     */
    getSessionToken(): string;
    
    /**
     * Returns the name of the time zone associated with the current user.
     * @returns {string} The time zone name..
     */
    getTimeZoneName(): string;
    
    /**
     * Gets the current URI for the session.
     * @returns {string} The URI..
     */
    getUrlOnStack(): string;
    
    /**
     * Returns a reference to the GlideUser object for the current user.
     * @returns {GlideUser} Reference to a user object..
     */
    getUser(): GlideUser;
    
    /**
     * Gets the display name of the current user.
     * @returns {string} The name field of the current user. Returns Abel Tuter, as opposed to abel.tuter..
     */
    getUserDisplayName(): string;
    
    /**
     * Gets the sys_id of the current user.
     * @returns {string} The sys_id of the current user..
     */
    getUserID(): string;
    
    /**
     * Gets the user name, or user id, of the current user.
     * @returns {string} The user name of the current user..
     */
    getUserName(): string;
    
    /**
     * Determines if the current user has the specified role.
     * @param {*} role - The role to check.
     * @returns {boolean} True if the user had the role. Returns true for users with the administrator role..
     */
    hasRole(role: any): boolean;
    
    /**
     * Returns the date and time for a specified number of hours ago.
     * @param {number} hours - Integer number of hours
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    hoursAgo(hours: number): string;
    
    /**
     * Returns the date and time for the end of the hour a specified number of hours ago.
     * @param {number} hours - Integer number of hours
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    hoursAgoEnd(hours: number): string;
    
    /**
     * Returns the date and time for the start of the hour a specified number of hours ago.
     * @param {number} hours - Integer number of hours
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    hoursAgoStart(hours: number): string;
    
    /**
     * Provides a safe way to call from the sandbox, allowing only trusted scripts to be included.
     * @param {string} name - The name fo the script to include.
     * @returns {boolean} True if the include worked..
     */
    include(name: string): boolean;
    
    /**
     * Writes an info message to the system log.
     * @param {string} message - The log message with place holders for any variable arguments.
     * @param {*} [param1] - First variable argument.
     * @param {*} [param2] - Second variable argument.
     * @param {*} [param3] - Third variable argument.
     * @param {*} [param4] - Fourth variable argument.
     * @param {*} [param5] - Fifth variable argument.
     */
    info(message: string, param1?: any, param2?: any, param3?: any, param4?: any, param5?: any): void;
    
    /**
     * Determines if debugging is active for a specific scope.
     * @returns {boolean} True if either session debugging is active or the log level is set to debug for the specified scope..
     */
    isDebugging(): boolean;
    
    /**
     * Checks if the current session is interactive. An example of an interactive session is when a user logs in normally. An example of a non-interactive session is using a SOAP request to retrieve data.
     * @returns {boolean} True if the session is interactive..
     */
    isInteractive(): boolean;
    
    /**
     * Determines if the current user is currently logged in.
     * @returns {boolean} True if the current user is logged in..
     */
    isLoggedIn(): boolean;
    
    /**
     * You can determine if a request comes from a mobile device.
     * @returns {boolean} True if the request comes from a mobile device; otherwise, false..
     */
    isMobile(): boolean;
    
    /**
     * Returns the date and time for the end of the minute a specified number of minutes ago.
     * @param {number} minutes - Integer number of minutes
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    minutesAgoEnd(minutes: number): string;
    
    /**
     * Returns the date and time for the start of the minute a specified number of minutes ago.
     * @param {number} minutes - Integer number of minutes
     * @returns {string} GMT in the format yyyy-mm-dd hh:mm:ss.
     */
    minutesAgoStart(minutes: number): string;
    
    /**
     * Returns the date and time for a specified number of months ago.
     * @param {number} months - Integer number of months
     * @returns {string} GMT on today's date of the specified month, in the format yyyy-mm-dd hh:mm:ss.
     */
    monthsAgo(months: number): string;
    
    /**
     * Returns the date and time for the start of the month a specified number of months ago.
     * @param {number} months - Integer number of months
     * @returns {string} GMT start of the month the specified number of months ago, in the format yyyy-mm-dd hh:mm:ss.
     */
    monthsAgoStart(months: number): string;
    
    /**
     * Queries an object and returns true if the object is null, undefined, or contains an empty string.
     * @param {*} o - The object to be checked.
     * @returns {boolean} True if the object is null, undefined, or contains an empty string; otherwise, returns false..
     */
    nil(o: any): boolean;
    
    /**
     * Returns the date and time for the last day of the quarter for a specified number of quarters ago.
     * @param {number} quarters - Integer number of quarters
     * @returns {string} GMT end of the quarter that was the specified number of quarters ago, in the format yyyy-mm-dd hh:mm:ss.
     */
    quartersAgoEnd(quarters: number): string;
    
    /**
     * Returns the date and time for the first day of the quarter for a specified number of quarters ago.
     * @param {number} quarters - Integer number of quarters
     * @returns {string} GMT end of the month that was the specified number of quarters ago, in the format yyyy-mm-dd hh:mm:ss.
     */
    quartersAgoStart(quarters: number): string;
    
    /**
     * Sets the specified key to the specified value if the property is within the script's scope.
     * @param {string} key - The key for the property to be set.
     * @param {string} value - The value of the property to be set.
     * @param {string} description - A description of the property.
     */
    setProperty(key: string, value: string, description: string): void;
    
    /**
     * Sets the redirect URI for this transaction, which then determines the next page the user will see.
     * @param {*} o - URI object or URI string to set as the redirect
     */
    setRedirect(o: any): void;
    
    /**
     * Determines if a database table exists.
     * @param {string} name - Name of the table to check for existence.
     * @returns {boolean} True if the table exists. False if the table was not found..
     */
    tableExists(name: string): boolean;
    
    /**
     * Replaces UTF-8 encoded characters with ASCII characters.
     * @param {string} url - A string with UTF-8 percent (%) encoded characters.
     * @returns {string} A string with encoded characters replaced with ASCII characters..
     */
    urlDecode(url: string): string;
    
    /**
     * Encodes non-ASCII characters, unsafe ASCII characters, and spaces so the returned string can be used on the Internet. Uses UTF-8 encoding. Uses percent (%) encoding.
     * @param {string} url - The string to be encoded.
     * @returns {string} A string with non-ASCII characters, unsafe ASCII characters, and spaces encoded..
     */
    urlEncode(url: string): string;
    
    /**
     * Writes a warning message to the system log.
     * @param {string} message - The log message with place holders for any variable arguments.
     * @param {*} [param1] - First variable argument.
     * @param {*} [param2] - Second variable argument.
     * @param {*} [param3] - Third variable argument.
     * @param {*} [param4] - Fourth variable argument.
     * @param {*} [param5] - Fifth variable argument.
     */
    warn(message: string, param1?: any, param2?: any, param3?: any, param4?: any, param5?: any): void;
    
    /**
     * Takes an XML string and returns a JSON object.
     * @param {string} xmlString - The XML string to be converted.
     * @returns {*} A JSON object representing the XML string. Null if unable to process the XML string..
     */
    xmlToJSON(xmlString: string): any;
    
    /**
     * Returns a date and time for a certain number of years ago.
     * @param {number} years - An integer number of years
     * @returns {string} GMT beginning of the year that is the specified number of years ago, in the format yyyy-mm-dd hh:mm:ss..
     */
    yearsAgo(years: number): string;
    
    /**
     * Returns yesterday's time (24 hours ago).
     * @returns {string} GMT for 24 hours ago, in the format yyyy-mm-dd hh:mm:ss.
     */
    yesterday(): string;
}

/**
 * The Scoped GlideTableHierarchy API provides methods for handling information about table relationships.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideTableHierarchyScoped/concept/c_GlideTableHierarchyScopedAPI.html}
 * @todo See if this exists on global
 */
declare class GlideTableHierarchy {
    /**
     * Instantiates a GlideTableHierarchy object.
     * @constructor
     * @param {string} tableName - The name of the table.
     */
    constructor(tableName: string);
    /**
     * Returns an array of strings containing all tables that extend the current table and includes the current table.
     * @returns {Array<*>} An array of strings containing the tables in the hierarchy that includes the current table.
     */
    getAllExtensions(): any[];
    /**
     * Returns the parent class.
     * @returns {string} The parent class.
     */
    getBase(): string;
    /**
     * Returns an array of strings containing all classes in the hierarchy of the current table.
     * @returns {Array<*>} An array of strings of the classes in the hierarchy.
     */
    getHierarchy(): any[];
    /**
     * Returns the table's name.
     * @returns {string} The table's name.
     */
    getName(): string;
    /**
     * Returns the top level class in the hierarchy.
     * @returns {string} The root class.
     */
    getRoot(): string;
    /**
     * Returns an array of strings containing all tables that extend the current table.
     * @returns {Array<*>} An array of strings containing the tables that extend the current table.
     */
    getTableExtensions(): any[];
    /**
     * Returns an array of strings of the table names in the hierarchy.
     * @returns {Array<*>} An array of strings containing the names of tables in the hierarchy.
     */
    getTables(): any[];
    /**
     * Returns true of this class has been extended.
     * @returns {boolean} True if the current table has extensions.
     */
    hasExtensions(): boolean;
    /**
     * Returns true if this is a base class.
     * @returns {boolean} True if the current table has no parent and has extensions.
     */
    isBaseClass(): boolean;
    /**
     * Returns true if this table is not in a hierarchy.
     * @returns {boolean} True if the current table has no parent and no extensions.
     */
    isSoloClass(): boolean;
}

/**
 * The GlideTime class provides methods for performing operations on GlideTime objects, such as instantiating GlideTime objects or working with GlideTime fields.
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/glideTimeScoped/concept/c_GlideTimeScopedAPI.html}
 * @todo See if this exists on global
 */
declare class GlideTime {
    /**
     * Instantiates a GlideTime object with the current time.
     * @constructor
     */
    constructor();

    /**
     * Instantiates a GlideTime object with the specified time.
     * @constructor
     * @param {number} milliseconds - The datetime in milliseconds.
     */
    constructor(milliseconds: number);
    
    equals(obj: GlideTime): boolean;
    
    /**
     * Gets the time in the specified format.
     * @param {string} format - The time format.
     * @returns {string} The time in the specified format.
     */
    getByFormat(format: string): string;

    /**
     * Gets the time in the current user's display format and time zone.
     * @returns {string} The time in the user's format and time zone.
     * @description 
     */
    getDisplayValue(): string;

    /**
     * Gets the display value in the current user's time zone and the internal format (HH:mm:ss).
     * @returns {string} The time value for the GlideTime object in the current user's time zone and the internal time format of HH:mm:ss.
     */
    getDisplayValueInternal(): string;

    /**
     * Returns the hours part of the time using the local time zone.
     * @returns {number} The hours using the local time zone.
     */
    getHourLocalTime(): number;

    /**
     * Returns the hours part of the time using the local time zone. The number of hours is based on a 24 hour clock.
     * @returns {number} The hours using the local time zone. The number of hours is based on a 24 hour clock.
     */
    getHourOfDayLocalTime(): number;

    /**
     * Returns the hours part of the time using the UTC time zone. The number of hours is based on a 24 hour clock.
     * @returns {number} The hours using the UTC time zone. The number of hours is based on a 24 hour clock.
     */
    getHourOfDayUTC(): number;

    /**
     * Returns the hours part of the time using the UTC time zone. The number of hours is based on a 12 hour clock. Noon and midnight are represented by 0, not 12.
     * @returns {number} The hours using the UTC time zone. The number of hours is based on a 12 hour clock. Noon and midnight are represented by 0, not 12.
     */
    getHourUTC(): number;

    /**
     * Returns the number of minutes using the local time zone.
     * @returns {number} The number of minutes using the local time zone.
     */
    getMinutesLocalTime(): number;

    /**
     * Returns the number of minutes in the hour based on the UTC time zone.
     * @returns {number} The number of minutes in the hour using the UTC time zone.
     */
    getMinutesUTC(): number;

    /**
     * Returns the number of seconds in the current minute.
     * @returns {number} The number of seconds in the minute.
     */
    getSeconds(): number;

    /**
     * Gets the time value stored in the database by the GlideTime object in the internal format, HH:mm:ss, and the system time zone.
     * @returns {string} The time value in the internal fomat and system time zone.
     */
    getValue(): string;

    /**
     * Sets a time value using the current user's display format and time zone.
     * @param {string} asDisplayed - The time in the current user's display format and time zone. The parameter must be formatted using the current user's preferred display format, such as HH:mm:ss.
     */
    setDisplayValue(asDisplayed: string): void;

    /**
     * Sets the time of the GlideTime object in the internal time zone.
     * @param {string} o - The time in hh:mm:ss format.
     */
    setValue(o: string): void;

    /**
     * Gets the duration difference between two GlideTime object values.
     * @param {GlideTime} startTime - The start value.
     * @param {GlideTime} endTime - The end value.
     * @returns {GlideDuration} The duration between the two values.
     */
    subtract(startTime: GlideTime, endTime: GlideTime): GlideDuration;

    // TODO: Implement getByFormat             from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getDisplayValue         from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getDisplayValueInternal from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getHourLocalTime        from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getHourOfDayLocalTime   from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getHourOfDayUTC         from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getHourUTC              from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getMinutesLocalTime     from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getMinutesUTC           from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getSeconds              from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement getValue                from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement setDisplayValue         from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement setValue                from com.glide.glideobject.GlideTime          under com.glide.script.fencing.ScopedGlideTime
    // TODO: Implement subtract                from com.glide.script.fencing.ScopedGlideTime under com.glide.script.fencing.ScopedGlideTime
    
    hashCode(): $$rhino.Number;
    toString(): string;
}

declare class GlideUpdateManager2 {
    allowBackout(sysId: string): boolean;
    allowVersionBackout(sysId: string): boolean;
    constructor(updateSetId?: string);
    getDefaultUpdateName(tableName: string, uniqueValue: string): string;
    getUpdateName(gr: GlideRecord): string;
    // load(updateName: string): void;
    // load(updateName: string, directory: string): void;
    load(updateName: string, directory?: string): void;
    loadFile(filePath: string): void;
    // loadFixes(updateName: string, before: boolean): void;
    // loadFixes(updateName: string): void;
    loadFixes(updateName: string, before?: boolean): void;
    loadFromDatabase(category: string): void;
    loadIntoDatabase(category: string): boolean;
    // loadXML(xml: string): void;
    // loadXML(xml: string, writeVersion: boolean, revertedFrom: string, sourceTable: string, sourceId: string): void;
    loadXML(xml: string, writeVersion?: boolean, revertedFrom?: string, sourceTable?: string, sourceId?: string): void;
    removeUpdateSet(setID: string): void;
    saveBaselineChoiceListElements(tableName: string, fieldName: string): void;
    saveChoiceListElements(tableName: string, fieldName: string): void;
    saveListElements(sl: GlideSysList): void;
    saveRecord(gr: GlideRecord): boolean;
    setInstalling(b: boolean): void;
}

/**
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideURIGlobal/concept/GlideURIGlobalAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/GlideURIGlobalAPI}
 * @todo Add members for GlideURI
 */
declare class GlideURI {
}

/**
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideUser_global/concept/GUserAPI.html}
 * @see {@link https://developer.servicenow.com/dev.do#!/reference/api/utah/server_legacy/GUserAPI}
 */
declare class GlideUser {
    /**
     * Returns the current user's company sys_id.
     * @returns {string} Company sys_id.
     */
    getCompanyID(): string;
    /**
     * Returns the current user's display name.
     * @returns {string} User's display name.
     */
    getDisplayName(): string;
    /**
     * Returns the display value of the user's session domain.
     * @returns {string} The display value of the user's session domain..
     */
    getDomainDisplayValue(): string;
    /**
     * Returns the user's email address.
     * @returns {string} User's email address.
     */
    getEmail(): string;
    /**
     * Returns the user's first name.
     * @returns {string} User's first name.
     */
    getFirstName(): string;
    /**
     * Returns the sys_id of the current user.
     * @returns {string} User's sys_id.
     */
    getID(): string;
    /**
     * Returns the user's last name.
     * @returns {string} User's last name.
     */
    getLastName(): string;
    /**
     * Returns an iterator containing the list of all groups to which the user belongs. Only active groups are returned.
     * @returns {Packages.java.util.Iterator<$$rhino.String>} A list of sys_ids for the active groups to which the user belongs..
     */
    getMyGroups(): Packages.java.util.Iterator<$$rhino.String>;
    /**
     * Returns the user ID, or login name, of the current user.
     * @returns {string} User ID or login name..
     */
    getName(): string;
    /**
     * Returns a list of roles that includes explicitly granted roles, inherited roles, and roles acquired by group membership.
     * @returns {Array<*>} List of all roles available to the user.
     */
    getRoles(): any[];
    /**
     * Returns the user object associated with the passed-in user ID (sys_id in sys_user) or user_name.
     * @param {string} id - Unique ID (sys_id) or user_name of the desired user record.
     * @returns {*} User object associated with the specified sys_id or user_name..
     */
    getUserByID(id: string): any;
    /**
     * Returns the list of roles explicitly granted to the user.
     * @returns {Array<*>} List of roles explicitly assigned to the user.
     */
    getUserRoles(): any[];
    /**
     * Determines if the current user has the specified role.
     * @param {string} role - Role to check
     * @returns {boolean} True if the user has the role..
     */
    hasRole(role: string): boolean;
    /**
     * Determines if the current user is a member of the specified group.
     * @param {string} group - Group to check
     * @returns {boolean} True if the user is a member of the group..
     */
    isMemberOf(group: string): boolean;
}

declare interface IGlideVariableComponent extends IGlideElementGlideObject {
    isMultiRow(): boolean;
}

/**
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/GlideXMLUtilScoped/concept/GlideXMLUtilScopedAPI.html}
 * @todo Add members for GlideXMLUtil
 */
declare class GlideXMLUtil {
}

declare class GlideXMLDocument {
    setNamespaceAware(value: boolean): void;
    parse(text: string): void;
    getDocumentElement(): Packages.org.w3c.dom.Element;
    createElement(name: string, value: any): Packages.org.w3c.dom.Element;
    createCDATAElement(name: string, value: any): Packages.org.w3c.dom.Element;
    getDocumentElement(): Packages.org.w3c.dom.Element;
    selectSingleNodeText(xpath: string): Packages.java.lang.String;
    load(f: string): void;
    toIndentedString(): Packages.java.lang.String;
    toString(): Packages.java.lang.String;
}

declare interface IItemOptionsQuestionSet extends ISharedItemOptionQuestionSet {
    getChoices(n: $$rhino.String, bln: $$rhino.Boolean): ChoiceList;
    getCartItem(): GlideRecord;
}

declare type QuestionList = Packages.java.util.ArrayList<Question>;

declare interface IQuestionSet {
    getMandatoryName(): Packages.java.lang.String;
    getLabelClass(): Packages.java.lang.String;
    getWidgetClass(): Packages.java.lang.String;
    getTableClass(): Packages.java.lang.String;
    isUseSpacers(): $$rhino.Boolean;
    setUseSpacers(bln: $$rhino.Boolean): void;
    getMode(): $$rhino.Number;
    setMode(i: $$rhino.Number): void;
    getQuestion(): Packages.java.util.List<Question>;
    setQuestion(list: QuestionList): void;
}

declare abstract class QuestionSet implements IQuestionSet {
    getMandatoryName(): Packages.java.lang.String;
    getLabelClass(): Packages.java.lang.String;
    getWidgetClass(): Packages.java.lang.String;
    getTableClass(): Packages.java.lang.String;
    isUseSpacers(): $$rhino.Boolean;
    setUseSpacers(bln: $$rhino.Boolean): void;
    getMode(): $$rhino.Number;
    setMode(i: $$rhino.Number): void;
    getQuestion(): Packages.java.util.List<Question>;
    setQuestion(list: QuestionList): void;
    isPopup(): $$rhino.Boolean;
    isPrint(): $$rhino.Boolean;
    isShowHelpOnLoad(): $$rhino.Boolean;
}

declare abstract class Question {
    getColumncount(): $$rhino.Number;
    setColumncount(i: $$rhino.Number): void;
    getContainerLayout(): Packages.java.lang.String;
    setContainerLayout(n: $$rhino.String): void;
    isValueEmpty(): $$rhino.Boolean;
    isVariableEditor(): $$rhino.Boolean;
    getSet(): QuestionSet;
    setSet(set: QuestionSet);
    getDisplayValue(): Packages.java.lang.String;
    setDisplayValue(n: $$rhino.String): void;
    canRead(): $$rhino.Boolean;
    canWrite(): $$rhino.Boolean;
    canCreate(): $$rhino.Boolean;
    getMode(): $$rhino.Number;
    getReadOnly(): $$rhino.Boolean;
    setReadOnly(bln: $$rhino.Boolean): void;
    isReadOnly(): $$rhino.Boolean;
    getName(): Packages.java.lang.String;
    setName(n: $$rhino.String): void;
    getId(): Packages.java.lang.String;
    setId(n: $$rhino.String): void;
    getLabel(): Packages.java.lang.String;
    setLabel(n: $$rhino.String): void;
    getMaxLength(): Packages.java.lang.Integer;
    setMaxLength(n: $$rhino.Number): void;
    getValue(): Packages.java.lang.String;
    setValue(n: $$rhino.String): void;
    getType(): Packages.java.lang.String;
    setType(n: $$rhino.Number): void;
    getCategory(): Packages.java.lang.String;
    setCategory(n: $$rhino.String): void;
    isMandatory(): $$rhino.Boolean;
    setMandatory(bln: $$rhino.Boolean): void;
    isShowHelp(): $$rhino.Boolean;
    setShowHelp(bln: $$rhino.Boolean): void;
    isNumeric(): $$rhino.Boolean;
    isDate(): $$rhino.Boolean;
    isActive(): $$rhino.Boolean;
    getCatalogItem(): Packages.java.lang.String;
    setCatalogItem(n: $$rhino.String): void;
}

declare interface ISharedItemOptionQuestionSet extends IQuestionSet {
    getCatalogItemID(): Packages.java.lang.String;
    setCatalogItemID(n: $$rhino.String): void;
    getFlatQuestions(): Packages.java.util.List<Question>;
    getFlatQuestionsAndContainers(): Packages.java.util.List<Question>;
}

declare interface IVariablePoolQuestionSet extends IItemOptionsQuestionSet {
    getItemOptionID(qstn: Question): Packages.java.lang.String;
    getRequestID(): Packages.java.lang.String;
    getTaskID(): Packages.java.lang.String;
}

/**
 * @see {@link https://docs.servicenow.com/bundle/utah-api-reference/page/app-store/dev_portal/API_reference/XMLDocument2ScopedAPI/concept/c_XMLDocument2ScopedAPI.html}
 * @todo Add XMLDocument2 members
 */
declare class XMLDocument2 {
}

declare namespace sn_kmf_ns {
    /**
     * @todo Add KMFCryptoModule members
     */
    export class KMFCryptoModule {
    }
}

declare namespace Packages {
    export namespace com {
        export namespace glide {
            export namespace script {
                /**
                 * @todo Add GlideController members
                 */
                export class GlideController {
                }
            }
        }
    }
}

declare var gs: GlideSystem;

/**
 * Contains helper types for table-specific GlideElement instances.
 * @namespace $$GlideElement
 * */
declare namespace $$GlideElement {
    /**
    * Reference
    * scalar_type: GUID; name: reference
    */
    export type ReferenceBase<TFields = {}, TRecord extends GlideRecord & TFields = GlideRecord & TFields> = TFields & {
        getRefRecord(): TRecord;
    } & GlideElementReference;

    /**
    * Reference
    * scalar_type: GUID; name: reference
    */
    export type Reference<TFields = $$tableFields.IBaseRecord, TRecord extends GlideRecord & TFields = GlideRecord & TFields> = TFields & {
        getRefRecord(): TRecord;
    } & GlideElementReference;
}

declare namespace $$tableFields {
    export interface IBaseRecord {
        /**
         * "Sys ID" column element.
         * Type: "Sys ID (GUID)"
         * Primary: true; Max Length: 32.
         */
        sys_id: GlideElement;

        /**
         * "Created by" column element.
         * Maximum length: 40
         */
        sys_created_by: GlideElement;

        /**
         * "Created" column element.
         * Type: "Date/Time" (glide_date_time)
         * Maximum length: 40
         */
        sys_created_on: GlideElementGlideObject;

        /**
         * "Updates" column element.
         * Type: "Integer" (integer)
         * Maximum length: 40
         */
        sys_mod_count: GlideElement;

        /**
         * "Updated by" column element.
         * Maximum length: 40
         */
        sys_updated_by: GlideElement;

        /**
         * "Updated" column element.
         * Type: "Date/Time" (glide_date_time)
         * Maximum length: 40
         */
        sys_updated_on: GlideElementGlideObject;
    }

}