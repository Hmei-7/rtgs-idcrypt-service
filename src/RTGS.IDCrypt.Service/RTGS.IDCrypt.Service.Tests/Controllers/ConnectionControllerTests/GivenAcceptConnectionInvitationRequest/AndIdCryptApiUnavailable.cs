﻿using Microsoft.Extensions.Logging;
using Moq;
using RTGS.IDCrypt.Service.Contracts.Connection;
using RTGS.IDCrypt.Service.Controllers;
using RTGS.IDCrypt.Service.Helpers;
using RTGS.IDCrypt.Service.Tests.Logging;
using RTGS.IDCryptSDK.Connections;
using RTGS.IDCryptSDK.Connections.Models;
using RTGS.IDCryptSDK.Wallet;

namespace RTGS.IDCrypt.Service.Tests.Controllers.ConnectionControllerTests.GivenAcceptConnectionInvitationRequest;

public class AndIdCryptApiUnavailable
{
	private readonly FakeLogger<ConnectionController> _logger;
	private readonly ConnectionController _connectionController;

	public AndIdCryptApiUnavailable()
	{
		var connectionsClientMock = new Mock<IConnectionsClient>();

		connectionsClientMock
			.Setup(connectionsClient => connectionsClient.ReceiveAndAcceptInvitationAsync(
				It.IsAny<ReceiveAndAcceptInvitationRequest>(),
				It.IsAny<CancellationToken>()))
			.Throws<Exception>();

		_logger = new FakeLogger<ConnectionController>();

		_connectionController = new ConnectionController(
			_logger,
			connectionsClientMock.Object,
			Mock.Of<IWalletClient>(),
			Mock.Of<IAliasProvider>());
	}

	[Fact]
	public async Task WhenPosting_ThenLog()
	{
		using var _ = new AssertionScope();

		var request = new AcceptConnectionInvitationRequest
		{
			Id = "id",
			Type = "type",
			Alias = "alias",
			Label = "label",
			RecipientKeys = new[] { "recipient-key" },
			ServiceEndpoint = "service-endpoint"
		};

		await FluentActions
			.Awaiting(() => _connectionController.Accept(request, default))
			.Should()
			.ThrowAsync<Exception>();

		_logger.Logs[LogLevel.Error].Should().BeEquivalentTo(new List<string>
			{
				$"Error occurred when accepting invitation"
			});
	}
}