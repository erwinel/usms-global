/// <reference path="../types/index.d.ts" />

declare namespace x_g_doj_usmsprogop {
    /**
     * GlideRecord types defined in the "USMS Program Operations" (x_g_doj_usmsprogop) ServiceNow application.
     */
    export namespace $$record {
        /**
         * "Group Access Assignment" GlideRecord.
         */
        export type cmdb_ci_group_access_assignment = $$fields.cmdb_ci_group_access_assignment & $$GlideRecord.cmdb_ci;
        
        /**
         * "Access Template" GlideRecord.
         */
        export type cmdb_ci_access_template = $$fields.cmdb_ci_access_template & cmdb_ci_user_acl_definition;
        
        /**
         * "Login Access Assignment" GlideRecord.
         */
        export type cmdb_ci_login_access_assignment = $$fields.cmdb_ci_login_access_assignment & $$GlideRecord.cmdb_ci;
        
        /**
         * "Login Account" GlideRecord.
         */
        export type cmdb_ci_login_account = $$fields.cmdb_ci_login_account & cmdb_ci_user_acl_definition;
        
        /**
         * "Network Drive Mapping" GlideRecord.
         */
        export type cmdb_ci_network_drive_mapping = $$fields.cmdb_ci_network_drive_mapping & $$GlideRecord.cmdb_ci;
        
        /**
         * "Person" GlideRecord.
         */
        export type program_person = $$fields.program_person & GlideRecord;
        
        /**
         * "Physical Network" GlideRecord.
         */
        export type cmdb_ci_physical_network = $$fields.cmdb_ci_physical_network & $$GlideRecord.cmdb_ci;
        
        /**
         * "Resource Access Definition" GlideRecord.
         */
        export type cmdb_ci_resource_access_definition = $$fields.cmdb_ci_resource_access_definition & $$GlideRecord.cmdb_ci;
        
        /**
         * "User Access Group" GlideRecord.
         */
        export type cmdb_ci_user_access_group = $$fields.cmdb_ci_user_access_group & cmdb_ci_user_acl_definition;
        
        /**
         * "User ACL Definition" GlideRecord.
         * Extendable: true
         */
        export type cmdb_ci_user_acl_definition = $$fields.cmdb_ci_user_acl_definition & $$GlideRecord.cmdb_ci;
    }

    /**
     * Glide Element referring to GlideRecord types defined in the "USMS Program Operations" (x_g_doj_usmsprogop) ServiceNow application.
     */
    export namespace $$element {
        /**
         * Reference element for the "Group Access Assignment" record type.
         */
        export type cmdb_ci_group_access_assignment = $$GlideElement.Reference<$$fields.cmdb_ci_group_access_assignment, $$record.cmdb_ci_group_access_assignment> & $$GlideElement.cmdb_ci;
        
        /**
         * Reference element for the "Access Template" record type.
         */
        export type cmdb_ci_access_template = $$GlideElement.Reference<$$fields.cmdb_ci_access_template, $$record.cmdb_ci_access_template> & cmdb_ci_user_acl_definition;
        
        /**
         * Reference element for the "Login Access Assignment" record type.
         */
        export type cmdb_ci_login_access_assignment = $$GlideElement.Reference<$$fields.cmdb_ci_login_access_assignment, $$record.cmdb_ci_login_access_assignment> & $$GlideElement.cmdb_ci;
        
        /**
         * Reference element for the "Login Account" record type.
         */
        export type cmdb_ci_login_account = $$GlideElement.Reference<$$fields.cmdb_ci_login_account, $$record.cmdb_ci_login_account> & cmdb_ci_user_acl_definition;
        
        /**
         * Reference element for the "Network Drive Mapping" record type.
         */
        export type cmdb_ci_network_drive_mapping = $$GlideElement.Reference<$$fields.cmdb_ci_network_drive_mapping, $$record.cmdb_ci_network_drive_mapping> & $$GlideElement.cmdb_ci;
        
