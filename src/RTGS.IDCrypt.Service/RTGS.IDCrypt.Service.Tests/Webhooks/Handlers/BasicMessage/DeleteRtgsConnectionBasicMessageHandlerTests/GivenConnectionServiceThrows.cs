﻿using System.Text.Json;
using Moq;
using RTGS.IDCrypt.Service.Services;
using RTGS.IDCrypt.Service.Webhooks.Handlers.BasicMessage;
using RTGS.IDCrypt.Service.Webhooks.Models.BasicMessageModels;
using RTGS.IDCryptSDK.BasicMessage.Models;

namespace RTGS.IDCrypt.Service.Tests.Webhooks.Handlers.BasicMessage.DeleteRtgsConnectionBasicMessageHandlerTests;

public class GivenConnectionServiceThrows
{
	[Fact]
	public async Task ThenThrows()
	{
		var connectionServiceMock = new Mock<IConnectionService>();

		connectionServiceMock
			.Setup(service => service.DeleteRtgsAsync(
				It.IsAny<string>(),
				It.IsAny<CancellationToken>()))
			.Throws<Exception>();

		var handler = new DeleteRtgsConnectionBasicMessageHandler(connectionServiceMock.Object);

		var message = JsonSerializer.Serialize(new BasicMessageContent<DeleteRtgsConnectionBasicMessage>());

		await FluentActions
			.Awaiting(() => handler.HandleAsync(message, "connection-id"))
			.Should()
			.ThrowAsync<Exception>();
	}
}