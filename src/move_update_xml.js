var gr = new GlideRecord('sys_update_xml');
gr.addQuery('update_set', 'source_sys_id');
//gr.addQuery('type', 'Test Step');
var sys_ids = [];
gr.query();
while(gr.next())
  sys_ids.push('' + gr.sys_id);
for (var i = 0; i < 26; i++) {
  gr = new GlideRecord('sys_update_xml');
  gr.get(sys_ids[i]);
  gr.setValue('update_set', 'target_sys_id');
  gr.update();
}
gs.info('Moved ' + sys_ids.length + ' update sets.');
