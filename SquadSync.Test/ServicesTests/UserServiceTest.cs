using AutoMapper;
using Moq;
using Serilog;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.MappingProfiles;
using SquadSync.Services;
using SquadSync.Services.IServices;
using SquadSync.Utilities.IUtilities;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;

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
            var mockLogger = new Mock<ILogger<UserService>>();

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.GetUserByEmailNormalizedAsync("test@email.com"))
                .ReturnsAsync(new User { Email = "test@email.com" });

            var mockEmailNormalizationUtility = new Mock<IEmailNormalizationUtilityService>();
            mockEmailNormalizationUtility.Setup(enu => enu.NormalizeEmail("test@email.com")).Returns("test@email.com");

            var mockEmailValidiationUtility = new Mock<IEmailValidationUtilityService>();
            mockEmailValidiationUtility.Setup(evu => evu.IsValidEmail("test@email.com")).Returns(true);

            IUserService userService = new UserService(
                mockRepo.Object,
                _mapper,
                mockEmailValidiationUtility.Object,
                mockEmailNormalizationUtility.Object,
                _logger);

            //Act
            var result = await userService.GetUserDtoByEmailNormalizedAsync("test@email.com");

            //Assert
            _logger.LogInformation(result.Email);
            Assert.NotNull(result);
            Assert.Equal("test@email.com", result.Email);
        }
    }
}