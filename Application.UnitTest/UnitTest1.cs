using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Test Grid 5X5
        /// Start position 1 2 N
        /// Direction sequance LMLMLMLMM
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            //var dto = new RoverDto
            //{
            //    Coordinates = new MarsRoverExerciseAPI.Dtos.Coordinates
            //    {
            //        X = 5,
            //        Y = 5
            //    },
            //    Position = new MarsRoverExerciseAPI.Dtos.Position
            //    {
            //        CurrentPosition = new MarsRoverExerciseAPI.Dtos.Coordinates
            //        {
            //            X = 1,
            //            Y = 2
            //        },
            //        Direction = "N"
            //    },
            //    Movements = "LMLMLMLMM"
            //};
            //// Arrange
            //var controller = new RoverController(new RoverRepo(), new RoverValidator(), new RoverMapper());
            //// Act
            //var result = (OkObjectResult)controller.MoveRover(dto).Result;
            //var okres = (RoverResultDto)result.Value;
            ////Assert
            //Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            //var expectedOutput = "1 3 N";
            //var currentResult = $"{okres.CurrentCoordinates.X} {okres.CurrentCoordinates.Y} {okres.Direction}";
            //Assert.AreEqual(expectedOutput, currentResult);
            //if (!okres.Message.Contains("successfully")) Assert.Fail(okres.Message);


        }
    }
}
