using AutoMapper;
using Moq;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.MappingProfiles;
using SquadSync.Services;
using SquadSync.Services.IServices;
using SquadSync.Utilities.IUtilities;

namespace SquadSync.Test.ServicesTests
{
    public class UserServiceTest
    {
        private readonly IMapper _mapper;

        public UserServiceTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserMappingProfile>();

            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async void GetUserByEmail_ReturnExpectedUser()
        {
            //Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(repo => repo.GetUserByEmailNormalizedAsync("test@email.com"))
                .ReturnsAsync(new UserModel { Email = "test@email.com" });

            var mockEmailNormalizationUtility = new Mock<IEmailNormalizationUtilityService>();
            mockEmailNormalizationUtility.Setup(enu => enu.NormalizeEmail("test@email.com")).Returns("test@email.com");

            var mockEmailValidiationUtility = new Mock<IEmailValidationUtilityService>();
            mockEmailValidiationUtility.Setup(evu => evu.IsValidEmail("test@email.com")).Returns(true);

            IUserService userService = new UserService(
                mockRepo.Object,
                _mapper,
                mockEmailValidiationUtility.Object,
                mockEmailNormalizationUtility.Object);

            //Act
            var result = await userService.GetUserDtoByEmailNormalizedAsync("test@email.com");

            //Assert
            Assert.NotNull(result);
            Assert.Equal("test@email.com", result.Email);
        }
    }
}