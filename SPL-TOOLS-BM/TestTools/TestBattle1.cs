using SPL_TOOLS_BM;


namespace TestTools
{
    [TestClass]
    public sealed class TestBattle1
    {
        



        [TestMethod]
        public async Task TestGetBattleallOk()
        {
            // correct account and correct encoded  key and   the real key of the account
            SPL_TOOLS_BM.Core myCore = new SPL_TOOLS_BM.Core();
            string keyfile = @"c:\devcenter\secrets\spidereyes-postingkey.txt";
            Assert.IsTrue (File.Exists (keyfile), "the file containig the key for the test is missing : " + keyfile);
            string HiveKey = File.ReadAllText(keyfile);
            Assert.IsTrue(HiveKey.Length == 51, "The hive key must have a length of 51 characters" );
            
            try
            {

                string Token = await myCore.GetTokenAsync("spidereyes", HiveKey);
                Assert.IsGreaterThan(9, Token.Length,Token );
                String Result = await myCore.GetBattlesAsync (Token , "blindmage")  ;
                Assert.IsGreaterThan(10000, Result.Length );
                
            }
            catch (Exception ex)
            {
                Assert.Fail (ex.Message);
            }



        } // end method
          // end method

        // todo add this file in the ressources of the project
        // "C:\devcenter\source\spl-tools-bm\SPL-TOOLS-BM\specs\battles_blindmage_20261125_140423.json.txt"
        // to get a relative path
        [TestMethod]
        public void TestProcessBattleallOk()
        {
            try
            {

                string BattleHistory = File.ReadAllText(@"C:\devcenter\source\spl-tools-bm\SPL-TOOLS-BM\specs\battles_blindmage_20261125_140423.json.txt");
                SPL_TOOLS_BM.Core myCore = new SPL_TOOLS_BM.Core();
                myCore.ProcessBattles(BattleHistory ); 

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

 

        } // method
    } // end class
    }  //end namespace
