var gr = new GlideRecord('sys_package');
gr.query();
var totalCount = gr.getRowCount();
var nilCount = 0;
while (gr.next()) {
    if (gs.nil(gr.active))
        nilCount++;
}
gs.info(nilCount + ' of ' + totalCount + ' are nil');