        /**
         * Reference element for the "Person" record type.
         */
        export type program_person = $$GlideElement.Reference<$$fields.program_person, $$record.program_person>;
        
        /**
         * Reference element for the "Physical Network" record type.
         */
        export type cmdb_ci_physical_network = $$GlideElement.Reference<$$fields.cmdb_ci_physical_network, $$record.cmdb_ci_physical_network> & $$GlideElement.cmdb_ci;
        
        /**
         * Reference element for the "Resource Access Definition" record type.
         */
        export type cmdb_ci_resource_access_definition = $$GlideElement.Reference<$$fields.cmdb_ci_resource_access_definition, $$record.cmdb_ci_resource_access_definition> & $$GlideElement.cmdb_ci;
        
        /**
         * Reference element for the "User Access Group" record type.
         */
        export type cmdb_ci_user_access_group = $$GlideElement.Reference<$$fields.cmdb_ci_user_access_group, $$record.cmdb_ci_user_access_group> & cmdb_ci_user_acl_definition;
        
        /**
         * Reference element for the "User ACL Definition" record type.
         */
        export type cmdb_ci_user_acl_definition = $$GlideElement.Reference<$$fields.cmdb_ci_user_acl_definition, $$record.cmdb_ci_user_acl_definition> & $$GlideElement.cmdb_ci;
    }

    /**
     * Glide Element representing fields defined by GlideRecord types in the "USMS Program Operations" (x_g_doj_usmsprogop) ServiceNow application.
     */
    export namespace $$fields {
        /**
         * "Group Access Assignment" GlideRecord fields.
         * @see {@link $$record.cmdb_ci_group_access_assignment}
         * @see {@link $$element.cmdb_ci_group_access_assignment}
         */
        export interface cmdb_ci_group_access_assignment extends $$tableFields.cmdb_ci {
            /**
             * "Additional Properties" column element.
             * Max Length: 40.
             *
             * Allows other arbitrary named values to be associated with this access assignment.
             */
            u_additional_properties: GlideElementSimpleNameValue;
            
            /**
             * "Effective Date" column element.
             * Type: "Date" (glide_date); Scalar type: date.
             * Mandatory: true; Max Length: 40.
             *
             * he date when the Target Group is eligible for the prescribed network resource access privileges and group inheritances.
             */
            u_effective_date: GlideElementGlideObject;
            
            /**
             * "Expiration Date" column element.
             * Type: "Date" (glide_date); Scalar type: date.
             * Max Length: 40.
             *
             * The optional date when the Target Group is no longer eligible for the specified network resource access privileges and inherited (nested) groups.
             */
            u_expiration_date: GlideElementGlideObject;
            
            /**
             * "Expiration Overridden By" column element.
             * Max Length: 32.
             *
             * This indicates who authorized a change to the expiration date.
             */
            u_expiration_overridden_by: $$GlideElement.sys_user;
            
            /**
             * "Included Groups" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * The prescribed inherited (nested) groups for which the Target Group is eligible on or after the Effictive Date and before the Expiration Date (if specified).
             */
            u_included_groups: $$element.cmdb_ci_user_access_group;
            
            /**
             * "Is Implicit Network" column element.
             * Max Length: 40.
             * Default Value: false.
             */
            u_is_implicit_network: GlideElementBoolean;
            
            /**
             * "Network" column element.
             * Read-only: true; Max Length: 32.
             *
             * The network that the Target Group, Included Groups and Network Resource Access Privileges belong to.
             */
            u_network: $$element.cmdb_ci_physical_network;
            
            /**
             * "Network Resource Access Privileges" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Mandatory: true; Max Length: 40.
             *
             * The prescribed Network Resource Access Privileges for which the Target Group is eligible on or after the Effictive Date and before the Expiration Date (if specified).
             */
            u_resources: $$element.cmdb_ci_resource_access_definition;
            
            /**
             * "Special Notes`" column element.
             * Type: "HTML" (html); Scalar type: string.
             * Max Length: 40.
             *
             * Special notes that apply to the prescribed network resource access privileges and inherited (nested) group memberships.
             */
            u_special_notes: GlideElementGlideObject;
            
