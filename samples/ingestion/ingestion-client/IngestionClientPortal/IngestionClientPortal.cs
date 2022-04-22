// <copyright file="IngestionClientPortal.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace ServeIngestionClientPortal
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MimeTypes;

    public class IngestionClientPortal
    {
        // This key is used by Azure Functions to tell you what is the root of this website.
        private const string ConfigurationKeyApplicationRoot = "AzureWebJobsScriptRoot";
        private const string StaticFilesFolder = "www";

        private readonly string contentRoot;

        private readonly string defaultPage;

        // The configuration is available for injection.
        // The used settings can be in any config (environment, host.json local.settings.json)
        public IngestionClientPortal(IConfiguration configuration)
        {
            this.contentRoot = Path.GetFullPath(Path.Combine(
              configuration.GetValue<string>(ConfigurationKeyApplicationRoot, GetScriptPath()),
              StaticFilesFolder));
            this.defaultPage = configuration.GetValue<string>("DEFAULT_PAGE", "index.html");
        }

        [FunctionName("IngestionClientPortal")]
        public async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            await Task.CompletedTask.ConfigureAwait(false);

#pragma warning disable CA1031 // Do not catch general exception types (Justification: Catching to return status code result)
            try
            {
                var filePath = GetFilePath(req.Query["file"]);
                if (File.Exists(filePath))
                {
                    var stream = File.OpenRead(filePath);
                    return new FileStreamResult(stream, GetMimeType(filePath))
                    {
                        LastModified = File.GetLastWriteTime(filePath)
                    };
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                return new InternalServerErrorResult();
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static bool IsInDirectory(string parentPath, string childPath) => childPath.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase);

        private static string GetMimeType(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return MimeTypeMap.GetMimeType(fileInfo.Extension);
        }

        private static string GetScriptPath()
            => Path.Combine(GetEnvironmentVariable("HOME"), @"site\wwwroot");

        private static string GetEnvironmentVariable(string name)
            => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        private string GetFilePath(string pathValue)
        {
            var path = pathValue ?? string.Empty;
            string fullPath = Path.GetFullPath(Path.Combine(contentRoot, pathValue));
            if (!IsInDirectory(this.contentRoot, fullPath))
            {
                throw new ArgumentException("Invalid path");
            }

            if (Directory.Exists(fullPath))
            {
                fullPath = Path.Combine(fullPath, defaultPage);
            }

            return fullPath;
        }
    }
}
