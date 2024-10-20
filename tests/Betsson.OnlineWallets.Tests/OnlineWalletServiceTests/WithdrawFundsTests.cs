﻿using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Exceptions;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Betsson.OnlineWallets.Tests.OnlineWalletServiceTests
{
    public class WithdrawFundsTests
    {
        private readonly Mock<IOnlineWalletRepository> _repositoryMock;
        private readonly IOnlineWalletService _service;

        public WithdrawFundsTests()
        {
            _repositoryMock = new Mock<IOnlineWalletRepository>();
            _service = new OnlineWalletService(_repositoryMock.Object);
        }

        [Fact]
        public async Task WithdrawFundsAsync_WithValidEntries_ShouldDecreaseBalance()
        {
            //Arrange
            _repositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = 300m, Amount = 255.5m });



            var withdrawal = new Withdrawal { Amount = 100m };

            //Act
            var result = await _service.WithdrawFundsAsync(withdrawal);

            //Assert
            result.Should().NotBeNull();
            result.Amount.Should().BeLessThan(555.5m);
            result.Amount.Should().Be(555.5m - withdrawal.Amount);
            result.Should().BeOfType<Balance>();

            _repositoryMock.Verify(repo => repo.InsertOnlineWalletEntryAsync(It.Is<OnlineWalletEntry>(b => b.Amount == -100m && b.BalanceBefore==555.5m)), Times.Once);            
        }

        [Fact]
        public async Task WithdrawFundsTest_InsufficientFunds_ShouldReturnInsufficientBalanceException()
        {
            //Arrange
            _repositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = 100, Amount = 50 });

            var withdrawal = new Withdrawal { Amount = 200 };

            //Act
            Func<Task> act = async () => await _service.WithdrawFundsAsync(withdrawal);

            //Assert
            await act.Should()
                .ThrowAsync<InsufficientBalanceException>()
                .WithMessage("Invalid withdrawal amount. There are insufficient funds.");

            _repositoryMock.Verify(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()), Times.Never);
            _repositoryMock.Verify(repo => repo.GetLastOnlineWalletEntryAsync(), Times.Once);
            withdrawal.Amount.Should().Be(200);
        }

        [Fact]
        public async Task WithdrawFunds_ZeroAmount_ShouldInsertTransaction()
        {
            //Arrange
            _repositoryMock
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = 100, Amount = 50 });

            var withdrawal = new Withdrawal { Amount = 0 };

            //Act
            var result = await _service.WithdrawFundsAsync(withdrawal);

            //Assert
            _repositoryMock.Verify(repo => repo.InsertOnlineWalletEntryAsync(It.Is<OnlineWalletEntry>(b => b.Amount == 0m && b.BalanceBefore == 150m)), Times.Once);
            result.Amount.Should().Be(150m);
        }
    }    
}
