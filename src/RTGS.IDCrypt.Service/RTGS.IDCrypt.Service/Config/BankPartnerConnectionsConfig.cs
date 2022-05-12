﻿namespace RTGS.IDCrypt.Service.Config;

public record BankPartnerConnectionsConfig
{
	public string BankPartnerConnectionsTableName { get; init; }

	public TimeSpan MinimumConnectionAge { get; init; }
}
