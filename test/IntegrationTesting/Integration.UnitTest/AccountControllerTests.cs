using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Application.DataTransfareObjects.Requests;
using Application.Services.Interfaces;
using Moq;
using Xunit;
using LocksAPI;
using System.Net;
using Application.DataTransfareObjects.Responses;
using System;
using Application.Common.Results;
using System.Net.Http.Headers;

namespace Integration.UnitTest
{
    public class AccountControllerTests : IAsyncLifetime
    {
        private readonly Mock<IUserService> _userServiceMock = new();

        private HttpClient _httpClient = null!;
        public async Task InitializeAsync()
        {
            var hostBuilder = Program.CreateHostBuilder(new string[0])
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseTestServer();
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton(_userServiceMock.Object);
                });

            var host = await hostBuilder.StartAsync();
            _httpClient = host.GetTestClient();
        }
        [Fact]
        public async void AllInOneGuestUser()
        {
            var registerUserDto = new RegisterUserRequestDto
            {
                UserEmail = "mohammadfkl@outlook.com",
                FirstName = "khair",
                LastName = "khair",
                Password = "Test2022",
                ConfirmPassword = "Test2022",
                UserType = Domain.Enums.UserTypeEnum.Guest
                
            };
            await AllInOnRegisterSignInDeleteUserTest(registerUserDto);
        }
        [Fact]
        public async void AllInOneEmployeeUser()
        {
            var registerUserDto = new RegisterUserRequestDto
            {
                UserEmail = "mohammadfkl@outlook.com",
                FirstName = "khair1",
                LastName = "khair1",
                Password = "Test20221",
                ConfirmPassword = "Test20221",
                UserType = Domain.Enums.UserTypeEnum.Employee

            };
            await AllInOnRegisterSignInDeleteUserTest(registerUserDto);
        }
        private async Task AllInOnRegisterSignInDeleteUserTest(RegisterUserRequestDto registerUserDto)
        {
            // Arrenge
            var registerResponseDto = new RegisterUserResponseDto
            {
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                UserEmail = registerUserDto.UserEmail,
                UserType = registerUserDto.UserType,
                Message = "Thank you for joining" + Environment.NewLine + "Please confrim your email"
            };
            _userServiceMock.Setup(s => s.RegisterUserAsync(registerUserDto))
               .ReturnsAsync(registerResponseDto);
            var myContent = JsonConvert.SerializeObject(registerResponseDto);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // Act
            var registrationResponse = await _httpClient.PostAsync($"api/account/registration", byteContent);
            registrationResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, registrationResponse.StatusCode);
            var returnedJson = await registrationResponse.Content.ReadAsStringAsync();
            var returnedUser = JsonConvert
                .DeserializeObject<Result<RegisterUserResponseDto>>(returnedJson);
            Assert.False(returnedUser.Succeeded);

            // Arrenge 
            var signInDto = new SignInRequestDto
            {
                UserEmail = registerUserDto.UserEmail,
                Password = registerUserDto.Password,
            };
            var signInResponse = new SignInResponseDto
            {
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Username = registerResponseDto.UserEmail,
                IsEmailConfirmed = false,
                JwtToken = "12345"
            };
            _userServiceMock.Setup(s => s.AuthenticateAsync(signInDto))
                .ReturnsAsync(signInResponse);
            myContent = JsonConvert.SerializeObject(signInResponse);
            buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // Act
            var signinResponse = await _httpClient.PostAsync($"api/account/signing_in", byteContent);
            signinResponse.EnsureSuccessStatusCode();
            // Assert
            Assert.Equal(HttpStatusCode.Created, signinResponse.StatusCode);
            returnedJson = await signinResponse.Content.ReadAsStringAsync();
            var returnedSignIn = JsonConvert
                .DeserializeObject<Result<SignInResponseDto>>(returnedJson);
            Assert.False(returnedSignIn.Succeeded);

            // Arrenge
            var token = signInResponse.JwtToken;
            _userServiceMock.Setup(s => s.DeleteAccountAsync(string.Empty))
                .Returns(Task.CompletedTask);
            // Act
            var deleteResponse = await _httpClient.DeleteAsync($"api/account/delete");
            deleteResponse.EnsureSuccessStatusCode();
            // Assert
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            returnedJson = await deleteResponse.Content.ReadAsStringAsync();
            var returnedDelete = JsonConvert
                .DeserializeObject<Result<string>>(returnedJson);
            Assert.False(returnedDelete.Succeeded);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

    }
}