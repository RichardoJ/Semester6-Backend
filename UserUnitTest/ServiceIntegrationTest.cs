using Docker.DotNet;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Data;
using UserService.Model;
using UserService.Repository;

namespace UserUnitTest
{
    public class UserRepoIntegrationTest : IDisposable
    {
        private readonly string _containerName = "integration-test-mysql";
        private readonly string _databaseName = "testdb";
        private readonly string _connectionString;
        private readonly UserRepo userRepo;
        private readonly DockerClient _dockerClient;
        private readonly TestcontainersContainer _mysqlContainer;
        private readonly AppDbContext _dbContext;

        public UserRepoIntegrationTest()
        {
            // Initialize DockerClient
            try
            {
                _dockerClient = new DockerClientConfiguration(new Uri("unix:/var/run/docker.sock")).CreateClient();
            }
            catch (DockerApiException ex)
            {
                throw new Exception("Failed to create Docker client. Make sure Docker is installed and running on your machine.", ex);
            }

            // Start MySQL container
            _mysqlContainer = new TestcontainersBuilder<TestcontainersContainer>()
                    .WithDockerEndpoint(new Uri("unix:/var/run/docker.sock"))
                    .WithImage("mysql:5.7")
                    .WithExposedPort(3306)
                    .WithEnvironment("MYSQL_ROOT_PASSWORD", "root")
                    .WithEnvironment("MYSQL_DATABASE", _databaseName)
                    .WithPortBinding(3307, 3306)
                    .WithName(_containerName)
                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(3306))
                    .Build();

             _mysqlContainer.StartAsync().GetAwaiter().GetResult();
            var server = _mysqlContainer.Hostname;

            _connectionString = $"Server={server};Port=3307;Database=testdb;User=root;Password=root;SslMode=None;";

            // Initialize AppDbContext
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));

            _dbContext = new AppDbContext(optionsBuilder.Options);
            _dbContext.Database.EnsureCreated();
            userRepo = new UserRepo(_dbContext);
        }

        

        [Fact]
        public void RemoveUser_Should_Remove_User_From_Database()
        {
            // Arrange
            var userIdToRemove = 1;
            var userToRemove = new User { Id = userIdToRemove, Name = "John", Email = "richardo.jason@gmail.com", Role = "READER", University = "Petra" };
            _dbContext.Users.Add(userToRemove);
            _dbContext.SaveChanges();
            //userRepo.AddUser(userToRemove);
            //userRepo.SaveChanges();


            // Act
            userRepo.DeleteUserById(userIdToRemove);
            userRepo.SaveChanges();

            // Assert
            var removedUser = _dbContext.Users.Find(userIdToRemove);
            Assert.Null(removedUser);
        }

        [Fact]
        public void AddUser_Should_Add_User_To_Database()
        {
            // Arrange
            var userToAdd = new User { Id = 1, Name = "John", Email = "richardo.jason@gmail.com", Role = "READER", University = "Petra" };

            // Act
            userRepo.AddUser(userToAdd);
            userRepo.SaveChanges();

            // Assert
            var addedUser = _dbContext.Users.Find(1);
            Assert.Equal(userToAdd, addedUser);
        }

        public void Dispose()
        {
            try
            {
                _dbContext.Dispose();
                _mysqlContainer.StopAsync().GetAwaiter().GetResult();
                _mysqlContainer.DisposeAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cleanup: {ex}");
            }
        }
    }

}
