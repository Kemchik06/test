using FluentAssertions;
using Moq;
using UserAuth.Application.Security;
using UserAuth.Application.Services;
using UserAuth.Application.Tests.Infrastructure;
using UserAuth.Domain.Entities;
using UserAuth.Domain.Models;

namespace UserAuth.Application.Tests.Services;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly FakePasswordHasher _passwordHasher;
    private readonly FakeUserRepository _userRepository;
    private readonly StubJwtProvider _jwtProvider;

    public UserServiceTests()
    {
        _userRepository = new FakeUserRepository();
        _passwordHasher = new FakePasswordHasher();
        _jwtProvider = new StubJwtProvider();
        _userService = new UserService(_passwordHasher, _userRepository, _jwtProvider);
    }

    [Fact]
    public async Task Register_Should_Create_User()
    {
        //Arrange
        var email = "kamil@gmail.com";
        var name = "Kamil";
        var password = "kamil123";
        
        //Act
        await _userService.Register(email, password, name, CancellationToken.None);
        
        //Assert
        var user =  (await _userRepository.SelectAsync(new SelectUserModel
        {
            Emails = new[] { email }
        }, CancellationToken.None)).FirstOrDefault();
        
        user.Should().NotBeNull();
        user.Email.Should().Be(email);
        user.Name.Should().Be(name);
    }

    [Fact]
    public async Task Register_Should_Call_PasswordHasher_Once()
    {
        //Arrange
        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns("hashedPassword");
        
        var service = new UserService(passwordHasher.Object, _userRepository, _jwtProvider);
        
        //Act
        await service.Register("email", "password", "kamil", CancellationToken.None);
        
        //Assert
        passwordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetById_Should_Return_User()
    {
        //Arrange
        var email = "kamil@gmail.com";
        var name = "Kamil";
        var password = "kamil123";
        
        await _userService.Register(email, password, name, CancellationToken.None);
        
        var model =  (await _userRepository.SelectAsync(new SelectUserModel
        {
            Emails = new[] { email }
        }, CancellationToken.None)).FirstOrDefault();

        var id = model!.Id;
        
        //Act
        var user = await _userService.GetById(id, CancellationToken.None);
        
        //Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(id);
    }

    [Fact]
    public async Task Login_Should_Return_Token()
    {
        //Arrange
        var email = "kamil@gmail.com";
        var name = "Kamil";
        var password = "kamil123";
        
        await _userService.Register(email, password, name, CancellationToken.None);
        
        //Act
        var token = await _userService.Login(email, password, CancellationToken.None);
        
        //Assert
        token.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_Should_Call_PasswordHasher_Once()
    {
        //Arrange
        var email = "kamil@gmail.com";
        var name = "Kamil";
        var password = "kamil123";
        
        await _userService.Register(email, password, name, CancellationToken.None);
        
        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
        
        var service = new UserService(passwordHasher.Object, _userRepository, _jwtProvider);
        
        //Act
        await service.Login(email, password, CancellationToken.None);
        
        //Assert
        passwordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Login_Wrong_Credentials_Should_Throw()
    {
        //Arrange
        var email = "kamil@gmail.com";
        var name = "Kamil";
        var password = "kamil123";
        
        await _userService.Register(email, password, name, CancellationToken.None);
        
        //Act
        var act = () => _userService.Login(email,"kamil1234", CancellationToken.None);
        
        //Assert
        await act.Should().ThrowAsync<Exception>();
    }
}