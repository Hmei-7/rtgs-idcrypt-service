﻿using Azure;
using Azure.Data.Tables;

namespace RTGS.IDCrypt.Service.Models;

public record PendingBankPartnerConnection : ITableEntity
{
	public string PartitionKey { get; set; }
	public string RowKey { get; set; }
	public string Alias { get; set; }
	public string ConnectionId { get; set; }
	public string PublicDid { get; set; }
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }
}
