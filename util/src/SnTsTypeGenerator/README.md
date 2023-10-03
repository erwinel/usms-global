# ServiceNow Typings Generator

Creates `.d.ts` from ServiceNow tables, using the [ServiceNow Table REST API](https://developer.servicenow.com/dev.do#!/reference/api/utah/rest/c_TableAPI).

## Development setup

1. If the element `Project/PropertyGroup/UserSecretsId` does not exist in [SnTsTypeGenerator.csproj](./SnTsTypeGenerator.csproj), run the following command from the project directory: `dotnet user-secrets init`.
2. Create Application Registry in target ServiceNow instance:
   1. Navigate to *System OAuth* => *Application Registries*.
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

- See [](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for more information about user secrets.
