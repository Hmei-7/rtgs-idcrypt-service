﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RTGS.IDCrypt.Service.Config;
using RTGS.IDCrypt.Service.Helpers;
using RTGS.IDCrypt.Service.Repositories;
using RTGS.IDCrypt.Service.Services;
using RTGS.IDCrypt.Service.Tests.Logging;
using RTGS.IDCryptSDK.Connections;
using RTGS.IDCryptSDK.Wallet;

namespace RTGS.IDCrypt.Service.Tests.Services.RtgsConnectionServiceTests.GivenDeleteRequest;

public class AndIdCryptApiCallFails
{
	private readonly Mock<IConnectionsClient> _connectionsClientMock = new();
	private readonly RtgsConnectionService _rtgsConnectionService;
	private readonly Mock<IRtgsConnectionRepository> _rtgsConnectionRepositoryMock = new();
	private const string ConnectionId = "connection-id";
	private readonly FakeLogger<RtgsConnectionService> _logger = new();

	public AndIdCryptApiCallFails()
	{
		var coreOptions = Options.Create(new CoreConfig
		{
			RtgsGlobalId = "rtgs-global-id"
		});

		_connectionsClientMock
			.Setup(client => client.DeleteConnectionAsync(ConnectionId, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Something went wrong"))
			.Verifiable();

		_rtgsConnectionRepositoryMock
			.Setup(service => service.DeleteAsync(ConnectionId,
				It.IsAny<CancellationToken>()))
			.Verifiable();

		_rtgsConnectionService = new RtgsConnectionService(
			_connectionsClientMock.Object,
			_logger,
			_rtgsConnectionRepositoryMock.Object,
			Mock.Of<IAliasProvider>(),
			Mock.Of<IWalletClient>(),
			coreOptions);
	}

	[Fact]
	public async Task WhenInvoked_ThenLog()
	{
		using var _ = new AssertionScope();

		await FluentActions
			.Awaiting(() => _rtgsConnectionService.DeleteAsync(ConnectionId))
			.Should()
			.ThrowAsync<Exception>().WithMessage("Something went wrong");

		_logger.Logs[LogLevel.Error].Should().BeEquivalentTo("Error occurred when deleting RTGS connection");
	}
}
