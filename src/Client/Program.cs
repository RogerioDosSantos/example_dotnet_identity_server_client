// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
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

                string identityServerUrl = Environment.GetEnvironmentVariable("identity_server_url");
                string apiUrl = Environment.GetEnvironmentVariable("api_url");
                bool requireHttps = Environment.GetEnvironmentVariable("requite_https") == "true";
                Console.WriteLine("\n00 - Current Configuration:");
                Console.WriteLine("- Identity Server URL (identity_server_url): " + identityServerUrl);
                Console.WriteLine("- API URL (api_url): " + identityServerUrl);
                Console.WriteLine("- Require HTTPS (requite_https): " + requireHttps.ToString());
            
                // discover endpoints from metadata
                Console.WriteLine("\n01 - Discovering Entrypoints:");
                DiscoveryResponse discoveryResponse = null;
                if (requireHttps)
                {
                    Console.WriteLine("- Using HTTPS");
                    var discoveryClient = new HttpClient();
                    discoveryResponse = await discoveryClient.GetDiscoveryDocumentAsync(identityServerUrl); //URL of the IdentityServer
                }
                else
                {
                    Console.WriteLine("- Using HTTPS");
#pragma warning disable CS0618 // Type or member is obsolete                
                    var discoveryClient = new DiscoveryClient(identityServerUrl);
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
                var client = new HttpClient();
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = discoveryResponse.TokenEndpoint,
                    ClientId = "client",
                    ClientSecret = "secret",
                    Scope = "api1"
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