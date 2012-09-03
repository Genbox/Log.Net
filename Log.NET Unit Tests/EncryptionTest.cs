using LogNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogNETUnitTests
{
    /// <summary>
    ///This is a test class for IanQvist.Logging.LogHelper.Encryption and is intended
    ///to contain all IanQvist.Logging.LogHelper.Encryption Unit Tests
    ///</summary>
    [TestClass()]
    public class EncryptionTest
    {
        /// <summary>
        ///A test for Decrypt (string)
        ///</summary>
        [TestMethod()]
        public void DecryptTest()
        {
            const string encryptedData = @"OEaoNjEIx+wElIArQpyRz22eJbouYBmaUMt9VC+VaVTUb5/2a9xHRReNDSWoN/ZpVqoyeCyrsvdEGLGkP2kp7HSelmLYuRLXuExNGKNII5wNFl8fhmKXAOiMtK5HBoda6uvT/qrmQ+u2/vt1/xsRGg==";
            const string expected = "Test data with special characters !\"#%&/()=?`¨'øæå,.";
            string actual = Encryption.Decrypt(encryptedData);

            Assert.AreEqual(expected, actual, "Decryption failed");
        }

        /// <summary>
        ///A test for Encrypt (string)
        ///</summary>
        [DeploymentItem("IanQvist.Logging.LogHelper.dll")]
        [TestMethod()]
        public void EncryptTest()
        {
            const string data = "Test data with special characters !\"#%&/()=?`¨'øæå,.";
            const string expected = @"OEaoNjEIx+wElIArQpyRz22eJbouYBmaUMt9VC+VaVTUb5/2a9xHRReNDSWoN/ZpVqoyeCyrsvdEGLGkP2kp7HSelmLYuRLXuExNGKNII5wNFl8fhmKXAOiMtK5HBoda6uvT/qrmQ+u2/vt1/xsRGg==";
            string actual = Encryption.Encrypt(data);

            Assert.AreEqual(expected, actual, "Encryption failed");
        }
    }
}