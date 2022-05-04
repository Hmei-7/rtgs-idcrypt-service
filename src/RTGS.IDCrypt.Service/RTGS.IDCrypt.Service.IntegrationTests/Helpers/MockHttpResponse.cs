﻿using System.Net;
using System.Net.Http;

namespace RTGS.IDCrypt.Service.IntegrationTests.Helpers;

public record MockHttpResponse
{
	public HttpContent Content { get; init; }
	public HttpStatusCode HttpStatusCode { get; init; }
}
