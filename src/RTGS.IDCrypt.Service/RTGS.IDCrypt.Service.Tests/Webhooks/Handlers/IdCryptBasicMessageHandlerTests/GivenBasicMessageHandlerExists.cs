﻿using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using RTGS.IDCrypt.Service.Tests.Logging;
using RTGS.IDCrypt.Service.Webhooks.Handlers;
using RTGS.IDCrypt.Service.Webhooks.Handlers.BasicMessage;
using RTGS.IDCrypt.Service.Webhooks.Models;
using RTGS.IDCryptSDK.BasicMessage.Models;

namespace RTGS.IDCrypt.Service.Tests.Webhooks.Handlers.IdCryptBasicMessageHandlerTests;

public class GivenBasicMessageHandlerExists : IAsyncLifetime
{
	private FakeLogger<IdCryptBasicMessageHandler> _logger;
	private Mock<IBasicMessageHandler> _mockBasicMessageHandler;

	public async Task InitializeAsync()
	{
		_logger = new FakeLogger<IdCryptBasicMessageHandler>();
		const string connectionId = "connection-id";
		var basicMessage = new IdCryptBasicMessage
		{
			ConnectionId = connectionId,
			Content = JsonSerializer.Serialize(new BasicMessageContent<string>
			{
				MessageType = "message-type",
				MessageContent = "hello"
			})
		};

		_mockBasicMessageHandler = new Mock<IBasicMessageHandler>();
		_mockBasicMessageHandler.SetupGet(handler => handler.MessageType).Returns("message-type");
		_mockBasicMessageHandler.Setup(handler =>
			handler.HandleAsync(It.Is<string>(value => value == basicMessage.Content), connectionId, It.IsAny<CancellationToken>()))
			.Verifiable();

		var handler = new IdCryptBasicMessageHandler(_logger, new[] { _mockBasicMessageHandler.Object });

		var message = JsonSerializer.Serialize(basicMessage);

		await handler.HandleAsync(message, default);
	}

	public Task DisposeAsync() => Task.CompletedTask;

	[Fact]
	public void ThenLogsExpected() =>
		_logger.Logs[LogLevel.Information].Should().BeEquivalentTo(new[]
		{
			"Received message-type BasicMessage from ConnectionId connection-id",
			"Handled message-type BasicMessage"
		}, options => options.WithStrictOrdering());

	[Fact]
	public void ThenCallsHandleAsync() => _mockBasicMessageHandler.Verify();
}
