using Lithnet.Laps.Web.Authorization;
using Moq;
using NLog;
using NUnit.Framework;
using System;

namespace Lithnet.Laps.Web.Test
{
    public class AceTests
    {
        private Mock<ILogger> dummyLogger;

        [SetUp()]
        public void TestInitialize()
        {
            dummyLogger = new Mock<ILogger>();
        }

        /// <summary>
        /// Test setup
        /// 
        /// These tests ensure we are correctly able to resolve the membership of cross-domain groups. 
        /// Membership of domain local groups can only be resolved in the domain the group is located in.
        /// 
        /// For this test, we have 3 domains. 
        ///  - The domain in which the unit test is run which in this case is 'idmdev1'
        ///  - A child domain of idmdev1 called 'subdev1'
        ///  - An external forest that trusts idmdev1 called 'extdev1'
        ///  
        /// 3 users are created in each domain, user1, user2, and user3
        /// 9 groups are created in each domain. 3 domain local groups (G-DL-x), 3 global groups (G-GG-x), and 3 universal groups (G-UG-x). Where x is a number of 1-3
        /// Each user is added to a domain local, global, and universal group, with a number matching their username. So user1 is added to G-DL-1, G-GG-1, and G-UG-1
        /// To each G-DL-1 group, add the G-UG-1 groups from the other domains
        /// To each G-DL-2 group, directly add the 'user2' objects from the other domains
        /// To each G-DL-3 group, add the G-GG-3 groups from the other domains
        /// To each G-UG-1 group, add the G-GG-1 groups from the other domains
        /// To each G-UG-2 group, add the G-UG-2 groups from the other domains
        /// To each G-UG-3 group, add the 'user3' objects from the other domains
        /// 
        /// This will ensure that each support group has 3 users in it, one from each domain, and allow us to query for the existance of those users when processing the ACE in the various domains
        /// 
        /// Note that as domain local groups cannot be used outside of the domain in which they are created, their membership can never be resolved outside of the domain they exist in. As such, the tests that check for the DL group memberships against another domain are expected to not match the ACE entry, and are included on a separate test below.
        /// </summary>
        /// <param name="acePrincipal">The name of the user or group that grants access</param>
        /// <param name="computer">The 'target' computer that we are simulating checking an ACE on. The computer itself doesn't play a part in its test, rather we use the computers domain to determine where to build the user's token. This way domain local groups in the domain where the computer exists will be respected</param>
        /// <param name="requestor">The user who is requesting access to the resource that we are checking the ACE against</param>

        // Test global groups in THIS domain against a computer in THIS domain
        [TestCase("idmdev1\\G-GG-1", "idmdev1\\PC1", "idmdev1\\user1")]
        [TestCase("idmdev1\\G-GG-2", "idmdev1\\PC1", "idmdev1\\user2")]
        [TestCase("idmdev1\\G-GG-3", "idmdev1\\PC1", "idmdev1\\user3")]

        // Test global groups in THIS domain against a computer in CHILD domain
        [TestCase("idmdev1\\G-GG-1", "subdev1\\PC1", "idmdev1\\user1")]
        [TestCase("idmdev1\\G-GG-2", "subdev1\\PC1", "idmdev1\\user2")]
        [TestCase("idmdev1\\G-GG-3", "subdev1\\PC1", "idmdev1\\user3")]

        // Test global groups in CHILD domain against a computer in CHILD domain
        [TestCase("subdev1\\G-GG-1", "subdev1\\PC1", "subdev1\\user1")]
        [TestCase("subdev1\\G-GG-2", "subdev1\\PC1", "subdev1\\user2")]
        [TestCase("subdev1\\G-GG-3", "subdev1\\PC1", "subdev1\\user3")]

        // Test global groups in CHILD domain against a computer in THIS domain
        [TestCase("subdev1\\G-GG-1", "idmdev1\\PC1", "subdev1\\user1")]
        [TestCase("subdev1\\G-GG-2", "idmdev1\\PC1", "subdev1\\user2")]
        [TestCase("subdev1\\G-GG-3", "idmdev1\\PC1", "subdev1\\user3")]

