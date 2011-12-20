﻿

namespace VisualMutator.Extensibility
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    public interface IMutationOperator
    {
      //  MutationResultDetails Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types);

        string Identificator { get; }
        string Name { get; }

        string Description { get; }

   
        IEnumerable<MutationTarget> FindTargets(ICollection<TypeDefinition> types);

        void Mutate(MutationContext context);
    }
}
