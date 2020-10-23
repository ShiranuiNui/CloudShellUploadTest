using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CloudShell_Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace CloudShell_Test.Controllers
{
    [ApiController]
    [Route("Signin-google")]
    public class GoogleSignInController : ControllerBase
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly IConfiguration Configuration;
        public GoogleSignInController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string code)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", "https://localhost:5001/signin-google" },
                { "client_id", Configuration["Google:ClientId"]},
                { "client_secret",Configuration["Google:ClientSecret"]}
            };
            var uri = new Uri(@"https://oauth2.googleapis.com/token");
            var content = new FormUrlEncodedContent(parameters);
            // Exchange Authorization Code for Access Tokens
            // https://developers.google.com/identity/protocols/oauth2/web-server#exchange-authorization-code
            var tokenResponse = await httpClient.PostAsync(uri.AbsoluteUri, content);
            string tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var googleToken = JsonSerializer.Deserialize<GoogleToken>(tokenResponseString, options);

            var envRequest = new HttpRequestMessage(HttpMethod.Get, @"https://cloudshell.googleapis.com/v1alpha1/users/me/environments/default");
            envRequest.Headers.Add("ContentType", "application/json");
            envRequest.Headers.Add("Authorization", "Bearer " + googleToken.AccessToken);
            // Get CloudShell Endpoint Infomation for SSH.
            var envResponse = await httpClient.SendAsync(envRequest);
            string envResponseString = await envResponse.Content.ReadAsStringAsync();

            var csEnv = JsonSerializer.Deserialize<CloudShellEnviromentResponse>(envResponseString, options);
            var privateKey = new PrivateKeyAuthenticationMethod(csEnv.SshUsername, new PrivateKeyFile[]{
                                    new PrivateKeyFile("id_rsa")
                                });
            var sftpConnInfo = new ConnectionInfo(csEnv.SshHost, csEnv.SshPort, csEnv.SshUsername,
                new AuthenticationMethod[] { privateKey }
            );
            using var sftpClient = new SftpClient(sftpConnInfo);
            // Connect SSH Session.
            sftpClient.Connect();
            // Create Directory and Upload File.
            sftpClient.CreateDirectory($"{sftpClient.WorkingDirectory}/.kube/");
            using var fileStream = System.IO.File.OpenRead("config");
            sftpClient.UploadFile(fileStream, $"{sftpClient.WorkingDirectory}/.kube/config", true);
            sftpClient.Disconnect();
            var openUri = new Uri($@"https://localhost:5001/open.html");
            // Close Session and Redirect to "Open in Cloud Shell" Button Page.
            return Redirect(openUri.AbsoluteUri);
        }
    }
    [ApiController]
    [Route("login")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        public GoogleAuthController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var parameters = new Dictionary<string, string>()
            {
                { "scope", "https://www.googleapis.com/auth/cloud-platform" },
                { "response_type", "code" },
                { "redirect_uri", "https://localhost:5001/signin-google" },
                { "client_id", Configuration["Google:ClientId"]}
            };
            var uri = new Uri($@"https://accounts.google.com/o/oauth2/v2/auth?{await new FormUrlEncodedContent(parameters).ReadAsStringAsync()}");
            // Redirect to Google's OAuth 2.0 server
            // https://developers.google.com/identity/protocols/oauth2/web-server#redirecting
            return Redirect(uri.AbsoluteUri);
        }
    }
}