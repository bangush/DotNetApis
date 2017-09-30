using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetApis.Common;
using FunctionApp.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SimpleInjector.Lifestyles;
using System.Collections.Generic;
using FunctionApp.CompositionRoot;

namespace FunctionApp
{
    public sealed class NugetSearchFunction
    {
        private static readonly HttpClient NugetClient = new HttpClient();

        private readonly ILogger _logger;

        public NugetSearchFunction(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<HttpResponseMessage> RunAsync(HttpRequestMessage req)
        {
            try
            {
                var query = req.GetQueryNameValuePairs().ToList();
                var q = query.Optional("query") ?? "";
                var skip = query.Optional("skip", int.Parse);
                var includePrerelease = query.Optional("includePrerelease", bool.Parse);
                var includeUnlisted = query.Optional("includeUnlisted", bool.Parse);

                _logger.LogDebug("Received request for query={query}, skip={skip}, includePrerelease={includePrerelease}, includeUnlisted={includeUnlisted}",
                    q, skip, includePrerelease, includeUnlisted);

                // Build NuGet API query
                var uri = new UriBuilder("http://www.nuget.org/api/v2/Search()");
                var args = new Dictionary<string, string>
                {
                    { "searchTerm", $"'{q.Replace("'", "''")}'" },
                    { "$skip", skip.ToString(CultureInfo.InvariantCulture) },
                    { "$top", "7" },
                    { "includePrerelease", includePrerelease.ToString(CultureInfo.InvariantCulture).ToLowerInvariant() }
                };
                if (includeUnlisted)
                    args["includeDelisted"] = "true";
                uri.Query = Nito.UniformResourceIdentifiers.Implementation.UriUtil.FormUrlEncode(args);
                _logger.LogDebug("Searching on NuGet: {uri}", uri.Uri);

                // Translate xml to json
                var result = await NugetClient.GetStringAsync(uri.Uri);
                var doc = XDocument.Parse(result);
                var atom = XNamespace.Get("http://www.w3.org/2005/Atom");
                var metadata = XNamespace.Get("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                var dataservices = XNamespace.Get("http://schemas.microsoft.com/ado/2007/08/dataservices");
                var hits = doc.Root.Elements(atom + "entry").Select(entry => entry.Element(metadata + "properties"))
                    .Select(properties => new SearchResponseMessage.Hit
                    {
                        Id = properties.Element(dataservices + "Id").Value,
                        Version = properties.Element(dataservices + "NormalizedVersion").Value,
                        Title = properties.Element(dataservices + "Title").Value,
                        IconUrl = properties.Element(dataservices + "IconUrl").Value,
                        Summary = properties.Element(dataservices + "Summary").Value,
                        Description = properties.Element(dataservices + "Description").Value,
                        TotalDownloads = properties.Element(dataservices + "DownloadCount").Value
                    });

                return req.CreateResponse(HttpStatusCode.OK, new SearchResponseMessage { Hits = hits.ToList() });
            }
            catch (ExpectedException ex)
            {
                _logger.LogDebug("Returning {httpStatusCode}: {errorMessage}", (int)ex.HttpStatusCode, ex.Message);
                return req.CreateErrorResponseWithLog(ex);
            }
        }

        [FunctionName("NugetSearch")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "0/doc")] HttpRequestMessage req,
            ILogger log, TraceWriter writer, ExecutionContext context)
        {
            GlobalConfig.Initialize();
            req.ApplyRequestHandlingDefaults(context);
            AmbientContext.InMemoryLogger = new InMemoryLogger();
            AmbientContext.OperationId = context.InvocationId;
            AmbientContext.RequestId = req.TryGetRequestId();
            AsyncLocalLogger.Logger = new CompositeLogger(Enumerables.Return(AmbientContext.InMemoryLogger, log, req.IsLocal() ? new TraceWriterLogger(writer) : null));

            var container = Containers.GetContainerForNugetSearch();
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                return await container.GetInstance<NugetSearchFunction>().RunAsync(req);
            }
        }
    }
}