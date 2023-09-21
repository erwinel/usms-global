/// <reference path="../types/index.d.ts" />
/// <reference path="x_g_doj_usmsprogop.d.ts" />
declare namespace x_g_doj_usmsprogop {
    export interface INameAndSysId {
        name: string;
        sys_id: string;
    }
    /**
     * Defines the public instance members added by the {@link UsmsProgramOpsUtil} type.
     */
    export interface UsmsProgramOpsUtilPublic extends Readonly<{}> {
    }
    /**
     * The general-purpose utility class for the "USMS Program Operations" (x_g_doj_usmsprogop) ServiceNow application.
     */
    export type UsmsProgramOpsUtil = UsmsProgramOpsUtilPublic & AbstractAjaxProcessor;
    /**
     * Defines the {@link UsmsProgramOpsUtil} constructor.
     */
    export interface UsmsProgramOpsUtilConstructor extends $$class.Constructor<UsmsProgramOpsUtil> {
        /**
         * Initializes a new {@link UsmsProgramOpsUtil} object.
         */
        new (): UsmsProgramOpsUtil;
        /**
         * The name of the "Physical Network" configuration items table.
         * @static
         */
        TABLENAME_physical_network: 'x_g_doj_usmsprogop_u_cmdb_ci_physical_network';
        /**
         * The name of the "Access Template" configuration items table.
         * @static
         */
        TABLENAME_cmdb_ci_access_template: 'x_g_doj_usmsprogop_u_cmdb_ci_access_template';
        /**
         * The name of the "Group Access Assignment" configuration items table.
         * @static
         */
        TABLENAME_cmdb_ci_group_access_assignment: 'x_g_doj_usmsprogop_u_cmdb_ci_group_access_assignment';
        /**
         * The name of the "User Access Group" configuration items table.
         * @static
         */
        TABLENAME_cmdb_ci_user_access_group: 'x_g_doj_usmsprogop_u_cmdb_ci_user_access_group';
        /**
         * The name of the "Login Account" configuration items table.
         * @static
         */
        TABLENAME_cmdb_ci_login_account: 'x_g_doj_usmsprogop_u_cmdb_ci_login_account';
        /**
         * The name of the "Network Drive Mapping" configuration items table.
         * @static
         */
        TABLENAME_cmdb_ci_network_drive_mapping: 'x_g_doj_usmsprogop_u_cmdb_ci_network_drive_mapping';
        /**
         * The name of the "Resource Access Definition" configuration items table.
         * @static
         */
        TABLENAME_cmdb_ci_resource_access_definition: 'x_g_doj_usmsprogop_cmdb_ci_resource_access_definition';
        /**
         * Constructor for the {@link DependenciesIterator} class.
         */
        DependenciesIterator: UsmsProgramOpsUtil.DependenciesIteratorConstructor;
        /**
         * Constructor for the {@link PhysicalNetworksIterator} class.
         */
        PhysicalNetworksIterator: UsmsProgramOpsUtil.PhysicalNetworksIteratorConstructor;
        /**
         * Joins elements of an array into a comma/conjunction-delimited phrase with the last element joined by the conjunctive word.
         * @param {string[]} arr - The phrase elements to join.
         * @param {string} [conjunction] - The conjunctive word between the last 2 elements, which defaults to 'or' if not provided.
         * @return {string} The comma/conjunction-delimited phrase.
         */
        toCommaSeparatedPhrase(arr: string[], conjunction?: string): string;
        /**
         * Gets whitespace-normalized string from a source value.
         * @param {(any | undefined)} value - The value to convert to a whitespace-normalized string.
         * @param {string} [defaultValue] - The optional string value to return if the normalized result is an empty string.
         * @return {(string | undefined)} - The whitespace-normalized string value or the default value if the normalized result is empty.
         */
        normalizeWhitespace(value: any | undefined, defaultValue?: string): string | undefined;
        copySlice<T>(source: T[], start: number, end?: number): T[];
        copySlice<T, U>(source: T[], start: number, mapfn: {
            (v: T, k: number): U;
        }): U[];
        copySlice<T, U>(source: T[], start: number, end: number, mapfn: {
            (v: T, k: number): U;
        }): U[];
        copySlice<T, U, V>(source: T[], start: number, mapfn: {
            (this: V, v: T, k: number): U;
        }, thisArg: V): U[];
        copySlice<T, U, V>(source: T[], start: number, end: number, mapfn: {
            (this: V, v: T, k: number): U;
        }, thisArg: V): U[];
        /**
         * Creates a clopy of an array.
         * @template T - The element type.
         * @param {T[]} source - The source array.
         * @return {T[]} A copy of the source array.
         */
        copyArray<T>(source: T[]): T[];
        /**
         * Creates a mapped copy of an array.
         * @template T - The source element type.
         * @template U - The output element type.
         * @param {T[]} source - The source array.
         * @param {{ (v: T, k: number): U; }} mapfn - The function that creates a result element from an input element.
         * @return {U[]} The mapped copy of the source array.
         */
        copyArray<T, U>(source: T[], mapfn: {
            (v: T, k: number): U;
        }): U[];
        /**
         * Creates a mapped copy of an array.
         * @template T - The source element type.
         * @template U - The output element type.
         * @template V - The type of the 'this' object for the mapper function.
         * @param {T[]} source - The source array.
         * @param {{ (this: V, v: T, k: number): U; }} mapfn - The function that creates a result element from an input element.
         * @param {V} thisArg - The object for the 'this' variable the mapper function invocation.
         * @return {U[]} The mapped copy of the source array.
         */
        copyArray<T, U, V>(source: T[], mapfn: {
            (this: V, v: T, k: number): U;
        }, thisArg: V): U[];
        /**
         * Tests where the expiration date occurs on or after the effective date, if it is defined.
         * @param {GlideElement} effectiveDate - The Glide Element containing the effective date.
         * @param {GlideElement} expirationDate - The Glide Element containing the expiration date.
         * @return {boolean} - True if the expiration date is not defined or if it does not occur before the effective date; otherwise, false, to indicate that the expiration date is earlier than the effective date.
         */
        validateExpirationDate(effectiveDate: GlideElement, expirationDate: GlideElement): boolean;
        /**
         *
         * Tests a {@link $$record.cmdb_ci_physical_network} GlideRecord for validity.
         * @param {$$record.cmdb_ci_physical_network} glideRecord - The GlideRecord to test.
         * @param {boolean} [setError] - If true and there is a redundant restricted networks reference, {@link GlideElement#setError} will be called to set the error message.
         * @return {(boolean | string[])} True if the GlideRecord represents a valid physical network; false if the GlideRecord itself was invalid; otherwise, an array of duplicate restricted networks that were removed.
         */
        validatePhysicalNetwork(glideRecord: $$record.cmdb_ci_physical_network, setError?: boolean): boolean | string[];
        /**
         * Gets an object that iterates through the dependencies of a specific CI.
         * @param {(string | GlideRecord | GlideElement)} dependent - The Sys ID, GlideRecord or Glide Element of the target CI.
         * @param {(string | string[])} [className] - Optional CI class name(s) of specific dependencies to look for. If not specified, then all dependencies are iterated.
         * @return {Iterator<GlideRecord>} The object that iterates through the CI dependencies.
         */
        getDependencyIterator(dependent: string | GlideRecord | GlideElement, className?: string | string[]): Iterator<GlideRecord>;
        /**
         * Searches for the first CI that a specified CI depends upon.
         * @param {(string | GlideRecord | GlideElement)} dependent - The Sys ID, GlideRecord or Glide Element of the target CI.
         * @param {(string | string[])} [className] - Optional CI class name(s) of specific dependencies to look for. If not specified, then the first dependency of any class is returned.
         * @return {GlideRecord | undefined} - The GlideRecord of the first found dependency or undefined if none were found.
         */
        getFirstDependency(dependent: string | GlideRecord | GlideElement, className?: string | string[]): GlideRecord | undefined;
        /**
         * Gets the Sys IDs of direct and indirect depencencies for a specified CI.
         * @param {(string | GlideRecord | GlideElement)} dependent - The Sys ID, GlideRecord or Glide Element of the target CI.
         * @param {(string | string[])} [className] - Optional CI class name(s) of specific dependencies to look for. If not specified, then the SysIDs of all dependencies are returned.
         * @return {string[]} The Sys IDs of direct and indirect depencencies for a specified CI.
         */
        getDependencies(dependent: string | GlideRecord | GlideElement, className?: string | string[]): string[];
        /**
         * Gets the first direct or indirect {@link $$record.cmdb_ci_physical_network} dependency for a specified CI.
         * @param {(GlideRecord | GlideElement)} dependent - The GlideRecord or Glide Element of the target CI.
         * @return {($$record.cmdb_ci_physical_network | undefined)} - The GlideRecord of the first {@link $$record.cmdb_ci_physical_network} dependency found or undefined if none were found.
         */
        getFirstPhysicalNetwork(dependent: GlideRecord | GlideElement): $$record.cmdb_ci_physical_network | undefined;
        /**
         * Gets the name and Sys ID of all physical network dependencies.
         * @param {(GlideRecord | GlideElement)} dependent - The GlideRecord or Glide Element of the target CI.
         * @return {INameAndSysId[]} - A list of the names and SysIDs of all direct and indirect {@link $$record.cmdb_ci_physical_network} dependencies.
         */
        getPhysicalNetworks(dependent: GlideRecord | GlideElement): INameAndSysId[];
        /**
         * Gets the first Location record where "Location type" is "Region" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "region" or undefined if none were found.
         */
        getGetRegionFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Country" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "country" or undefined if none were found.
         */
        getGetCountryFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "State/Province" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "state/province" or undefined if none were found.
         */
        getGetStateFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "City" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "city" or undefined if none were found.
         */
        getGetCityFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Site" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "site" or undefined if none were found.
         */
        getGetSiteFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Building/Structure" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "building/structure" or undefined if none were found.
         */
        getGetBuildingFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Floor" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "floor" or undefined if none were found.
         */
        getGetFloorFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Zone" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "zone" or undefined if none were found.
         */
        getGetZoneFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Room" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "room" or undefined if none were found.
         */
        getGetRoomFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Region" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "region" or undefined if none were found.
         */
        getGetRegionFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Country" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "country" or undefined if none were found.
         */
        getGetCountryFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "State/Province" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "state/province" or undefined if none were found.
         */
        getGetStateFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "City" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "city" or undefined if none were found.
         */
        getGetCityFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Site" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "site" or undefined if none were found.
         */
        getGetSiteFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Building/Structure" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "building/structure" or undefined if none were found.
         */
        getGetBuildingFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Floor" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "floor" or undefined if none were found.
         */
        getGetFloorFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Room" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "room" or undefined if none were found.
         */
        getGetRoomFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the first Location record where "Location type" is "Zone" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {$$GlideRecord.cmn_location | undefined)} - The first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "zone" or undefined if none were found.
         */
        getGetZoneFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): $$GlideRecord.cmn_location | undefined;
        /**
         * Gets the name of first Location record where "Location type" is "Region" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "region" or undefined if none were found.
         */
        getGetRegionNameFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): string | undefined;
        /**
         * Gets the first country name within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {(string | undefined)} - The value of the first {@link $$tableFields.cmn_location#country} element that is not empty;
         *         or the name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "region"; otherwise or undefined if none were found.
         */
        getGetCountryNameFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): string | undefined;
        /**
         * Gets the name of first Location record where "Location type" is "State/Province" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {(string | undefined)} - The value of the first {@link $$tableFields.cmn_location#state} element that is not empty;
         *         or the name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "region"; otherwise or undefined if none were found.
         */
        getGetStateNameFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): string | undefined;
        /**
         * Gets the first city name within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {(string | undefined)} - The value of the first {@link $$tableFields.cmn_location#city} element that is not empty;
         *         or the name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "region"; otherwise or undefined if none were found.
         */
        getGetCityNameFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): string | undefined;
        /**
         * Gets the name of first Location record where "Location type" is "Site" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "site" or undefined if none were found.
         */
        getGetSiteNameFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): string | undefined;
        /**
         * Gets the name of first Location record where "Location type" is "Building/Structure" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "building/structure" or undefined if none were found.
         */
        getGetBuildingNameFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): string | undefined;
        /**
         * Gets the name of first Location record where "Location type" is "Floor" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "floor" or undefined if none were found.
         */
        getGetFloorNameFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): string | undefined;
        /**
         * Gets the name of first Location record where "Location type" is "Zone" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "zone" or undefined if none were found.
         */
        getGetZoneNameFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): string | undefined;
        /**
         * Gets the name of first Location record where "Location type" is "Room" within the location's ancenstrial chain, including the provided location.
         * @param {($$GlideRecord.cmn_location | $$GlideElement.cmn_location)} glideObj - The cmn_location GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "room" or undefined if none were found.
         */
        getGetRoomNameFromLocation(glideObj: $$GlideRecord.cmn_location | $$GlideElement.cmn_location): string | undefined;
        /**
         * Gets the name of the first Location record where "Location type" is "Region" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "region" or undefined if none were found.
         */
        getGetRegionNameFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): string | undefined;
        /**
         * Gets the name of the country for the given user or the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {(string | undefined)} - The value of {@link $$tableFields.sys_user.country} if not empty;
         *         or value of the first {@link $$tableFields.cmn_location#country} element that is not empty;
         *         or the name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "country"; otherwise undefined if none were found.
         */
        getGetCountryNameFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): string | undefined;
        /**
         * Gets the name of the state for the given user or the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {(string | undefined)} - The value of {@link $$tableFields.sys_user.state} if not empty;
         *         or value of the first {@link $$tableFields.cmn_location#state} element that is not empty;
         *         or the name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "state"; otherwise undefined if none were found.
         */
        getGetStateNameFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): string | undefined;
        /**
         * Gets the name of the city for the given user or the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {(string | undefined)} - The value of {@link $$tableFields.sys_user.city} if not empty;
         *         or value of the first {@link $$tableFields.cmn_location#city} element that is not empty;
         *         or the name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "city"; otherwise undefined if none were found.
         */
        getGetCityNameFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): string | undefined;
        /**
         * Gets the name of the first Location record where "Location type" is "Site" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "site" or undefined if none were found.
         */
        getGetSiteNameFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): string | undefined;
        /**
         * Gets the name of the given user's building or the name of the location name in in user's location and it's ancenstrial chain where the "Location type" is "Building/Structure".
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of {@link $$tableFields.sys_user.building} if not empty;
         *         or the name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "building/structure"; otherwise undefined if none were found.
         */
        getGetBuildingNameFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): string | undefined;
        /**
         * Gets the name of the first Location record where "Location type" is "Floor" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "floor" or undefined if none were found.
         */
        getGetFloorNameFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): string | undefined;
        /**
         * Gets the name of the first Location record where "Location type" is "Room" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "room" or undefined if none were found.
         */
        getGetRoomNameFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): string | undefined;
        /**
         * Gets the name of the first Location record where "Location type" is "Zone" within the user's location and it's ancenstrial chain or the location of the user's building.
         * @param {($$GlideRecord.sys_user | $$GlideElement.sys_user)} glideObj - The sys_user GlideRecord or GlideElement.
         * @return {(string | undefined)} - The name of the first Location record where {@link $$tableFields.cmn_location#cmn_location_type} is "zone" or undefined if none were found.
         */
        getGetZoneNameFromUser(glideObj: $$GlideRecord.sys_user | $$GlideElement.sys_user): string | undefined;
    }
    interface ITableExtendsThisObj {
        [key: string]: string | null;
    }
    interface IDependenciesIteratorPrototype extends UsmsProgramOpsUtil.DependenciesIterator {
        _visited: string[];
        _extendsThisObj: ITableExtendsThisObj;
        _current?: $$GlideRecord.cmdb_rel_ci;
        _iterating: $$GlideRecord.cmdb_rel_ci[];
        _className?: string | string[];
    }
    interface IPhysicalNetworksIteratorPrototype extends UsmsProgramOpsUtil.PhysicalNetworksIterator {
        _backingIterator: UsmsProgramOpsUtil.DependenciesIterator;
        _networkIterator?: UsmsProgramOpsUtil.DependenciesIterator;
    }
    /**
     * Nested types for the UsmsProgramOpsUtil class.
     */
    export namespace UsmsProgramOpsUtil {
        /**
         * Iterates through the configuration items that a specific CI depends upon, including indirect dependencies.
         */
        interface DependenciesIterator extends Readonly<Iterator<GlideRecord>> {
        }
        /**
         * Constructor for the {@link DependenciesIterator} class.
         */
        interface DependenciesIteratorConstructor extends $$class.Constructor3Opt2<string | GlideRecord | GlideElement, string | string[], DependenciesIterator, DependenciesIterator, IDependenciesIteratorPrototype> {
            /**
             * Initializes a new instance of the {@link DependenciesIterator} class.
             * @param {(string | GlideRecord | GlideElement)} dependent - The Sys ID, GlideRecord or Glide Element of the dependent configuration item.
             * @param {(string | string[])} [className] - Optional CI class names(s) of specific dependencies to search for.
             * @param {DependenciesIterator} [cacheSource] - Optional parent iterator (used interally for lookup caching optimization).
             */
            new (dependent: string | GlideRecord | GlideElement, className?: string | string[], cacheSource?: DependenciesIterator): DependenciesIterator;
        }
        /**
         * Iterates through the physical network dependencies for a specific CI, including indirect dependencies.
         */
        interface PhysicalNetworksIterator extends Readonly<Iterator<$$record.cmdb_ci_physical_network>> {
        }
        /**
         * Constructor for the {@link PhysicalNetworksIterator} class.
         */
        interface PhysicalNetworksIteratorConstructor extends $$class.Constructor1<GlideRecord | GlideElement, PhysicalNetworksIterator, IPhysicalNetworksIteratorPrototype> {
            /**
             * Initializes a new instance of the {@link PhysicalNetworksIterator} class.
             * @param {(GlideRecord|GlideElement)} dependent - The dependent configuration item.
             * @memberof PhysicalNetworksIterator
             */
            new (dependent: GlideRecord | GlideElement): PhysicalNetworksIterator;
        }
    }
    /**
     * The {@link UsmsProgramOpsUtil} constructor object.
     */
    export const UsmsProgramOpsUtil: UsmsProgramOpsUtilConstructor;
    export {};
}
