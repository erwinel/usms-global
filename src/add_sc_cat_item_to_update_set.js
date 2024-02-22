var current = new GlideRecord('sc_cat_item');
(function() {
    current.update();
    var um = new GlideUpdateManager2();
    var includedUniqueIds = [];
    var notIncludedUniqueIds = [];
    var notIncludedLinks = {};
    function addNotIncluded(el) {
        if (gs.nil(el)) return;
        var gr = (el instanceof GlideRecord) ? el : el.getRefRecord();
        if (gs.nil(gr)) return;
        var id = gr.getUniqueValue();
        if (notIncludedUniqueIds.indexOf(id) >= 0 || includedUniqueIds.indexOf(id) >= 0) return;
        notIncludedUniqueIds.push(id);
        var label = el.getLabel();
        var display = gr.getDisplayValue();
        if (gs.nil(label)) {
            label = gr.getRecordClassName();
            if (gs.nil(label)) label = gr.getTableName();
            if (gs.nil(display)) display = id;
        }
        else if (gs.nil(display))
        {
            display = gr.getRecordClassName();
            display = (gs.nil(display) ? gr.getTableName() : display) + ' [' + id + ']';
        }
        notIncludedLinks[id] = label.replace('<', '&lt;').replace('>', '&gt;') + ': <a href="' + gr.getLink() + '">' + display.replace('<', '&lt;').replace('>', '&gt;') + '</a>';
    }
    function addRecordToUpdateSet(gr) {
        if (gs.nil(gr)) return false;
        var id = gr.getUniqueValue();
        if (includedUniqueIds.indexOf(id) >= 0) return false;
        var index = notIncludedUniqueIds.indexOf(id);
        if (index >= 0) {
            if (index == 0)
            notIncludedUniqueIds.shift();
            else if (index < notIncludedUniqueIds.length - 1)
                notIncludedUniqueIds.splice(index, 1);
            else
                notIncludedUniqueIds.pop();
        }
        um.saveRecord(gr);
        includedUniqueIds.push(id);
        return true;
    }
    function addRefToUpdateSet(el) {
        if (gs.nil(el)) return false;
        var gr = el.getRefRecord();
        if (gs.nil(gr)) return false;
        var id = gr.getUniqueValue();
        if (includedUniqueIds.indexOf(id) >= 0) return false;
        var index = notIncludedUniqueIds.indexOf(id);
        if (index >= 0) {
            if (index == 0)
            notIncludedUniqueIds.shift();
            else if (index < notIncludedUniqueIds.length - 1)
                notIncludedUniqueIds.splice(index, 1);
            else
                notIncludedUniqueIds.pop();
        }
        um.saveRecord(gr);
        includedUniqueIds.push(id);
        return true;
    }
    function addQuestion(question) {
        addNotIncluded(question.dynamic_default_value);
        addNotIncluded(question.dynamic_ref_qual);
        addNotIncluded(question.macro);
        addNotIncluded(question.summary_macro);
        addNotIncluded(question.ui_page);
        var id = question.sys_id;
        var gr = new GlideRecord('question_choice');
        gr.addQuery('question', id);
        gr.query();
        while (gr.next()) {
            if (addRecordToUpdateSet(gr))
                addNotIncluded(gr.published_ref);
        }
    }
    function addVariableRecord(item_option_new) {
        if (!addRecordToUpdateSet(item_option_new)) return;
        addQuestion(item_option_new);
        addNotIncluded(item_option_new.category);
        addNotIncluded(item_option_new.delivery_plan);
        addNotIncluded(item_option_new.dynamic_value_field);
        addNotIncluded(item_option_new.macroponent);
        addNotIncluded(item_option_new.published_ref);
        addNotIncluded(item_option_new.sp_widget);
        addNotIncluded(item_option_new.validate_regex);
        var id = item_option_new.sys_id;
        var gr = new GlideRecord('item_option_layout');
        gr.addQuery('variable', id);
        gr.query();
        while (gr.next()) addRecordToUpdateSet(gr);
    }
    function addUIPolicy(sys_ui_policy) {
        addNotIncluded(sys_ui_policy.sys_overrides);
        addNotIncluded(sys_ui_policy.view);
        var id = sys_ui_policy.sys_id;
        var gr = new GlideRecord('sys_ui_policy_action');
        gr.addQuery('ui_policy', id);
        gr.query();
        while (gr.next()) addRecordToUpdateSet(gr);
    }
    function addCatalogClientScriptRecord(catalog_script_client) {
        if (!addRecordToUpdateSet(catalog_script_client)) return;
        addNotIncluded(catalog_script_client.published_ref);
        addNotIncluded(catalog_script_client.sys_overrides);
    }
    addRecordToUpdateSet(current)
    addNotIncluded(current.category);
    addRefToUpdateSet(current.custom_cart);
    addRefToUpdateSet(current.ordered_item_link);
    addNotIncluded(current.published_ref);
    addNotIncluded(current.sc_template);
    addNotIncluded(current.template);
    addNotIncluded(current.delivery_plan);
    addNotIncluded(current.flow_designer_flow);
    addNotIncluded(current.group);
    addNotIncluded(current.location);
    addNotIncluded(current.model);
    addNotIncluded(current.owner);
    addNotIncluded(current.sc_ic_item_staging);
    addNotIncluded(current.taxonomy_topic);
    addNotIncluded(current.vendor);
    addNotIncluded(current.workflow);
    var sys_id = current.getValue('sys_id');
    var gr = new GlideRecord('io_set_item');
    gr.addQuery('sc_cat_item', sys_id);
    gr.query();
    while (gr.next()) {
        if (addRecordToUpdateSet(gr)) addNotIncluded(gr.variable_set);
    }
    gr = new GlideRecord('sc_cat_item_catalog');
    gr.addQuery('sc_cat_item', sys_id);
    gr.query();
    while (gr.next()) {
        if (addRecordToUpdateSet(gr)) addNotIncluded(gr.sc_catalog);
    }
    gr = new GlideRecord('sc_cat_item_category');
    gr.addQuery('sc_cat_item', sys_id);
    gr.query();
    while (gr.next()) {
        if (addRecordToUpdateSet(gr)) addNotIncluded(gr.sc_category);
    }
    gr = new GlideRecord('item_option_new');
    gr.addQuery('cat_item', sys_id);
    gr.query();
    while (gr.next()) addVariableRecord(gr);
    gr = new GlideRecord('sc_cat_item_option');
    gr.addQuery('cat_item', sys_id);
    gr.query();
    while (gr.next()) addRecordToUpdateSet(gr);
    gr = new GlideRecord('catalog_ui_policy_action');
    gr.addQuery('catalog_item', sys_id);
    gr.query();
    while (gr.next()) addRecordToUpdateSet(gr);
    gr = new GlideRecord('catalog_ui_policy');
    gr.addQuery('catalog_item', sys_id);
    gr.query();
    while (gr.next()) {
        if (addRecordToUpdateSet(gr)) addUIPolicy(gr);
    }
    gr = new GlideRecord('catalog_script_client');
    gr.addQuery('cat_item', sys_id);
    gr.query();
    while (gr.next()) addCatalogClientScriptRecord(gr);
    
    gr = new GlideRecord('sc_2_kb');
    gr.addQuery('sc_cat_item', sys_id);
    gr.query();
    while (gr.next()) {
        if (addRecordToUpdateSet(gr)) addNotIncluded(gr.kb_knowledge);
    }
    gr = new GlideRecord('sc_2_sc');
    gr.addQuery('sc_cat_item', sys_id);
    gr.query();
    while (gr.next()) {
        if (addRecordToUpdateSet(gr)) addNotIncluded(gr.related_sc_cat_item);
    }
    gr = new GlideRecord('sc_cat_item_user_criteria_mtom');
    gr.addQuery('sc_cat_item', sys_id);
    gr.query();
    while (gr.next()) {
        if (addRecordToUpdateSet(gr)) addNotIncluded(gr.user_criteria);
    }
    gr = new GlideRecord('sc_cat_item_user_criteria_no_mtom');
    gr.addQuery('sc_cat_item', sys_id);
    gr.query();
    while (gr.next()) {
        if (addRecordToUpdateSet(gr)) addNotIncluded(gr.user_criteria);
    }
    gr = new GlideRecord('catalog_dl_definition');
    gr.addQuery('catalog_item', sys_id);
    gr.query();
    while (gr.next()) addNotIncluded(gr);
    if (!current.sc_catalogs.nil()) {
        var idList = ('' + current.sc_catalogs).split(',');
        for (var i = 0; i < idList.length; i++) {
            gr = new GlideRecord('sc_catalog');
            if (gr.get(idList[i])) addNotIncluded(gr);
        }
    }
    gr = new GlideRecord('sys_update_set');
    sys_id = gs.getPreference('sys_update_set');
    gr.get(sys_id);
    switch (notIncludedUniqueIds.length) {
        case 0:
            if (includedUniqueIds.length > 1)
                gs.addInfoMessage('1 Record included in <a href="sys_update_set.do?sys_id=' + sys_id + '">' + gr.name.replace('<', '&lt;').replace('>', '&gt;') + '</a> update set.');
            else
                gs.addInfoMessage(includedUniqueIds.length + ' Records included in <a href="sys_update_set.do?sys_id=' + sys_id + '">' + gr.name.replace('<', '&lt;').replace('>', '&gt;') + '</a> update set.');
            break;
        case 1:
            if (includedUniqueIds.length > 1)
                gs.addInfoMessage('1 Record included in <a href="sys_update_set.do?sys_id=' + sys_id + '">' + gr.name + '</a> update set; 1 record was not included:<ul><li>' + notIncludedLinks[notIncludedUniqueIds[0]] + '</li></ul>');
            else
                gs.addInfoMessage(includedUniqueIds.length + ' Records included in <a href="sys_update_set.do?sys_id=' + sys_id + '">' + gr.name.replace('<', '&lt;').replace('>', '&gt;') + '</a> update set; 1 record was not included:<ul><li>' + notIncludedLinks[notIncludedUniqueIds[0]] + '</li></ul>');
            break;
        default:
            var msg = ((includedUniqueIds.length > 1) ? '1 record' : includedUniqueIds.length + ' records') + ' included in <a href="sys_update_set.do?sys_id=' + sys_id + '">' + gr.name + '</a> update set; ' +
                notIncludedUniqueIds.length + ' records were not included:<ul>';
            for (var i = 0; i < notIncludedUniqueIds.length; i++)
                msg += '<li>' + notIncludedLinks[notIncludedUniqueIds[i]] + '</li>';
            gs.addInfoMessage(msg + '</ul>');
            break;
    }
})();
