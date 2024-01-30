using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Principal;
using WebAPI.Contract;
using WebAPI.Controllers;
using WebAPI.Database;
using WebAPI.Exceptions;
using WebAPI.Service;

namespace WebAPIXUnit
{
    public class WebApiUnitTest
    {
        private readonly Mock<ISystemDB> _mockDB;
        private readonly RouletteService _service;
        public WebApiUnitTest() {
            _mockDB = new Mock<ISystemDB>();
            _service = new RouletteService(_mockDB.Object);
        }

        [Fact]
        public void Post_PlaceBet_Success()
        {
            //Arrage
            var controller = new RouletteControler(_service);
            var bet = new Bet() { Amount = 50, ChosenNumber = 3, SpinId = 1 };

            _mockDB.Setup(mock => mock.SaveBet(It.IsAny<Bet>()));

            //Act
            var result = controller.PostPlaceBet(bet);

            //Assert
            Assert.True(result is OkResult);

            var okResult = (OkResult)result;
            Assert.Equal(200, okResult.StatusCode);

            _mockDB.Verify(mock => mock.SaveBet(It.Is<Bet>(x => x.Amount == bet.Amount && x.ChosenNumber == bet.ChosenNumber)), Times.Once);
        }

        [Fact]
        public void Post_PlaceBet_BetOutofRange_Fails()
        {
            //Arrage
            var controller = new RouletteControler(_service);
            var bet = new Bet() { Amount = 50, ChosenNumber = 456, SpinId = 1 };

            _mockDB.Setup(mock => mock.SaveBet(It.IsAny<Bet>()));

            //Act
            var result = controller.PostPlaceBet(bet);

            //Assert
            Assert.True(result is BadRequestObjectResult);

            var badRequest = (BadRequestObjectResult)result;
            Assert.Equal(400, badRequest.StatusCode);
            Assert.Equal(badRequest.Value, new BetOutofRangeException(bet.ChosenNumber).Message);

            _mockDB.Verify(mock => mock.SaveBet(It.Is<Bet>(x => x.Amount == bet.Amount && x.ChosenNumber == bet.ChosenNumber)), Times.Never);
        }

        /*
          [Fact]
        public void Post_Deposit_Success()
        {
            //Arrange
            var controller = new BankControler(_service);
            var startingAmount = 4000;
            var IdAndDeposit = new IdAndAmount();
            var account = new SavingsAccount(1, startingAmount, "TestCustomer");
            IdAndDeposit.Id = account.Id;
            IdAndDeposit.Deposit = 2000;

            _mockDB
                .Setup(mock => mock.GetAccount(It.IsAny<long>())).Returns(account);

            //Act
            var result = controller.PostDeposit(IdAndDeposit);

            //Assert
            Assert.Equal(HttpStatusCode.Accepted, result.StatusCode);
            _mockDB.Verify(mock => mock.GetAccount(It.IsAny<long>()), Times.Once);
            Assert.Equal(startingAmount + IdAndDeposit.Deposit, account.Balance);
            _mockDB.Verify(mock => mock.UpdateAccount(It.Is<IAccount>(x => x.Balance == startingAmount + IdAndDeposit.Deposit)), Times.Once);  
        }
         */
    }
}