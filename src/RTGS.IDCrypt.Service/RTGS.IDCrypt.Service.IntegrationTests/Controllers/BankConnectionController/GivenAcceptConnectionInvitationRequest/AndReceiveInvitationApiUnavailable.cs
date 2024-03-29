﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using RTGS.IDCrypt.Service.Contracts.Connection;
using RTGS.IDCrypt.Service.IntegrationTests.Fixtures.Connection;

namespace RTGS.IDCrypt.Service.IntegrationTests.Controllers.BankConnectionController.GivenAcceptConnectionInvitationRequest;

public class AndReceiveInvitationApiUnavailable : IClassFixture<ReceiveInvitationEndpointUnavailableFixture>, IAsyncLifetime
{
	private readonly HttpClient _client;

	private HttpResponseMessage _httpResponse;

	public AndReceiveInvitationApiUnavailable(ReceiveInvitationEndpointUnavailableFixture testFixture)
	{
		testFixture.IdCryptStatusCodeHttpHandler.Reset();

		_client = testFixture.CreateClient();
	}

	public async Task InitializeAsync()
	{
		var request = new AcceptConnectionInvitationRequest
		{
			Id = "id",
			Type = "type",
			Alias = "alias",
			Label = "label",
			RecipientKeys = new[] { "recipient-key" },
			ServiceEndpoint = "service-endpoint",
			AgentPublicDid = "agent-public-did"
		};

		_httpResponse = await _client.PostAsJsonAsync("api/bank-connection/accept", request);
	}

	public Task DisposeAsync() =>
		Task.CompletedTask;

	[Fact]
	public async Task ThenReturnInternalServerError()
	{
		using var _ = new AssertionScope();

		_httpResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

		var content = await _httpResponse.Content.ReadAsStringAsync();

		content.Should().Be("{\"error\":\"Error receiving invitation\"}");
	}
}
