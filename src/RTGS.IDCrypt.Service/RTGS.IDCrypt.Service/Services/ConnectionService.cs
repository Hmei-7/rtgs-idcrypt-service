﻿using Microsoft.Extensions.Options;
using RTGS.IDCrypt.Service.Config;
using RTGS.IDCrypt.Service.Extensions;
using RTGS.IDCrypt.Service.Helpers;
using RTGS.IDCrypt.Service.Models;
using RTGS.IDCrypt.Service.Repositories;
using RTGS.IDCryptSDK.Connections;
using RTGS.IDCryptSDK.Connections.Models;
using RTGS.IDCryptSDK.Wallet;
using ConnectionInvitation = RTGS.IDCrypt.Service.Models.ConnectionInvitation;

namespace RTGS.IDCrypt.Service.Services;

public class ConnectionService : IConnectionService
{
	private readonly IConnectionsClient _connectionsClient;
	private readonly ILogger<ConnectionService> _logger;
	private readonly IConnectionRepository _connectionRepository;
	private readonly IAliasProvider _aliasProvider;
	private readonly IWalletClient _walletClient;
	private readonly string _rtgsGlobalId;

	public ConnectionService(
		IConnectionsClient connectionsClient,
		ILogger<ConnectionService> logger,
		IConnectionRepository connectionRepository,
		IAliasProvider aliasProvider,
		IWalletClient walletClient,
		IOptions<CoreConfig> coreOptions)
	{
		_connectionsClient = connectionsClient;
		_logger = logger;
		_connectionRepository = connectionRepository;
		_aliasProvider = aliasProvider;
		_walletClient = walletClient;

		if (string.IsNullOrWhiteSpace(coreOptions.Value.RtgsGlobalId))
		{
			throw new ArgumentException("RtgsGlobalId configuration option is not set.");
		}

		_rtgsGlobalId = coreOptions.Value.RtgsGlobalId;
	}

	public async Task AcceptInvitationAsync(ConnectionInvitation invitation, CancellationToken cancellationToken = default)
	{
		try
		{
			var receiveAndAcceptInvitationRequest = new ReceiveAndAcceptInvitationRequest
			{
				Alias = invitation.Alias,
				Id = invitation.Id,
				Label = invitation.Label,
				RecipientKeys = invitation.RecipientKeys,
				ServiceEndpoint = invitation.ServiceEndpoint,
				Type = invitation.Type
			};

			var response = await _connectionsClient.ReceiveAndAcceptInvitationAsync(receiveAndAcceptInvitationRequest, cancellationToken);

			var connection = new BankPartnerConnection
			{
				PartitionKey = invitation.FromRtgsGlobalId,
				RowKey = response.Alias,
				ConnectionId = response.ConnectionId,
				Alias = response.Alias,
				PublicDid = invitation.PublicDid,
				Status = BankPartnerConnectionStatuses.Pending
			};

			await _connectionRepository.SaveBankPartnerConnectionAsync(connection, cancellationToken);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred when accepting invitation");

			throw;
		}
	}

	public async Task<ConnectionInvitation> CreateConnectionInvitationForBankAsync(
		string toRtgsGlobalId,
		CancellationToken cancellationToken = default)
	{
		var alias = _aliasProvider.Provide();

		try
		{
			var createConnectionInvitationResponse = await CreateConnectionInvitationAsync(alias, cancellationToken);

			var publicDid = await _walletClient.GetPublicDidAsync(cancellationToken);

			var connection = new BankPartnerConnection
			{
				PartitionKey = toRtgsGlobalId,
				RowKey = alias,
				Alias = alias,
				ConnectionId = createConnectionInvitationResponse.ConnectionId,
				Status = BankPartnerConnectionStatuses.Pending,
				PublicDid = publicDid
			};

			await _connectionRepository.SaveBankPartnerConnectionAsync(connection, cancellationToken);

			var connectionInvitation = createConnectionInvitationResponse.MapToConnectionInvitation(publicDid, _rtgsGlobalId);

			return connectionInvitation;
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Error occurred when creating connection invitation for bank {RtgsGlobalId}",
				toRtgsGlobalId);

			throw;
		}
	}

	public async Task<ConnectionInvitation> CreateConnectionInvitationForRtgsAsync(
		CancellationToken cancellationToken = default)
	{
		var alias = _aliasProvider.Provide();

		try
		{
			var createConnectionInvitationResponse = await CreateConnectionInvitationAsync(alias, cancellationToken);

			var publicDid = await _walletClient.GetPublicDidAsync(cancellationToken);

			var connectionInvitation = createConnectionInvitationResponse.MapToConnectionInvitation(publicDid, _rtgsGlobalId);

			return connectionInvitation;
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Error occurred when creating connection invitation for RTGS.global");

			throw;
		}
	}

	private async Task<CreateConnectionInvitationResponse> CreateConnectionInvitationAsync(string alias, CancellationToken cancellationToken)
	{
		const bool autoAccept = true;
		const bool multiUse = false;
		const bool usePublicDid = false;

		var createConnectionInvitationResponse = await _connectionsClient.CreateConnectionInvitationAsync(
				alias,
				autoAccept,
				multiUse,
				usePublicDid,
				cancellationToken);

		return createConnectionInvitationResponse;
	}
}