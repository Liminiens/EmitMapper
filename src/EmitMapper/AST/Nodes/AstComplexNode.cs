using System.Collections.Generic;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
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