            /**
             * "Target Group" column element.
             * Mandatory: true; Max Length: 32.
             *
             * The User Access Group that the prescribed group inheritances and network resource access privileges applies to.
             */
            u_target_group: $$element.cmdb_ci_user_access_group;
        }
        
        /**
         * "Access Template" GlideRecord fields.
         * @see {@link $$record.cmdb_ci_access_template}
         * @see {@link $$element.cmdb_ci_access_template}
         */
        export interface cmdb_ci_access_template extends cmdb_ci_user_acl_definition {
            /**
             * "Drive Mappings" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * The prescribed Network Shared Drive mappings.
             */
            u_drive_mappings: $$element.cmdb_ci_network_drive_mapping;
            
            /**
             * "For Role Type" column element.
             * Type: "Choice" (choice).
             * Mandatory: true; Max Length: 40.
             *
             * The type of role that the Access Template is intended for.
             */
            u_for_role_type: GlideElement;
            
            /**
             * "Groups" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * The prescribed group memberships.
             */
            u_groups: $$element.cmdb_ci_user_access_group;
            
            /**
             * "Is Primary Role" column element.
             * Max Length: 40.
             *
             * Indicates whether the Access Template can be selected as the Primary Role on a Person record.
             */
            u_is_primary_role: GlideElementBoolean;
            
            /**
             * "Network Resource Access Privileges" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * The prescribed Network Resource Access Privileges.
             */
            u_resources: $$element.cmdb_ci_resource_access_definition;
        }
        
        /**
         * "Login Access Assignment" GlideRecord fields.
         * @see {@link $$record.cmdb_ci_login_access_assignment}
         * @see {@link $$element.cmdb_ci_login_access_assignment}
         */
        export interface cmdb_ci_login_access_assignment extends $$tableFields.cmdb_ci {
            /**
             * "Drive Mappings" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * The prescribed Network Shared Drive mappings for which the Target Account is eligible on or after the Effictive Date and before the Expiration Date (if specified).
             */
            u_drive_mappings: $$element.cmdb_ci_network_drive_mapping;
            
            /**
             * "Effective Date" column element.
             * Type: "Date" (glide_date); Scalar type: date.
             * Mandatory: true; Max Length: 40.
             *
             * The date when the Target Account is eligible for the prescribed group memberships and network resource access privileges.
             */
            u_effective_date: GlideElementGlideObject;
            
            /**
             * "Expiration Date" column element.
             * Type: "Date" (glide_date); Scalar type: date.
             * Max Length: 40.
             *
             * The optional date when the Target Account is no longer eligible for the specified group memberships and network resource access privileges.
             */
            u_expiration_date: GlideElementGlideObject;
            
            /**
             * "Expiration Overridden By" column element.
             * Max Length: 32.
             *
             * This indicates who authorized a change to the expiration date.
             */
            u_expiration_overridden_by: $$GlideElement.sys_user;
            
            /**
             * "Groups" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * The prescribed group memberships for which the Target Account is eligible on or after the Effictive Date and before the Expiration Date (if specified).
             */
            u_groups: $$element.cmdb_ci_user_access_group;
            
            /**
             * "Is Implicit Network" column element.
             * Max Length: 40.
             * Default Value: false.
             */
            u_is_implicit_network: GlideElementBoolean;
            
            /**
             * "Network" column element.
             * Read-only: true; Max Length: 32.
             *
             * The network that the target item, groups and network resource access privileges apply to.
             */
            u_network: $$element.cmdb_ci_physical_network;
            
            /**
             * "Other Properties" column element.
             * Max Length: 40.
             *
             * Allows other arbitrary named values to be associated with this acccess assignment.
             */
            u_other_properties: GlideElementSimpleNameValue;
            
            /**
             * "Network Resource Access Privileges" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * The prescribed Network Resource Access Privileges for which the Target Accountis eligible on or after the Effictive Date and before the Expiration Date (if specified).
             */
            u_resources: $$element.cmdb_ci_resource_access_definition;
            
