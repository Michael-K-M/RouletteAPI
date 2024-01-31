using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NuGet.Frameworks;
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
            var bet = new Bet(1,50,3);

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
            var bet = new Bet(1,456,50);

            //Act
            var result = controller.PostPlaceBet(bet);

            //Assert
            Assert.True(result is BadRequestObjectResult);

            var badRequest = (BadRequestObjectResult)result;
            Assert.Equal(400, badRequest.StatusCode);
            Assert.Equal(badRequest.Value, new BetOutofRangeException(bet.ChosenNumber).Message);

            _mockDB.Verify(mock => mock.SaveBet(It.IsAny<Bet>()), Times.Never);
        }

        [Fact]
        public void Post_PlaceBet_InsufficientFunds_Fails()
        {
            //Arrage
            var controller = new RouletteControler(_service);
            var bet = new Bet(1,0,25);

            //Act
            var result = controller.PostPlaceBet(bet);

            //Assert
            Assert.True(result is BadRequestObjectResult);

            var badRequest = (BadRequestObjectResult)result;
            Assert.Equal(400, badRequest.StatusCode);
            Assert.Equal(badRequest.Value, new InsufficientFunds(bet.Amount).Message);

            _mockDB.Verify(mock => mock.SaveBet(It.IsAny<Bet>()), Times.Never);
        }

        [Fact]
        public void Post_Get_Spin_Success()
        {
            //Arrage
            var controller = new RouletteControler(_service);
            var spinId = 6;
            _mockDB.Setup(mock => mock.GetBetsFromSpinId(It.IsAny<long>())).Returns( new List<Bet>() {new Bet(1,2,3) });
            _mockDB.Setup(mock => mock.SaveSpinNumber(It.IsAny<SpinResult>()));

            //Act
            var result = controller.GetSpin(spinId);

            //Assert
            Assert.True(result is OkObjectResult);

            var okResult = (OkObjectResult)result;
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(spinId, ((SpinResult)okResult.Value).SpinId);

            _mockDB.Verify(mock => mock.SaveSpinNumber(It.Is<SpinResult>(x => x.SpinId == spinId)), Times.Once);
            _mockDB.Verify(mock => mock.GetBetsFromSpinId(It.IsAny<long>()), Times.Once);

        }

        [Fact]
        public void Get_Spin_InvalidSpinException_Failed()
        {
            //Arrage
            var controller = new RouletteControler(_service);
            var spinId = 6;
            _mockDB.Setup(mock => mock.GetBetsFromSpinId(It.IsAny<long>())).Returns(new List<Bet>());

            //Act
            var result = controller.GetSpin(spinId);

            //Assert
            Assert.True(result is BadRequestObjectResult);

            var badRequest = (BadRequestObjectResult)result;
            Assert.Equal(400, badRequest.StatusCode);
            Assert.Equal(badRequest.Value, new InvalidSpinException(spinId).Message);

            _mockDB.Verify(mock => mock.SaveSpinNumber(It.IsAny<SpinResult>()), Times.Never);
            _mockDB.Verify(mock => mock.GetBetsFromSpinId(It.IsAny<long>()), Times.Once);

        }

        [Fact]
        public void Get_PreviousSpins_Success()
        {
            //Arrage
            var controller = new RouletteControler(_service);

               var spinResult1 = new SpinResult(1,15);
               var spinResult2 = new SpinResult(2,24);

            var spinResultList = new List<SpinResult>
            {
                spinResult1,
                spinResult2
            };

            _mockDB.Setup(mock => mock.GetAllSpins()).Returns(spinResultList);

            //Act
            var result = controller.GetPreviousSpins();

            //Assert
            Assert.True(result is OkObjectResult);

            var okResult = (OkObjectResult)result;
            Assert.Equal(200, okResult.StatusCode);
            var resultSpinList = (List<SpinResult>)okResult.Value;

            Assert.True(resultSpinList.Contains(spinResult1));
            Assert.True(resultSpinList.Contains(spinResult2));

            _mockDB.Verify(mock => mock.GetAllSpins(), Times.Once);
        }

        [Fact]
        public void Get_Payout_Success()
        {
            //Arrage
            var controller = new RouletteControler(_service);

            var betResult1 = new Bet(1,500,24);
            var betResult2 = new Bet(2,500, 19);
            var betResult3 = new Bet(3, 500, 17);

            var betResultList = new List<Bet>
            {
                betResult1,
                betResult2,
                betResult3
            };

            var spinResult1 = new SpinResult(betResult1.SpinId, betResult1.ChosenNumber);
            var spinResult2 = new SpinResult(betResult2.SpinId, 8);
            var spinResult3 = new SpinResult(betResult3.SpinId, betResult3.ChosenNumber);
            var spinResultList = new List<SpinResult>
            {
                spinResult1,
                spinResult2,
                spinResult3
            };

            _mockDB.Setup(mock => mock.GetBetsFromUser(It.IsAny<long>())).Returns(betResultList);
            _mockDB.Setup(mock => mock.GetSpinsFromSpinIds(It.IsAny<List<long>>())).Returns(spinResultList);

            //Act
            var result = controller.GetPayout();

            //Assert
            Assert.True(result is OkObjectResult);

            var okResult = (OkObjectResult)result;
            Assert.Equal(200, okResult.StatusCode);
            var expectedWinnings = betResult1.Amount * 35 + betResult3.Amount * 35;
            Assert.Equal(expectedWinnings, ((Payout)okResult.Value).Amount);

            _mockDB.Verify(mock => mock.GetBetsFromUser(It.IsAny<long>()), Times.Once);
            _mockDB.Verify(mock => mock.GetSpinsFromSpinIds(It.IsAny<List<long>>()), Times.Once);
        }
    }
}