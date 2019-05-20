# Identity Server

Implement a basic Identity Server Client. Additional information about *Identity Server* can be found [here](http://rogeriodossantos.github.io/Wiki/stage/identity_server.html)

## How-To execute the example 

From this folder:

```sh
# Go to the debug folder
cd ./debug

# Build the Solution (IdentityServer, Api, Client)
docker-compose -f ./docker-compose-linux.yaml build

# Run the Solution (IdentityServer, Api, Client)
docker-compose -f ./docker-compose-linux.yaml up -d

# Attach to the Client
docker attach debug_client_<id>

# Stop the Solution (IdentityServer, Api, Client)
docker-compose -f ./docker-compose-linux.yaml down
```

As result you should see every 5 seconds the client getting the token from the IdentityServer and Consult the Api using the token:

![](http://tinyurl.com/yyu547py)

**Note 01**: You can change both the API and Client to connect to other IdentityServer. To do it change the following environment variables in the *docker-compose-linux.yaml*:

- identity_server_url=<desired_identity_server>
- requite_https=<true_if_the_identity_server_uses_http>

**Note 02**: If you are not using proxy, you can change the proxy environment variable information (*xx*) in the *./debug/.env* file.

## Development Notes

### Install Templates 

`dotnet new -i IdentityServer4.Templates`

### Create new solution 

```sh
dotnet new sln -n identity_server_example
```

### Create Empty Identity Server (Server) Project

```sh
dotnet new is4empty -n IdentityServer
```

### Create an API Project

```sh
dotnet new web -n Api
dotnet sln add ./Api/Api.csproj
```
### Create a Client Project

```sh
dotnet new contole -n Client
dotnet sln add ./Client/Client.csproj

# Add IdentityModel package that encapsulates the OAuth 2.0 protocol interaction in an easy to use API
dotnet add package IdentityModel 
```

### Entrypoints

#### IdentityServer (Server)

- Discovery document: [http://localhost:8000/.well-known/openid-configuration](http://localhost:8000/.well-known/openid-configuration)

#### API 

- Identity Validation: [http://localhost:8001/identity](http://localhost:8001/identity)
  - 401: API requires a credential





