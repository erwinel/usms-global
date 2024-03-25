// #region Definitions for the UpdateSetHelper API

/**
 * Defines the public instance members of a {@link UpdateSetHelper} type.
 */
export interface UpdateSetHelper
    extends Readonly<{
        add(glideObj: GlideRecord | $$rhino.Nilable<GlideElementReference>): boolean;
        addNotIncluded(glideObj: GlideRecord | $$rhino.Nilable<GlideElementReference>): boolean;
        addNotIncludedList(glideElement: $$rhino.Nilable<GlideElement>, tableName: string): boolean;
        addLabelEntries(glideRecord: GlideRecord, label: $$GlideRecord.label | $$GlideElement.label | string): void;
        addNotificationDeviceRelated(glideRecord: $$GlideRecord.cmn_notif_device): void;
        addQuestionChoiceRelated(glideRecord: $$GlideRecord.question_choice): void;
        addQuestionRelated(glideRecord: $$GlideRecord.question): void;
        addDataLookupDefinitionsRelated(glideRecord: $$GlideRecord.dl_definition): void;
        addClientScriptRelated(glideRecord: $$GlideRecord.sys_script_client): void;
        addUIPolicyActionRelated(glideRecord: $$GlideRecord.sys_ui_policy_action): void;
        addUIPolicyRelated(glideRecord: $$GlideRecord.sys_ui_policy): void;
        addVariableSetRelated(glideRecord: $$GlideRecord.item_option_new_set): void;
        addCatalogVariableSetRelated(glideRecord: $$GlideRecord.io_set_item): void;
        addCatalogItemRelated(glideRecord: $$GlideRecord.sc_cat_item): void;
        addConnectedContentRelated(glideRecord: $$GlideRecord.m2m_connected_content): void;
        addVendorCatalogItemRelated(glideRecord: $$GlideRecord.pc_vendor_cat_item): void;
        addRuleRelated(glideRecord: $$GlideRecord.sc_cat_item_guide_items): void;
        addApprovalDefinitionRelated(glideRecord: $$GlideRecord.sc_ic_aprvl_defn): void;
        addTaskDefnRelated(glideRecord: $$GlideRecord.sc_ic_task_defn): void;
        addStandardChangeTemplateVersionRelated(glideRecord: $$GlideRecord.std_change_producer_version): void;
        addSysUserRelated(glideRecord: $$GlideRecord.sys_user): void;
        addSysUserGroupRelated(glideRecord: $$GlideRecord.sys_user_group): void;
    }> { }

interface IInclusionItem {
    order: number;
    display: string;
}

interface IInclusionHash { [key: string]: IInclusionItem; }

/**
 * Defines the prototype for a {@link UpdateSetHelper} constructor.
 */
interface IUpdateSetHelperPrototype extends UpdateSetHelper {
    _current: GlideRecord;
    _id: string;
    _manager: GlideUpdateManager2;
    _sequenceIndex: number;
    _included: IInclusionHash;
    _notIncluded: IInclusionHash;
}

/**
 * Defines the {@link UpdateSetHelper} constructor.
 */
export interface UpdateSetHelperConstructor extends $$class.Constructor1<GlideRecord, UpdateSetHelper, IUpdateSetHelperPrototype> {
    /**
     * Creates a new instance of the implementing class.
     * @param {GlideRecord} glideRecord - The initial GlideRecord to be added to the curent update set.
     */
    new(glideRecord: GlideRecord): UpdateSetHelper;
}

/**
 * Defines the'this' object for {@link UpdateSetHelper} instance methods.
 */
type UpdateSetHelperThisObj = IUpdateSetHelperPrototype & $$class.IPrototype1<GlideRecord>;

/**
 * The {@link UpdateSetHelper} constructor object.
 */
