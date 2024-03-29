﻿using RTGS.IDCrypt.Service.Models;

namespace RTGS.IDCrypt.Service.Tests.TestData;

public static class TestBankPartnerConnections
{
	public static IEnumerable<BankPartnerConnection> Connections => new[]
	{
		new BankPartnerConnection
		{
			PartitionKey = "rtgs-global-id-1",
			RowKey = "alias-1",
			ConnectionId = "connection-id-1",
			CreatedAt = new DateTime(2000, 01, 01).ToUniversalTime(),
			ActivatedAt = new DateTime(2022, 01, 01).ToUniversalTime(),
			Status = "Active",
			Role = "Inviter"
		},
		new BankPartnerConnection
		{
			PartitionKey = "rtgs-global-id-1",
			RowKey = "alias-2",
			ConnectionId = "connection-id-2",
			CreatedAt = new DateTime(2000, 01, 01).ToUniversalTime(),
			ActivatedAt = new DateTime(2022, 01, 02).ToUniversalTime(),
			Status = "Active",
			Role = "Inviter"
		},
		new BankPartnerConnection
		{
			PartitionKey = "rtgs-global-id-1",
			RowKey = "alias-2",
			ConnectionId = "connection-id-3",
			CreatedAt = new DateTime(2000, 01, 01).ToUniversalTime(),
			Status = "Pending",
			Role = "Inviter"
		},
		new BankPartnerConnection
		{
			PartitionKey = "rtgs-global-id-2",
			RowKey = "alias-3",
			ConnectionId = "connection-id-4",
			CreatedAt = new DateTime(2000, 01, 01).ToUniversalTime(),
			ActivatedAt = new DateTime(2022, 01, 03).ToUniversalTime(),
			Status = "Active",
			Role = "Inviter"
		},
		new BankPartnerConnection
		{
			PartitionKey = "rtgs-global-id-2",
			RowKey = "alias-4",
			ConnectionId = "connection-id-5",
			CreatedAt = new DateTime(2000, 01, 01).ToUniversalTime(),
			ActivatedAt = new DateTime(2022, 01, 04).ToUniversalTime(),
			Status = "Active",
			Role = "Inviter"
		}
	};
}