            /**
             * "Special Notes" column element.
             * Type: "HTML" (html); Scalar type: string.
             * Max Length: 40.
             *
             * Special notes that apply to the group s and network resource access eligibilities defined in this record.
             */
            u_special_notes: GlideElementGlideObject;
            
            /**
             * "Target Account" column element.
             * Mandatory: true; Max Length: 32.
             *
             * The Login Account that the prescribed group memberships and network resource access privileges applies to.
             */
            u_target_account: $$element.cmdb_ci_login_account;
        }
        
        /**
         * "Login Account" GlideRecord fields.
         * @see {@link $$record.cmdb_ci_login_account}
         * @see {@link $$element.cmdb_ci_login_account}
         */
        export interface cmdb_ci_login_account extends cmdb_ci_user_acl_definition {
            /**
             * "Account Name" column element.
             * Mandatory: true; Max Length: 128.
             *
             * The login account name.
             */
            u_account_name: GlideElement;
            
            /**
             * "Effective Date" column element.
             * Type: "Date" (glide_date); Scalar type: date.
             * Mandatory: true; Max Length: 40.
             *
             * The requested creation date for the Login Account.
             */
            u_effective_date: GlideElementGlideObject;
            
            /**
             * "Expiration Date" column element.
             * Type: "Date" (glide_date); Scalar type: date.
             * Max Length: 40.
             *
             * The optional date when the Login Account should be disabled or deactivated.
             */
            u_expiration_date: GlideElementGlideObject;
            
            /**
             * "Expiration Overridden By" column element.
             * Max Length: 32.
             *
             * If not empty, this is the person who last overrode the original expiration date.
             */
            u_expiration_overridden_by: $$GlideElement.sys_user;
            
            /**
             * "Person" column element.
             * Max Length: 32.
             *
             * The associated Person record. This will be empty if the login account is for a service account.
             */
            u_person: $$element.program_person;
            
            /**
             * "Target Resource" column element.
             * Mandatory: true; Max Length: 32.
             *
             * The configuration item to which the login applies to, which is usually a network resource, such as Active Directory.
             */
            u_target_resource: $$GlideElement.cmdb_ci;
            
            /**
             * "Type" column element.
             * Type: "Choice" (choice).
             * Mandatory: true; Max Length: 40.
             *
             * The account type.
             */
            u_type: GlideElement;
            
            /**
             * "User Record" column element.
             * Max Length: 32.
             */
            u_user_record: $$GlideElement.sys_user;
        }
        
        /**
         * "Network Drive Mapping" GlideRecord fields.
         * @see {@link $$record.cmdb_ci_network_drive_mapping}
         * @see {@link $$element.cmdb_ci_network_drive_mapping}
         */
        export interface cmdb_ci_network_drive_mapping extends $$tableFields.cmdb_ci {
            /**
             * "Drive Letter" column element.
             * Type: "Char" (char); Scalar type: GUID.
             * Mandatory: true; Max Length: 40.
             *
             * The preferred drive letter for the drive mapping.
             */
            u_drive_letter: GlideElement;
            
            /**
             * "File Share" column element.
             * Mandatory: true; Max Length: 32.
             *
             * The Storage File Share that the drive is mapped to.
             */
            u_file_share: $$GlideElement.cmdb_ci_storage_fileshare;
            
            /**
             * "For Role Type" column element.
             * Type: "Choice" (choice).
             * Mandatory: true; Max Length: 40.
             *
             * The type of role that the mapped drive is intended for.
             */
            u_for_role_type: GlideElement;
            
            /**
             * "Required Groups" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * The optional group memberships required for this drive mapping.
             */
            u_groups: $$element.cmdb_ci_user_access_group;
            
            /**
             * "Is Implicit Network" column element.
             * Max Length: 40.
             * Default Value: false.
             */
            u_is_implicit_network: GlideElementBoolean;
            
            /**
             * "Network" column element.
             * Max Length: 32.
             *
             * The network of the File Share, Groups and Network Resource Access Privileges.
             */
            u_network: $$element.cmdb_ci_physical_network;
            
