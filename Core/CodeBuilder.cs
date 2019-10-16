namespace ABPCodeGenerator.Core
{
    public class CodeBuilder
    {
        private CodeBuilderOption _option = new CodeBuilderOption();

        public CodeBuilder() { }

        public CodeBuilder(CodeBuilderOption option)
        {
            this.SetOption(option);
        }

        public void SetOption(CodeBuilderOption option)
        {
            this._option = option;
        }
    }
}
