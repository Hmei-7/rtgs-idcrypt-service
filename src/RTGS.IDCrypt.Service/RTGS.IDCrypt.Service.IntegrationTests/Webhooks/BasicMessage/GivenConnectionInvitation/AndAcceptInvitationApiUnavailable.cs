﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using RTGS.IDCrypt.Service.IntegrationTests.Fixtures.Connection;
using RTGS.IDCrypt.Service.Models;
using RTGS.IDCrypt.Service.Webhooks.Models;
using RTGS.IDCryptSDK.BasicMessage.Models;

namespace RTGS.IDCrypt.Service.IntegrationTests.Webhooks.BasicMessage.GivenConnectionInvitation;

public class AndAcceptInvitationApiUnavailable : IClassFixture<AcceptInvitationEndpointUnavailableFixture>, IAsyncLifetime
{
	private readonly HttpClient _client;

	private HttpResponseMessage _httpResponse;

	public AndAcceptInvitationApiUnavailable(AcceptInvitationEndpointUnavailableFixture testFixture)
	{
		testFixture.IdCryptStatusCodeHttpHandler.Reset();

		_client = testFixture.CreateClient();
	}

	public async Task InitializeAsync()
	{
		var connectionInvitation = new ConnectionInvitation
		{
			Alias = "alias",
			ImageUrl = "image-url",
			InvitationUrl = "invitation-url",
			Did = "did",
			Label = "label",
			RecipientKeys = new[] { "recipient-key-1" },
			ServiceEndpoint = "service-endpoint",
			Id = "id",
			PublicDid = "public-did",
			Type = "type",
			FromRtgsGlobalId = "rtgs-global-id"
		};

		var basicMessage = new IdCryptBasicMessage
		{
			ConnectionId = "connection_id",
			Content = JsonSerializer.Serialize(new BasicMessageContent<ConnectionInvitation>
			{
				MessageType = nameof(ConnectionInvitation),
				MessageContent = connectionInvitation
			})
		};

		_httpResponse = await _client.PostAsJsonAsync("/v1/idcrypt/topic/basicmessages", basicMessage);
	}

	public Task DisposeAsync() =>
		Task.CompletedTask;

	[Fact]
	public async Task ThenReturnInternalServerError()
	{
		using var _ = new AssertionScope();

		_httpResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

		var content = await _httpResponse.Content.ReadAsStringAsync();

		content.Should().Be("{\"error\":\"Error accepting invitation\"}");
	}
}
