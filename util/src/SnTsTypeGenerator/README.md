# ServiceNow Typings Generator

- [ServiceNow Typings Generator](#servicenow-typings-generator)
  - [Settings and Command Line Options](#settings-and-command-line-options)
    - [Table Names](#table-names)
    - [Output for globally-scoped scripting](#output-for-globally-scoped-scripting)
    - [ServiceNow Instance URL](#servicenow-instance-url)
    - [Login User Name](#login-user-name)
    - [Login Password](#login-password)
    - [Client ID](#client-id)
    - [Client Secret](#client-secret)
    - [Override Output File Path](#override-output-file-path)
      - [Force Overwrite](#force-overwrite)
    - [Override Database File Path](#override-database-file-path)
    - [Emitting Base Types](#emitting-base-types)
    - [Including Referenced and Parent Types](#including-referenced-and-parent-types)
    - [Get Package Groups](#get-package-groups)
    - [Get Remote Sources](#get-remote-sources)
    - [Show Help](#show-help)
  - [Development Environment setup](#development-environment-setup)
    - [Create Application Registry](#create-application-registry)
    - [Store Client Secret and User Account Password](#store-client-secret-and-user-account-password)

Creates `.d.ts` from ServiceNow tables, using the [ServiceNow Table REST API](https://developer.servicenow.com/dev.do#!/reference/api/utah/rest/c_TableAPI).

## Settings and Command Line Options

Options specified as command line switches will override those specified in the [Application settings](./appsettings.json) file.

### Table Names

This specifies the names of the tables to render typings for. This is specified using the `-t` command line switch or the `SnTsTypeGenerator/Tables` setting. If specified on the command line, multiple table names are separated by commas with no spaces. Table names are individually specified within an array in the [Application settings](./appsettings.json) file.

Command Line Example uses comma-delimited names (no spaces):

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "Tables": ["sys_metadata", "task", "sys_user"],
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Output for globally-scoped scripting

To generate typings for globally-scoped scripting, use the `-g` command line switch or set the the `SnTsTypeGenerator/GlobalScope` settings to `true`.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -g
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "GlobalScope": true,
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### ServiceNow Instance URL

This is the base URL of the source ServiceNow instance, which should not include any path, query or fragment. This can be specified using the `-r` command line switch or the `SnTsTypeGenerator/RemoteURL` setting. If this is not specified, you will be prompted for the URL.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -r https://dev00000.service-now.com
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Login User Name

The account name to use as account credentials on the source ServiceNow instance. This can be specified using the `-u` command line switch or the `SnTsTypeGenerator/UserName` setting. If this is not specified, you will be prompted for the user account login name.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -u my_login
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "UserName": "my_login",
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Login Password

The password to use as account credentials on on the source ServiceNow instance. This can be specified using the `-o` command line switch or the `SnTsTypeGenerator/Password` setting. If this is not specified, you will be prompted for the user account password.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -u my_login -p "My*\$ecret!"
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "UserName": "my_login",
        "Password": "Ny*$ecret!",
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Client ID

This is the Client ID of the Application Registry entry in the target ServiceNow instance. This can be specified using the `SnTsTypeGenerator/ClientId` setting. If this setting is not specified, this will connect using only the account credentials and will not attempt to get an OAuth access token.

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "ClientId": "00000000000000000000000000000000",
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Client Secret

This is the Client Secrent from the Application Registry entry in the target ServiceNow instance. This can be specified using the `-x` command line switch or the `SnTsTypeGenerator/ClientSecret` setting. If the Client ID is specified, but the Client Secret is not, you will be prompted for the client secret.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -x "My*\$ecret!"
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "ClientId": "00000000000000000000000000000000",
        "ClientSecret": "My*$ecret!",
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Override Output File Path

The file output path relative to the current working subdirectory. This defaults to a file named `types.d.ts`.

You can override this using the  `-o` command line switch or the `SnTsTypeGenerator/Output` setting. If the file extension is omitted, it will have the default extension of `.d.ts`.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -o=myTypes.d.ts
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "Output": "myTypes.d.ts",
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

#### Force Overwrite

To overwrite any existing output file, use the `-f` or `--force` command line switch or set the `SnTsTypeGenerator/Force` setting to `true`;
otherwise the type generation will fail if the output file already exists.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -o=myTypes.d.ts -f
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "DbFile": "MyTypings.db",
        "ForceOverwrite": true,
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Override Database File Path

This defaults to a file named `Typings.db` in the same subdirectory as the application executable.

You can override this using the  `-d` command line switch or the `SnTsTypeGenerator/DbFile` setting. The path for this setting is relative to the current working subdirectory.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -d=MyTypings.db
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "DbFile": "MyTypings.db",
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Emitting Base Types

To include the `$$GlideElement.Reference<TFields, TRecord>` type definition and the `$$tableFields.IBaseRecord` interface, use the `-b` command line switch or set the `SnTsTypeGenerator/DbFile` setting to `true`.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -b
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "EmitBaseTypes": true,
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Including Referenced and Parent Types

To include the parent type definitions and type definitions referenced by elements, which are not explicitly included in the `-t` command line argument or `SnTsTypeGenerator/Tables` setting, use the `-i` command line switch or set the `SnTsTypeGenerator/IncludeReferenced` setting to `true`.

Command Line Example:

```sh
SnTsTypeGenerator -t=sys_metadata,task,sys_user -i
```

Settings Example:

```json
{
    "SnTsTypeGenerator": {
        "IncludeReferenced": true,
        "RemoteURL": "https://dev00000.service-now.com"
    }
}
```

### Get Package Groups

To show a list of package groups defined in app settings and in the database, use the '--get-package-groups' switch.
This switch cannot be used with other command line switches.

Command Line Example:

```sh
SnTsTypeGenerator --get-package-groups
```

### Get Remote Sources

To show a list of remote sources in the database, use the '--get-remote-sources' switch.
This switch cannot be used with other command line switches.

Command Line Example:

```sh
SnTsTypeGenerator --get-remote-sources
```

### Show Help

To display help text, use the `-?`, `-h` or `--help` command line switch. If this is specified, no typings will be generated.

## Development Environment setup

### Create Application Registry

If you wish to use OAuth tokens, you will need to add and application registry in the target ServiceNow instance.

Note: If the element `Project/PropertyGroup/UserSecretsId` does not exist in [SnTsTypeGenerator.csproj](./SnTsTypeGenerator.csproj), run `dotnet user-secrets init` from the project directory.

In the target ServiceNow instance:

1. Navigate to *System OAuth* => *Application Registry*.
2. Click the *New* button.
3. Click the *Create an OAuth API endpoint for external clients* link.
4. Fill in the following fields and save changes:
   - **Name**: SnTsTypeGenerator
   - **DataSync**: Unchecked
   - **Accessible from**: All application scopes
   - **Active**: Checked
   - *Leave all other fields as-is.*

Add/modify the following keys in the "SnTsTypeGenerator" section of [appsettings.Development.json](./appsettings.Development.json):

- **RemoteURL**: This is the base URL of the ServiceNow instance.
- **UserName**: The user account name to authenticate as.
- **ClientId**: This is the value of the *Client ID* field from the Application Registry previously created.

### Store Client Secret and User Account Password

Set Client Secret and user account password in user-secrets, where `"{client_secret}"` is the valid of the Client Secret field in the Application Registry, and `"{password}"` is the actual password for the account specified by the `UserName` setting.

```sh
dotnet user-secrets set "SnTsTypeGenerator:ClientSecret" "{client_secret}" --project util/src/SnTsTypeGenerator
dotnet user-secrets set "SnTsTypeGenerator:Password" "{password}" --project util/src/SnTsTypeGenerator
```

- See [Safe storage of app secrets in development in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for more information about user secrets.
