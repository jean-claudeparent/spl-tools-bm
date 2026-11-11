using SPL_TOOLS_BM;


namespace TestTools
{
    [TestClass]
    public sealed class TestCore1
    {
        [TestMethod]
        public async Task TestGetToken()
        {
            // bad account and bad key
            SPL_TOOLS_BM.Core  myCore = new SPL_TOOLS_BM.Core();
            try
            {
                string Result = await myCore.GetTokenAsync("unknownuser1756","falsepostingkey");
            } catch   (Exception ex)
            { 
            Assert.IsTrue (ex.Message.Contains("Invalid base58 data")  );
            }



        }
    }
}
