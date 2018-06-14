namespace Common.Helper
{
    public class ParamDictionary
    {
        private string key;
        public string Value { get; set; }
        public string Key
        {
            get { return this.key; }
            set { this.key = "{" + value + "}"; }
        }
    }
}
