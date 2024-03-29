﻿using RTGS.IDCrypt.Service.IntegrationTests.Controllers.BankConnectionController.TestData;
using RTGS.IDCrypt.Service.IntegrationTests.Extensions;
using RTGS.IDCrypt.Service.IntegrationTests.Helpers;
using RTGS.IDCrypt.Service.Models;

namespace RTGS.IDCrypt.Service.IntegrationTests.Fixtures.Connection;

public class DeleteConnectionFixture : ConnectionsTestFixtureBase
{
	public DeleteConnectionFixture()
	{
		IdCryptStatusCodeHttpHandler = StatusCodeHttpHandler.Builder
			.Create()
			.WithOkResponse(DeleteConnection.HttpRequestResponseContext)
			.WithOkResponse(SendBasicMessage.HttpRequestResponseContext)
			.Build();
	}

	public StatusCodeHttpHandler IdCryptStatusCodeHttpHandler { get; }

	public async Task TestSeed()
	{
		var aDate = DateTime.SpecifyKind(new(2022, 4, 1, 0, 0, 0), DateTimeKind.Utc);

		var bankPartnerConnection = new BankPartnerConnection
		{
			PartitionKey = "rtgs-global-id",
			RowKey = "alias-1",
			ConnectionId = "connection-id-1",
			Alias = "alias-1",
			CreatedAt = new DateTime(2000, 01, 01).ToUniversalTime(),
			ActivatedAt = aDate,
			PublicDid = "public-did-1",
			Status = "Pending",
			Role = "Inviter"
		};

		await InsertBankPartnerConnectionAsync(bankPartnerConnection);
	}

	protected override void CustomiseHost(IHostBuilder builder) =>
		builder.ConfigureServices(services =>
			services
				.AddTestIdCryptHttpClient(IdCryptStatusCodeHttpHandler)
		);
}
