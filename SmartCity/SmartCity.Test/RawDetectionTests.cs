using Bogus;
using Bogus.DataSets;
using Faker;
using FizzWare.NBuilder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SmartCity.Core;
using SmartCity.Core.Services;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.RawDetection;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services;
using Xunit;


namespace SmartCity.Test
{

    public class RawDetectionTests
    {
        [Fact]
        public async void Test1()
        {
            try
            {
                //var applicationContextMock2 = new Mock<IApplicationContext <User>>();
                //var serviceProvider = new Mock<IServiceProvider>();
                //var applicationContextMock = serviceProvider.Object.GetRequiredService<IApplicationContext<User>>();
                //var rawDetectionRepositoryMoc = new Mock<IGenericRepositorySimpleUniqueIdentifier<RawDetection>>();

                //var rawDetectionService = new RawDetectionService(applicationContextMock, rawDetectionRepositoryMoc.Object);

                // Act

                var rawDetectionRequest = GenerateMoqRawDetectionRequest();
                var rawDetectionRequestJson = Newtonsoft.Json.JsonConvert.SerializeObject(rawDetectionRequest);


                // await rawDetectionService.CreateRawDetectionAsync(rawDetectionRequestJson, Guid.NewGuid().ToString());

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            // Arrange

            // Assert
        }






        private RawDetectionRequest GenerateMoqRawDetectionRequest()
        {

            //        var users = Builder<RawDetectionRequest>.CreateListOfSize(100)
            //.All()
            //.With(u => u.MainClass = MainClass.FullName())
            //.With(u => u.Orders = Builder<RawDetectedObjectRequest>.CreateListOfSize(RandomNumber.Next(1, 5))
            //    .All()
            //    .With(o => o.OrderDate = DateTime.Now.AddDays(-RandomNumber.Next(1, 365)))
            //    .With(o => o.Total = Math.Round((decimal)(RandomNumber. * 100), 2))
            //    .Build())
            //.Build();



            //var generator = new SequentialGenerator<> { Direction = GeneratorDirection.Ascending, Increment = 1000 }; 
            //generator.StartingWith(10000);
            //var product = Builder<RawDetectionRequest>.CreateNew().With(x => x.PriceBeforeTax = generator.Next(50, 1000))
            //    .Build();

            var hierarchySpec = Builder<HierarchySpec<RawDetectionRequest>>.CreateNew();
            //Builder<RawDetectionRequest>.CreateNew.BuildHierarchy(hierarchySpec);

            var faker = new Faker<RawDetectionRequest>();
            var rawDetectionMock = faker.Generate();

            return rawDetectionMock;
        }
    }






}