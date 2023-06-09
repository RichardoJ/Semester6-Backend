﻿using CatalogNoSQL.Model;
using Docker.DotNet;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MongoDB.Driver;
using System;


namespace CatalogTesting
{
    public class MongoDBContainerFixture : IDisposable
    {
        private const string MongoDbImage = "mongo:5.0";
        private const string MongoDbContainerName = "mongo-test-container";
        private const int MongoDbPort = 27017;
        private readonly IContainer _container;
        private readonly DockerClient _dockerClient;

        public MongoClient MongoClient { get; }
        public IMongoDatabase MongoDatabase { get; }
        public IMongoCollection<Paper> PapersCollection { get; }

        public string ConnectionString { get; }

        public MongoDBContainerFixture()
        {

            // Create container
            var builder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage(MongoDbImage)
                .WithName(MongoDbContainerName)
                .WithPortBinding(MongoDbPort, MongoDbPort)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MongoDbPort));
            _container = builder.Build();
            _container.StartAsync().GetAwaiter().GetResult();

            // Get client and database
            ConnectionString = $"mongodb://{_container.Hostname}:{_container.GetMappedPublicPort(MongoDbPort)}";
            MongoClient = new MongoClient(ConnectionString);
            MongoDatabase = MongoClient.GetDatabase("test-database");

            // Create collection
            var collectionName = "test-collection";
            var options = new CreateCollectionOptions
            {
                AutoIndexId = true
            };
            MongoDatabase.CreateCollection(collectionName, options);

            // Get papers collection
            PapersCollection = MongoDatabase.GetCollection<Paper>(collectionName);
        }

        public void Dispose()
        {
            ClearData();
            _container.StopAsync().GetAwaiter().GetResult();
        }

        public void ClearData()
        {
            MongoDatabase.DropCollection("test-collection");
            MongoDatabase.CreateCollection("test-collection");
        }

    }
}
