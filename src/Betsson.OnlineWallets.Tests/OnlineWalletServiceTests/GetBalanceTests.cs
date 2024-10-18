using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Betsson.OnlineWallets.Tests.OnlineWalletServiceTests
{
    public class GetBalanceTests
    {
        private readonly Mock<IOnlineWalletRepository> _repositoryMock;
        private readonly IOnlineWalletService _service;

        public GetBalanceTests()
        {
            _repositoryMock = new Mock<IOnlineWalletRepository>();
            _service = new OnlineWalletService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetBalanceAsync_WithValidEntries_ShouldRetrieveValidBalance()
        {
            //Arrange
            var wallet = new OnlineWalletEntry
            {
                Amount = 100.5m,
                BalanceBefore = 300.75m
            };

            _repositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(wallet);

            //Act
            IOnlineWalletService service = new OnlineWalletService(_repositoryMock.Object);
            var balance = await service.GetBalanceAsync();

            //Assert
            balance.Should().NotBeNull();
            balance.Should().Be(400.8m);
        }

        [Fact]
        public async Task GetBalanceAsync_WhenNoTransactions_ShouldReturnZero()
        {
            //Arrange           
            _repositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync((OnlineWalletEntry)null);

            IOnlineWalletService service = new OnlineWalletService(_repositoryMock.Object);

            //Act            
            var balance = await service.GetBalanceAsync();

            //Assert
            balance.Amount.Should().Be(0);
        }
    }
}
