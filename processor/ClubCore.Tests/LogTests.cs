using ClubCore.Utilities;
using FluentAssertions;

namespace ClubCore.Tests.Utilities
{
    public class LogTests
    {
        [Fact]
        public void Info_WritesInfoPrefix()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            Log.Info("Hello world");

            // Assert
            sw.ToString().TrimEnd()
              .Should().Be("[INFO] Hello world");
        }

        [Fact]
        public void Warn_WritesWarnPrefix()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            Log.Warn("Be careful");

            // Assert
            sw.ToString().TrimEnd()
              .Should().Be("[WARN] Be careful");
        }

        [Fact]
        public void Error_WritesErrorPrefix()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            Log.Error("Something broke");

            // Assert
            sw.ToString().TrimEnd()
              .Should().Be("[ERROR] Something broke");
        }
    }
}