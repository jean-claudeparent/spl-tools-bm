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
                Assert.Contains  ("Invalid base58 data", ex.Message);
            }



            } // end method

} // end class
}  //end namespace
