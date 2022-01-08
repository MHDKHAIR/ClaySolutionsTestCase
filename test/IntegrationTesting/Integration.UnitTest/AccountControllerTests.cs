using Application.DataTransfareObjects.Requests;
using Application.DataTransfareObjects.Responses;
using Xunit;
using Application.Common.Results;
using EmployeesApp.IntegrationTests;
using LocksAPI;

namespace Integration.UnitTest
{
    public class AccountControllerTests : BaseTest,IClassFixture<WebAppFactory<Startup>>
    {
        readonly string route = "api/Account";

        [Fact]
        public async void AllInOnGuestUserTest()
        {
            // Arrenge
            var registerUserDto = new RegisterUserRequestDto
            {
                UserEmail = "mohammadfkl@outlook.com",
                FirstName = "khair",
                LastName = "khair",
                Password = "Test2022",
                ConfirmPassword = "Test2022",
                UserType = Domain.Enums.UserTypeEnum.Guest
            };
            // Act
            var registeredResponse = await PostAsync<IResult<RegisterUserResponseDto>>($"{route}/registration", registerUserDto);
            // Assert
            Assert.True(registeredResponse.Succeeded);

            // Arrenge 
            var signInDto = new SignInRequestDto
            {
                UserEmail = registerUserDto.UserEmail,
                Password = registerUserDto.Password,
            };
            // Act 
            var signInResponse = await PostAsync<IResult<SignInResponseDto>>($"{route}/signing_in", signInDto);
            // Assert
            Assert.True(signInResponse.Succeeded);
            Assert.NotEmpty(signInResponse.Data.JwtToken);

            // Arrenge
            var token = signInResponse.Data.JwtToken;
            // Act
            var deleteResponse = await DeleteAsync<IResult<string>>($"{route}/delete", token);
            // Assert
            Assert.True(deleteResponse.Succeeded);
            Assert.Equal("Account id deleted", deleteResponse.Data);
        }
    }
}