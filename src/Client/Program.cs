// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        private static bool keepRunning = true;

        private static async Task Main()
        {
            Console.WriteLine("Client Application started. Press Ctrl+C to shut down.");
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                Program.keepRunning = false;
                Console.WriteLine("Stop Requested");
            };

            while (Program.keepRunning)
            {
                Console.WriteLine("\n==================\nWaitting next cycle...");
                await Task.Delay(5000);

                string proxyUrl = Environment.GetEnvironmentVariable("proxy_url");
                string identityServerUrl = Environment.GetEnvironmentVariable("identity_server_url");
                string apiUrl = Environment.GetEnvironmentVariable("api_url");
                bool identityServerRequireHttps = Environment.GetEnvironmentVariable("identity_server_require_https") == "true";
                string identityServerClientId = Environment.GetEnvironmentVariable("identity_server_client_id");
                string identityServerClientSecret = Environment.GetEnvironmentVariable("identity_server_client_secret");
                string identityServerScope = Environment.GetEnvironmentVariable("identity_server_scope");                
                Console.WriteLine("\n00 - Current Configuration:");
                Console.WriteLine("- Proxy URL (proxy_url): " + proxyUrl);
                Console.WriteLine("- Identity Server URL (identity_server_url): " + identityServerUrl);
                Console.WriteLine("- Identity Server Scope (identity_server_scope): " + identityServerScope);
                Console.WriteLine("- Identity Server Client ID (identity_server_client_id): " + identityServerClientId);
                Console.WriteLine("- Identity Server Client Secret (identity_server_client_secret): " + identityServerClientSecret);
                Console.WriteLine("- API URL (api_url): " + apiUrl);
                Console.WriteLine("- Require HTTPS (require_https): " + identityServerRequireHttps.ToString());

                //define a HTTPHandler with Proxy configuration
                HttpClientHandler httpHandlerWithProxy = null;
                if (proxyUrl != null && proxyUrl != "")
                {
                    Console.WriteLine("\n00 - Creating Proxy Handler: " + proxyUrl);
                    httpHandlerWithProxy = new HttpClientHandler
                    {
                        //UseDefaultCredentials = true
                        UseProxy = true,
                        PreAuthenticate = true,
                        Proxy = new WebProxy()
                        {
                            Address = new Uri(proxyUrl), //proxy address
                            UseDefaultCredentials = false,
                            Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
                        }
                    };
                }
                
                // discover endpoints from metadata
                Console.WriteLine("\n01 - Discovering Entrypoints:");
                DiscoveryResponse discoveryResponse = null;
                if (identityServerRequireHttps)
                {
                    Console.WriteLine("- Using HTTPS");
                    var discoveryClient = new HttpClient(httpHandlerWithProxy);
                    discoveryResponse = await discoveryClient.GetDiscoveryDocumentAsync(identityServerUrl); //URL of the IdentityServer
                }
                else
                {
                    Console.WriteLine("- Not Using HTTPS");
#pragma warning disable CS0618 // Type or member is obsolete                        
                    var discoveryClient = httpHandlerWithProxy != null ? new DiscoveryClient(identityServerUrl, httpHandlerWithProxy) : new DiscoveryClient(identityServerUrl);
                    //var discoveryClient = new DiscoveryClient(identityServerUrl);
#pragma warning restore CS0618 // Type or member is obsolete
                    discoveryClient.Policy.RequireHttps = false;
                    discoveryResponse = await discoveryClient.GetAsync();
                }

                if (discoveryResponse.IsError)
                {
                    Console.WriteLine("Error: " + discoveryResponse.Error);
                    continue;
                }

                // request token
                Console.WriteLine("\n02 - Requesting Tokens:");
                var client = httpHandlerWithProxy != null ? new HttpClient(httpHandlerWithProxy) : new HttpClient();

                // Requesting a token using the client_credentials Grant Type
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = discoveryResponse.TokenEndpoint,
                    ClientId = identityServerClientId,
                    ClientSecret = identityServerClientSecret,
                    Scope = identityServerScope
                });

                if (tokenResponse.IsError)
                {
                    Console.WriteLine("Error: " + tokenResponse.Error);
                    continue;
                }

                Console.WriteLine(tokenResponse.Json);

                // call api                
                var apiClient = new HttpClient();
                Console.WriteLine("\n03 - Calling API:");
                apiClient.SetBearerToken(tokenResponse.AccessToken); //Use HTTP Authorization header to send the access token to the API
                try
                {
                    var response = await apiClient.GetAsync(apiUrl + "/identity"); //API address
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error: " + response.StatusCode);                        
                        continue;
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(JArray.Parse(content));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Accessing the API. Source: " + ex.Source + " ; Description: " + ex.Message);
                    continue;
                }

                Console.WriteLine("\n04 - Done :)");
            }

            Console.WriteLine("Exited gracefully!");
        }
    }
}