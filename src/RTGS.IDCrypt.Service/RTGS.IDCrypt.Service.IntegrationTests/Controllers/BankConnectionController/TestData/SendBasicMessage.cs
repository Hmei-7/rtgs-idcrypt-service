﻿using RTGS.IDCrypt.Service.IntegrationTests.Helpers;

namespace RTGS.IDCrypt.Service.IntegrationTests.Controllers.BankConnectionController.TestData;

public static class SendBasicMessage
{
	public const string Path = "/connections/connection-id-1/send-message";

	private static string Response => "{}";

	public static HttpRequestResponseContext HttpRequestResponseContext =>
		new(Path, Response);
}
