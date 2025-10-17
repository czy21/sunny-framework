using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Sunny.Framework.Core.Util;

namespace Sunny.Framework.Web.Handler;

public class RefitLogHandler(ILogger<RefitLogHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!logger.IsEnabled(LogLevel.Information)) return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var traceId = Guid.NewGuid().ToString();
        var stopwatch = Stopwatch.StartNew();

        var headers = new SortedDictionary<string, string>();
        var requestBody = "";

        foreach (var t in request.Headers) headers.Add(t.Key, string.Join("; ", t.Value.ToList()));

        if (request.Content != null)
        {
            foreach (var t in request.Content.Headers) headers.Add(t.Key, string.Join("; ", t.Value.ToList()));
            requestBody = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (headers.TryGetValue("Content-Type", out var requestContentType) && requestContentType.Contains("application/json", StringComparison.CurrentCultureIgnoreCase))
            {
                requestBody = JsonUtil.Serialize(JsonUtil.Deserialize<dynamic>(requestBody));
            }
        }

        logger.LogInformation("[Request {TraceId}] {Method} {Path} \nHeaders: {Headers}\nBody: {Body}", traceId, request.Method, request.RequestUri, JsonUtil.Serialize(headers, true), requestBody);

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        stopwatch.Stop();

        logger.LogInformation("[Response {TraceId}] {Method} {Path} - {StatusCode} - {Duration}ms\nBody: {Body}", traceId, request.Method, request.RequestUri, (int)response.StatusCode, stopwatch.ElapsedMilliseconds, responseBody);

        return response;
    }
}