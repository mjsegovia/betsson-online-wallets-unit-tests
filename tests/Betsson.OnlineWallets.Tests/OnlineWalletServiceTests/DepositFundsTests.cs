using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Betsson.OnlineWallets.Tests.OnlineWalletServiceTests
{
    public class DepositFundsTests
    {
        private readonly Mock<IOnlineWalletRepository> _repositoryMock;
        private readonly IOnlineWalletService _service;

        public DepositFundsTests()
        {
            _repositoryMock = new Mock<IOnlineWalletRepository>();
            _service = new OnlineWalletService(_repositoryMock.Object);
        }

        [Fact]
        public async Task DepositFundsAsync_WhenDepositIsValid_ShouldIncreaseBalance()
        {
            //Arrange
            _repositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = 144m, Amount = 200m });

            _repositoryMock
                .Setup(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
                .Returns(Task.CompletedTask);

            var deposit = new Deposit { Amount = 100 };

            //Act
            var result = await _service.DepositFundsAsync(deposit);

            //Assert
            result.Should().NotBeNull();
            result.Amount.Should().BeGreaterThan(344);
            result.Amount.Should().Be(344 + deposit.Amount);
            result.Should().BeOfType<Balance>();

            _repositoryMock.Verify(repo => repo.InsertOnlineWalletEntryAsync(It.Is<OnlineWalletEntry>(x => x.Amount == 100 && x.BalanceBefore == 344m)), Times.Once);
        }

        [Fact]
        public async Task DepositFundsAsync_DepositAmountIsZero_ShouldReturnException()
        {
            //Arrange
            var initialBalance = new OnlineWalletEntry
            {
                BalanceBefore = 111,
                Amount = 222
            };

            _repositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(initialBalance);

            var deposit = new Deposit { Amount = 100 };

            //Act
            Func<Task> act = async () => await _service.DepositFundsAsync(deposit);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("lalalal")
                .WithMessage("Hello is not allowed at this moment");

            _repositoryMock.Verify(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()), Times.Never);

            initialBalance.BalanceBefore.Should().Be(111);
            initialBalance.Amount.Should().Be(222);
        }
    }
}
