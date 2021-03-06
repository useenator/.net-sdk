﻿namespace DreamFactory.Tests.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DreamFactory.Api;
    using DreamFactory.Api.Implementation;
    using DreamFactory.Http;
    using DreamFactory.Model.Database;
    using DreamFactory.Model.System.Custom;
    using DreamFactory.Rest;
    using DreamFactory.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Shouldly;

    [TestClass]
    public class CustomSettingsApiTests
    {
        [TestMethod]
        public void ShouldGetCustomSettingsAsync()
        {
            // Arrange
            ICustomSettingsApi settingsApi = CreateSettingsApi();

            // Act
            IEnumerable<string> names = settingsApi.GetCustomSettingsAsync().Result.Records.Select(x => x.Name);

            // Assert
            names.ShouldContain("preferences");
        }

        [TestMethod]
        public void ShouldSetCustomSettingAsync()
        {
            // Arrange
            ICustomSettingsApi settingsApi = CreateSettingsApi();
            List<CustomRequest> userSettings = CreateUserSettings();

            // Act
            bool ok = settingsApi.SetCustomSettingsAsync(userSettings).Result.Records.Any();

            // Assert
            ok.ShouldBe(true);

            Should.Throw<ArgumentNullException>(() => settingsApi.SetCustomSettingsAsync(null));
        }

        [TestMethod]
        public void ShouldGetCustomSettingAsync()
        {
            // Arrange
            ICustomSettingsApi settingsApi = CreateSettingsApi();

            // Act
            CustomResponse setting = settingsApi.GetCustomSettingAsync("Language").Result;

            // Assert
            setting.Value.ShouldBe("en-us");

            Should.Throw<ArgumentNullException>(() => settingsApi.GetCustomSettingAsync(null));
        }

        [TestMethod]
        public void ShouldDeleteCustomSettingAsync()
        {
            // Arrange
            ICustomSettingsApi settingsApi = CreateSettingsApi();

            // Act
            CustomResponse settings = settingsApi.DeleteCustomSettingAsync("Language").Result;

            // Assert
            settings.Name.ShouldBe("Language");

            Should.Throw<ArgumentNullException>(() => settingsApi.DeleteCustomSettingAsync(null));
        }

        [TestMethod]
        public void ShouldUpdateCustomSettingAsync()
        {
            // Arrange
            ICustomSettingsApi settingsApi = CreateSettingsApi();
            CustomRequest userSetting = CreateUserSettings().First();

            // Act
            CustomResponse setting = settingsApi.UpdateCustomSettingAsync("Language", userSetting).Result;

            // Assert
            setting.Name.ShouldBe("Language");

            Should.Throw<ArgumentNullException>(() => settingsApi.UpdateCustomSettingAsync(null, userSetting));
            Should.Throw<ArgumentNullException>(() => settingsApi.UpdateCustomSettingAsync("Language", null));
        }

        [TestMethod]
        public void ShouldUpdateCustomSettingsAsync()
        {
            // Arrange
            ICustomSettingsApi settingsApi = CreateSettingsApi();
            List<CustomRequest> userSettings = CreateUserSettings();

            // Act
            IEnumerable<CustomResponse> settings = settingsApi.UpdateCustomSettingsAsync(userSettings).Result.Records.ToList();

            // Assert
            settings.Count().ShouldBe(2);
            settings.First().Name.ShouldBe("Language");

            Should.Throw<ArgumentNullException>(() => settingsApi.UpdateCustomSettingsAsync(null));
        }

        private static ICustomSettingsApi CreateSettingsApi()
        {
            IHttpFacade httpFacade = new TestDataHttpFacade();
            HttpAddress address = new HttpAddress("http://base_address", RestApiVersion.V1);
            return new CustomSettingsApi(address, httpFacade, new JsonContentSerializer(), new HttpHeaders(), "user");
        }

        private static List<CustomRequest> CreateUserSettings()
        {
            return new List<CustomRequest>
            {
                new CustomRequest
                {
                    Name = "Language",
                    Value = "en-us"
                },
                new CustomRequest
                {
                    Name = "TimeZone",
                    Value = "ET"
                }
            };

        }
    }
}