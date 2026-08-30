using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.Steps.Query
{
   public class TranparentIdentifierInjector: QueryTransformerStep
   {
      private Declaration _l;
      private Declaration _r;
      private string _name;
      private int _depth;
      
      public TranparentIdentifierInjector(Declaration l, Declaration r, string name, int depth)
      {
         _l = l;
         _r = r;
         _name = name;
         _depth = depth;
      }
      
      override public void OnReferenceExpression(ReferenceExpression node)
      {
         if (node.Name.Equals(_r.Name))
            node.Name = _name + ".Item" + _depth.ToString();
         else if (node.Name.Equals(_l.Name))
            node.Name = _name + ".Item1";
         else if (node.Name.StartsWith(_l.Name + '.'))
            node.Name = node.Name.Replace(_l.Name, _name + ".Item1");
      }
   }
}