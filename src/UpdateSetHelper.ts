// #region Definitions for the UpdateSetHelper API

/**
 * Defines the public instance members of a {@link UpdateSetHelper} type.
 */
export interface UpdateSetHelper
    extends Readonly<{
        add(glideObj: GlideRecord | $$rhino.Nilable<GlideElementReference>): boolean;
        addNotIncluded(glideObj: GlideRecord | $$rhino.Nilable<GlideElementReference>): boolean;
        addLabelEntries(glideRecord: GlideRecord, label: $$GlideRecord.label | $$GlideElement.label | string): void;
        addNotificationDeviceRelated(glideRecord: $$GlideRecord.cmn_notif_device): void;
        addCatalogItemRelated(glideRecord: $$GlideRecord.sc_cat_item): void;
        addSysUserRelated(glideRecord: $$GlideRecord.sys_user): void;
        addSysUserGroupRelated(glideRecord: $$GlideRecord.sys_user_group): void;
    }> {
    // Declare public read/write instance members here.
}

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
            
            add: function(glideObj: GlideRecord | $$rhino.Nilable<GlideElementReference>): boolean {
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

            addNotIncluded: function(glideObj: GlideRecord | $$rhino.Nilable<GlideElementReference>): boolean {
                if (gs.nil(glideObj)) return false;
                var glideRecord = (glideObj instanceof GlideRecord) ? glideObj : <GlideRecord>(<GlideElementReference>glideObj).getRefRecord();
                if (gs.nil(glideRecord)) return false;
                var key = this._manager.getDefaultUpdateName(glideRecord.getTableName(), glideRecord.getUniqueValue());
                if (typeof this._included[key] !== 'undefined' || typeof this._notIncluded[key] != 'undefined') return false;
                this._notIncluded[key] = { order: this._sequenceIndex++, display: getDisplayLink(glideRecord) };
                return true;
            },
            
            addLabelEntries: function(glideRecord: GlideRecord, label: $$GlideRecord.label | $$GlideElement.label | string): void {
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

            addCatalogItemRelated: function(glideRecord: $$GlideRecord.sc_cat_item): void {
                this.addNotIncluded((<$$GlideRecord.std_change_record_producer>glideRecord).current_version);
                this.addNotIncluded(glideRecord.custom_cart);
                this.addNotIncluded(glideRecord.delivery_plan);
                this.addNotIncluded(glideRecord.flow_designer_flow);
                this.addNotIncluded(glideRecord.group);
                this.addNotIncluded(glideRecord.location);
                this.addNotIncluded(glideRecord.model);
                this.addNotIncluded(glideRecord.ordered_item_link);
                this.addNotIncluded(glideRecord.owner);
                this.addNotIncluded(glideRecord.published_ref);
                this.addNotIncluded(glideRecord.sc_ic_item_staging);
                this.addNotIncluded(glideRecord.sc_template);
                this.addNotIncluded(glideRecord.taxonomy_topic);
                this.addNotIncluded(glideRecord.workflow);
            },
            
            addNotificationDeviceRelated: function(glideRecord: $$GlideRecord.cmn_notif_device): void {
                this.addNotIncluded(glideRecord.group);
                this.addNotIncluded(glideRecord.push_app);
                this.addNotIncluded(glideRecord.service_provider);
                this.addNotIncluded(glideRecord.user);
                var gr = new GlideRecord('cmn_notif_device_variable');
                gr.addQuery('device', glideRecord.sys_id);
                gr.query();
                while (gr.next())
                    this.add(gr);
            },
            
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