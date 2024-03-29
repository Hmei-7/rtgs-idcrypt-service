﻿using FluentAssertions;
using RTGS.IDCrypt.Service.ConnectionCycleScheduler.IntegrationTests.Fixtures;

namespace RTGS.IDCrypt.Service.ConnectionCycleScheduler.IntegrationTests.BankConnectionCycleServiceTests.GivenSchedulerRan;

public class WhenIdCryptServiceAddressNotSet
{
	[Fact]
	public async Task ThenBaseAddressIsNotSet()
	{
		Environment.SetEnvironmentVariable("IdCryptServiceAddress", string.Empty);

		var exitCode = await TestFixture.RunProgramAsync();
		exitCode.Should().Be(1, "exit code should be 1, if not there is an exception missing");
	}
}
