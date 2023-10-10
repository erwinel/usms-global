# ServiceNow Typings Generator

Creates `.d.ts` from ServiceNow tables, using the [ServiceNow Table REST API](https://developer.servicenow.com/dev.do#!/reference/api/utah/rest/c_TableAPI).

## Command Line Options

`-d=`*filename*: The Path to the typings database.
This path is relative to the subdirectory containing the executable.
If this option is not present, then this will use the `SnTsTypeGenerator:DbFile` setting in `appsettings.json`, if defined; otherwise it will use a database named `Typings.db` in the same subdirectory as the executable.

`-t=`*name,name,...*
The names of the table to generate typings for.
If this option is not present, then this will use the `SnTsTypeGenerator:Table` setting in `appsettings.json`, if defined.

`-u=`*login*
The user name credentials to use when connecting to the remote instance.
If this option is not present, then this will use the `SnTsTypeGenerator:UserName` setting in `appsettings.json`, if defined; otherwise, you will be prompted for the user name.

`-p=`*password*
The password credentials to use when connecting to the remote instance.
If this option is not present, then this will use the `SnTsTypeGenerator:Password` setting in `appsettings.json`, if defined; otherwise, you will be prompted for the password.

`-i=`*id*
Specifies client ID in the remote ServiceNow instance's Application Registry.
If this option is not present, then this will use the `SnTsTypeGenerator:ClientId` setting in `appsettings.json`, if defined; otherwise, you will be prompted for the client ID.

`-x=`*secret*
The the client secret in the remote ServiceNow instance's Application Registry.
If this option is not present, then this will use the `SnTsTypeGenerator:ClientSecret` setting in `appsettings.json`, if defined; otherwise, you will be prompted for the client secret.

`-r=`*url*
The base URL of the remote ServiceNow instance.
If this option is not present, then this will use the `SnTsTypeGenerator:RemoteURL` setting in `appsettings.json`, if defined; otherwise, an error message will be displayed.

`-s=true`
Generate typings for use with scoped apps.
This cannot be used with the `-g=true` option.
If this option is not present, then this will use the `SnTsTypeGenerator:Scoped` setting in `appsettings.json`, if it is set to true.

`-g=true`
Generate typings for use with scoped apps.
This cannot be used with the `-s=true` option.
If this option is not present, then this will use the `SnTsTypeGenerator:Global` setting in `appsettings.json`, if it is set to true.
This is the default behaviour if neither this option, the `SnTsTypeGenerator:Global` setting, the `-s=true` option, nor the `SnTsTypeGenerator:Scoped` is present.

`-o=`*filename*`.d.ts`
The output file name.
If this option is not present, then this will use the `SnTsTypeGenerator:Output` setting in `appsettings.json`, if present; otherwise, an error message will be displayed.

`-f=true`
Force overwrite of the output file.
If this option is not present, then this will use the `SnTsTypeGenerator:Force` setting in `appsettings.json`, if set to true; otherwise, it will write the output to a file named `types.d.ts` in the current working directory.

`-?`
  or
`-h`
  or
`--help`
Displays this help information.
If this option is used, then all other options are ignored.

## Development setup

1. If the element `Project/PropertyGroup/UserSecretsId` does not exist in [SnTsTypeGenerator.csproj](./SnTsTypeGenerator.csproj), run the following command from the project directory: `dotnet user-secrets init`.
2. Create Application Registry in target ServiceNow instance:
   1. Navigate to *System OAuth* => *Application Registry*.
   2. Click the *New* button.
   3. Click the *Create an OAuth API endpoint for external clients* link.
   4. Fill in the following fields and submit the form:
      - **Name**: SnTsTypeGenerator
      - **DataSync**: Unchecked
      - **Accessible from**: All application scopes
      - **Active**: Checked
      - Leave all other fields as-is.
3. Edit [appsettings.Development.json](./appsettings.Development.json) and add/modify the following keys in the "SnTsTypeGenerator" section:
   - **RemoteURL**: This is the base URL of the ServiceNow instance.
   - **UserName**: The user account name to authenticate as.
   - **ClientId**: This is the value of the *Client ID* field from the Application Registry previously created.
4. Set the client secret from the 'Client Secret' field of the Application Registry previously created, using the following command: `dotnet user-secrets set "SnTsTypeGenerator:ClientSecret" "{client_secret}"`
5. Set the ServiceNow user account password using the following command: `dotnet user-secrets set "SnTsTypeGenerator:Password" "{password}"`

- See [Safe storage of app secrets in development in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for more information about user secrets.