            /**
             * "Relative Path" column element.
             * Max Length: 1024.
             *
             * The optional path, relative to the root of the File Share, for the drive mapping.
             */
            u_relative_path: GlideElement;
            
            /**
             * "Required Network Resource Access Privileges" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * The optional Network Resource Access Privileges that are required for this drive mapping.
             */
            u_resources: $$element.cmdb_ci_resource_access_definition;
        }
        
        /**
         * "Person" GlideRecord fields.
         * @see {@link $$record.program_person}
         * @see {@link $$element.program_person}
         */
        export interface program_person extends $$tableFields.IBaseRecord {
            /**
             * "Affiliation" column element.
             * Type: "Choice" (choice).
             * Mandatory: true; Max Length: 40.
             *
             * Indicates whether the person is a government employee or a contractor.
             */
            affiliation: GlideElement;
            
            /**
             * "Business Phone" column element.
             * Max Length: 40.
             *
             * The office phone of the person.
             */
            business_phone: GlideElement;
            
            /**
             * "Department" column element.
             * Max Length: 32.
             *
             * The organizational depertment of the person.
             */
            department: $$GlideElement.cmn_department;
            
            /**
             * "First Name" column element.
             * Mandatory: true; Max Length: 50.
             */
            first_name: GlideElement;
            
            /**
             * "Full Name" column element.
             * Read-only: true; Display: true; Max Length: 151.
             */
            full_name: GlideElement;
            
            /**
             * "Inactive" column element.
             * Max Length: 40.
             * Default Value: false.
             */
            inactive: GlideElementBoolean;
            
            /**
             * "Last Name" column element.
             * Mandatory: true; Max Length: 40.
             */
            last_name: GlideElement;
            
            /**
             * "Location" column element.
             * Mandatory: true; Max Length: 32.
             *
             * The Site (Metro Office) and Region of the person.
             */
            location: $$GlideElement.cmn_location;
            
            /**
             * "Middle Name" column element.
             * Max Length: 40.
             */
            middle_name: GlideElement;
            
            /**
             * "Mobile Phone" column element.
             * Max Length: 40.
             */
            mobile_phone: GlideElement;
            
            /**
             * "Primary Role" column element.
             * Max Length: 32.
             *
             * The primary role of the person, which defines the baseline access privileges and group memberships.
             */
            primary_role: $$element.cmdb_ci_access_template;
            
            /**
             * "Special Notes" column element.
             * Type: "HTML" (html); Scalar type: string.
             * Max Length: 8000.
             */
            special_notes: GlideElementGlideObject;
            
            /**
             * "Title" column element.
             * Max Length: 60.
             */
            title: GlideElement;
        }
        
        /**
         * "Physical Network" GlideRecord fields.
         * @see {@link $$record.cmdb_ci_physical_network}
         * @see {@link $$element.cmdb_ci_physical_network}
         */
        export interface cmdb_ci_physical_network extends $$tableFields.cmdb_ci {
            /**
             * "Display Order" column element.
             * Type: "Integer" (integer).
             * Max Length: 40.
             *
             * The default listing display order in relation to other Physical Network records.
             */
            u_display_order: GlideElementNumeric;
            
            /**
             * "Restricted Transfer Networks" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * This specifies networks which have a higher classification or stricter access policies than the current network.
             */
            u_restricted_transfer_networks: $$element.cmdb_ci_physical_network;
        }
        
        /**
         * "Resource Access Definition" GlideRecord fields.
         * @see {@link $$record.cmdb_ci_resource_access_definition}
         * @see {@link $$element.cmdb_ci_resource_access_definition}
         */
        export interface cmdb_ci_resource_access_definition extends $$tableFields.cmdb_ci {
            /**
             * "For Role Type" column element.
             * Type: "Choice" (choice).
             * Mandatory: true; Max Length: 40.
             *
             * The type of role that the access definition is intended for.
             */
            u_for_role_type: GlideElement;
            
