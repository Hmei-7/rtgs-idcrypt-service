﻿using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;

namespace RTGS.IDCrypt.Service.ConnectionCycleScheduler.Tests.Http;

public sealed class StatusCodeHttpHandler : DelegatingHandler
{
	private readonly Dictionary<string, MockHttpResponse> _mockHttpResponses;

	public ConcurrentDictionary<string, ConcurrentBag<HttpRequestMessage>> Requests { get; }

	private StatusCodeHttpHandler(Dictionary<string, MockHttpResponse> mockHttpResponses)
	{
		Requests = new ConcurrentDictionary<string, ConcurrentBag<HttpRequestMessage>>();
		_mockHttpResponses = mockHttpResponses;
	}

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var requestPath = request.RequestUri!.LocalPath;

		Requests.TryAdd(requestPath, new ConcurrentBag<HttpRequestMessage>());
		Requests[requestPath].Add(request);

		var responseMock = _mockHttpResponses[requestPath];

		var response = new HttpResponseMessage(responseMock.HttpStatusCode)
		{
			Content = new StringContent(responseMock.Content),
			RequestMessage = request
		};

		return Task.FromResult(response);
	}

	public sealed class Builder
	{
		private Dictionary<string, MockHttpResponse> Responses { get; } = new();

		public static Builder Create() => new();

		public Builder WithServiceUnavailableResponse(string path) =>
			WithResponse(path, null, HttpStatusCode.ServiceUnavailable);

		public Builder WithOkResponse(HttpRequestResponseContext httpRequestResponseContext) =>
			WithResponse(
				httpRequestResponseContext.RequestPath,
				httpRequestResponseContext.ResponseContent,
				HttpStatusCode.OK);

		public Builder WithNotFoundResponse(string path) =>
			WithResponse(path, null, HttpStatusCode.NotFound);

		public StatusCodeHttpHandler Build() => new(Responses);

		private Builder WithResponse(string path, string content, HttpStatusCode statusCode)
		{
			var mockResponse = new MockHttpResponse
			{
				HttpStatusCode = statusCode,
				Content = content
			};

			Responses[path] = mockResponse;

			return this;
		}
	}
}