        // Test domain local groups in THIS domain against a computer in THIS domain
        [TestCase("idmdev1\\G-DL-1", "idmdev1\\PC1", "idmdev1\\user1")]
        [TestCase("idmdev1\\G-DL-1", "idmdev1\\PC1", "subdev1\\user1")]
        [TestCase("idmdev1\\G-DL-2", "idmdev1\\PC1", "idmdev1\\user2")]
        [TestCase("idmdev1\\G-DL-2", "idmdev1\\PC1", "subdev1\\user2")]
        [TestCase("idmdev1\\G-DL-3", "idmdev1\\PC1", "idmdev1\\user3")]
        [TestCase("idmdev1\\G-DL-3", "idmdev1\\PC1", "subdev1\\user3")]

        // Test universal groups in THIS domain against a computer in THIS domain
        [TestCase("idmdev1\\G-UG-1", "idmdev1\\PC1", "idmdev1\\user1")]
        [TestCase("idmdev1\\G-UG-1", "idmdev1\\PC1", "subdev1\\user1")]
        [TestCase("idmdev1\\G-UG-2", "idmdev1\\PC1", "idmdev1\\user2")]
        [TestCase("idmdev1\\G-UG-2", "idmdev1\\PC1", "subdev1\\user2")]
        [TestCase("idmdev1\\G-UG-3", "idmdev1\\PC1", "idmdev1\\user3")]
        [TestCase("idmdev1\\G-UG-3", "idmdev1\\PC1", "subdev1\\user3")]

        // Test universal groups in THIS domain against a computer in CHILD domain
        [TestCase("idmdev1\\G-UG-1", "subdev1\\PC1", "idmdev1\\user1")]
        [TestCase("idmdev1\\G-UG-1", "subdev1\\PC1", "subdev1\\user1")]
        [TestCase("idmdev1\\G-UG-2", "subdev1\\PC1", "idmdev1\\user2")]
        [TestCase("idmdev1\\G-UG-2", "subdev1\\PC1", "subdev1\\user2")]
        [TestCase("idmdev1\\G-UG-3", "subdev1\\PC1", "idmdev1\\user3")]
        [TestCase("idmdev1\\G-UG-3", "subdev1\\PC1", "subdev1\\user3")]

        // Test universal groups in CHILD domain against a computer in THIS domain
        [TestCase("subdev1\\G-UG-1", "idmdev1\\PC1", "idmdev1\\user1")]
        [TestCase("subdev1\\G-UG-1", "idmdev1\\PC1", "subdev1\\user1")]
        [TestCase("subdev1\\G-UG-2", "idmdev1\\PC1", "idmdev1\\user2")]
        [TestCase("subdev1\\G-UG-2", "idmdev1\\PC1", "subdev1\\user2")]
        [TestCase("subdev1\\G-UG-3", "idmdev1\\PC1", "idmdev1\\user3")]
        [TestCase("subdev1\\G-UG-3", "idmdev1\\PC1", "subdev1\\user3")]

        // Test universal groups in CHILD domain against a computer in CHILD domain
        [TestCase("subdev1\\G-UG-1", "subdev1\\PC1", "idmdev1\\user1")]
        [TestCase("subdev1\\G-UG-1", "subdev1\\PC1", "subdev1\\user1")]
        [TestCase("subdev1\\G-UG-2", "subdev1\\PC1", "idmdev1\\user2")]
        [TestCase("subdev1\\G-UG-2", "subdev1\\PC1", "subdev1\\user2")]
        [TestCase("subdev1\\G-UG-3", "subdev1\\PC1", "idmdev1\\user3")]
        [TestCase("subdev1\\G-UG-3", "subdev1\\PC1", "subdev1\\user3")]

        // Test domain local groups in CHILD domain against a computer in CHILD domain
        [TestCase("subdev1\\G-DL-1", "subdev1\\PC1", "idmdev1\\user1")]
        [TestCase("subdev1\\G-DL-1", "subdev1\\PC1", "subdev1\\user1")]
        [TestCase("subdev1\\G-DL-2", "subdev1\\PC1", "idmdev1\\user2")]
        [TestCase("subdev1\\G-DL-2", "subdev1\\PC1", "subdev1\\user2")]
        [TestCase("subdev1\\G-DL-3", "subdev1\\PC1", "idmdev1\\user3")]
        [TestCase("subdev1\\G-DL-3", "subdev1\\PC1", "subdev1\\user3")]

