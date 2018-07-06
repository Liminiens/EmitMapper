namespace EmitMapper.NetStandard.AST.Interfaces
{
    interface IAstNode
    {
        void Compile(CompilationContext context);
    }
}