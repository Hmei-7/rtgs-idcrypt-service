﻿using System.Net.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using RTGS.IDCrypt.Service.ConnectionCycleScheduler.Tests.Http;
using RTGS.IDCrypt.Service.ConnectionCycleScheduler.Tests.Logging;

namespace RTGS.IDCrypt.Service.ConnectionCycleScheduler.Tests.BankConnectionCycleServiceTests.GivenTimerTriggered;

public class AndInvitedPartnerIdsEndpointUnavailable
{
	private readonly FakeLogger<BankConnectionCycleService> _fakeLogger;
	private readonly BankConnectionCycleService _bankConnectionCycleService;

	public AndInvitedPartnerIdsEndpointUnavailable()
	{
		StatusCodeHttpHandler statusCodeHandler = StatusCodeHttpHandler.Builder.Create()
			.WithServiceUnavailableResponse("/api/bank-connection/InvitedPartnerIds")
			.Build();

		var client = new HttpClient(statusCodeHandler)
		{
			BaseAddress = new Uri("https://localhost")
		};

		_fakeLogger = new FakeLogger<BankConnectionCycleService>();
		var hostApplicationLifetimeMock = new Mock<IHostApplicationLifetime>();

		var httpClientFactoryMock = new Mock<IHttpClientFactory>();
		httpClientFactoryMock
			.Setup(factory => factory.CreateClient("IdCryptServiceClient"))
			.Returns(client);

		_bankConnectionCycleService =
			new BankConnectionCycleService(_fakeLogger, hostApplicationLifetimeMock.Object, httpClientFactoryMock.Object);
	}

	[Fact]
	public async Task ThenThrowExceptionAndLog()
	{
		using var _ = new AssertionScope();

		await FluentActions
			.Awaiting(() => _bankConnectionCycleService.StartAsync(default))
			.Should()
			.ThrowAsync<Exception>();

		_fakeLogger.Logs[LogLevel.Error].Should().BeEquivalentTo("Error while getting partner ids");
	}
}