        // Test EXT groups in EXT domain against a computer in EXT domain
        [TestCase("extdev1\\G-DL-1", "extdev1\\PC1", "extdev1\\user1")]
        [TestCase("extdev1\\G-DL-2", "extdev1\\PC1", "extdev1\\user2")]
        [TestCase("extdev1\\G-DL-3", "extdev1\\PC1", "extdev1\\user3")]
        [TestCase("extdev1\\G-GG-1", "extdev1\\PC1", "extdev1\\user1")]
        [TestCase("extdev1\\G-GG-2", "extdev1\\PC1", "extdev1\\user2")]
        [TestCase("extdev1\\G-GG-3", "extdev1\\PC1", "extdev1\\user3")]
        [TestCase("extdev1\\G-UG-1", "extdev1\\PC1", "extdev1\\user1")]
        [TestCase("extdev1\\G-UG-2", "extdev1\\PC1", "extdev1\\user2")]
        [TestCase("extdev1\\G-UG-3", "extdev1\\PC1", "extdev1\\user3")]

        [TestCase("extdev1\\G-DL-1", "extdev1\\PC1", "idmdev1\\user1")]
        [TestCase("extdev1\\G-DL-1", "extdev1\\PC1", "subdev1\\user1")]

        [TestCase("extdev1\\G-DL-2", "extdev1\\PC1", "idmdev1\\user2")]
        [TestCase("extdev1\\G-DL-2", "extdev1\\PC1", "subdev1\\user2")]

        [TestCase("extdev1\\G-DL-3", "extdev1\\PC1", "idmdev1\\user3")]
        [TestCase("extdev1\\G-DL-3", "extdev1\\PC1", "subdev1\\user3")]

        public void TestAceMatch(string acePrincipal, string computer, string requestor)
        {
            Assert.IsTrue(this.IsMatch(acePrincipal, computer, requestor));
        }

        // Test domain local groups in OTHER domain against a computer in THIS domain
        [TestCase("subdev1\\G-DL-1", "idmdev1\\PC1", "idmdev1\\user1")]
        [TestCase("subdev1\\G-DL-1", "idmdev1\\PC1", "subdev1\\user1")]
        [TestCase("subdev1\\G-DL-2", "idmdev1\\PC1", "idmdev1\\user2")]
        [TestCase("subdev1\\G-DL-2", "idmdev1\\PC1", "subdev1\\user2")]
        [TestCase("subdev1\\G-DL-3", "idmdev1\\PC1", "idmdev1\\user3")]
        [TestCase("subdev1\\G-DL-3", "idmdev1\\PC1", "subdev1\\user3")]

        // Test domain local groups in THIS domain against a computer in OTHER domain
        [TestCase("idmdev1\\G-DL-1", "subdev1\\PC1", "idmdev1\\user1")]
        [TestCase("idmdev1\\G-DL-1", "subdev1\\PC1", "subdev1\\user1")]
        [TestCase("idmdev1\\G-DL-2", "subdev1\\PC1", "idmdev1\\user2")]
        [TestCase("idmdev1\\G-DL-2", "subdev1\\PC1", "subdev1\\user2")]
        [TestCase("idmdev1\\G-DL-3", "subdev1\\PC1", "idmdev1\\user3")]
        [TestCase("idmdev1\\G-DL-3", "subdev1\\PC1", "subdev1\\user3")]
        public void TestAceNotMatch(string acePrincipal, string computer, string requestor)
        {
            Assert.IsFalse(this.IsMatch(acePrincipal, computer, requestor));
        }

        private bool IsMatch(string acePrincipal, string computer, string requestor)
        {
            Mock<IAce> ace = new Mock<IAce>();
            ace.SetupGet(a => a.Name).Returns(acePrincipal);
            ace.SetupGet(x => x.Type).Returns(AceType.Allow);

            ActiveDirectory.ActiveDirectory d = new ActiveDirectory.ActiveDirectory();
            
            AceEvaluator evaluator = new AceEvaluator(d, dummyLogger.Object);

            return evaluator.IsMatchingAce(ace.Object, d.GetComputer(computer), d.GetUser(requestor));
        }
    }
}