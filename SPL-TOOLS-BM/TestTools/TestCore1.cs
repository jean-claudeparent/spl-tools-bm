using SPL_TOOLS_BM;


namespace TestTools
{
    [TestClass]
    public sealed class TestCore1
    {
        [TestMethod]
        public async Task TestGetToken()
        {
            SPL_TOOLS_BM.Core  myCore = new SPL_TOOLS_BM.Core()   ;
            string Result = await myCore.GetTokenAsync("unknownuser1756","falsepostingkey");
            
                           Assert.Fail(Result ); 
        }
    }
}
