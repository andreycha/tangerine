namespace Tangerine.BLL
{
    public struct DeployerThreadConfig
    {
        public int ZipWaitTime;

        public DeployerThreadConfig(int zipWaitTime)
        {
            this.ZipWaitTime = zipWaitTime;
        }
    }
}
