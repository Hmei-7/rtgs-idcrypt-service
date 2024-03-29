﻿using Moq;
using RTGS.IDCrypt.Service.Contracts.Connection;
using RTGS.IDCrypt.Service.Controllers;
using RTGS.IDCrypt.Service.Models.ConnectionInvitations;
using RTGS.IDCrypt.Service.Repositories;
using RTGS.IDCrypt.Service.Services;

namespace RTGS.IDCrypt.Service.Tests.Controllers.BankConnectionControllerTests.GivenAcceptConnectionInvitationRequest;

public class AndConnectionServiceUnavailable
{
	private readonly BankConnectionController _bankConnectionController;

	public AndConnectionServiceUnavailable()
	{
		var bankConnectionServiceMock = new Mock<IBankConnectionService>();

		bankConnectionServiceMock
			.Setup(service => service.AcceptInvitationAsync(
				It.IsAny<BankConnectionInvitation>(),
				It.IsAny<CancellationToken>()))
			.Throws<Exception>();

		_bankConnectionController = new BankConnectionController(
			bankConnectionServiceMock.Object,
			Mock.Of<IBankPartnerConnectionRepository>());
	}

	[Fact]
	public async Task WhenPosting_ThenThrows()
	{
		using var _ = new AssertionScope();

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

		await FluentActions
			.Awaiting(() => _bankConnectionController.Accept(request))
			.Should()
			.ThrowAsync<Exception>();
	}
}
