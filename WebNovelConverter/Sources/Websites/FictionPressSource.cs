namespace WebNovelConverter.Sources.Websites
{
    public class FictionPressSource : FanFictionSource
    {
        public override string BaseUrl => "https://www.fictionpress.com";

        public FictionPressSource() : base("Fictionpress")
        {
        }
    }
}
