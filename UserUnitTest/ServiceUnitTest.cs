using Moq;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Model;
using UserService.Repository;

namespace UserUnitTest
{
    public class ServiceUnitTest
    {
        private Mock<IUserRepo> _userRepoMock;
        private UserService.Service.UserService _userService;

        public ServiceUnitTest()
        {
            _userRepoMock = new Mock<IUserRepo>();
            _userService = new UserService.Service.UserService(_userRepoMock.Object);
        }

        [Fact]
        public void GetAllUser_ShouldCall_GetAllUsers_Method_On_UserRepo()
        {
            // Arrange
            var expectedUsers = new List<User>
        {
            new User { Id = 1, Name = "John" },
            new User { Id = 2, Name = "Jane" }
        };

            _userRepoMock.Setup(x => x.GetAllUsers()).Returns(expectedUsers);

            // Act
            var result = _userService.getAllUser();

            // Assert
            _userRepoMock.Verify(x => x.GetAllUsers(), Moq.Times.Once);
            Assert.Equal(expectedUsers, result);
        }

        [Fact]
        public void GetUserById_ShouldCall_GetById_Method_On_UserRepo_With_Correct_Id()
        {
            // Arrange
            var expectedUser = new User { Id = 1, Name = "John" };
            var userId = 1;

            _userRepoMock.Setup(x => x.GetById(userId)).Returns(expectedUser);

            // Act
            var result = _userService.getUserById(userId);

            // Assert
            _userRepoMock.Verify(x => x.GetById(userId), Moq.Times.Once);
            Assert.Equal(expectedUser, result);
        }

        [Fact]
        public void AddUser_ShouldCall_AddUser_And_SaveChanges_Methods_On_UserRepo()
        {
            // Arrange
            var userToAdd = new User { Id = 1, Name = "John" };

            // Act
            _userService.addUser(userToAdd);

            // Assert
            _userRepoMock.Verify(x => x.AddUser(userToAdd), Moq.Times.Once);
            _userRepoMock.Verify(x => x.SaveChanges(), Moq.Times.Once);
        }

        [Fact]
        public void RemoveUser_ShouldCall_DeleteUserById_And_SaveChanges_Methods_On_UserRepo_With_Correct_Id()
        {
            // Arrange
            var userToAdd = new User { Id = 1, Name = "John" };
            _userRepoMock.Setup(x => x.GetById(userToAdd.Id)).Returns(userToAdd);
            var userIdToRemove = 1;

            // Act
            _userService.removeUser(userIdToRemove);

            // Assert
            _userRepoMock.Verify(x => x.DeleteUserById(userIdToRemove), Moq.Times.Once);
            _userRepoMock.Verify(x => x.SaveChanges(), Moq.Times.Once);
        }
    }

}
