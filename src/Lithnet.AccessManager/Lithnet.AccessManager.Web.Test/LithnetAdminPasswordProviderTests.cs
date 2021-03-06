﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Lithnet.AccessManager.Test
{
    public class LithnetAdminPasswordProviderTests
    {
        private ActiveDirectory directory;

        private LithnetAdminPasswordProvider provider;

        [SetUp()]
        public void TestInitialize()
        {
            directory = new ActiveDirectory();
            provider = new LithnetAdminPasswordProvider(Mock.Of<ILogger<LithnetAdminPasswordProvider>>());
        }

        [TestCase("EXTDEV1\\PC1")]
        public void SetFirstPassword(string computerName)
        {
            IComputer computer = directory.GetComputer(computerName);

            this.provider.ClearPassword(computer);
            this.provider.ClearPasswordHistory(computer);

            CollectionAssert.IsEmpty(this.provider.GetPasswordHistory(computer));
            Assert.IsNull(this.provider.GetCurrentPassword(computer, null));

            DateTime created = DateTime.UtcNow.Trim(TimeSpan.TicksPerSecond);
            DateTime expired = DateTime.UtcNow.AddDays(-5).Trim(TimeSpan.TicksPerSecond);

            string password = "this is my data";

            this.provider.UpdateCurrentPassword(computer, password, created, expired, 0);

            ProtectedPasswordHistoryItem current = this.provider.GetCurrentPassword(computer, null);
            IReadOnlyList<ProtectedPasswordHistoryItem> history = this.provider.GetPasswordHistory(computer);

            Assert.AreEqual(password, current.EncryptedData);
            Assert.AreEqual(created, current.Created);

            CollectionAssert.IsEmpty(history);
        }

        [TestCase("EXTDEV1\\PC1")]
        public void AddToPasswordHistory(string computerName)
        {
            IComputer computer = directory.GetComputer(computerName);
            provider.ClearPassword(computer);
            provider.ClearPasswordHistory(computer);
            CollectionAssert.IsEmpty(provider.GetPasswordHistory(computer));

            DateTime firstCreated = DateTime.UtcNow.Trim(TimeSpan.TicksPerSecond);
            DateTime firstExpiry = DateTime.UtcNow.AddDays(-3).Trim(TimeSpan.TicksPerSecond);
            string firstPassword = "first password";

            provider.UpdateCurrentPassword(computer, firstPassword, firstCreated, firstExpiry, 0);
            IReadOnlyList<ProtectedPasswordHistoryItem> history =  provider.GetPasswordHistory(computer);
            ProtectedPasswordHistoryItem currentPassword = provider.GetCurrentPassword(computer, null);
            DateTime? currentExpiry = provider.GetExpiry(computer);
            
            Assert.IsNotNull(currentExpiry);
            Assert.AreEqual(0, history.Count);
            Assert.AreEqual(firstCreated, currentPassword.Created);
            Assert.AreEqual(null, currentPassword.Retired);
            Assert.AreEqual(firstPassword, currentPassword.EncryptedData);
            Assert.AreEqual(firstExpiry.Ticks, currentExpiry.Value.Ticks);
            Assert.AreEqual(firstExpiry, currentExpiry);

            DateTime secondCreated = DateTime.UtcNow.AddDays(2).Trim(TimeSpan.TicksPerSecond);
            DateTime secondExpiry = DateTime.UtcNow.AddDays(-5).Trim(TimeSpan.TicksPerSecond);
            string secondPassword = "second password";

            provider.UpdateCurrentPassword(computer, secondPassword, secondCreated, secondExpiry, 30);


            history = provider.GetPasswordHistory(computer);
            currentPassword = provider.GetCurrentPassword(computer, null);
            currentExpiry = provider.GetExpiry(computer);

            Assert.IsNotNull(currentExpiry);
            Assert.AreEqual(1, history.Count);
            ProtectedPasswordHistoryItem firstHistoryItem = history.First();

            Assert.AreEqual(firstCreated, firstHistoryItem.Created);
            Assert.AreEqual(firstPassword, firstHistoryItem.EncryptedData);
            
            Assert.AreEqual(secondCreated, firstHistoryItem.Retired);
            Assert.AreEqual(secondExpiry, currentExpiry);
            Assert.AreEqual(secondPassword, currentPassword.EncryptedData);
            Assert.AreEqual(secondCreated, currentPassword.Created);
            Assert.AreEqual(null, currentPassword.Retired);
        }

        [TestCase("EXTDEV1\\PC1")]
        public void AgeOutPasswordHistory(string computerName)
        {
            IComputer computer = directory.GetComputer(computerName);
            provider.ClearPassword(computer);
            provider.ClearPasswordHistory(computer);
            CollectionAssert.IsEmpty(provider.GetPasswordHistory(computer));

            DateTime firstCreated = DateTime.UtcNow.AddDays(-8).Trim(TimeSpan.TicksPerSecond);
            DateTime firstExpiry = DateTime.UtcNow.AddDays(-5).Trim(TimeSpan.TicksPerSecond);
            string firstPassword = "first password";

            provider.UpdateCurrentPassword(computer, firstPassword, firstCreated, firstExpiry, 0);
            IReadOnlyList<ProtectedPasswordHistoryItem> history = provider.GetPasswordHistory(computer);
            ProtectedPasswordHistoryItem currentPassword = provider.GetCurrentPassword(computer, null);
            DateTime? currentExpiry = provider.GetExpiry(computer);
            
            Assert.IsNotNull(currentExpiry);
            Assert.AreEqual(0, history.Count);
            Assert.AreEqual(firstCreated, currentPassword.Created);
            Assert.AreEqual(null, currentPassword.Retired);
            Assert.AreEqual(firstPassword, currentPassword.EncryptedData);
            Assert.AreEqual(firstExpiry.Ticks, currentExpiry.Value.Ticks);
            Assert.AreEqual(firstExpiry, currentExpiry);

            DateTime secondCreated = DateTime.UtcNow.AddDays(-4).Trim(TimeSpan.TicksPerSecond);
            DateTime secondExpiry = DateTime.UtcNow.AddDays(-5).Trim(TimeSpan.TicksPerSecond);
            string secondPassword = "second password";

            provider.UpdateCurrentPassword(computer, secondPassword, secondCreated, secondExpiry, 30);
            history = provider.GetPasswordHistory(computer);
            currentPassword = provider.GetCurrentPassword(computer, null);
            currentExpiry = provider.GetExpiry(computer);
            
            Assert.IsNotNull(currentExpiry);
            Assert.AreEqual(1, history.Count);
            ProtectedPasswordHistoryItem firstHistoryItem = history.First();
            Assert.AreEqual(firstCreated, firstHistoryItem.Created);
            Assert.AreEqual(firstPassword, firstHistoryItem.EncryptedData);

            Assert.AreEqual(secondCreated, firstHistoryItem.Retired);
            Assert.AreEqual(secondExpiry, currentExpiry);
            Assert.AreEqual(secondPassword, currentPassword.EncryptedData);
            Assert.AreEqual(secondCreated, currentPassword.Created);
            Assert.AreEqual(null, currentPassword.Retired);

            DateTime thirdCreated = DateTime.UtcNow.AddDays(-1).Trim(TimeSpan.TicksPerSecond);
            DateTime thirdExpiry = DateTime.UtcNow.AddDays(-5).Trim(TimeSpan.TicksPerSecond);
            string thirdPassword = "third password";

            provider.UpdateCurrentPassword(computer, thirdPassword, thirdCreated, thirdExpiry, 2);
            history = provider.GetPasswordHistory(computer);
            
            Assert.AreEqual(1, history.Count);
            firstHistoryItem = history.First();

            Assert.AreEqual(secondCreated, firstHistoryItem.Created);
            Assert.AreEqual(secondPassword, firstHistoryItem.EncryptedData);
            Assert.AreEqual(thirdCreated, firstHistoryItem.Retired);
        }
    }
}