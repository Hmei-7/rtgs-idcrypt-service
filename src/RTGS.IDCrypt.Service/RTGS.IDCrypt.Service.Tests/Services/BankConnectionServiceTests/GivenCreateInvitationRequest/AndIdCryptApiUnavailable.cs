﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RTGS.IDCrypt.Service.Config;
using RTGS.IDCrypt.Service.Helpers;
using RTGS.IDCrypt.Service.Repositories;
using RTGS.IDCrypt.Service.Services;
using RTGS.IDCrypt.Service.Tests.Logging;
using RTGS.IDCryptSDK.BasicMessage;
using RTGS.IDCryptSDK.Connections;
using RTGS.IDCryptSDK.Wallet;

namespace RTGS.IDCrypt.Service.Tests.Services.BankConnectionServiceTests.GivenCreateInvitationRequest;

public class AndIdCryptApiUnavailable
{
	private readonly BankConnectionService _bankConnectionService;
	private const string Alias = "alias";
	private readonly FakeLogger<BankConnectionService> _logger = new();

	public AndIdCryptApiUnavailable()
	{
		var coreOptions = Options.Create(new CoreConfig
		{
			RtgsGlobalId = "rtgs-global-id"
		});

		var connectionsClientMock = new Mock<IConnectionsClient>();

		connectionsClientMock
			.Setup(client => client.CreateConnectionInvitationAsync(
				Alias,
				It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(),
				It.IsAny<CancellationToken>()))
			.Throws<Exception>()
			.Verifiable();

		var aliasProviderMock = new Mock<IAliasProvider>();
		aliasProviderMock.Setup(provider => provider.Provide()).Returns(Alias);

		_bankConnectionService = new BankConnectionService(
			connectionsClientMock.Object,
			_logger,
			Mock.Of<IBankPartnerConnectionRepository>(),
			aliasProviderMock.Object,
			Mock.Of<IWalletClient>(),
			coreOptions,
			Mock.Of<IBasicMessageClient>()
		);
	}

	[Fact]
	public async Task WhenInvoked_ThenThrows() =>
		await FluentActions
			.Awaiting(() => _bankConnectionService.CreateInvitationAsync("rtgs-global-id"))
			.Should()
			.ThrowAsync<Exception>();

	[Fact]
	public async Task WhenInvoked_ThenLogs()
	{
		using var _ = new AssertionScope();

		await FluentActions
			.Awaiting(() => _bankConnectionService.CreateInvitationAsync("rtgs-global-id"))
			.Should()
			.ThrowAsync<Exception>();

		_logger.Logs[LogLevel.Error].Should().BeEquivalentTo("Error occurred when creating connection invitation for bank rtgs-global-id");
	}
}
