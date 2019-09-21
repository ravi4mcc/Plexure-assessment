using System;
using System.Net;
using NUnit.Framework;

namespace PlexureAPITest
{
    [TestFixture]
    public class Test
    {
        Service service;

        [OneTimeSetUp]
        public void Setup()
        {
            service = new Service();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            if (service != null)
            {
                service.Dispose();
                service = null;
            }
        }

        [Test]
        public void TEST_001_Login_With_Valid_User()
        {   
            //Act
            var response = service.Login("Tester", "Plexure123");
            String userName = response.Entity.UserName;
            int userId = response.Entity.UserId;
            String token = response.Entity.AccessToken;
            //Assert
            response.Expect(HttpStatusCode.OK);
            Assert.AreEqual("Tester", response.Entity.UserName);
            Assert.AreEqual(1, response.Entity.UserId);
            Assert.AreEqual("37cb9e58-99db-423c-9da5-42d5627614c5", response.Entity.AccessToken);

        }
        [Test]
        public void TEST_001a_Login_With_InValid_Username()
        {   
            //Act
            var response = service.Login("Testar", "Plexure123");
            //Assert
            response.Expect(HttpStatusCode.Unauthorized);
            Assert.AreEqual("\"Error: Unauthorized\"", response.Error);

        }
        [Test]
        public void TEST_001b_Login_With_InValid_Password()
        {   
            //Act
            var response = service.Login("Tester", "Plexure12");
            //Assert
            response.Expect(HttpStatusCode.Unauthorized);
            Assert.AreEqual("\"Error: Unauthorized\"", response.Error);

        }

        [Test]
        public void TEST_001c_Login_With_Missing_Password()
        {   
            //Act
            var response = service.Login("Tester", "");
            //Assert
            response.Expect(HttpStatusCode.Unauthorized);
            Assert.AreEqual("\"Error: Unauthorized\"", response.Error);
        }

        [Test]
        public void TEST_001d_Login_With_Missing_Username()
        {   
            //Act
            var response = service.Login("", "Plexure123");
            //Assert
            response.Expect(HttpStatusCode.Unauthorized);
            Assert.AreEqual("\"Error: Unauthorized\"", response.Error);
        }

        [Test]
        public void TEST_001e_Login_With_Missing_Credentials()
        {
            //Act
            var response = service.Login("", "");
            //Assert
            response.Expect(HttpStatusCode.BadRequest);
            Assert.AreEqual("\"Error: Username and password required.\"", response.Error);
        }
                   
        
        [Test][Order(1)]
        public void TEST_003_Purchase_Product()
        {
            //Arrange
            int productId = 1;
            //Act
            var response = service.Purchase(productId);
            string message = response.Entity.Message;
            int points = response.Entity.Points;
            //Assert that points earned are 100
            Assert.AreEqual(100,points);
            Assert.AreEqual("Purchase completed.", message);
            response.Expect(HttpStatusCode.Accepted);
        }

        [Test][Order(2)]
        public void TEST_003a_Purchase_Product_Invalid_Product_Id()
        {
            //Arrange
            int productId = 0;
            //Act
            var response = service.Purchase(productId);
            response.Expect(HttpStatusCode.BadRequest);
            //Assert
            Assert.AreEqual("\"Error: Invalid product id\"", response.Error);
        }

        [Test][Order(3)]
        public void TEST_002_Get_Points_For_Logged_In_User()
        {   
            //Act
            var points = service.GetPoints();
            int userId = points.Entity.UserId;
            int point = points.Entity.Value;
            //Assert
            points.Expect(HttpStatusCode.Accepted);
            //Verifying points earned are not null as points are added after each purchase
            Assert.IsNotNull(point);
            Assert.IsNotNull(userId);
            Assert.AreEqual(1, userId);
        }

        [Test][Order(4)]
        public void TEST_002a_Get_Points_For_Invalid_Token()
        {
            //Act -- Calling Login to generate another token to render the current token invalid
            service.Login("Tester", "Plexure123");
            var response = service.GetPoints();
            //Assert
            response.Expect(HttpStatusCode.Unauthorized);
            Assert.AreEqual("\"Error: Unauthorized\"", response.Error);

        }
    }
}