export const UpdateSetHelper: UpdateSetHelperConstructor =
    (function (): UpdateSetHelperConstructor {
        const UpdateSetHelper: UpdateSetHelperConstructor = Class.create<
            GlideRecord,
            UpdateSetHelper,
            UpdateSetHelperConstructor,
            IUpdateSetHelperPrototype
        >();

        function getDisplayLink(glideRecord: GlideRecord): string {
            var cd = ("" + glideRecord.getClassDisplayValue()).trim();
            if (cd.length == 0) cd = glideRecord.getTableName();
            var display = ("" + glideRecord.getDisplayValue()).trim();
            if (display.length == 0)
                return cd + " <a href=\"" + glideRecord.getLink(true) + "\" target=\"_blank\"><em>" + glideRecord.getUniqueValue() + "</em></a>";
            
            var e = JSON.stringify(display);
            if (e.length > display.length + 2)
                return cd + " \"<a href=\"" + glideRecord.getLink(true) + "\" target=\"_blank\">" + e.substring(1, e.length - 2) + "\"</a>";
            return cd + " <a href=\"" + glideRecord.getLink(true) + "\" target=\"_blank\">" + display + "</a>";
        }

        UpdateSetHelper.prototype = {
            _sequenceIndex: 1,
            _included: {},
            _notIncluded: {},
            _current: new GlideRecord('task'),
            _manager: new GlideUpdateManager2(),
            _id: "",
            initialize: function (this: UpdateSetHelperThisObj, glideRecord: GlideRecord): void {
                this._current = glideRecord;
                this._manager = new GlideUpdateManager2();
                this._id = glideRecord.getUniqueValue();
                var cd = ("" + glideRecord.getClassDisplayValue()).trim();
                var tn = glideRecord.getTableName();
                if (cd.length == 0)
                    cd = tn;
                var display = ("" + glideRecord.getDisplayValue()).trim();
                if (display.length == 0)
                    display = this._id;
                else
                {
                    var e = JSON.stringify(display);
                    if (e.length > display.length + 2)
                        display = e;
                }
                this._included[this._manager.getDefaultUpdateName(tn, this._id)] = { order: 0, display: JSUtil.escapeText(cd + ' ' + display) };
            },
            
            add: function(glideObj: GlideRecord | GlideElementReference): boolean {
                if (gs.nil(glideObj)) return false;
                var glideRecord = (glideObj instanceof GlideRecord) ? glideObj : <GlideRecord>(<GlideElementReference>glideObj).getRefRecord();
                if (gs.nil(glideRecord)) return false;
                var key = this._manager.getDefaultUpdateName(glideRecord.getTableName(), glideRecord.getUniqueValue());
                if (typeof this._included[key] !== 'undefined') return false;
                var item = this._notIncluded[key];
                if (typeof item === 'undefined')
                    this._included[key] = { order: this._sequenceIndex++, display: getDisplayLink(glideRecord) };
                else {
                    this._included[key] = item;
                    delete this._notIncluded[key];
                }
                return true;
            },

            addNotIncluded: function(glideObj: GlideRecord | GlideElementReference): boolean {
                if (gs.nil(glideObj)) return false;
                var glideRecord = (glideObj instanceof GlideRecord) ? glideObj : <GlideRecord>(<GlideElementReference>glideObj).getRefRecord();
                if (gs.nil(glideRecord)) return false;
                var key = this._manager.getDefaultUpdateName(glideRecord.getTableName(), glideRecord.getUniqueValue());
                if (typeof this._included[key] !== 'undefined' || typeof this._notIncluded[key] != 'undefined') return false;
                this._notIncluded[key] = { order: this._sequenceIndex++, display: getDisplayLink(glideRecord) };
                return true;
            },

            addNotIncludedList: function(glideElement: GlideElement, tableName: string): boolean {
                if (gs.nil(glideElement)) return false;
                var idArr = glideElement.toString().split(',').map(function(s: string): string { return s.trim() }).filter(function(s: string): boolean { return s.length > 0; });
                var result = false;
                for (var i = 0; i < idArr.length; i++) {
                    var gr = new GlideRecord(tableName);
                    if (gr.get(idArr[i]) && this.addNotIncluded(gr))
                        result = true;
                }
                return result;
            },
            
            addLabelEntries: function(glideRecord: GlideRecord, label: $$GlideRecord.label | $$GlideElement.label | string): void {
                if (gs.nil(glideRecord)) return;
                var gr = <$$GlideRecord.label_entry>new GlideRecord('label_entry');
                gr.addQuery('label', label);
                gr.addQuery('table', glideRecord.getTableName());
                gr.addQuery('table_key', glideRecord.getUniqueValue());
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                    {
                        this.addNotIncluded(gr.auto);
                        this.addNotIncluded(gr.notify_script);
                    }
                }
            },

            addNotificationDeviceRelated: function(glideRecord: $$GlideRecord.cmn_notif_device): void {
                if (gs.nil(glideRecord)) return;
                this.addNotIncluded(glideRecord.group);
                this.addNotIncluded(glideRecord.push_app);
                this.addNotIncluded(glideRecord.schedule);
                this.addNotIncluded(glideRecord.service_provider);
                this.addNotIncluded(glideRecord.user);
                var gr = <$$GlideRecord.cmn_notif_device_variable>new GlideRecord('cmn_notif_device_variable');
                gr.addQuery('device', glideRecord.sys_id);
                gr.query();
                while (gr.next())
                    this.add(gr);
            },

            addQuestionChoiceRelated: function(glideRecord: $$GlideRecord.question_choice): void {
                if (gs.nil(glideRecord)) return;
                this.addNotIncluded(glideRecord.published_ref);
                this.addNotIncluded(glideRecord.question);
            },

            addQuestionRelated: function(glideRecord: $$GlideRecord.question): void {
                if (gs.nil(glideRecord)) return;
                this.addNotIncluded(glideRecord.dynamic_default_value);
                this.addNotIncluded(glideRecord.dynamic_ref_qual);
                this.addNotIncluded(glideRecord.macro);
                this.addNotIncluded(glideRecord.summary_macro);
                this.addNotIncluded(glideRecord.ui_page);
                this.addNotIncluded((<$$GlideRecord.expert_variable>glideRecord).expert);
                this.addNotIncluded((<$$GlideRecord.item_option_layout>glideRecord).variable);
                this.addNotIncluded((<$$GlideRecord.item_option_new>glideRecord).cat_item);
                this.addNotIncluded((<$$GlideRecord.item_option_new>glideRecord).category);
                this.addNotIncluded((<$$GlideRecord.item_option_new>glideRecord).delivery_plan);
                this.addNotIncluded((<$$GlideRecord.item_option_new>glideRecord).dynamic_value_field);
                this.addNotIncluded((<$$GlideRecord.item_option_new>glideRecord).macroponent);
                this.addNotIncluded((<$$GlideRecord.item_option_new>glideRecord).published_ref);
                this.addNotIncluded((<$$GlideRecord.item_option_new>glideRecord).sp_widget);
                this.addNotIncluded((<$$GlideRecord.item_option_new>glideRecord).validate_regex);
                this.addNotIncluded((<$$GlideRecord.item_option_new>glideRecord).variable_set);
                this.addNotIncluded((<$$GlideRecord.wf_variable>glideRecord).workflow);
                var gr = new GlideRecord('question_choice');
                var sys_id = glideRecord.sys_id;
                gr.addQuery('question', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addQuestionChoiceRelated(<$$GlideRecord.question_choice>gr);
                }
            },

            addUIPolicyActionRelated: function(glideRecord: $$GlideRecord.sys_ui_policy_action): void {
                if (gs.nil(glideRecord)) return;
                this.addNotIncluded(glideRecord.ui_policy);
                this.addNotIncluded((<$$GlideRecord.catalog_ui_policy_action>glideRecord).catalog_item);
                this.addNotIncluded((<$$GlideRecord.catalog_ui_policy_action>glideRecord).variable_set);
                this.addNotIncluded((<$$GlideRecord.expert_ui_policy_action>glideRecord).expert);
                this.addNotIncluded((<$$GlideRecord.wf_ui_policy_action>glideRecord).activity_definition);
            },
 
            addUIPolicyRelated: function(glideRecord: $$GlideRecord.sys_ui_policy): void {
                if (gs.nil(glideRecord)) return;
                this.addNotIncluded(glideRecord.sys_overrides);
                this.addNotIncluded(glideRecord.view);
                this.addNotIncluded((<$$GlideRecord.catalog_ui_policy>glideRecord).catalog_item);
                this.addNotIncluded((<$$GlideRecord.catalog_ui_policy>glideRecord).variable_set);
                this.addNotIncluded((<$$GlideRecord.expert_ui_policy>glideRecord).expert);
                this.addNotIncluded((<$$GlideRecord.wf_ui_policy>glideRecord).activity_definition);
                var sys_id = glideRecord.sys_id;
                var gr = new GlideRecord('sys_ui_policy_rl_action');
                gr.addQuery('ui_policy', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sys_ui_policy_rl_action>gr).ui_policy);
                }
                gr = new GlideRecord('sys_ui_policy_action');
                gr.addQuery('ui_policy', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addUIPolicyActionRelated(<$$GlideRecord.sys_ui_policy_action>gr);
                }
                gr = new GlideRecord('catalog_ui_policy_action');
                gr.addQuery('ui_policy', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addUIPolicyActionRelated(<$$GlideRecord.catalog_ui_policy_action>gr);
                }
                gr = new GlideRecord('expert_ui_policy_action');
                gr.addQuery('ui_policy', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addUIPolicyActionRelated(<$$GlideRecord.expert_ui_policy_action>gr);
                }
                gr = new GlideRecord('wf_ui_policy_action');
                gr.addQuery('ui_policy', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addUIPolicyActionRelated(<$$GlideRecord.expert_ui_policy_action>gr);
                }
            },
            
            addDataLookupDefinitionsRelated: function(glideRecord: $$GlideRecord.dl_definition): void {
                if (gs.nil(glideRecord)) return;
                this.addNotIncluded((<$$GlideRecord.catalog_dl_definition>glideRecord).catalog_item);
                this.addNotIncluded((<$$GlideRecord.catalog_dl_definition>glideRecord).variable_set);
                var sys_id = glideRecord.sys_id;
                var gr = new GlideRecord('dl_definition_rel_match');
                gr.addQuery('dl_definition', sys_id);
                gr.query();
                while (gr.next()) {
                    // if (this.add(gr))
                    //     this.addRelated(<$$GlideRecord.dl_definition_rel_match>gr);
                }
                gr = new GlideRecord('dl_definition_rel_set');
                gr.addQuery('variable_set', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addDataLookupDefinitionsRelated(<$$GlideRecord.dl_definition>gr);
                }
                gr = new GlideRecord('catalog_dl_definition');
                gr.addQuery('catalog_item', sys_id);
                gr.query();
                while (gr.next()) {
                    // if (this.add(gr))
                    //     this.addRelated(<$$GlideRecord.dl_definition_rel_match>gr);
                }
                gr = new GlideRecord('catalog_dl_definition');
                gr.addQuery('variable_set', sys_id);
                gr.query();
                while (gr.next()) {
                    // if (this.add(gr))
                    //     this.addRelated(<$$GlideRecord.dl_definition_rel_match>gr);
                }
                gr = new GlideRecord('catalog_dl_definition_rel_mat');
                gr.addQuery('dl_definition', sys_id);
                gr.query();
                while (gr.next()) {
                    // if (this.add(gr))
                    //     this.addRelated(<$$GlideRecord.dl_definition_rel_match>gr);
                }
                gr = new GlideRecord('catalog_dl_definition_rel_set');
                gr.addQuery('dl_definition', sys_id);
                gr.query();
                while (gr.next()) {
                    // if (this.add(gr))
                    //     this.addRelated(<$$GlideRecord.dl_definition_rel_match>gr);
                }
            },

            addClientScriptRelated: function(glideRecord: $$GlideRecord.sys_script_client): void {
                if (gs.nil(glideRecord)) return;
                this.addNotIncluded(glideRecord.sys_overrides);
                this.addNotIncluded((<$$GlideRecord.expert_script_client>glideRecord).expert);
                this.addNotIncluded((<$$GlideRecord.expert_script_client>glideRecord).expert_panel);
                this.addNotIncluded((<$$GlideRecord.catalog_script_client>glideRecord).cat_item);
                this.addNotIncluded((<$$GlideRecord.catalog_script_client>glideRecord).published_ref);
                this.addNotIncluded((<$$GlideRecord.catalog_script_client>glideRecord).variable_set);
            },
            
            addVariableSetRelated: function(glideRecord: $$GlideRecord.item_option_new_set): void {
                if (gs.nil(glideRecord)) return;
                var sys_id = glideRecord.sys_id;
                var gr = new GlideRecord('item_option_new');
                gr.addQuery('variable_set', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addQuestionRelated(<$$GlideRecord.item_option_new>gr);
                }
                gr = new GlideRecord('catalog_dl_definition');
                gr.addQuery('variable_set', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addDataLookupDefinitionsRelated(<$$GlideRecord.catalog_dl_definition>gr);
                }
                gr = new GlideRecord('catalog_ui_policy');
                gr.addQuery('variable_set', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addUIPolicyRelated(<$$GlideRecord.catalog_ui_policy>gr);
                }
                gr = new GlideRecord('catalog_script_client');
                gr.addQuery('variable_set', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addClientScriptRelated(<$$GlideRecord.catalog_script_client>gr);
                }
                gr = new GlideRecord('io_set_item');
                gr.addQuery('variable_set', sys_id);
                gr.query();
                while (gr.next()) {
                    // if (this.add(gr))
                    //     this.addCatalogVariableSetRelated(<$$GlideRecord.io_set_item>gr);
                }
            },

            addCatalogVariableSetRelated(glideRecord: $$GlideRecord.io_set_item): void {
                this.addNotIncluded(glideRecord.sc_cat_item);
                this.addNotIncluded(glideRecord.variable_set);
            },

            addConnectedContentRelated(glideRecord: $$GlideRecord.m2m_connected_content): void {
                // TODO: Verify Reference fields: m2m_connected_content
                // TODO: Verify extended tables: https://dev203287.service-now.com/sys_db_object_list.do?sysparm_query=super_class.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=list_id.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listSTARTSWITHm2m_connected_content.
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listENDSWITH.m2m_connected_content
            },
            
            addVendorCatalogItemRelated(glideRecord: $$GlideRecord.pc_vendor_cat_item): void {
                // TODO: Verify Reference fields: m2m_connected_content
                // TODO: Verify extended tables: https://dev203287.service-now.com/sys_db_object_list.do?sysparm_query=super_class.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=list_id.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listSTARTSWITHm2m_connected_content.
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listENDSWITH.m2m_connected_content
            },
            
            addRuleRelated(glideRecord: $$GlideRecord.sc_cat_item_guide_items): void {
                // TODO: Verify Reference fields: m2m_connected_content
                // TODO: Verify extended tables: https://dev203287.service-now.com/sys_db_object_list.do?sysparm_query=super_class.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=list_id.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listSTARTSWITHm2m_connected_content.
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listENDSWITH.m2m_connected_content
            },
            
            addApprovalDefinitionRelated(glideRecord: $$GlideRecord.sc_ic_aprvl_defn): void {
                // TODO: Verify Reference fields: m2m_connected_content
                // TODO: Add related records: sc_ic_aprvl_defn.sc_cat_item
                // TODO: Verify extended tables: https://dev203287.service-now.com/sys_db_object_list.do?sysparm_query=super_class.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=list_id.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listSTARTSWITHm2m_connected_content.
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listENDSWITH.m2m_connected_content
            },
            
            addTaskDefnRelated(glideRecord: $$GlideRecord.sc_ic_task_defn): void {
                // TODO: Verify Reference fields: m2m_connected_content
                // TODO: Add related records: sc_ic_task_defn.sc_cat_item
                // TODO: Verify extended tables: https://dev203287.service-now.com/sys_db_object_list.do?sysparm_query=super_class.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=list_id.name%3Dm2m_connected_content
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listSTARTSWITHm2m_connected_content.
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listENDSWITH.m2m_connected_content
            },
            
            addStandardChangeTemplateVersionRelated(glideRecord: $$GlideRecord.std_change_producer_version): void { },
            
            // TODO: Add related record: sn_ex_sp_ext_link_user_criteria_no_mtom.sn_ex_sp_external_link => sn_ex_sp_external_link
            // TODO: Add related record: sn_ex_sp_ext_link_user_criteria_mtom.sn_ex_sp_external_link => sn_ex_sp_external_link
            // TODO: Add related records: sn_ex_sp_quick_link_user_criteria_mtom.sn_ex_sp_quick_link => sn_ex_sp_quick_link
            // TODO: Add related records: sc_ic_aprvl_defn_staging.sc_ic_item_staging
            // TODO: Add related records: sc_ic_task_defn_staging.sc_ic_item_staging
            // TODO: Add related records: sc_ic_task_defn_staging.sc_ic_item_staging
            addCatalogItemRelated: function(glideRecord: $$GlideRecord.sc_cat_item): void {
                if (gs.nil(glideRecord)) return;

                this.addNotIncluded(glideRecord.category);
                this.addNotIncluded(glideRecord.custom_cart);
                this.addNotIncluded(glideRecord.delivery_plan);
                this.addNotIncluded(glideRecord.flow_designer_flow);
                this.addNotIncluded(glideRecord.group);
                this.addNotIncluded(glideRecord.location);
                this.addNotIncluded(glideRecord.model);
                this.addNotIncluded(glideRecord.ordered_item_link);
                this.addNotIncluded(glideRecord.owner);
                this.addNotIncluded(glideRecord.published_ref);
                this.addNotIncludedList(glideRecord.sc_catalogs, 'sc_catalog');
                this.addNotIncluded(glideRecord.sc_ic_item_staging);
                this.addNotIncluded(glideRecord.sc_template);
                this.addNotIncluded(glideRecord.taxonomy_topic);
                this.addNotIncluded(glideRecord.template);
                this.addNotIncluded(glideRecord.vendor);
                this.addNotIncluded(glideRecord.workflow);
                this.addNotIncluded((<$$GlideRecord.pc_product_cat_item>glideRecord).vendor_catalog_item);
                this.addNotIncluded((<$$GlideRecord.sc_cat_item_content>glideRecord).kb_article);
                this.addNotIncluded((<$$GlideRecord.sc_cat_item_content>glideRecord).module);
                this.addNotIncluded((<$$GlideRecord.sc_cat_item_producer_service>glideRecord).fulfillment_user);
                this.addNotIncluded((<$$GlideRecord.sc_cat_item_service>glideRecord).cmdb_ci_service);
                this.addNotIncluded((<$$GlideRecord.sc_cat_item_wizard>glideRecord).expert);
                this.addNotIncluded((<$$GlideRecord.std_change_record_producer>glideRecord).current_version);

                var sys_id = glideRecord.sys_id;
                var gr = new GlideRecord('item_option_new');
                gr.addQuery('cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addQuestionRelated(<$$GlideRecord.item_option_new>gr);
                }
                gr = new GlideRecord('item_option_new');
                gr.addQuery('variable_set', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addQuestionRelated(<$$GlideRecord.item_option_new>gr);
                }
                gr = new GlideRecord('catalog_dl_definition');
                gr.addQuery('catalog_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addDataLookupDefinitionsRelated(<$$GlideRecord.catalog_dl_definition>gr);
                }
                gr = new GlideRecord('catalog_script_client');
                gr.addQuery('cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addClientScriptRelated(<$$GlideRecord.catalog_script_client>gr);
                }
                gr = new GlideRecord('catalog_ui_policy');
                gr.addQuery('catalog_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addUIPolicyRelated(<$$GlideRecord.catalog_ui_policy>gr);
                }
                gr = new GlideRecord('io_set_item');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addCatalogVariableSetRelated(<$$GlideRecord.io_set_item>gr);
                }
                gr = new GlideRecord('m2m_connected_content');
                gr.addQuery('catalog_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addConnectedContentRelated(<$$GlideRecord.m2m_connected_content>gr);
                }
                gr = new GlideRecord('pc_vendor_cat_item');
                gr.addQuery('product_catalog_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addVendorCatalogItemRelated(<$$GlideRecord.pc_vendor_cat_item>gr);
                }
                gr = new GlideRecord('sc_2_kb');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_2_kb>gr).kb_knowledge);
                }
                gr = new GlideRecord('sc_cat_item_catalog');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_catalog>gr).sc_catalog);
                }
                gr = new GlideRecord('sc_cat_item_category');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_category>gr).sc_category);
                }
                gr = new GlideRecord('sc_cat_item_children');
                gr.addQuery('parent', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr)) {
                        if (!gs.nil((<$$GlideRecord.sc_cat_item_children>gr).child)) {
                            var cr = (<$$GlideRecord.sc_cat_item_children>gr).child.getRefRecord();
                            if (this.add(cr))
                                this.addCatalogItemRelated(cr);
                        }
                    }
                }
                gr = new GlideRecord('sc_cat_item_company_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_company_mtom>gr).sc_avail_company);
                }
                gr = new GlideRecord('sc_cat_item_company_no_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_company_no_mtom>gr).sc_avail_company);
                }
                gr = new GlideRecord('sc_cat_item_dept_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_dept_mtom>gr).sc_avail_dept);
                }
                gr = new GlideRecord('sc_cat_item_dept_no_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_dept_no_mtom>gr).sc_avail_dept);
                }
                gr = new GlideRecord('sc_cat_item_group_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_group_mtom>gr).sc_avail_group);
                }
                gr = new GlideRecord('sc_cat_item_group_no_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_group_no_mtom>gr).sc_avail_group);
                }
                gr = new GlideRecord('sc_cat_item_guide_items');
                gr.addQuery('guide', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addRuleRelated(<$$GlideRecord.sc_cat_item_guide_items>gr);
                }
                gr = new GlideRecord('sc_cat_item_location_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_location_mtom>gr).sc_avail_location);
                }
                gr = new GlideRecord('sc_cat_item_location_no_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_location_no_mtom>gr).sc_avail_location);
                }
                gr = new GlideRecord('sc_cat_item_user_criteria_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_user_criteria_mtom>gr).user_criteria);
                }
                gr = new GlideRecord('sc_cat_item_user_criteria_no_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_user_criteria_no_mtom>gr).user_criteria);
                }
                gr = new GlideRecord('sc_cat_item_user_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_user_mtom>gr).sc_avail_user);
                }
                gr = new GlideRecord('sc_cat_item_user_no_mtom');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sc_cat_item_user_no_mtom>gr).sc_avail_user);
                }
                gr = new GlideRecord('sc_ic_aprvl_defn');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addApprovalDefinitionRelated(<$$GlideRecord.sc_ic_aprvl_defn>gr);
                }
                gr = new GlideRecord('sc_ic_task_defn');
                gr.addQuery('sc_cat_item', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addTaskDefnRelated(<$$GlideRecord.sc_ic_task_defn>gr);
                }
                gr = new GlideRecord('std_change_producer_version');
                gr.addQuery('std_change_producer', sys_id);
                gr.query();
                while (gr.next()) {
                    if (this.add(gr))
                        this.addStandardChangeTemplateVersionRelated(<$$GlideRecord.std_change_producer_version>gr);
                }
            },

                // TODO: Verify Reference fields: 
                // TODO: Verify extended tables: https://dev203287.service-now.com/sys_db_object_list.do?sysparm_query=super_class.name%3D
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=list_id.name%3D
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listSTARTSWITH.
                // TODO: Verify related lists: https://dev203287.service-now.com/sys_ui_related_list_entry_list.do?sysparm_query=related_listENDSWITH.

            addSysUserRelated: function(glideRecord: $$GlideRecord.sys_user): void {
                if (gs.nil(glideRecord)) return;
                this.addNotIncluded(glideRecord.building);
                this.addNotIncluded(glideRecord.company);
                this.addNotIncluded(glideRecord.cost_center);
                this.addNotIncluded(glideRecord.default_perspective);
                this.addNotIncluded(glideRecord.department);
                this.addNotIncluded(glideRecord.location);
                this.addNotIncluded(glideRecord.manager);
                this.addNotIncluded(glideRecord.schedule);
                var sys_id = glideRecord.sys_id;
                var gr = new GlideRecord('sys_user_has_role');
                gr.addQuery('user', sys_id);
                gr.query();
                while (gr.next())
                    this.add(gr);
                gr = new GlideRecord('sys_user_grmember');
                gr.addQuery('user', sys_id);
                gr.query();
                while (gr.next())
                {
                    if (this.add(gr))
                        this.addNotIncluded((<$$GlideRecord.sys_user_grmember>gr).group);
                }
                gr = new GlideRecord('cmn_notif_device');
                gr.addQuery('user', sys_id);
                gr.query();
                while (gr.next) {
                    if (this.add(gr))
                        this.addNotificationDeviceRelated(<$$GlideRecord.cmn_notif_device>gr);
                }
            },
            
            addSysUserGroupRelated: function(glideRecord: $$GlideRecord.sys_user_group ): void {
                if (gs.nil(glideRecord)) return;
                var gr: $$GlideRecord.sys_user_group = <$$GlideRecord.sys_user_group>new GlideRecord('sys_user_group');
                var sys_id = glideRecord.sys_id;
                this.addNotIncluded(glideRecord.cost_center);
                this.addNotIncluded(glideRecord.default_assignee);
                this.addNotIncluded(glideRecord.manager);
                this.addNotIncluded(glideRecord.parent);
                // TODO: Add not included for items in glideRecord.type
            },
            
            type: "UpdateSetHelper",
        };

        return UpdateSetHelper;
    })();

// #endregion