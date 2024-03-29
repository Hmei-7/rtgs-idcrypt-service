﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RTGS.IDCrypt.Service.Config;
using RTGS.IDCrypt.Service.Helpers;
using RTGS.IDCrypt.Service.Repositories;
using RTGS.IDCrypt.Service.Services;
using RTGS.IDCryptSDK.Connections;
using RTGS.IDCryptSDK.Wallet;

namespace RTGS.IDCrypt.Service.Tests.Services.RtgsConnectionServiceTests;

public class GivenConnectionService
{
	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void WhenRtgsGlobalIdIsEmpty_ThenThrow(string rtgsGlobalId) =>
		FluentActions.Invoking(() =>
			new RtgsConnectionService(
				Mock.Of<IConnectionsClient>(),
				Mock.Of<ILogger<RtgsConnectionService>>(),
				Mock.Of<IRtgsConnectionRepository>(),
				Mock.Of<IAliasProvider>(),
				Mock.Of<IWalletClient>(),
				Options.Create(new CoreConfig
				{
					RtgsGlobalId = rtgsGlobalId
				})))
			.Should().Throw<ArgumentException>()
				.Which.Message.Should().Be("RtgsGlobalId configuration option is not set.");
}
