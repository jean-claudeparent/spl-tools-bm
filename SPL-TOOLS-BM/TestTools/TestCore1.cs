using SPL_TOOLS_BM;


namespace TestTools
{
    [TestClass]
    public sealed class TestCore1
    {
        [TestMethod]
        public async Task TestGetTokenAllWrong()
        {
            // bad account and bad key
            SPL_TOOLS_BM.Core  myCore = new SPL_TOOLS_BM.Core();
            try
            {
                string Result = await myCore.GetTokenAsync("unknownuser1756","falsepostingkey");
            } catch   (Exception ex)
            { 
            Assert.Contains  ("Invalid base58 data", ex.Message);
            }



        }

        [TestMethod]
        public async Task TestGetTokenkeyOk()
        {
            // bad account and correct encoded  key
            SPL_TOOLS_BM.Core myCore = new SPL_TOOLS_BM.Core();
            try
            {
                string Result = await myCore.GetTokenAsync("unknownuser1756", "L4FYU2Q8pALbXdsyiFVdUT4DS2miNCYZKmoBV7VhLGX9gvawtkvo");
            }
            catch (Exception ex)
            {
                Assert.Contains  ("Connection was successfull but the token has a problem", ex.Message);
            }



            } // end method


        [TestMethod]
        public async Task TestGetTokenkeyOk2()
        {
            // correct account and correct encoded  key but not the real key of the account
            SPL_TOOLS_BM.Core myCore = new SPL_TOOLS_BM.Core();
            try
            {
                string Result = await myCore.GetTokenAsync("spidereyes", "L4FYU2Q8pALbXdsyiFVdUT4DS2miNCYZKmoBV7VhLGX9gvawtkvo");
            }
            catch (Exception ex)
            {
                Assert.Contains("Connection was successfull but the token has a problem", ex.Message);
            }



        } // end method

        [TestMethod]
        public async Task TestGetTokenallOk2()
        {
            // correct account and correct encoded  key and   the real key of the account
            SPL_TOOLS_BM.Core myCore = new SPL_TOOLS_BM.Core();
            string keyfile = @"c:\devcenter\secrets\spidereyes-postingkey.txt";
            Assert.IsTrue (File.Exists (keyfile), "the file containig the key for the test is missing : " + keyfile);
            string HiveKey = File.ReadAllText(keyfile);
            Assert.IsTrue(HiveKey.Length == 51, "The hive key must have a length of 51 characters" );
            
            try
            {

                string Result = await myCore.GetTokenAsync("spidereyes", HiveKey);
            }
            catch (Exception ex)
            {
                Assert.Fail (ex.Message);
            }



        } // end method


    } // end class
}  //end namespace
