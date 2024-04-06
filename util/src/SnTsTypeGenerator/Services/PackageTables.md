# Package Tables

## Package

- Name: `sys_package`
- [Table](https://dev243690.service-now.com/sys_db_object.do?sys_id=81aa4020d9313110cedf7dffddb48f44)
- [List](https://dev243690.service-now.com/sys_package_list.do)

Columns

| Column label             | Column name     | Type          | Reference | May be nil |
|--------------------------|-----------------|---------------|-----------|------------|
| Active                   | active          | True/False    |           | false      |
| Subscription requirement | enforce_license | String        |           | true       |
| Licensable               | licensable      | True/False    |           | false      |
| Name                     | name            | String        |           | false      |
| ID                       | source          | String        |           | true       |
| Sys ID                   | sys_id          | Sys ID (GUID) |           | false      |
| Version                  | version         | Version       |           | true       |

Extending Tables:

- [Application](#application) *(sys_scope)*
- [Sys Plugins](#sys-plugins) *(sys_plugins)*

Referred by:

- [Package dependency](#package-dependency).`Package`: *(sys_package_dependency_m2m.sys_package)*
- [Package dependency](#package-dependency).`Dependency`: *(sys_package_dependency_m2m.dependency)*

## Package dependency

- Name: `sys_package_dependency_m2m`
- [Table](https://dev243690.service-now.com/sys_db_object.do?sys_id=ba8b8428d9313110cedf7dffddb48fac)
- [List](https://dev243690.service-now.com/sys_package_dependency_m2m_list.do)

Columns:

| Column label | Column name | Type          | Reference           | May be nil |
|--------------|-------------|---------------|---------------------|------------|
| Dependency   | dependency  | Reference     | [Package](#package) | false      |
| Min version  | min_version | Version       |                     | true       |
| Sys ID       | sys_id      | Sys ID (GUID) |                     | false      |
| Package      | sys_package | Reference     | [Package](#package) | false      |

- Dependency:  *(sys_package)*
- Package:  *(sys_package)*

## Application

- Name: `sys_scope`
- [Table](https://dev243690.service-now.com/sys_db_object.do?sys_id=8daa4020d9313110cedf7dffddb48f57)
- [List](https://dev243690.service-now.com/sys_scope_list.do)
- Extends: [Package](#package) *(sys_package)*

Columns:

| Column label      | Column name       | Type            | Reference                     | May be nil |
|-------------------|-------------------|-----------------|-------------------------------|------------|
| Private           | private           | True/False      |                               | false      |
| Scope             | scope             | String          |                               | false      |
| Short description | short_description | Translated Text |                               | true       |
| Vendor            | vendor            | String          |                               | true       |
| Vendor prefix     | vendor_prefix     | String          |                               | true       |

Extending Tables:

- [Custom Application](#custom-application) *(sys_app)*
- [Store Application](#store-application) *(sys_store_app)*

## Custom Application

- Name: `sys_app`
- [Table](https://dev243690.service-now.com/sys_db_object.do?sys_id=a16b88a4d9313110cedf7dffddb48f03)
- [List](https://dev243690.service-now.com/sys_app_list.do)
- Extends: [Application](#application) *(sys_scope)*

Columns:

| Column label             | Column name             | Type       | Reference | May be nil |
|--------------------------|-------------------------|------------|-----------|------------|
| Installed via Dependency | installed_as_dependency | True/False |           | false      |
| Store correlation ID     | store_correlation_id    | String     |           | true       |
| Store URL                | store_url               | String     |           | true       |
| Code                     | sys_code                | String     |           | true       |

## Store Application

- Name: `sys_store_app`
- [Table](https://dev243690.service-now.com/sys_db_object.do?sys_id=f60bc8e0d9313110cedf7dffddb48f4f)
- [List](https://dev243690.service-now.com/sys_store_app_list.do)
- Extends: [Application](#application) *(sys_scope)*

Columns:

| Column label            | Column name      | Type       | Reference                               | May be nil |
|-------------------------|------------------|------------|-----------------------------------------|------------|
| Dependencies            | dependencies     | List       | [Store Application](#store-application) | true       |
| Install date            | install_date     | Date/Time  |                                         | true       |
| Is Store App            | is_store_app     | True/False |                                         | false      |
| Code                    | sys_code         | String     |                                         | true       |

## Sys Plugins

- Name: `sys_plugins`
- [Table](https://dev243690.service-now.com/sys_db_object.do?sys_id=d2aa4420d9313110cedf7dffddb48f49)
- [List](https://dev243690.service-now.com/sys_plugins_list.do)
- Extends: [Package](#package) *(sys_package)*
com.snc.itom.license

Columns:

| Column label     | Column name  | Type      | Reference           | May be nil |
|------------------|--------------|-----------|---------------------|------------|
| Install date     | install_date | Date/Time |                     | false      |
| Parent           | parent       | String    | [Package](#package) | true       |

```javascript
var tableName = 'sys_glide_object';
var gr = new GlideRecord('sys_db_object');
gr.addQuery('name', tableName);
gr.query();
gr.next();
var result = '## ' + gr.label + "\n\n- Name: `" + gr.name + "`\n- [Table](https://dev243690.service-now.com/" + gr.getLink(true) + ")\n- [List](https://dev243690.service-now.com/" + tableName + "_list.do)";
if (!gs.nil(gr.super_class)) {
    result += "\n- Extends: [" + gr.super_class.label + "](https://dev243690.service-now.com/" +
        gr.super_class.getGlideRecord().getLink(true) + ") *(" + gr.name + ")*";
}
var sys_id = gr.sys_id;
gr = new GlideRecord('sys_db_object');
gr.addQuery('super_class', sys_id);
gr.query();
if (gr.next()) {
    result += "\n\nExtending Tables:\n\n- [" + gr.label + "](https://dev243690.service-now.com/" + gr.getLink(true) + ") *(" + gr.name + ")*";
    while (gr.next())
        result += "\n- [" + gr.label + "](https://dev243690.service-now.com/" + gr.getLink(true) + ") *(" + gr.name + ")*";
}

gr = new GlideRecord('sys_dictionary');
gr.addQuery('name', tableName);
gr.addQuery('internal_type', 'reference');
gr.query();
if (gr.next()) {
    result += "\n\nRefers to:\n\n- " + gr.column_label + ": [" + gr.reference.label + "](https://dev243690.service-now.com/" +
        gr.reference.getGlideRecord().getLink(true) + ") *(" + gr.reference.name + ")*";
    while (gr.next())
        result += "\n- " + gr.column_label + ": [" + gr.reference.label + "](https://dev243690.service-now.com/" +
            gr.reference.getGlideRecord().getLink(true) + ") *(" + gr.reference.name + ")*";
}

gr = new GlideRecord('sys_dictionary');
gr.addQuery('internal_type', 'reference');
gr.addQuery('reference', tableName);
gr.query();
if (gr.next()) {
    result += "\n\nReferred by:\n\n- " + gr.column_label + ": *(" + gr.name + "." + gr.element + ")*";
    while (gr.next())
        result += "\n- " + gr.column_label + ": *(" + gr.name + "." + gr.element + ")*";
}
gs.info(result);
```
