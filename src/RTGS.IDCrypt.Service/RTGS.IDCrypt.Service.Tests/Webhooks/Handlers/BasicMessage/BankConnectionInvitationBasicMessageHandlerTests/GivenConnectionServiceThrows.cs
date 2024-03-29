﻿using System.Text.Json;
using Moq;
using RTGS.IDCrypt.Service.Models.ConnectionInvitations;
using RTGS.IDCrypt.Service.Services;
using RTGS.IDCrypt.Service.Webhooks.Handlers.BasicMessage;

namespace RTGS.IDCrypt.Service.Tests.Webhooks.Handlers.BasicMessage.BankConnectionInvitationBasicMessageHandlerTests;

public class GivenConnectionServiceThrows
{
	[Fact]
	public async Task ThenThrows()
	{
		var bankConnectionServiceMock = new Mock<IBankConnectionService>();

		bankConnectionServiceMock
			.Setup(service => service.AcceptInvitationAsync(
				It.IsAny<BankConnectionInvitation>(),
				It.IsAny<CancellationToken>()))
			.Throws<Exception>();

		var handler = new BankConnectionInvitationBasicMessageHandler(bankConnectionServiceMock.Object);

		var message = JsonSerializer.Serialize(new BankConnectionInvitation());

		await FluentActions
			.Awaiting(() => handler.HandleAsync(message, "connection-id"))
			.Should()
			.ThrowAsync<Exception>();
	}
}