            /**
             * "Includes" column element.
             * Type: "List" (glide_list); Scalar type: string.
             * Max Length: 40.
             *
             * Other resource access definitions that are included in the current definition.
             */
            u_includes: $$element.cmdb_ci_resource_access_definition;
            
            /**
             * "Is Implicit Network" column element.
             * Max Length: 40.
             * Default Value: false.
             */
            u_is_implicit_network: GlideElementBoolean;
            
            /**
             * "Network" column element.
             * Max Length: 32.
             *
             * The network of the target resource.
             */
            u_network: $$element.cmdb_ci_physical_network;
            
            /**
             * "Other Properties" column element.
             * Max Length: 40.
             *
             * Allows other arbitrary named values to be associated with this access definition.
             */
            u_other_properties: GlideElementSimpleNameValue;
            
            /**
             * "Target Resource" column element.
             * Mandatory: true; Max Length: 32.
             *
             * The configuration item, typically a network resource, that the access definition applies to.
             */
            u_target_resource: $$GlideElement.cmdb_ci;
            
            /**
             * "URI" column element.
             * Max Length: 1024.
             *
             * The optional path or resource identifier, specific to the Target Resource, that the access privileges are applicable to.
             */
            u_uri: GlideElement;
        }
        
        /**
         * "User Access Group" GlideRecord fields.
         * @see {@link $$record.cmdb_ci_user_access_group}
         * @see {@link $$element.cmdb_ci_user_access_group}
         */
        export interface cmdb_ci_user_access_group extends cmdb_ci_user_acl_definition {
            /**
             * "Effective Date" column element.
             * Type: "Date" (glide_date); Scalar type: date.
             * Mandatory: true; Max Length: 40.
             *
             * The requested creation date for the User Access Group.
             */
            u_effective_date: GlideElementGlideObject;
            
            /**
             * "Effective Intended Role Type" column element.
             * Type: "Choice" (choice).
             * Read-only: true; Max Length: 40.
             */
            u_effective_intended_role_type: GlideElement;
            
            /**
             * "Expiration Date" column element.
             * Type: "Date" (glide_date); Scalar type: date.
             * Max Length: 40.
             *
             * The optional date when the User Access Group should be disabled or deactivated.
             */
            u_expiration_date: GlideElementGlideObject;
            
            /**
             * "Expiration Overridden By" column element.
             * Max Length: 32.
             *
             * If not empty, this is the person who last overrode the original expiration date.
             */
            u_expiration_overridden_by: $$GlideElement.sys_user;
            
            /**
             * "For Role Type" column element.
             * Type: "Choice" (choice).
             * Mandatory: true; Max Length: 40.
             *
             * The type of role that the User Access Group is intended for.
             */
            u_for_role_type: GlideElement;
            
            /**
             * "Group Name" column element.
             * Mandatory: true; Max Length: 40.
             *
             * The name of the User Access Group.
             */
            u_group_name: GlideElement;
            
            /**
             * "Target Resource" column element.
             * Mandatory: true; Max Length: 32.
             *
             * The configuration item on which the group is defined. This is typically a network resource, such as Active Directory, SharePoint and ServiceNow.
             */
            u_target_resource: $$GlideElement.cmdb_ci;
        }
        
        /**
         * "User ACL Definition" GlideRecord fields.
         * @see {@link $$record.cmdb_ci_user_acl_definition}
         * @see {@link $$element.cmdb_ci_user_acl_definition}
         */
        export interface cmdb_ci_user_acl_definition extends $$tableFields.cmdb_ci {
            /**
             * "Is Implicit Network" column element.
             * Max Length: 40.
             * Default Value: false.
             */
            u_is_implicit_network: GlideElementBoolean;
            
            /**
             * "Network" column element.
             * Max Length: 32.
             */
            u_network: $$element.cmdb_ci_physical_network;
            
            /**
             * "Other Properties" column element.
             * Max Length: 40.
             */
            u_other_properties: GlideElementSimpleNameValue;
        }
    }
}
