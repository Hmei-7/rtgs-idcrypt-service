﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RTGS.IDCrypt.Service.Config;
using RTGS.IDCrypt.Service.Models;
using RTGS.IDCrypt.Service.Storage;
using RTGS.IDCrypt.Service.Tests.Logging;

namespace RTGS.IDCrypt.Service.Tests.Repositories.ConnectionRepository.GivenSavingPendingBankPartnerConnection;

public class AndTableStorageUnavailable
{
	private readonly Service.Repositories.ConnectionRepository _connectionRepository;
	private readonly PendingBankPartnerConnection _pendingConnection;
	private readonly FakeLogger<Service.Repositories.ConnectionRepository> _logger = new();

	public AndTableStorageUnavailable()
	{
		_pendingConnection = new PendingBankPartnerConnection
		{
			PartitionKey = "connection-id",
			RowKey = "alias",
			ConnectionId = "connection-id",
			Alias = "alias"
		};

		var storageTableResolverMock = new Mock<IStorageTableResolver>();
		storageTableResolverMock
			.Setup(resolver => resolver.GetTable("pendingBankPartnerConnections"))
			.Throws<Exception>();

		var options = Options.Create(new BankPartnerConnectionsConfig
		{
			PendingBankPartnerConnectionsTableName = "pendingBankPartnerConnections"
		});

		_connectionRepository =
			new Service.Repositories.ConnectionRepository(storageTableResolverMock.Object, options, _logger);
	}

	[Fact]
	public async Task WhenInvoked_ThenThrows() => await FluentActions
		.Awaiting(() => _connectionRepository.SavePendingBankPartnerConnectionAsync(_pendingConnection))
		.Should()
		.ThrowAsync<Exception>();

	[Fact]
	public async Task WhenInvoked_ThenLogs()
	{
		using var _ = new AssertionScope();

		await FluentActions
			.Awaiting(() => _connectionRepository.SavePendingBankPartnerConnectionAsync(_pendingConnection))
			.Should()
			.ThrowAsync<Exception>();

		_logger.Logs[LogLevel.Error].Should().BeEquivalentTo(new List<string>
		{
			"Error occurred when saving pending bank partner connection"
		});
	}
}
