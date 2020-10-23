# CloudShellUploadTest

Testing Program for Access Google CloudShell API Triggered by HTTP Request.

## Prerequisites

- .NET Core SDK 3.1

## How to Use

1. Create Google API Key. redirect URIs are `https://localhost:5001/signin-google` . Please see [https://developers.google.com/identity/protocols/oauth2/web-server#prerequisites](https://developers.google.com/identity/protocols/oauth2/web-server#prerequisites)

1. Set `Google:ClientId` and `Google:ClientSecret` on EnviromentVariables or `appsettings.json`

1. Place RSA Private Key named `id_rsa` on Project Root Directory.

1. Upload Pair Public Key for Google CloudShell. Use [https://cloud.google.com/shell/docs/reference/rest/v1alpha1/users.environments.publicKeys/create](https://cloud.google.com/shell/docs/reference/rest/v1alpha1/users.environments.publicKeys/create)

1. Run Google CloudShell on Web Console or API.

1. Run `dotnet run`
