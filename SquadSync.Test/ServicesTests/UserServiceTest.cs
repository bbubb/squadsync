using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.MappingProfiles;
using SquadSync.Services;
using SquadSync.Services.IServices;
using SquadSync.Utilities.IUtilities;
using Xunit.Abstractions;

namespace SquadSync.Test.ServicesTests
{
    public class UserServiceTest
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserServiceTest(ITestOutputHelper outputHelper)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserMappingProfile>();
            });

            _mapper = config.CreateMapper();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TestOutput(outputHelper)
                .CreateLogger();

            var loggerFactory = new LoggerFactory().AddSerilog();
            _logger = loggerFactory.CreateLogger<UserService>();
        }

        [Fact]
        public async Task GetUserByEmail_ReturnExpectedUser()
        {
            //Arrange
            var expectedEmail = "test@email.com";

            var mockLogger = new Mock<ILogger<UserService>>();

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.GetUserByEmailNormalizedAsync(expectedEmail))
                .ReturnsAsync(new User { Email = expectedEmail });

            var mockEmailNormalizationUtility = new Mock<IEmailNormalizationService>();
            mockEmailNormalizationUtility.Setup(enu => enu.NormalizeEmail(expectedEmail)).Returns(expectedEmail);

            var mockEmailValidiationUtility = new Mock<IEmailValidationService>();
            mockEmailValidiationUtility.Setup(evu => evu.IsValidEmail(expectedEmail)).Returns(true);

            IUserService userService = new UserService(
                mockRepo.Object,
                _mapper,
                mockEmailValidiationUtility.Object,
                mockEmailNormalizationUtility.Object,
                _logger);

            //Act
            var serviceResult = await userService.GetUserDtoByEmailNormalizedAsync(expectedEmail);

            //Assert
            Assert.True(serviceResult.Success);
            Assert.NotNull(serviceResult.Data);

            var result = serviceResult.Data;

            Assert.Equal(expectedEmail, result.Email);
            _logger.LogInformation($"{expectedEmail} | {result.Email}");
        }
    }
}