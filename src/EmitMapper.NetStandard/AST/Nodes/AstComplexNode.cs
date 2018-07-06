using System.Collections.Generic;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstComplexNode: IAstNode
    {
        public List<IAstNode> Nodes = new List<IAstNode>();

        public void Compile(CompilationContext context)
        {
            foreach (IAstNode node in Nodes)
            {
                if (node != null)
                {
                    node.Compile(context);
                }
            }
        }
    }
}